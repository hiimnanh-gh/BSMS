using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Models;

[PrimaryKey("CartId", "BookId")]
[Table("CartItem")]
public partial class CartItem
{
    [Key]
    [Column("cartID")]
    [StringLength(255)]
    [Unicode(false)]
    public string CartId { get; set; } = null!;

    [Key]
    [Column("bookID")]
    [StringLength(255)]
    [Unicode(false)]
    public string BookId { get; set; } = null!;

    [Column("quantity")]
    public int? Quantity { get; set; }

    [ForeignKey("BookId")]
    [InverseProperty("CartItems")]
    public virtual Book Book { get; set; } = null!;

    [ForeignKey("CartId")]
    [InverseProperty("CartItems")]
    public virtual ShoppingCart Cart { get; set; } = null!;
}
