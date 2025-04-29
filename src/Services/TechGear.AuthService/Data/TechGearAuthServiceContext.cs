using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TechGear.AuthService.Models;

namespace TechGear.AuthService.Data;

public partial class TechGearAuthServiceContext : DbContext
{
    public TechGearAuthServiceContext()
    {
    }

    public TechGearAuthServiceContext(DbContextOptions<TechGearAuthServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuthUser> AuthUsers { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Otp> Otps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AuthUser__3214EC070ED0C5B1");

            entity.HasIndex(e => e.Email, "UQ__AuthUser__536C85E41EC3B0C8").IsUnique();

            entity.Property(e => e.HashedPassword).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("User");
            entity.Property(e => e.Email).HasMaxLength(255);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC078F7FEE73");

            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(255);

            entity.Property(e => e.IsRevoked)
                .IsRequired()
                .HasDefaultValue(false);

        });

        modelBuilder.Entity<Otp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OTPs__3214EC0712DB16C2");

            entity.Property(e => e.ExpiryTime).HasColumnType("datetime");
            entity.Property(e => e.Code).HasMaxLength(6);

            entity.Property(e => e.IsUsed)
                .IsRequired()
                .HasDefaultValue(false);

        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
