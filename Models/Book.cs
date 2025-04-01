using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Models;

[Table("Book")]
public partial class Book
{
    [Key]
    [Column("bookID")]
    [StringLength(255)]
    [Unicode(false)]
    public string BookId { get; set; } = null!;

    [Column("title")]
    [StringLength(255)]
    public string? Title { get; set; }

    [Column("author")]
    [StringLength(255)]
    public string? Author { get; set; }

    [Column("releaseDate")]
    public DateOnly? ReleaseDate { get; set; }

    [Column("price", TypeName = "decimal(10, 2)")]
    public decimal? Price { get; set; }

    [Column("stockQuantity")]
    public int? StockQuantity { get; set; }

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }
    [Column("category")]
    public string? Category { get; set; }

    [Column("CoverImagePath")] // Đảm bảo trùng với cột trong database
    [StringLength(500)]
    public string? CoverImagePath { get; set; } // Lưu đường dẫn ảnh

    [InverseProperty("Book")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [InverseProperty("Book")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Book")]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
