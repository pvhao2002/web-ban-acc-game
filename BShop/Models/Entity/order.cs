using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BShop.Models.Entity
{
    [Table("orders")]
    public sealed class Order
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            OrderItems = new HashSet<OrderItems>();
        }

        [Key]
        [Column("order_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Column("user_id")] public int? UserId { get; set; }

        [StringLength(255)] [Column("email")] public string Email { get; set; }

        [StringLength(20)]
        [Column("phone_number")]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        [Column("full_name")]
        public string FullName { get; set; }

        [Column("total_price", TypeName = "decimal")]
        public decimal? TotalPrice { get; set; }

        [Column("total_quantity", TypeName = "decimal")]
        public int? TotalQuantity { get; set; }

        [Column("status")] [StringLength(50)] public string Status { get; set; }

        [Column("payment_method")]
        [StringLength(100)]
        public string PaymentMethod { get; set; }

        [Column("tx_ref")] [StringLength(500)] public string TxRef { get; set; }

        [Column("created_at", TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at", TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<OrderItems> OrderItems { get; set; }

        public User User { get; set; }
    }
}