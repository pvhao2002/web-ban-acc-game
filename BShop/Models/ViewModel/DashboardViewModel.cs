using System.Collections.Generic;

namespace BShop.Models.ViewModel
{
    public class DashboardViewModel
    {
        public int totalCustomer { get; set; }
        public int totalProduct { get; set; }
        public decimal? totalRevenue { get; set; }
        public decimal? totalRevenueMonth { get; set; }
        public decimal? percentRevenueGrowth { get; set; }
        public decimal? totalRevenueLastMonth { get; set; }
        

        public decimal? totalRevenueWeek { get; set; }
        public decimal? totalRevenuePrevWeek { get; set; }
        public decimal? totalRevenueDay { get; set; }

        public ICollection<CategoryRevenue> categoryRevenues { get; set; } = new List<CategoryRevenue>();
    }

    public class CategoryRevenue
    {
        public string category_name { get; set; }
        public decimal revenue { get; set; }

        public CategoryRevenue()
        {
        }
    }
}