using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MiniWalletBackendSystem.Models;

public partial class MiniWalletDbContext : DbContext
{
    public MiniWalletDbContext()
    {
    }

    public MiniWalletDbContext(DbContextOptions<MiniWalletDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<WalletTransaction> WalletTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.WalletId).HasName("PK__Wallets__84D4F90EA2D220D0");

            entity.HasIndex(e => e.MobileNumber, "UQ__Wallets__250375B1C077F2B4").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Wallets__A9D105344A262D3D").IsUnique();

            entity.Property(e => e.WalletId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.MobileNumber).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<WalletTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__WalletTr__55433A6BFF1171D6");

            entity.HasIndex(e => e.CreatedAt, "IX_CreatedAt");

            entity.HasIndex(e => e.ReferenceId, "IX_ReferenceId").IsUnique();

            entity.HasIndex(e => e.TransactionType, "IX_Type");

            entity.HasIndex(e => e.WalletId, "IX_WalletId");

            entity.Property(e => e.TransactionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BalanceAfter).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BalanceBefore).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ReferenceId)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TransactionType)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Wallet).WithMany(p => p.WalletTransactions)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Wallet");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
