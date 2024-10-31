using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using BShop.Models.Entity;
using BShop.Utils;

namespace BShop.Controllers
{
    public class RegisterController : Controller
    {
        [HttpPost]
        public async Task<JsonResult> Index(string email, string password, string fullname)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                var failed = new { success = false, message = "Email hoặc mật khẩu không được để trống!" };
                return Json(failed);
            }

            var emailLower = email.ToLower();

            var user = await DBContext.Instance.Users
                .FirstOrDefaultAsync(item => emailLower.Equals(item.Email.ToLower()));

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
                Role = Constant.RoleUser,
                Status = Constant.Active,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            DBContext.Instance.Users.Add(newUser);
            await DBContext.Instance.SaveChangesAsync();

            var result = new { success = true, message = "Đăng ký thành công!", page = "Register" };
            return Json(result);
        }
    }
}