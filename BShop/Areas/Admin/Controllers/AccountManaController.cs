using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BShop.Models.Entity;
using BShop.Utils;

namespace BShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Constant.RoleAdmin)]
    public class AccountManaController : Controller
    {
        // GET
        public async Task<ActionResult> Index()
        {
            var users = await DBContext.Instance.Users
                .Where(item => Constant.RoleUser.Equals(item.Role))
                .ToListAsync();
            return View(users);
        }

        public async Task<ActionResult> Lock(int id)
        {
            var result = await UpdateStatus(id, Constant.Block);
            TempData[Constant.StatusRs] = result ? Constant.Success : Constant.Error;
            TempData[Constant.MessageRs] = result ? "Khóa tài khoản thành công" : "Khóa tài khoản thất bại";
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Unlock(int id)
        {
            var result = await UpdateStatus(id, Constant.Active);
            TempData[Constant.StatusRs] = result ? Constant.Success : Constant.Error;
            TempData[Constant.MessageRs] = result ? "Mở khóa tài khoản thành công" : "Mở khóa tài khoản thất bại";
            return RedirectToAction("Index");
        }

        private async Task<bool> UpdateStatus(int userId, string status)
        {
            var user = await DBContext.Instance.Users
                .FirstOrDefaultAsync(item => item.UserId == userId);
            if (user == null)
            {
                return false;
            }

            user.Status = status;
            user.UpdatedAt = DateTime.Now;
            await DBContext.Instance.SaveChangesAsync();
            return true;
        }
    }
}