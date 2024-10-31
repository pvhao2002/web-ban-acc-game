using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BShop.Models.Entity;
using BShop.Models.ViewModel;
using BShop.Utils;

namespace BShop.Controllers
{
    [CustomAuthorize(Constant.RoleUser)]
    public class HistoryController : Controller
    {
        // GET
        public async Task<ActionResult> Index()
        {
            var userId = AuthenticationUtil.GetUserId(Request, Session);
            var user = await DBContext.Instance.Users.FirstOrDefaultAsync(item => item.UserId == userId);
            var listOrder = await DBContext.Instance.Orders
                .Include(item => item.OrderItems)
                .Include(item => item.OrderItems.Select(orderItem => orderItem.product))
                .Where(item => item.UserId == userId)
                .ToListAsync();
            
            var pendingOrder = listOrder.Where(item => item.Status == Constant.OrderStatusPending).ToList();
            var doneOrder = listOrder.Where(item => item.Status == Constant.OrderStatusDelivered).ToList();
            var cancelOrder = listOrder.Where(item => item.Status == Constant.OrderStatusCancel).ToList();
            var model = new ProfileViewModel()
            {
                information = new Information()
                {
                    email = user?.Email,
                    fullName = user?.FullName,
                },
                listPendingOrder = pendingOrder,
                listDoneOrder = doneOrder,
                listCancelOrder = cancelOrder
            };
            return View(model);
        }
    }
}