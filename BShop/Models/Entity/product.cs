using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ProjectWeb.Models.Entity
{
    [Table("product")]
    public sealed class Product
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            CartItems = new HashSet<CartItems>();
            OrderItems = new HashSet<order_items>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("product_id")]
        public int ProductId { get; set; }

        [StringLength(255)]
        [Column("product_name")]
        public string ProductName { get; set; }

        [Column("price", TypeName = "decimal")]
        public decimal? Price { get; set; }

        [Column("discount", TypeName = "decimal")]
        public decimal? Discount { get; set; }

        [StringLength(int.MaxValue)]
        [Column("description")]
        public string Description { get; set; }

        [StringLength(int.MaxValue)]
        [Column("product_image")]
        public string ProductImage { get; set; }

        [Column("category_id")] public int? CategoryId { get; set; }

        [StringLength(50)] [Column("status")] public string Status { get; set; }

        [Column("created_at", TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }


        [Column("updated_at", TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<CartItems> CartItems { get; set; }

        public Category Category { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<order_items> OrderItems { get; set; }
    }
}