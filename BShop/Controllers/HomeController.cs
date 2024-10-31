using System;
using BShop.Models.Entity;
using BShop.Models.ViewModel;
using BShop.Utils;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BShop.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index(string act)
        {
            var listTrendProduct = await DBContext.Instance.Products
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

        public PartialViewResult Client()
        {
            return PartialView();
        }

        public ActionResult Fanpage()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
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