using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using BShop.Models.Entity;
using BShop.Utils;

namespace BShop.Controllers
{
    [CustomAuthorize(Constant.RoleUser)]
    public class OrderController : Controller
    {
        public async Task<ActionResult> Cancel(int id)
        {
            var order = await DBContext.Instance.Orders.FirstOrDefaultAsync(item => item.OrderId == id);
            if (order == null)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Đơn hàng không tồn tại!";
                return RedirectToAction("Index", "History");
            }

            if (order.Status != Constant.OrderStatusPending)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Đơn hàng không thể hủy!";
                return RedirectToAction("Index", "History");
            }

            order.Status = Constant.OrderStatusCancel;
            order.UpdatedAt = DateTime.Now;
            await DBContext.Instance.SaveChangesAsync();
            return RedirectToAction("Index", "History");
        }
    }
}