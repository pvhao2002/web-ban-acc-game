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
    [CustomAuthorize(Constant.RoleAdmin)]
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
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Hóa đơn không tồn tại";
                return RedirectToAction("Index");
            }

            var body = ACCEPT.Equals(status) ? MailUtils.BuildBody(order) : MailUtils.BuildBodyFail(order);
            var message = await MailUtils.SendEmail(order.Email, "Thông báo đơn hàng B Shop", body);

            if (Constant.Success.Equals(message))
            {
                if (ACCEPT.Equals(status))
                {
                    order.Status = Constant.OrderStatusDelivered;
                    order.UpdatedAt = DateTime.Now;

                    order.OrderItems.ForEach(item =>
                    {
                        item.product.Status = Constant.Inactive;
                        item.product.UpdatedAt = DateTime.Now;
                    });
                    TempData[Constant.StatusRs] = Constant.Success.Equals(message) ? Constant.Success : Constant.Error;
                    TempData[Constant.MessageRs] =
                        Constant.Success.Equals(message) ? "Đã xác nhận đơn hàng!" : message;
                }
                else
                {
                    order.Status = Constant.OrderStatusCancel;
                    order.UpdatedAt = DateTime.Now;
                    TempData[Constant.StatusRs] = Constant.Success.Equals(message) ? Constant.Success : Constant.Error;
                    TempData[Constant.MessageRs] =
                        Constant.Success.Equals(message) ? "Đã hủy đơn hàng!" : message;
                }

                await DBContext.Instance.SaveChangesAsync();
            }
            else
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = message;
            }

            return RedirectToAction("Index");
        }
    }
}