using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using ProjectWeb.Models.Entity;
using ProjectWeb.Models.ViewModel;
using ProjectWeb.Utils;

namespace ProjectWeb.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Constant.ROLE_USER)]
    public class ProfileController : Controller
    {
        private DBContext ctx { get; } = DbConnect.instance;

        // GET
        public async Task<ActionResult> Index()
        {
            var userId = AuthenticationUtil.GetUserId(Request, Session);
            var user = await ctx.Users.FirstOrDefaultAsync(item => item.UserId == userId);
            var model = new ProfileViewModel()
            {
                u = user
            };
            return View(model);
        }

        public async Task<JsonResult> Update(string fullName, string password)
        {
            var userId = AuthenticationUtil.GetUserId(Request, Session);
            var user = await ctx.Users.FirstOrDefaultAsync(item => item.UserId == userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Bạn chưa đăng nhập" });
            }

            user.FullName = string.IsNullOrEmpty(fullName) ? user.FullName : fullName;
            user.Password = string.IsNullOrEmpty(password) ? user.Password : password;
            user.UpdatedAt = DateTime.Now;

            await ctx.SaveChangesAsync();
            return Json(new { success = true, message = "Cập nhật thông tin thành công!" });
        }
    }
}