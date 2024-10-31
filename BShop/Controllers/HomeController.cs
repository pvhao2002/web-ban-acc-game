using System;
using ProjectWeb.Models.Entity;
using ProjectWeb.Models.ViewModel;
using ProjectWeb.Utils;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ProjectWeb.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private DBContext Ctx { get; } = DBContext.Instance;

        public async Task<ActionResult> Index(string act)
        {
            var listTrendProduct = await Ctx.Products
                .Include(item => item.Category)
                .Where(item => Constant.ACTIVE.Equals(item.Status) && Constant.ACTIVE.Equals(item.Category.Status))
                .OrderByDescending(item => item.ProductId)
                .Take(8)
                .ToListAsync();
            var model = new HomeViewModel()
            {
                ListProduct = listTrendProduct
            };
            ViewData[Constant.LOGIN] = act;
            return View(model);
        }

        public PartialViewResult IconBlock()
        {
            return PartialView();
        }

        public PartialViewResult Hero()
        {
            return PartialView();
        }

        public ActionResult IntroduceMinecraft()
        {
            return View();
        }

        public PartialViewResult Client()
        {
            return PartialView();
        }

        public ActionResult ReasonBuyGameOfficial()
        {
            return View();
        }

        public ActionResult Fanpage()
        {
            return View();
        }

        public async Task<ActionResult> About()
        {
            var about = await Ctx.abouts.FirstOrDefaultAsync();
            return View(about ?? new about());
        }

        public ActionResult UnAuthorized()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            foreach (var cookie in Request.Cookies.AllKeys)
            {
                Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}