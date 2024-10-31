using System.Collections.Generic;
using System.Web.Mvc;
using ProjectWeb.Models.Entity;

namespace ProjectWeb.Models.ViewModel
{
    public class ProductViewModel
    {
        public Product product { get; set; } = new Product();
        public ICollection<Product> listProduct { get; set; } = new List<Product>();
        public ICollection<Category> listCategory { get; set; } = new List<Category>();
        public ICollection<CateView> listCateView { get; set; } = new List<CateView>();
        public ICollection<SelectListItem> categorySelectList { get; set; } = new List<SelectListItem>();
        public MvcHtmlString paging { get; set; }

        public int currentPage { get; set; } = 0;
        public int totalPage { get; set; } = 0;
        public int currentCategory { get; set; } = 0;
        public int totalProduct { get; set; } = 0;
        public decimal minPrice { get; set; } = 0;
        public decimal maxPrice { get; set; } = 0;
        
        public string sort { get; set; } = "ASC";
    }

    public class CateView
    {
        public int categoryId { get; set; }
        public int totalProduct { get; set; }
        public string categoryName { get; set; }
    }
}