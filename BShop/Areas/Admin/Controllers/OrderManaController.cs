using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using BShop.Models.Entity;
using BShop.Utils;

namespace BShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Constant.ROLE_ADMIN)]
    public class OrderManaController : Controller
    {
        public const string ACCEPT = "accept";

        // GET
        public async Task<ActionResult> Index(string status)
        {
            var orders = await DBContext.Instance.Orders
                .Include(item => item.OrderItems)
                .Include(item => item.OrderItems.Select(oItem => oItem.product))
                .Where(item => status == null || item.Status.Equals(status))
                .ToListAsync();

            return View(orders);
        }

        public async Task<ActionResult> Accept(int id)
        {
            return await Process(id, ACCEPT);
        }

        public async Task<ActionResult> Reject(int id)
        {
            return await Process(id, "reject");
        }

        private async Task<ActionResult> Process(int id, string status)
        {
            var order = await DBContext.Instance.Orders
                .Include(item => item.OrderItems)
                .Include(item => item.OrderItems.Select(oItem => oItem.product))
                .FirstOrDefaultAsync(item => item.OrderId == id);

            if (order == null)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Hóa đơn không tồn tại";
                return RedirectToAction("Index");
            }

            var body = ACCEPT.Equals(status) ? MailUtils.BuildBody(order) : MailUtils.BuildBodyFail(order);
            var message = await MailUtils.SendEmail(order.Email, "Thông báo đơn hàng B Shop", body);

            if (Constant.SUCCESS.Equals(message))
            {
                if (ACCEPT.Equals(status))
                {
                    order.Status = Constant.ORDER_STATUS_DELIVERED;
                    order.UpdatedAt = DateTime.Now;

                    order.OrderItems.ForEach(item =>
                    {
                        item.product.Status = Constant.INACTIVE;
                        item.product.UpdatedAt = DateTime.Now;
                    });
                    TempData[Constant.STATUS_RS] = Constant.SUCCESS.Equals(message) ? Constant.SUCCESS : Constant.ERROR;
                    TempData[Constant.MESSAGE_RS] =
                        Constant.SUCCESS.Equals(message) ? "Đã xác nhận đơn hàng!" : message;
                }
                else
                {
                    order.Status = Constant.ORDER_STATUS_CANCEL;
                    order.UpdatedAt = DateTime.Now;
                    TempData[Constant.STATUS_RS] = Constant.SUCCESS.Equals(message) ? Constant.SUCCESS : Constant.ERROR;
                    TempData[Constant.MESSAGE_RS] =
                        Constant.SUCCESS.Equals(message) ? "Đã hủy đơn hàng!" : message;
                }

                await DBContext.Instance.SaveChangesAsync();
            }
            else
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = message;
            }

            return RedirectToAction("Index");
        }
    }
}