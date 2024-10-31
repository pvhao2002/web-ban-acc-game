using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BShop.Models.Entity
{
    [Table("order_items")]
    public sealed class OrderItems
    {
        [Key]
        public int OrderItemId { get; set; }

        public int? OrderId { get; set; }

        public int? ProductId { get; set; }

        public int? quantity { get; set; }

        public decimal? TotalPrice { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Order order { get; set; }

        public Product product { get; set; }
    }
}
