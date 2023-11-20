using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PMDataRepository.Models;

namespace PMDataRepository.Context;

public partial class PlayerManagementContext : DbContext
{
    public PlayerManagementContext()
    {
    }

    public PlayerManagementContext(DbContextOptions<PlayerManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CasinoGroup> CasinoGroups { get; set; }

    public virtual DbSet<ConfigGlobal> ConfigGlobals { get; set; }

    public virtual DbSet<ConfigItem> ConfigItems { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayerGroup> PlayerGroups { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CasinoGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PKCasinoGroup");

            entity.Property(e => e.GroupId).ValueGeneratedNever();
            entity.Property(e => e.PmCreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PmLastUpdate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).IsFixedLength();
        });

        modelBuilder.Entity<ConfigGlobal>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PKConfigGlobal");

            entity.ToTable("ConfigGlobal", tb => tb.HasTrigger("Trig_ConfigGlobal"));

            entity.Property(e => e.ItemId).ValueGeneratedNever();

            entity.HasOne(d => d.Item).WithOne(p => p.ConfigGlobal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConfigGlobal_ConfigItem");
        });

        modelBuilder.Entity<ConfigItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PKConfigItem");

            entity.ToTable("ConfigItem", tb => tb.HasTrigger("Trig_Config"));

            entity.Property(e => e.ItemId).ValueGeneratedNever();
            entity.Property(e => e.ConfigType).IsFixedLength();

            entity.HasOne(d => d.Group).WithMany(p => p.InverseGroup).HasConstraintName("FK_ConfigItem_ConfigItem");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("PKPlayer");

            entity.ToTable("Player", tb => tb.HasTrigger("Trig_PlayerIUD"));

            entity.Property(e => e.PlayerId).ValueGeneratedNever();
            entity.Property(e => e.AbandonedCard)
                .HasDefaultValue("N")
                .IsFixedLength();
            entity.Property(e => e.CurrentDay).HasDefaultValue(new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            entity.Property(e => e.CurrentDayBeginDate).HasDefaultValue(new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            entity.Property(e => e.CurrentTrip).HasDefaultValue(1);
            entity.Property(e => e.Exempt).IsFixedLength();
            entity.Property(e => e.FailedPinattempts).HasDefaultValue((short)0);
            entity.Property(e => e.Gender).IsFixedLength();
            entity.Property(e => e.Mosaic1).IsFixedLength();
            entity.Property(e => e.Pindigest).IsFixedLength();
            entity.Property(e => e.Pinlocked).IsFixedLength();
            entity.Property(e => e.PipPep).IsFixedLength();
            entity.Property(e => e.RefusedId).IsFixedLength();
            entity.Property(e => e.Seed).IsFixedLength();
            entity.Property(e => e.SmartCardEnabled).HasDefaultValue(false);
            entity.Property(e => e.Status).IsFixedLength();
            entity.Property(e => e.WebEnabled)
                .HasDefaultValue("N")
                .IsFixedLength();
        });

        modelBuilder.Entity<PlayerGroup>(entity =>
        {
            entity.HasKey(e => new { e.PlayerId, e.GroupId }).HasName("PKPlayerGroup");

            entity.Property(e => e.PmCreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PmLastUpdate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Group).WithMany(p => p.PlayerGroups)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PlayerGroup_FK_CasinoGroup");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerGroups)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PlayerGroup_FK_Player");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
