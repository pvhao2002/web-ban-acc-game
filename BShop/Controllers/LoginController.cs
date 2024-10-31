using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ProjectWeb.Models.Entity;
using ProjectWeb.Utils;

namespace ProjectWeb.Controllers
{
    public class LoginController : Controller
    {
        private DBContext ctx { get; } = DbConnect.instance;

        private const int MaxAge = 30; // 30 days

        // GET: Login
        [HttpPost]
        public async Task<JsonResult> Index(string email, string password)
        {
            var user = await ctx.Users.FirstOrDefaultAsync(item =>
                email.Equals(item.Email) && password.Equals(item.Password));

            if (user == null)
            {
                var failed = new { success = false, message = "Email hoặc mật khẩu không chính xác!" };
                return Json(failed);
            }

            // Set session
            Session[Constant.USER_ID] = user.UserId;
            Session[Constant.EMAIL] = user.Email;

            // Set cookie
            var cookieUserId = new HttpCookie(Constant.USER_ID, user.UserId.ToString())
            {
                Expires = DateTime.Now.AddDays(MaxAge)
            };
            Response.Cookies.Add(cookieUserId);

            var cookieEmail = new HttpCookie(Constant.EMAIL, user.Email)
            {
                Expires = DateTime.Now.AddDays(MaxAge)
            };
            Response.Cookies.Add(cookieEmail);

            var result = new { success = true, message = "Đăng nhập thành công!", page = "Login", role = user.Role };
            return Json(result);
        }
    }
}