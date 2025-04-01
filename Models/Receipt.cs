using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Models;

[Table("Receipt")]
public partial class Receipt
{
    [Key]
    [Column("receiptID")]
    [StringLength(255)]
    [Unicode(false)]
    public string ReceiptId { get; set; } = null!;

    [Column("orderID")]
    [StringLength(255)]
    [Unicode(false)]
    public string? OrderId { get; set; }

    [Column("createdTime", TypeName = "datetime")]
    public DateTime? CreatedTime { get; set; }

    [Column("totalAmount", TypeName = "decimal(10, 2)")]
    public decimal? TotalAmount { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("Receipts")]
    public virtual Order? Order { get; set; }
}
