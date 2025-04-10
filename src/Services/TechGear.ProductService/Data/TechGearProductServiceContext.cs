using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Data;

public partial class TechGearProductServiceContext : DbContext
{
    public TechGearProductServiceContext()
    {
    }

    public TechGearProductServiceContext(DbContextOptions<TechGearProductServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductItem> ProductItems { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Variation> Variations { get; set; }

    public virtual DbSet<VariationOption> VariationOptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brands__3214EC072AC7B1BE");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC079425464F");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07D6480821");

            entity.Property(e => e.Available).HasDefaultValue(true);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ProductImage).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__BrandI__29572725");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Catego__286302EC");
        });

        modelBuilder.Entity<ProductItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductI__3214EC07E9AEF4E9");

            entity.Property(e => e.ProductImage).HasMaxLength(255);
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("SKU");
            entity.Property(e => e.QtyInStock)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            entity.HasOne(d => d.Product).WithMany(p => p.ProductItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductIt__Produ__2E1BDC42");

            entity.HasMany(d => d.VariationOptions).WithMany(p => p.ProductItems)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductConfiguration",
                    r => r.HasOne<VariationOption>().WithMany()
                        .HasForeignKey("VariationOptionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ProductCo__Varia__38996AB5"),
                    l => l.HasOne<ProductItem>().WithMany()
                        .HasForeignKey("ProductItemId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ProductCo__Produ__37A5467C"),
                    j =>
                    {
                        j.HasKey("ProductItemId", "VariationOptionId").HasName("PK__ProductC__BD1742746C447134");
                        j.ToTable("ProductConfigurations");
                    });
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ratings__3214EC07AC3532D9");

            entity.HasOne(d => d.Product).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ratings__Product__403A8C7D");
        });

        modelBuilder.Entity<Variation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Variatio__3214EC0742D7DA79");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.Variations)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Variation__Categ__31EC6D26");
        });

        modelBuilder.Entity<VariationOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Variatio__3214EC07E91DF835");

            entity.Property(e => e.Value).HasMaxLength(100);

            entity.HasOne(d => d.Variation).WithMany(p => p.VariationOptions)
                .HasForeignKey(d => d.VariationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Variation__Varia__34C8D9D1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
