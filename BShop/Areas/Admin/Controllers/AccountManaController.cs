using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ProjectWeb.Models.Entity;
using ProjectWeb.Utils;

namespace ProjectWeb.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Constant.ROLE_ADMIN)]
    public class AccountManaController : Controller
    {
        private DBContext ctx { get; } = DbConnect.instance;

        // GET
        public async Task<ActionResult> Index()
        {
            var users = await ctx.Users
                .Where(item => Constant.ROLE_USER.Equals(item.Role))
                .ToListAsync();
            return View(users);
        }

        public async Task<ActionResult> Lock(int id)
        {
            var result = await UpdateStatus(id, Constant.BLOCK);
            TempData[Constant.STATUS_RS] = result ? Constant.SUCCESS : Constant.ERROR;
            TempData[Constant.MESSAGE_RS] = result ? "Khóa tài khoản thành công" : "Khóa tài khoản thất bại";
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Unlock(int id)
        {
            var result = await UpdateStatus(id, Constant.ACTIVE);
            TempData[Constant.STATUS_RS] = result ? Constant.SUCCESS : Constant.ERROR;
            TempData[Constant.MESSAGE_RS] = result ? "Mở khóa tài khoản thành công" : "Mở khóa tài khoản thất bại";
            return RedirectToAction("Index");
        }

        private async Task<bool> UpdateStatus(int userId, string status)
        {
            var user = await ctx.Users.FirstOrDefaultAsync(item => item.UserId == userId);
            if (user == null)
            {
                return false;
            }

            user.Status = status;
            user.UpdatedAt = DateTime.Now;
            await ctx.SaveChangesAsync();
            return true;
        }
    }
}