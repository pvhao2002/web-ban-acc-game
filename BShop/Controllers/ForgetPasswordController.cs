using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using BShop.Models.Entity;
using BShop.Utils;

namespace BShop.Controllers
{
    public class ForgetPasswordController : Controller
    {
        [HttpPost]
        public async Task<JsonResult> Index(string email)
        {
            var user = await DBContext.Instance.Users.FirstOrDefaultAsync(item => email.Equals(item.Email));

            if (user == null)
            {
                var failed = new { success = false, message = "Email chưa đăng ký thành viên!" };
                return Json(failed);
            }

            if (Constant.BLOCK.Equals(user.Status))
            {
                var failed = new { success = false, message = "Tài khoản đã bị khóa!" };
                return Json(failed);
            }

            var newPassword = Guid.NewGuid().ToString().Substring(0, 8);

            var body = "Mật khẩu mới của bạn là: " + newPassword;
            const string subject = "Quên mật khẩu";
            var success = await MailUtils.SendEmail(email, subject, body);

            if (Constant.SUCCESS.Equals(success))
            {
                user.Password = newPassword;
                user.UpdatedAt = DateTime.Now;
                await DBContext.Instance.SaveChangesAsync();
            }
            
            var mess = Constant.SUCCESS.Equals(success)  ? "Mật khẩu mới đã được gửi vào email của bạn!" : success;

            var result = new
            {
                success = Constant.SUCCESS.Equals(success),
                message = mess,
                page = "Forget"
            };
            return Json(result);
        }
    }
}