using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BShop.Models.Entity;
using BShop.Utils;

namespace BShop.Controllers
{
    [CustomAuthorize(Constant.RoleUser)]
    public class CartController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var userId = AuthenticationUtil.GetUserId(Request, Session);
            var cart = await DBContext.Instance.Carts
                .Include(item => item.CartItems)
                .Include(item => item.CartItems.Select(cItem => cItem.Product))
                .FirstOrDefaultAsync(item => item.UserId == userId);
            return View(cart ?? new Cart());
        }

        public async Task<ActionResult> Add(int id)
        {
            var success = true;
            var p = await DBContext.Instance.Products
                .Include(item => item.Category)
                .FirstOrDefaultAsync(item => item.ProductId == id
                                             && Constant.Active.Equals(item.Status)
                                             && Constant.Active.Equals(item.Category.Status));

            if (p == null || Constant.ProductPending.Equals(p.Status))
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Sản phẩm không tồn tại!";
                return RedirectToAction("Index", "Product");
            }

            var userId = AuthenticationUtil.GetUserId(Request, Session);
            var cart = await DBContext.Instance.Carts
                .FirstOrDefaultAsync(item => item.UserId == userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    TotalPrice = p.Discount,
                    TotalQuantity = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                var cartItem = new CartItems
                {
                    ProductId = p.ProductId,
                    Quantity = 1,
                    TotalPrice = p.Discount,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Cart = cart,
                    Product = p
                };
                cart.CartItems.Add(cartItem);
                DBContext.Instance.Carts.Add(cart);
            }
            else
            {
                // check if product already in cart then skip
                var cartItem = await DBContext.Instance.CartItems
                    .FirstOrDefaultAsync(item => item.CartId == cart.CartId
                                                 && item.ProductId == p.ProductId);
                if (cartItem == null)
                {
                    var cartItemNew = new CartItems
                    {
                        CartId = cart.CartId,
                        ProductId = p.ProductId,
                        Quantity = 1,
                        TotalPrice = p.Discount,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Cart = cart,
                        Product = p
                    };
                    cart.CartItems.Add(cartItemNew);
                    cart.TotalPrice += p.Discount;
                    cart.TotalQuantity += 1;
                    cart.UpdatedAt = DateTime.Now;
                }
                else
                {
                    success = false;
                    TempData[Constant.StatusRs] = Constant.Error;
                    TempData[Constant.MessageRs] = "Sản phẩm đã tồn tại trong giỏ hàng!";
                }

                // check if you have product have status pending then remove it
                await RemoveProductPending(cart);
            }

            await DBContext.Instance.SaveChangesAsync();
            if (!success) return RedirectToAction("Index", "Product");

            TempData[Constant.StatusRs] = Constant.Success;
            TempData[Constant.MessageRs] = "Thêm sản phẩm vào giỏ hàng thành công!";
            return RedirectToAction("Index", "Product");
        }

        private async Task RemoveProductPending(Cart c)
        {
            var cartItems = await DBContext.Instance.CartItems
                .Where(item => item.CartId == c.CartId)
                .ToListAsync();

            foreach (var item in cartItems)
            {
                var product = await DBContext.Instance.Products
                    .FirstOrDefaultAsync(pItem => pItem.ProductId == item.ProductId);
                if (product != null && !Constant.ProductPending.Equals(product.Status)) continue;
                c.TotalPrice -= item.TotalPrice;
                c.TotalQuantity -= item.Quantity;
                DBContext.Instance.CartItems.Remove(item);
            }

            await DBContext.Instance.SaveChangesAsync();
        }

        public async Task<ActionResult> Update(int cid, int pid, int quantity)
        {
            var cart = await DBContext.Instance.Carts
                .Include(item => item.CartItems)
                .FirstOrDefaultAsync(item => item.CartId == cid);

            if (cart == null)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Giỏ hàng không tồn tại!";
                return RedirectToAction("Index", "Product");
            }

            var cartItem = await DBContext.Instance.CartItems
                .Include(item => item.Product)
                .FirstOrDefaultAsync(item => item.CartId == cid && item.ProductId == pid);

            if (cartItem == null)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Sản phẩm không tồn tại trong giỏ hàng!";
                return RedirectToAction("Index", "Cart");
            }

            if (Constant.ProductPending.Equals(cartItem.Product.Status))
            {
                // remove product pending from cart
                cart.TotalPrice -= cartItem.TotalPrice;
                cart.TotalQuantity -= cartItem.Quantity;
                DBContext.Instance.CartItems.Remove(cartItem);
            }
            else
            {
                if (cartItem.Quantity + quantity <= 0)
                {
                    TempData[Constant.StatusRs] = Constant.Error;
                    TempData[Constant.MessageRs] = "Số lượng sản phẩm phải lớn hơn 0!";
                    return RedirectToAction("Index", "Cart");
                }

                cartItem.Quantity += quantity;
                cartItem.TotalPrice = cartItem.Product.Discount * quantity;
                cartItem.UpdatedAt = DateTime.Now;
                cart.TotalPrice += quantity < 0
                    ? -cartItem.Product.Discount
                    : cartItem.Product.Discount;
            }

            cart.UpdatedAt = DateTime.Now;
            await DBContext.Instance.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Remove(int cid, int pid)
        {
            var cart = await DBContext.Instance.Carts
                .Include(item => item.CartItems)
                .FirstOrDefaultAsync(item => item.CartId == cid);

            if (cart == null)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Giỏ hàng không tồn tại!";
                return RedirectToAction("Index", "Product");
            }

            var cartItem = await DBContext.Instance.CartItems
                .FirstOrDefaultAsync(item => item.CartId == cid && item.ProductId == pid);

            if (cartItem == null)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Sản phẩm không tồn tại trong giỏ hàng!";
                return RedirectToAction("Index", "Cart");
            }

            cart.TotalPrice -= cartItem.TotalPrice;
            cart.TotalQuantity -= cartItem.Quantity;
            DBContext.Instance.CartItems.Remove(cartItem);
            cart.UpdatedAt = DateTime.Now;
            await DBContext.Instance.SaveChangesAsync();
            TempData[Constant.StatusRs] = Constant.Success;
            TempData[Constant.MessageRs] = "Xóa sản phẩm khỏi giỏ hàng thành công!";
            return RedirectToAction("Index", "Cart");
        }
    }
}