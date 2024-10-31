using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using ProjectWeb.Models.Entity;
using ProjectWeb.Utils;

namespace ProjectWeb.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Constant.ROLE_USER)]
    public class OrderController : Controller
    {
        private DBContext ctx { get; } = DbConnect.instance;

        // GET
        public async Task<ActionResult> Cancel(int id)
        {
            var order = await ctx.Orders.FirstOrDefaultAsync(item => item.OrderId == id);
            if (order == null)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Đơn hàng không tồn tại!";
                return RedirectToAction("Index", "History");
            }

            if (order.Status != Constant.ORDER_STATUS_PENDING)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Đơn hàng không thể hủy!";
                return RedirectToAction("Index", "History");
            }

            order.Status = Constant.ORDER_STATUS_CANCEL;
            order.UpdatedAt = DateTime.Now;
            await ctx.SaveChangesAsync();
            return RedirectToAction("Index", "History");
        }
    }
}