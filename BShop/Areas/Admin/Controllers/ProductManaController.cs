using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BShop.Models.Entity;
using BShop.Models.ViewModel;
using BShop.Utils;

namespace BShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Constant.RoleAdmin)]
    public class ProductManaController : Controller
    {
        // GET
        public async Task<ActionResult> Index()
        {
            var list = await DBContext.Instance.Products
                .Include(item => item.Category)
                .Where(item => Constant.Active.Equals(item.Category.Status) && Constant.Active.Equals(item.Status))
                .ToListAsync();
            return View(list);
        }

        public async Task<ActionResult> Add()
        {
            var listCategory = await DBContext.Instance.Categories
                .Where(item => Constant.Active.Equals(item.Status))
                .ToListAsync();
            var categorySelectList = listCategory.Select(item => new SelectListItem()
            {
                Text = item.CategoryName,
                Value = item.CategoryId.ToString()
            }).ToList();

            var model = new ProductViewModel()
            {
                product = new Product() { Price = Decimal.Zero, Discount = decimal.Zero },
                categorySelectList = categorySelectList
            };
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var product = await DBContext.Instance.Products
                .FirstOrDefaultAsync(item => item.ProductId == id);
            var listCategory = await DBContext.Instance.Categories
                .Where(item => Constant.Active.Equals(item.Status))
                .ToListAsync();
            var categorySelectList = listCategory.Select(item => new SelectListItem()
            {
                Text = item.CategoryName,
                Value = item.CategoryId.ToString()
            }).ToList();
            var model = new ProductViewModel()
            {
                product = product,
                categorySelectList = categorySelectList
            };
            return View(model);
        }


        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> DoAdd(ProductViewModel productViewModel, HttpPostedFileBase img)
        {
            if (!ModelState.IsValid)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Thêm sản phẩm thất bại";
                return View("Add", productViewModel);
            }

            var product = productViewModel.product;

            if (decimal.Compare(product.Price ?? decimal.Zero, product.Discount ?? decimal.Zero) < 0)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Giá khuyến mãi phải nhỏ hơn giá gốc";
                return View("Add", productViewModel);
            }

            if (img != null && img.ContentLength > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await img.InputStream.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();
                    var base64String = Convert.ToBase64String(imageBytes);
                    var mimeType = img.ContentType;
                    product.ProductImage = $"data:{mimeType};base64,{base64String}";
                }
            }

            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            product.Status = Constant.Active;

            DBContext.Instance.Products.Add(product);
            using (var trans = DBContext.Instance.Database.BeginTransaction())
            {
                try
                {
                    await DBContext.Instance.SaveChangesAsync();
                    trans.Commit();
                    TempData[Constant.StatusRs] = Constant.Success;
                    TempData[Constant.MessageRs] = "Thêm sản phẩm thành công";
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    TempData[Constant.StatusRs] = Constant.Error;
                    TempData[Constant.MessageRs] = "Thêm sản phẩm thất bại: " + e.Message;
                    Console.WriteLine(e);
                }
            }

            return RedirectToAction("Add");
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> DoUpdate(ProductViewModel productViewModel, HttpPostedFileBase img)
        {
            var p = productViewModel.product;
            var product = await DBContext.Instance.Products.FirstOrDefaultAsync(item => item.ProductId == p.ProductId);

            if (product == null)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Sản phẩm không tồn tại";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Cập nhật sản phẩm thất bại";
                return RedirectToAction("Index");
            }

            if (decimal.Compare(p.Price ?? decimal.Zero, p.Discount ?? decimal.Zero) < 0)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Giá khuyến mãi phải nhỏ hơn giá gốc";
                return RedirectToAction("Edit", new { id = p.ProductId });
            }

            if (img != null && img.ContentLength > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await img.InputStream.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();
                    var base64String = Convert.ToBase64String(imageBytes);
                    var mimeType = img.ContentType;
                    product.ProductImage = $"data:{mimeType};base64,{base64String}";
                }
            }

            product.ProductName = p.ProductName;
            product.Price = p.Price;
            product.Discount = p.Discount;
            product.Description = p.Description;
            product.CategoryId = p.CategoryId;
            product.UpdatedAt = DateTime.Now;

            await DBContext.Instance.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public async Task<ActionResult> Delete(int id)
        {
            var product = await DBContext.Instance.Products.FirstOrDefaultAsync(item => item.ProductId == id);
            if (product == null)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Sản phẩm không tồn tại";
                return RedirectToAction("Index");
            }

            product.Status = Constant.Inactive;
            product.UpdatedAt = DateTime.Now;
            await DBContext.Instance.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}