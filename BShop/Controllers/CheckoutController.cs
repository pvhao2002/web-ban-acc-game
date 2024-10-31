using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using BShop.Models.Entity;
using BShop.Utils;

namespace BShop.Controllers
{
    [CustomAuthorize(Constant.ROLE_USER)]
    public class CheckoutController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var userId = AuthenticationUtil.GetUserId(Request, Session);
            var cart = await DBContext.Instance.Carts
                .Include(item => item.CartItems)
                .FirstOrDefaultAsync(item => item.UserId == userId);
            if (cart != null && cart.CartItems.Count != 0) return View(cart);
            TempData[Constant.STATUS_RS] = Constant.ERROR;
            TempData[Constant.MESSAGE_RS] = "Giỏ hàng trống!";
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public async Task<JsonResult> Continue(string email, string phone, string fullName, string paymentMethod)
        {
            var userId = AuthenticationUtil.GetUserId(Request, Session);
            var taxRef = $"{DateTime.Now.Ticks}{userId}";
            var cart = await DBContext.Instance.Carts
                .Include(item => item.CartItems)
                .FirstOrDefaultAsync(item => item.UserId == userId);
            if (cart == null || cart.CartItems.Count == 0)
                return Json(new { success = false, message = "Giỏ hàng trống!" });

            var order = new Order()
            {
                UserId = userId,
                Email = email,
                PhoneNumber = phone,
                FullName = fullName,
                TotalPrice = cart.TotalPrice,
                TotalQuantity = cart.TotalQuantity,
                Status = Constant.ORDER_STATUS_PENDING,
                PaymentMethod = paymentMethod,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                TxRef = taxRef
            };
            foreach (var item in cart.CartItems)
            {
                var orderDetail = new order_items()
                {
                    order = order,
                    product_id = item.ProductId,
                    quantity = item.Quantity,
                    total_price = item.TotalPrice,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };
                order.OrderItems.Add(orderDetail);
            }

            var o = DBContext.Instance.Orders.Add(order);
            DBContext.Instance.Carts.Remove(cart);
            await DBContext.Instance.SaveChangesAsync();

            if (!"VNPAY".Equals(paymentMethod))
                return Json(new { success = true, message = "Thanh toán thành công", isVnpay = false, url = "" });

            const string vnReturner = "https://localhost:44368/Thank/Index";
            const string vnpUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            var gmtPlus7 = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var gmtPlus7CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(o.CreatedAt.Value.ToUniversalTime(), gmtPlus7);


            const string vnpTmnCode = "GLE8YXG4";
            const string vnpHashSecret = "ZCVPMHAELZKRPGTFLWJDPLQVPHBWEKXG";
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.Version);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnpTmnCode);
            vnpay.AddRequestData("vnp_Amount", (Convert.ToInt64(o.TotalPrice) * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", gmtPlus7CreatedAt.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang: {o.OrderId}");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnReturner);
            vnpay.AddRequestData("vnp_TxnRef", $"{taxRef}");
            var gmtPlus7ExpireDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMinutes(15), gmtPlus7);
            vnpay.AddRequestData("vnp_ExpireDate", gmtPlus7ExpireDate.ToString("yyyyMMddHHmmss"));
            var paymentUrl = vnpay.CreateRequestUrl(vnpUrl, vnpHashSecret);
            return Json(new { success = true, message = "Thanh toán thành công", isVnpay = true, url = paymentUrl });
        }
    }
}