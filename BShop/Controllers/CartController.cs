using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ProjectWeb.Models.Entity;
using ProjectWeb.Utils;

namespace ProjectWeb.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Constant.ROLE_USER)]
    public class CartController : Controller
    {
        private DBContext ctx { get; } = DbConnect.instance;

        // GET
        public async Task<ActionResult> Index()
        {
            var userId = AuthenticationUtil.GetUserId(Request, Session);
            var cart = await ctx.Carts
                .Include(item => item.CartItems)
                .Include(item => item.CartItems.Select(cItem => cItem.Product))
                .FirstOrDefaultAsync(item => item.UserId == userId);
            return View(cart ?? new Cart());
        }

        public async Task<ActionResult> Add(int id)
        {
            var success = true;
            var p = await ctx.Products
                .Include(item => item.Category)
                .FirstOrDefaultAsync(item => item.ProductId == id
                                             && Constant.ACTIVE.Equals(item.Status)
                                             && Constant.ACTIVE.Equals(item.Category.Status));

            if (p == null || Constant.PRODUCT_PENDING.Equals(p.Status))
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Sản phẩm không tồn tại!";
                return RedirectToAction("Index", "Product");
            }

            var userId = AuthenticationUtil.GetUserId(Request, Session);
            var cart = await ctx.Carts
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
                ctx.Carts.Add(cart);
            }
            else
            {
                // check if product already in cart then skip
                var cartItem = await ctx.CartItems
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
                    TempData[Constant.STATUS_RS] = Constant.ERROR;
                    TempData[Constant.MESSAGE_RS] = "Sản phẩm đã tồn tại trong giỏ hàng!";
                }

                // check if you have product have status pending then remove it
                await RemoveProductPending(cart);
            }

            await ctx.SaveChangesAsync();
            if (!success) return RedirectToAction("Index", "Product");

            TempData[Constant.STATUS_RS] = Constant.SUCCESS;
            TempData[Constant.MESSAGE_RS] = "Thêm sản phẩm vào giỏ hàng thành công!";
            return RedirectToAction("Index", "Product");
        }

        private async Task RemoveProductPending(Cart c)
        {
            var cartItems = await ctx.CartItems
                .Where(item => item.CartId == c.CartId)
                .ToListAsync();

            foreach (var item in cartItems)
            {
                var product = await ctx.Products
                    .FirstOrDefaultAsync(pItem => pItem.ProductId == item.ProductId);
                if (product != null && !Constant.PRODUCT_PENDING.Equals(product.Status)) continue;
                c.TotalPrice -= item.TotalPrice;
                c.TotalQuantity -= item.Quantity;
                ctx.CartItems.Remove(item);
            }

            await ctx.SaveChangesAsync();
        }

        public async Task<ActionResult> Update(int cid, int pid, int quantity)
        {
            var cart = await ctx.Carts
                .Include(item => item.CartItems)
                .FirstOrDefaultAsync(item => item.CartId == cid);

            if (cart == null)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Giỏ hàng không tồn tại!";
                return RedirectToAction("Index", "Product");
            }

            var cartItem = await ctx.CartItems
                .Include(item => item.Product)
                .FirstOrDefaultAsync(item => item.CartId == cid && item.ProductId == pid);

            if (cartItem == null)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Sản phẩm không tồn tại trong giỏ hàng!";
                return RedirectToAction("Index", "Cart");
            }

            if (Constant.PRODUCT_PENDING.Equals(cartItem.Product.Status))
            {
                // remove product pending from cart
                cart.TotalPrice -= cartItem.TotalPrice;
                cart.TotalQuantity -= cartItem.Quantity;
                ctx.CartItems.Remove(cartItem);
            }
            else
            {
                if (cartItem.Quantity + quantity <= 0)
                {
                    TempData[Constant.STATUS_RS] = Constant.ERROR;
                    TempData[Constant.MESSAGE_RS] = "Số lượng sản phẩm phải lớn hơn 0!";
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
            await ctx.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Remove(int cid, int pid)
        {
            var cart = await ctx.Carts
                .Include(item => item.CartItems)
                .FirstOrDefaultAsync(item => item.CartId == cid);

            if (cart == null)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Giỏ hàng không tồn tại!";
                return RedirectToAction("Index", "Product");
            }

            var cartItem = await ctx.CartItems
                .FirstOrDefaultAsync(item => item.CartId == cid && item.ProductId == pid);

            if (cartItem == null)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Sản phẩm không tồn tại trong giỏ hàng!";
                return RedirectToAction("Index", "Cart");
            }

            cart.TotalPrice -= cartItem.TotalPrice;
            cart.TotalQuantity -= cartItem.Quantity;
            ctx.CartItems.Remove(cartItem);
            cart.UpdatedAt = DateTime.Now;
            await ctx.SaveChangesAsync();
            TempData[Constant.STATUS_RS] = Constant.SUCCESS;
            TempData[Constant.MESSAGE_RS] = "Xóa sản phẩm khỏi giỏ hàng thành công!";
            return RedirectToAction("Index", "Cart");
        }
    }
}