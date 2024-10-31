using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using BShop.Models.Entity;

namespace BShop.Utils
{
    public static class MailUtils
    {
        public static async Task<string> SendEmail(string to, string subject, string body)
        {
            try
            {
                var from = "";
                var password = "";
                
                var message = new MailMessage(from, to, subject, body)
                {
                    BodyEncoding = System.Text.Encoding.UTF8,
                    SubjectEncoding = System.Text.Encoding.UTF8,
                    IsBodyHtml = true,
                };

                message.ReplyToList.Add(new MailAddress(from));
                message.Sender = new MailAddress(from);

                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Credentials = new NetworkCredential(from, password);
                    client.EnableSsl = true;
                    await client.SendMailAsync(message);
                }

                return Constant.SUCCESS;
            }
            catch (SmtpFailedRecipientException)
            {
                // Xử lý lỗi cho email không gửi được tới người nhận cụ thể
                return "Email không tồn tại!";
            }
            catch (Exception)
            {
                // Xử lý lỗi chung
                return "Hệ thống lỗi. Vui lòng thử lại sau!";
            }
        }

        public static string BuildBody(Order o)
        {
            var tbody = string.Empty;
            o.OrderItems.ForEach(item =>
            {
                tbody += $@"
                <tr>
                    <td>{item.product.ProductName}</td>
                    <td><img src='{item.product.ProductImage}' alt='{item.product.ProductName}' width='100' height='100'></td>
                    <td>{item.product.Price}</td>
                </tr>
            ";
            });
            return $@"
                <h3>Cảm ơn bạn đã mua hàng tại B Shop</h3> 
                <p>Đơn hàng của bạn đã được xác nhận và đang được xử lý. Mã đơn hàng: {o.TxRef}</p> 
                <p>Thông tin đơn hàng:</p>
                <table border='1'>
                   <thead>
                    <tr>
                        <th>Tên sản phẩm</th>
                        <th>Ảnh</th>
                        <th>Giá</th>
                    </tr>
                     </thead>
                <tbody>
                    {tbody}
                </tbody>
                </table>
                <p>Cảm ơn bạn đã mua hàng tại B Shop!</p>
            ";
        }

        public static string BuildBodyFail(Order o)
        {
            return $@"
                <h3>Đơn hàng của bạn đã bị hủy</h3> 
                <p>Đơn hàng của bạn đã bị hủy. Mã đơn hàng: {o.TxRef}</p> 
                <p>Cảm ơn bạn đã mua hàng tại B Shop!</p>";
        }
    }
}