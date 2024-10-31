using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ProjectWeb.Models.Entity;
using ProjectWeb.Models.ViewModel;
using ProjectWeb.Utils;

namespace ProjectWeb.Controllers
{
    public class ProductController : Controller
    {
        private DBContext ctx { get; } = DbConnect.instance;

        // GET
        public async Task<ActionResult> Index(int? page, int? cate, decimal? min,
            decimal? max, string search, string sort = "ASC")
        {
            var listCategory = await ctx.Categories
                .Include(item => item.Products)
                .Where(item => Constant.ACTIVE.Equals(item.Status))
                .ToListAsync();

            var products = await ctx.Products
                .Include(item => item.Category)
                .Where(item => Constant.ACTIVE.Equals(item.Status) && Constant.ACTIVE.Equals(item.Category.Status))
                .ToListAsync();
            var cateId = cate ?? 0;
            var minPrice = min ?? 0;
            var maxPrice = max ?? decimal.MaxValue;

            var totalProduct = products.Count;
            search = search?.Trim().ToLower();

            products = products
                .Where(item => item.Discount >= minPrice && item.Discount <= maxPrice)
                .Where(item => string.IsNullOrEmpty(search) || item.ProductName.ToLower().Contains(search))
                .ToList();

            if (cateId != 0)
            {
                products = products
                    .Where(item => item.CategoryId == cateId)
                    .ToList();
            }
            

            switch (sort)
            {
                case "DESC":
                    products = products.OrderByDescending(item => item.ProductName).ToList();
                    break;
                case "pASC":
                    products = products.OrderBy(item => item.Discount).ToList();
                    break;
                case "pDESC":
                    products = products.OrderByDescending(item => item.Discount).ToList();
                    break;
                default:
                    products = products.OrderBy(item => item.ProductName).ToList();
                    break;
            }

            var tempTotalProduct = products.Count;

            var pageSize = 9;
            var pageNumber = (page ?? 1);
            products = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var totalPage = tempTotalProduct % pageSize == 0
                ? tempTotalProduct / pageSize
                : tempTotalProduct / pageSize + 1;
            listCategory.Insert(0, new Category()
            {
                CategoryId = 0,
                CategoryName = "Tất cả"
            });
            var paging = RenderPaging(pageNumber, totalPage, 5, cateId, minPrice, maxPrice);
            var model = new ProductViewModel()
            {
                listCateView = listCategory.Select(item => new CateView()
                {
                    categoryId = item.CategoryId,
                    categoryName = item.CategoryName,
                    totalProduct = item.CategoryId == 0
                        ? totalProduct
                        : item.Products.Count(c => Constant.ACTIVE.Equals(c.Status))
                }).ToList(),
                listProduct = products,
                currentCategory = cateId,
                currentPage = pageNumber,
                totalPage = totalPage,
                paging = paging,
                totalProduct = tempTotalProduct,
                minPrice = minPrice,
                maxPrice = maxPrice,
                sort = sort
            };
            return View(model);
        }

        public MvcHtmlString RenderPaging(int currentPage, int totalPages, int pageWindow = 5, int cateId = 0,
            decimal min = 0, decimal max = decimal.MaxValue)
        {
            var paginationHtml = new StringBuilder();
            paginationHtml.AppendLine("<nav aria-label='Page navigation'>");
            paginationHtml.AppendLine("<ul class='pagination justify-content-center'>");

            // Thêm nút "Previous"
            AppendPreviousButton(paginationHtml, currentPage, cateId, min, max);

            // Hiển thị trang đầu tiên và nút "..."
            AppendStartPages(paginationHtml, currentPage, totalPages, pageWindow, cateId, min, max);

            // Hiển thị các trang liền kề (chính giữa)
            AppendMiddlePages(paginationHtml, currentPage, totalPages, pageWindow, cateId, min, max);

            // Hiển thị trang cuối và nút "..."
            AppendEndPages(paginationHtml, currentPage, totalPages, pageWindow, cateId, min, max);

            // Thêm nút "Next"
            AppendNextButton(paginationHtml, currentPage, totalPages, cateId, min, max);

            paginationHtml.AppendLine("</ul>");
            paginationHtml.AppendLine("</nav>");

            return new MvcHtmlString(paginationHtml.ToString());
        }

        private void AppendPreviousButton(StringBuilder html, int currentPage, int cateId = 0, decimal min = 0,
            decimal max = decimal.MaxValue)
        {
            html.AppendLine(currentPage > 1
                ? $@"
                <li class='page-item'>
                    <a class='page-link' href='?page={currentPage - 1}&cate={cateId}&min={min}&max={max}' aria-label='Previous'>
                        <i class='bi-chevron-double-left small'></i>
                    </a>
                </li>"
                : @"
                <li class='page-item disabled'>
                    <a class='page-link' href='javascript:void(0)' aria-label='Previous'>
                        <i class='bi-chevron-double-left small'></i>
                    </a>
                </li>");
        }

        private void AppendStartPages(StringBuilder html, int currentPage, int totalPages, int pageWindow, int cate = 0,
            decimal min = 0, decimal max = decimal.MaxValue)
        {
            var startPage = Math.Max(1, currentPage - pageWindow / 2);
            if (startPage > 1)
            {
                html.AppendLine($@"
                <li class='page-item'>
                    <a class='page-link' href='?page=1@cate={cate}&min={min}&max={max}'>1</a>
                </li>
                <li class='page-item disabled'>
                    <a class='page-link' href='javascript:void(0)'>...</a>
                </li>");
            }
        }

        private void AppendMiddlePages(StringBuilder html, int currentPage, int totalPages, int pageWindow,
            int cate = 0, decimal min = 0, decimal max = decimal.MaxValue)
        {
            var startPage = Math.Max(1, currentPage - pageWindow / 2);
            var endPage = Math.Min(totalPages, startPage + pageWindow - 1);

            for (var i = startPage; i <= endPage; i++)
            {
                html.AppendLine(i == currentPage
                    ? $@"
                    <li class='page-item active'>
                        <a class='page-link' href='?page={i}&cate={cate}&min={min}&max={max}'>{i}</a>
                    </li>"
                    : $@"
                    <li class='page-item'>
                        <a class='page-link' href='?page={i}&cate={cate}&min={min}&max={max}'>{i}</a>
                    </li>");
            }
        }

        // Trang cuối và "..."
        private void AppendEndPages(StringBuilder html, int currentPage, int totalPages, int pageWindow, int cate = 0,
            decimal min = 0, decimal max = decimal.MaxValue)
        {
            var endPage = Math.Max(totalPages, currentPage + pageWindow / 2);
            if (endPage < totalPages)
            {
                html.AppendLine($@"
                <li class='page-item disabled'>
                    <a class='page-link' href='javascript:void(0)'>...</a>
                </li>
                <li class='page-item'>
                    <a class='page-link' href='?page={totalPages}&cate={cate}&min={min}&max={max}'>{totalPages}</a>
                </li>");
            }
        }

        // Nút "Next"
        private void AppendNextButton(StringBuilder html, int currentPage, int totalPages, int cate = 0,
            decimal min = 0, decimal max = decimal.MaxValue)
        {
            html.AppendLine(currentPage < totalPages
                ? $@"
                <li class='page-item'>
                    <a class='page-link' href='?page={currentPage + 1}&cate={cate}&min={min}&max={max}' aria-label='Next'>
                        <i class='bi-chevron-double-right small'></i>
                    </a>
                </li>"
                : $@"
                <li class='page-item disabled'>
                    <a class='page-link' href='javascript:void(0)' aria-label='Next'>
                        <i class='bi-chevron-double-right small'></i>
                    </a>
                </li>");
        }

        public async Task<ActionResult> Detail(int id)
        {
            var product = await ctx.Products
                .Include(item => item.Category)
                .FirstOrDefaultAsync(item => item.ProductId == id
                                             && Constant.ACTIVE.Equals(item.Status)
                                             && Constant.ACTIVE.Equals(item.Category.Status));
            if (product == null)
            {
                return RedirectToAction("Index");
            }
            
            var relatedProducts = await ctx.Products
                .Include(item => item.Category)
                .Where(item => item.CategoryId == product.CategoryId
                               && item.ProductId != product.ProductId
                               && Constant.ACTIVE.Equals(item.Status)
                               && Constant.ACTIVE.Equals(item.Category.Status))
                .Take(4)
                .ToListAsync();

            

            var model = new ProductDetailViewModel()
            {
                productModel = product,
                listProductRelated = relatedProducts
            };
            return View(model);
        }
    }
}