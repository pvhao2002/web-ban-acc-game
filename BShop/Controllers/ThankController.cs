using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using ProjectWeb.Models.Entity;
using ProjectWeb.Utils;

namespace ProjectWeb.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Constant.ROLE_USER)]
    public class ThankController : Controller
    {
        private DBContext ctx { get; } = DbConnect.instance;

        // GET
        public async Task<ActionResult> Index(string vnp_ResponseCode, string vnp_TransactionStatus, string vnp_TxnRef)
        {
            if (!"00".Equals(vnp_TransactionStatus) || !"00".Equals(vnp_ResponseCode))
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Thanh toán thất bại!";
                return RedirectToAction("Index", "History");
            }

            var userId = AuthenticationUtil.GetUserId(Request, Session);
            var order = await ctx.Orders
                .Include(item => item.OrderItems)
                .Include(item => item.OrderItems.Select(oItem => oItem.product))
                .FirstOrDefaultAsync(item =>
                    item.UserId == userId && item.TxRef.Equals(vnp_TxnRef));

            if (order == null)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Đơn hàng không tồn tại!";
                return RedirectToAction("Index", "History");
            }
            var body = MailUtils.BuildBody(order);
            var message = await MailUtils.SendEmail(order.Email, "Tài khoản HP Shop", body);

            if (Constant.SUCCESS.Equals(message))
            {
                order.Status = Constant.ORDER_STATUS_DELIVERED;
                order.UpdatedAt = DateTime.Now;

                order.OrderItems.ForEach(item =>
                {
                    item.product.Status = Constant.INACTIVE;
                    item.product.UpdatedAt = DateTime.Now;
                });

                await ctx.SaveChangesAsync();
            }

            var status = Constant.SUCCESS.Equals(message) ? Constant.SUCCESS : Constant.ERROR;
            var msg = Constant.SUCCESS.Equals(message) ? "Tài khoản đã được gửi vào email của bạn!" : message;
            TempData[Constant.STATUS_RS] = status;
            TempData[Constant.MESSAGE_RS] = msg;
            return RedirectToAction("Index", "History");
        }
    }
}