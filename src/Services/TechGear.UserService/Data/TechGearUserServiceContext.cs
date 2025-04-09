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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Loyalty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Loyalty__3214EC075981F74B");

            entity.ToTable("Loyalty");

            entity.HasOne(d => d.User).WithMany(p => p.Loyalties)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Loyalty__UserId__276EDEB3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0784057A85");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105342EEFE7C6").IsUnique();

            entity.Property(e => e.DeliveryAddress).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
