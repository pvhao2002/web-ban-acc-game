using System.Collections.Generic;
using ProjectWeb.Models.Entity;

namespace ProjectWeb.Models.ViewModel
{
    public class ProductDetailViewModel
    {
        public Product productModel { get; set; }
        public ICollection<Product> listProductRelated { get; set; } = new List<Product>();
    }
}