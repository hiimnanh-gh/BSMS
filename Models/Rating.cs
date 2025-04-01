using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Models;

[Table("Rating")]
public partial class Rating
{
    [Key]
    [Column("ratingID")]
    [StringLength(255)]
    [Unicode(false)]
    public string RatingId { get; set; } = null!;

    [Column("userID")]
    [StringLength(255)]
    [Unicode(false)]
    public string? UserId { get; set; }

    [Column("bookID")]
    [StringLength(255)]
    [Unicode(false)]
    public string? BookId { get; set; }

    [Column("rate")]
    public int? Rate { get; set; }

    [Column("ratingDate", TypeName = "datetime")]
    public DateTime? RatingDate { get; set; }

    [ForeignKey("BookId")]
    [InverseProperty("Ratings")]
    public virtual Book? Book { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Ratings")]
    public virtual User? User { get; set; }
}
