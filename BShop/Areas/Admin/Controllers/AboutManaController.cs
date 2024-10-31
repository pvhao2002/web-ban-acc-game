using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ProjectWeb.Models.Entity;
using ProjectWeb.Utils;

namespace ProjectWeb.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Constant.ROLE_ADMIN)]
    public class AboutManaController : Controller
    {
        private DBContext ctx { get; } = DbConnect.instance;

        // GET
        public async Task<ActionResult> Index()
        {
            var about = await ctx.abouts.FirstOrDefaultAsync();
            return View(about ?? new about());
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> Edit(FormCollection form)
        {
            var about = await ctx.abouts.FirstOrDefaultAsync();
            var content = form["content"];
            if (about == null)
            {
                var newItem = new about
                {
                    content = content
                };
                ctx.abouts.Add(newItem);
            }
            else
            {
                about.content = content;
            }

            await ctx.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}