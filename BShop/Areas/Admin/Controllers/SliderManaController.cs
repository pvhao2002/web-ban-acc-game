using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using ProjectWeb.Models.Entity;
using ProjectWeb.Utils;

namespace ProjectWeb.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Constant.ROLE_ADMIN)]
    public class SliderManaController : Controller
    {
        private DBContext ctx { get; } = DbConnect.instance;

        // GET
        public async Task<ActionResult> Index()
        {
            var sliders = await ctx.sliders.OrderBy(item => item.slider_id).ToListAsync();
            return View(sliders);
        }

        public ActionResult Add()
        {
            return View(new slider());
        }

        public async Task<ActionResult> Edit(int id)
        {
            var slider = await ctx.sliders.FirstOrDefaultAsync(item => item.slider_id == id);

            if (slider != null) return View(slider);

            TempData[Constant.STATUS_RS] = Constant.ERROR;
            TempData[Constant.MESSAGE_RS] = "Không tìm thấy slider";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> DoAdd(HttpPostedFileBase[] imgs)
        {
            if (!ModelState.IsValid)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Thêm slider thất bại";
            }
            else
            {
                if (imgs.Length == 0)
                {
                    TempData[Constant.STATUS_RS] = Constant.ERROR;
                    TempData[Constant.MESSAGE_RS] = "Thêm slider thất bại";
                    return RedirectToAction("Add");
                }

                var tasks = imgs
                    .Where(img => img.ContentLength > 0)
                    .Select(async img =>
                    {
                        var slider = new slider();
                        var fileName = Path.GetFileName(img.FileName);
                        var path = Path.Combine(Server.MapPath(Constant.PATH_IMAGE), fileName);
                        img.SaveAs(path);
                        slider.slider_img = Constant.PATH_IMAGE + "/" + fileName;
                        var lastSlider = await ctx.sliders
                            .OrderByDescending(item => item.slider_id)
                            .FirstOrDefaultAsync();
                        slider.position = lastSlider == null ? 1 : lastSlider.position + 1;
                        ctx.sliders.Add(slider);
                    });

                await Task.WhenAll(tasks);
                await ctx.SaveChangesAsync();
                TempData[Constant.STATUS_RS] = Constant.SUCCESS;
                TempData[Constant.MESSAGE_RS] = "Thêm slider thành công";
            }

            return RedirectToAction("Add");
        }

        [HttpPost]
        public async Task<ActionResult> DoUpdate(HttpPostedFileBase img, slider slider)
        {
            var sliderUpdate = await ctx.sliders.FirstOrDefaultAsync(item => item.slider_id == slider.slider_id);
            if (sliderUpdate == null)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Slider không tồn tại";
                return RedirectToAction("Index");
            }

            if (img.ContentLength <= 0)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Ảnh bị lỗi";
                return RedirectToAction("Index");
            }
            
            var fileName = Path.GetFileName(img.FileName);
            var path = Path.Combine(Server.MapPath(Constant.PATH_IMAGE), fileName);
            img.SaveAs(path);
            
            sliderUpdate.slider_img = Constant.PATH_IMAGE + "/" + fileName;
            await ctx.SaveChangesAsync();
            TempData[Constant.STATUS_RS] = Constant.SUCCESS;
            TempData[Constant.MESSAGE_RS] = "Cập nhật slider thành công";

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Up(int id)
        {
            var slider = await ctx.sliders.FirstOrDefaultAsync(item => item.slider_id == id);
            if (slider == null)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Slider không tồn tại";
                return RedirectToAction("Index");
            }

            if (slider.position == 1)
            {
                var lastSlider = await ctx.sliders
                    .OrderByDescending(item => item.position)
                    .FirstOrDefaultAsync();
                if (lastSlider != null)
                {
                    (lastSlider.slider_img, slider.slider_img) = (slider.slider_img, lastSlider.slider_img);
                }
            }
            else
            {
                var preSlider = await ctx.sliders
                    .FirstOrDefaultAsync(item => item.position == slider.position - 1);
                if (preSlider != null)
                {
                    (preSlider.slider_img, slider.slider_img) = (slider.slider_img, preSlider.slider_img);
                }
            }

            await ctx.SaveChangesAsync();
            TempData[Constant.STATUS_RS] = Constant.SUCCESS;
            TempData[Constant.MESSAGE_RS] = "Thay đổi vị trí thành công";
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Down(int id)
        {
            var slider = await ctx.sliders.FirstOrDefaultAsync(item => item.slider_id == id);
            if (slider == null)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Slider không tồn tại";
                return RedirectToAction("Index");
            }

            var lastSlider = await ctx.sliders
                .OrderByDescending(item => item.slider_id)
                .FirstOrDefaultAsync();

            if (slider.position == lastSlider?.position)
            {
                var firstSlider = await ctx.sliders
                    .OrderBy(item => item.position)
                    .FirstOrDefaultAsync();
                if (firstSlider != null)
                {
                    (firstSlider.slider_img, slider.slider_img) = (slider.slider_img, firstSlider.slider_img);
                }
            }
            else
            {
                var preSlider = await ctx.sliders
                    .FirstOrDefaultAsync(item => item.position == slider.position + 1);
                if (preSlider != null)
                {
                    (preSlider.slider_img, slider.slider_img) = (slider.slider_img, preSlider.slider_img);
                }
            }

            await ctx.SaveChangesAsync();
            TempData[Constant.STATUS_RS] = Constant.SUCCESS;
            TempData[Constant.MESSAGE_RS] = "Thay đổi vị trí thành công";
            return RedirectToAction("Index");
        }
        
        public async Task<ActionResult> Delete(int id)
        {
            var slider = await ctx.sliders.FirstOrDefaultAsync(item => item.slider_id == id);
            if (slider == null)
            {
                TempData[Constant.STATUS_RS] = Constant.ERROR;
                TempData[Constant.MESSAGE_RS] = "Slider không tồn tại";
                return RedirectToAction("Index");
            }

            ctx.sliders.Remove(slider);
            
            var path = Server.MapPath(slider.slider_img);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            
            // Update position
            var sliders = await ctx.sliders
                .Where(item => item.position > slider.position)
                .ToListAsync();
            
            sliders.ForEach(item => item.position--);
            await ctx.SaveChangesAsync();
            TempData[Constant.STATUS_RS] = Constant.SUCCESS;
            TempData[Constant.MESSAGE_RS] = "Xóa slider thành công";
            return RedirectToAction("Index");
        }
    }
}