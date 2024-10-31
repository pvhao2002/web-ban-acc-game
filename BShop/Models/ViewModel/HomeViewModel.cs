using BShop.Models.Entity;
using System;
using System.Collections.Generic;

namespace BShop.Models.ViewModel
{
    public class HomeViewModel
    {
        public ICollection<Product> ListProduct { get; set; } = new List<Product>();
    }
}