using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BShop.Models.Entity;
using BShop.Utils;

namespace BShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Constant.RoleAdmin)]
    public class CategoryManaController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var listCategory = await DBContext.Instance.Categories
                .Where(item => Constant.Active.Equals(item.Status))
                .ToListAsync();
            return View(listCategory);
        }

        public ActionResult Add()
        {
            return View(new Category());
        }

        public async Task<ActionResult> Edit(int id)
        {
            var category = await DBContext.Instance.Categories
                .FirstOrDefaultAsync(item => item.CategoryId == id);
            return View(category);
        }

        [HttpPost]
        public async Task<ActionResult> DoAdd(Category cate)
        {
            if (!ModelState.IsValid)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Thêm danh mục thất bại";
                return View("Add", cate);
            }

            cate.CreatedAt = DateTime.Now;
            cate.UpdatedAt = DateTime.Now;
            cate.Status = Constant.Active;
            DBContext.Instance.Categories.Add(cate);
            await DBContext.Instance.SaveChangesAsync();

            TempData[Constant.StatusRs] = Constant.Success;
            TempData[Constant.MessageRs] = "Thêm danh mục thành công";
            return RedirectToAction("Add");
        }

        [HttpPost]
        public async Task<ActionResult> DoUpdate(Category cate)
        {
            var category = await DBContext.Instance.Categories
                .FirstOrDefaultAsync(item => item.CategoryId == cate.CategoryId);
            if (category == null)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Danh mục không tồn tại";
                return RedirectToAction("Index");
            }

            category.CategoryName = cate.CategoryName;
            category.UpdatedAt = DateTime.Now;
            await DBContext.Instance.SaveChangesAsync();
            TempData[Constant.StatusRs] = Constant.Success;
            TempData[Constant.MessageRs] = "Cập nhật danh mục thành công";
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> Del(int id)
        {
            var category = await DBContext.Instance.Categories
                .FirstOrDefaultAsync(item => item.CategoryId == id);
            if(category == null)
            {
                TempData[Constant.StatusRs] = Constant.Error;
                TempData[Constant.MessageRs] = "Danh mục không tồn tại";
                return RedirectToAction("Index");
            }
            category.Status = Constant.Inactive;
            category.UpdatedAt = DateTime.Now;
            await DBContext.Instance.SaveChangesAsync();
            TempData[Constant.StatusRs] = Constant.Success;
            TempData[Constant.MessageRs] = "Xóa danh mục thành công";
            
            return RedirectToAction("Index");
        }
    }
}