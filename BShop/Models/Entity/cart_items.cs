using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectWeb.Models.Entity
{
    [Table("cart_items")]
    public sealed class CartItems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("cart_item_id")]
        public int CartItemId { get; set; }

        [Column("cart_id")] public int? CartId { get; set; }

        [Column("product_id")] public int? ProductId { get; set; }

        [Column("quantity")] public int? Quantity { get; set; }

        [Column("total_price", TypeName = "decimal")]
        public decimal? TotalPrice { get; set; }

        [Column("created_at", TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at", TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        public Cart Cart { get; set; }

        public Product Product { get; set; }
    }
}