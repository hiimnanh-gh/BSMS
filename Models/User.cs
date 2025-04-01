using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Models;

[Index("Email", Name = "UQ__Users__AB6E6164781AE909", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("userID")]
    [StringLength(255)]
    [Unicode(false)]
    public string UserId { get; set; } = Guid.NewGuid().ToString(); // ✅ Fix lỗi thiếu UserId

    [Column("username")]
    [StringLength(255)]
    [Required]
    public string Username { get; set; } = null!;

    [Column("email")]
    [StringLength(255)]
    [Required]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [Column("password")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Password { get; set; }

    [Column("paymentinfo")]
    [StringLength(255)]
    [Unicode(false)]
    public string? PaymentInfo { get; set; }

    [Column("role")]
    [StringLength(20)]
    [Unicode(false)]
    public string Role { get; set; } = "user"; // ✅ Fix lỗi thiếu Role


    [Column("birthdate")]
    public DateTime? Birthdate { get; set; }

    // ✅ Fix lỗi JSON không thể chuyển đổi dữ liệu (EF tự load quan hệ)
    [JsonIgnore]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [JsonIgnore]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    [JsonIgnore]
    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
}
