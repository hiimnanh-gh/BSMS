using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Models;

[PrimaryKey("OrderId", "BookId")]
[Table("OrderItem")]
public partial class OrderItem
{
    [Key]
    [Column("orderID")]
    [StringLength(255)]
    [Unicode(false)]
    public string OrderId { get; set; } = null!;

    [Key]
    [Column("bookID")]
    [StringLength(255)]
    [Unicode(false)]
    public string BookId { get; set; } = null!;

    [Column("quantity")]
    public int? Quantity { get; set; }

    [ForeignKey("BookId")]
    [InverseProperty("OrderItems")]
    public virtual Book Book { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("OrderItems")]
    public virtual Order Order { get; set; } = null!;
}
