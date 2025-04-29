using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TechGear.ChatService.Models;

namespace TechGear.ChatService.Data;

public partial class TechGearChatServiceContext : DbContext
{
    public TechGearChatServiceContext()
    {
    }

    public TechGearChatServiceContext(DbContextOptions<TechGearChatServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Messages__3214EC07C0E78E70");

            entity.Property(e => e.Content).HasColumnType("nvarchar(max)");

            entity.Property(e => e.IsImage)
                .HasDefaultValue(false);

            entity.Property(e => e.IsRead)
                .HasDefaultValue(false);

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255);

            entity.Property(e => e.SentAt)
                .HasColumnName("SendAt")
                .HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
