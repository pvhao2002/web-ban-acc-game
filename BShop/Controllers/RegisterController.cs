using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using ProjectWeb.Models.Entity;
using ProjectWeb.Utils;

namespace ProjectWeb.Controllers
{
    public class RegisterController : Controller
    {
        private DBContext ctx { get; } = DbConnect.instance;

        [HttpPost]
        public async Task<JsonResult> Index(string email, string password, string fullname)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                var failed = new { success = false, message = "Email hoặc mật khẩu không được để trống!" };
                return Json(failed);
            }
            var emailLower = email.ToLower();

            var user = await ctx.Users.FirstOrDefaultAsync(item => emailLower.Equals(item.Email.ToLower()));
            
            if (user != null)
            {
                var failed = new { success = false, message = "Email đã tồn tại!" };
                return Json(failed);
            }
            
            var newUser = new User
            {
                Email = email,
                Password = password,
                FullName = fullname,
                Role = Constant.ROLE_USER,
                Status = Constant.ACTIVE,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            ctx.Users.Add(newUser);
            await ctx.SaveChangesAsync();

            var result = new { success = true, message = "Đăng ký thành công!", page = "Register" };
            return Json(result);
        }
    }
}