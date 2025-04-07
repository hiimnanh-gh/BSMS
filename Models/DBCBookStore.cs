using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Models;

public partial class DBCBookStore : DbContext
{
    public DBCBookStore()
    {
    }

    public DBCBookStore(DbContextOptions<DBCBookStore> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Receipt> Receipts { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public virtual DbSet<User> Users { get; set; }




    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-UQHR23R;Database=Bookstore;User Id=sa;Password=123;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Book__8BE5A12D7722BFF7");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => new { e.CartId, e.BookId }).HasName("PK__CartItem__89E559CA36BCE41E");

            entity.HasOne(d => d.Book).WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartItem__bookID__4F7CD00D");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartItem__cartID__4E88ABD4");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__0809337D73740503");

            entity.HasOne(d => d.User).WithMany(p => p.Orders).HasConstraintName("FK__Orders__userID__403A8C7D");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.BookId }).HasName("PK__OrderIte__C0B7696F2D22B2FB");

            entity.HasOne(d => d.Book).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__bookI__5441852A");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__order__534D60F1");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Rating__2D290D49C75581AC");

            entity.HasOne(d => d.Book).WithMany(p => p.Ratings).HasConstraintName("FK__Rating__bookID__47DBAE45");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings).HasConstraintName("FK__Rating__userID__46E78A0C");
        });

        modelBuilder.Entity<Receipt>(entity =>
        {
            entity.HasKey(e => e.ReceiptId).HasName("PK__Receipt__CAA7E8986AAA760D");

            entity.HasOne(d => d.Order).WithMany(p => p.Receipts).HasConstraintName("FK__Receipt__orderID__4316F928");
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Shopping__415B03D871B03BF1");

            entity.HasOne(d => d.User).WithMany(p => p.ShoppingCarts).HasConstraintName("FK__ShoppingC__userI__4AB81AF0");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__CB9A1CDF4F479279");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
