using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TechGear.UserService.Models;

namespace TechGear.UserService.Data;

public partial class TechGearUserServiceContext : DbContext
{
    public TechGearUserServiceContext()
    {
    }

    public TechGearUserServiceContext(DbContextOptions<TechGearUserServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Loyalty> Loyalties { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public DbSet<UserAddress> UserAddresses { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Loyalty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Loyalty__3214EC075981F74B");

            entity.ToTable("Loyalty");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.User).WithMany(p => p.Loyalties)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Loyalty__UserId__276EDEB3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0784057A85");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105342EEFE7C6").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Favorite__3214EC07EA1646D7");

            entity.ToTable("Favorites");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorites__UserI__398D8EEE");
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.ToTable("UserAddress");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.RecipientName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.RecipientPhone)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.IsDefault)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            // Foreign key to User
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserAddresses)
                .HasForeignKey(e => e.UserId)
                .IsRequired();
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
