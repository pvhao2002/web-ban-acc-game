using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using BShop.Models.Entity;
using BShop.Utils;

namespace BShop.Controllers
{
    [CustomAuthorize(Constant.RoleUser)]
    public class ThankController : Controller
    {
        // GET
        public async Task<ActionResult> Index(string vnp_ResponseCode, string vnp_TransactionStatus, string vnp_TxnRef)
        {
            if (!"00".Equals(vnp_TransactionStatus) || !"00".Equals(vnp_ResponseCode))
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Thanh toán thất bại!";
                return RedirectToAction("Index", "History");
            }

            var userId = AuthenticationUtil.GetUserId(Request, Session);
            var order = await DBContext.Instance.Orders
                .Include(item => item.OrderItems)
                .Include(item => item.OrderItems.Select(oItem => oItem.product))
                .FirstOrDefaultAsync(item =>
                    item.UserId == userId && item.TxRef.Equals(vnp_TxnRef));

            if (order == null)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Đơn hàng không tồn tại!";
                return RedirectToAction("Index", "History");
            }
            var body = MailUtils.BuildBody(order);
            var message = await MailUtils.SendEmail(order.Email, "Tài khoản B Shop", body);

            if (Constant.Success.Equals(message))
            {
                order.Status = Constant.OrderStatusDelivered;
                order.UpdatedAt = DateTime.Now;

                order.OrderItems.ForEach(item =>
                {
                    item.product.Status = Constant.Inactive;
                    item.product.UpdatedAt = DateTime.Now;
                });

                await DBContext.Instance.SaveChangesAsync();
            }

            var status = Constant.Success.Equals(message) ? Constant.Success : Constant.Error;
            var msg = Constant.Success.Equals(message) ? "Tài khoản đã được gửi vào email của bạn!" : message;
            TempData[Constant.StatusRs] = status;
            TempData[Constant.MessageRs] = msg;
            return RedirectToAction("Index", "History");
        }
    }
}