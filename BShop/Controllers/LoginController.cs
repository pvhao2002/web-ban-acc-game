using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BShop.Models.Entity;
using BShop.Utils;

namespace BShop.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpPost]
        public async Task<JsonResult> Index(string email, string password)
        {
            var user = await DBContext.Instance.Users
                .FirstOrDefaultAsync(item => email.Equals(item.Email) && password.Equals(item.Password));

            if (user == null)
            {
                var failed = new { success = false, message = "Email hoặc mật khẩu không chính xác!" };
                return Json(failed);
            }

            // Set cookie
            var cookieUserId = new HttpCookie(Constant.UserId, user.UserId.ToString())
            {
                Expires = DateTime.Now.AddDays(30)
            };
            Response.Cookies.Add(cookieUserId);

            var result = new { success = true, message = "Đăng nhập thành công!", page = "Login", role = user.Role };
            return Json(result);
        }
    }
}