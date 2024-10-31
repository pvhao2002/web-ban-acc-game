using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BShop.Models.Entity
{
    [Table("carts")]
    public sealed class Cart
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cart()
        {
            CartItems = new HashSet<CartItems>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("cart_id")]
        public int CartId { get; set; }

        [Column("user_id")]
        public int? UserId { get; set; }
        
        [Column("total_price", TypeName = "decimal")]
        public decimal? TotalPrice { get; set; }

        [Column("total_quantity")]
        public int? TotalQuantity { get; set; }

        [Column("created_at", TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at", TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<CartItems> CartItems { get; set; }

        public User User { get; set; }
    }
}
