using System.Collections.Generic;
using BShop.Models.Entity;

namespace BShop.Models.ViewModel
{
    public class ProductDetailViewModel
    {
        public Product productModel { get; set; }
        public ICollection<Product> listProductRelated { get; set; } = new List<Product>();
    }
}