using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Models;

public partial class Order
{
    [Key]
    [Column("orderID")]
    [StringLength(255)]
    [Unicode(false)]
    public string OrderId { get; set; } = null!;

    [Column("userID")]
    [StringLength(255)]
    [Unicode(false)]
    public string? UserId { get; set; }

    [Column("orderDate", TypeName = "datetime")]
    public DateTime? OrderDate { get; set; }

    [Column("status")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Status { get; set; }

    [Column("totalAmount", TypeName = "decimal(10, 2)")]
    public decimal? TotalAmount { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Order")]
    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual User? User { get; set; }
}
