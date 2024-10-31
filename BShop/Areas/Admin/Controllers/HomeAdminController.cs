using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BShop.Models.Entity;
using BShop.Models.ViewModel;
using BShop.Utils;

namespace BShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Constant.RoleAdmin)]
    public class HomeAdminController : Controller
    {
        // GET: Admin/HomeAdmin
        public async Task<ActionResult> Index()
        {
            var totalUser = await DBContext.Instance.Users.CountAsync(item => Constant.RoleUser.Equals(item.Role)
                                                               && Constant.Active.Equals(item.Status));
            var totalProduct = await DBContext.Instance.Products.CountAsync(item => Constant.Active.Equals(item.Status));
            var totalRevenue = await DBContext.Instance.Orders
                .Where(item => Constant.OrderStatusDelivered.Equals(item.Status))
                .SumAsync(item => item.TotalPrice);

            var currentDate = DateTime.Now;

            // Xác định mốc ngày đầu tiên của tháng hiện tại và tháng trước
            var firstDayOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            
            var firstDayOfLastMonth = firstDayOfCurrentMonth.AddMonths(-1);
            firstDayOfLastMonth = new DateTime(firstDayOfLastMonth.Year, firstDayOfLastMonth.Month, 1);
            var lastDayOfLastMonth = firstDayOfCurrentMonth.AddDays(-1);
            lastDayOfLastMonth = new DateTime(lastDayOfLastMonth.Year, lastDayOfLastMonth.Month,
                DateTime.DaysInMonth(lastDayOfLastMonth.Year, lastDayOfLastMonth.Month));
            
            var diff = (7 + (currentDate.DayOfWeek - DayOfWeek.Monday)) % 7;

            // Xác định ngày đầu tiên của tuần hiện tại (Thứ Hai)
            var startOfCurrentWeek = currentDate.AddDays(-1 * diff);

            
            // Xác định ngày đầu tiên của tuần trước và ngày kết thúc tuần trước (Chủ nhật)
            var startOfLastWeek = startOfCurrentWeek.AddDays(-7);
            startOfCurrentWeek = new DateTime(startOfCurrentWeek.Year, startOfCurrentWeek.Month, startOfCurrentWeek.Day);
            var endOfLastWeek = startOfCurrentWeek.AddDays(-1);
            endOfLastWeek = new DateTime(endOfLastWeek.Year, endOfLastWeek.Month, endOfLastWeek.Day);

            // Tổng doanh thu cho tháng hiện tại
            var totalRevenueCurrentMonth = await DBContext.Instance.Orders
                .Where(item => Constant.OrderStatusDelivered.Equals(item.Status)
                               && item.CreatedAt.HasValue
                               && item.CreatedAt.Value >= firstDayOfCurrentMonth
                               && item.CreatedAt.Value <= currentDate)
                .SumAsync(item => item.TotalPrice);

            // Tổng doanh thu cho tháng trước
            var totalRevenueLastMonth = await DBContext.Instance.Orders
                .Where(item => Constant.OrderStatusDelivered.Equals(item.Status)
                               && item.CreatedAt.HasValue
                               && item.CreatedAt.Value >= firstDayOfLastMonth
                               && item.CreatedAt.Value <= lastDayOfLastMonth)
                .SumAsync(item => item.TotalPrice);

            // Tổng doanh thu cho tuần hiện tại
            var totalRevenueCurrentWeek = await DBContext.Instance.Orders
                .Where(item => Constant.OrderStatusDelivered.Equals(item.Status)
                               && item.CreatedAt.HasValue
                               && item.CreatedAt.Value >= startOfCurrentWeek
                               && item.CreatedAt.Value <= currentDate)
                .SumAsync(item => item.TotalPrice);

            // Tổng doanh thu cho tuần trước (Thứ Hai tuần trước đến Chủ nhật tuần trước)
            var totalRevenueLastWeek = await DBContext.Instance.Orders
                .Where(item => Constant.OrderStatusDelivered.Equals(item.Status)
                               && item.CreatedAt.HasValue
                               && item.CreatedAt.Value >= startOfLastWeek
                               && item.CreatedAt.Value <= endOfLastWeek)
                .SumAsync(item => item.TotalPrice);

            // Tổng doanh thu cho ngày hiện tại
            var totalRevenueCurrentDay = await DBContext.Instance.Orders
                .Where(item => Constant.OrderStatusDelivered.Equals(item.Status)
                               && DbFunctions.TruncateTime(item.CreatedAt) == DbFunctions.TruncateTime(currentDate))
                .SumAsync(item => item.TotalPrice);

            // calculate the percentage of revenue growth from last month to this month
            var revenueGrowth = totalRevenueLastMonth == 0
                ? 100
                : Math.Round(
                    Convert.ToDecimal((totalRevenueCurrentMonth - totalRevenueLastMonth) / totalRevenueLastMonth * 100),
                    2);

            var model = new DashboardViewModel()
            {
                totalCustomer = totalUser,
                totalProduct = totalProduct,
                totalRevenue = totalRevenue,
                totalRevenueMonth = totalRevenueCurrentMonth ?? 0,
                percentRevenueGrowth = revenueGrowth,
                totalRevenueLastMonth = totalRevenueLastMonth ?? 0,
                totalRevenueWeek = totalRevenueCurrentWeek ?? 0,
                totalRevenuePrevWeek = totalRevenueLastWeek ?? 0,
                totalRevenueDay = totalRevenueCurrentDay ?? 0
            };
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            foreach (var cookie in Request.Cookies.AllKeys)
            {
                Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}