using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Models;

[Table("ShoppingCart")]
public partial class ShoppingCart
{
    [Key]
    [Column("cartID")]
    [StringLength(255)]
    [Unicode(false)]
    public string CartId { get; set; } = null!;

    [Column("userID")]
    [StringLength(255)]
    [Unicode(false)]
    public string? UserId { get; set; }

    [Column("totalAmount", TypeName = "decimal(10, 2)")]
    public decimal? TotalAmount { get; set; }

    [InverseProperty("Cart")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [ForeignKey("UserId")]
    [InverseProperty("ShoppingCarts")]
    public virtual User? User { get; set; }
}
