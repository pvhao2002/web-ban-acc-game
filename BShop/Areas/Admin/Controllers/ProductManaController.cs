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
    [CustomAuthorize(Constant.ROLE_ADMIN)]
    public class ProductManaController : Controller
    {
        // GET
        public async Task<ActionResult>  Index()
        {
            var list = await DBContext.Instance.Products
                .Include(item => item.Category)
                .Where(item => Constant.ACTIVE.Equals(item.Category.Status) && Constant.ACTIVE.Equals(item.Status))
                .ToListAsync();
            return View(list);
        }

        public async Task<ActionResult> SaleAccount()
        {
            var list = await DBContext.Instance.Products
                .Include(item => item.Category)
                .Where(item => Constant.INACTIVE.Equals(item.Status))
                .ToListAsync();
            return View(list);
        }

        public async Task<ActionResult> Add()
        {
            var listCategory = await DBContext.Instance.Categories.Where(item => Constant.ACTIVE.Equals(item.Status)).ToListAsync();
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
            var product = await DBContext.Instance.Products.FirstOrDefaultAsync(item => item.ProductId == id);
            var listCategory = await DBContext.Instance.Categories.Where(item => Constant.ACTIVE.Equals(item.Status)).ToListAsync();
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
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Thêm sản phẩm thất bại";
                return RedirectToAction("Index");
            }

            var product = productViewModel.product;

            if (decimal.Compare(product.Price ?? decimal.Zero, product.Discount ?? decimal.Zero) < 0)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Giá khuyến mãi phải nhỏ hơn giá gốc";
                return RedirectToAction("Add");
            }

            if (img.ContentLength > 0)
            {
                var fileName = Path.GetFileName(img.FileName);
                var path = Path.Combine(Server.MapPath(Constant.PATH_IMAGE), fileName);
                img.SaveAs(path);
                product.ProductImage = Constant.PATH_IMAGE + "/" + fileName;
            }
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            product.Status = Constant.ACTIVE;

            DBContext.Instance.Products.Add(product);
            using (var trans = DBContext.Instance.Database.BeginTransaction())
            {
                try
                {
                    await DBContext.Instance.SaveChangesAsync();
                    trans.Commit();
                    TempData[Constant.STATUS_RS] = Constant.SUCCESS;
                    TempData[Constant.MESSAGE_RS] = "Thêm sản phẩm thành công";
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    TempData[Constant.STATUS_RS] = Constant.ERROR;
                    TempData[Constant.MESSAGE_RS] = "Thêm sản phẩm thất bại: " + e.Message;
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
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Sản phẩm không tồn tại";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Cập nhật sản phẩm thất bại";
                return RedirectToAction("Index");
            }

            if (decimal.Compare(p.Price ?? decimal.Zero, p.Discount ?? decimal.Zero) < 0)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Giá khuyến mãi phải nhỏ hơn giá gốc";
                return RedirectToAction("Edit", new { id = p.ProductId });
            }

            if (img.ContentLength > 0)
            {
                var fileName = Path.GetFileName(img.FileName);
                var path = Path.Combine(Server.MapPath(Constant.PATH_IMAGE), fileName);
                img.SaveAs(path);
                product.ProductImage = Constant.PATH_IMAGE + "/" + fileName;
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
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Sản phẩm không tồn tại";
                return RedirectToAction("Index");
            }
            
            product.Status = Constant.INACTIVE;
            product.UpdatedAt = DateTime.Now;
            await DBContext.Instance.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}