using System;
using System.Collections.Generic;
using ArchivalDataRepository.Models;
using Microsoft.EntityFrameworkCore;

namespace ArchivalDataRepository.Context;

public partial class PmdataArchivalContext : DbContext
{
    public PmdataArchivalContext()
    {
    }

    public PmdataArchivalContext(DbContextOptions<PmdataArchivalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<IgtVersion> IgtVersions { get; set; }

    public virtual DbSet<PlayersToBePurged> PlayersToBePurgeds { get; set; }

    public virtual DbSet<PlayersToBePurgedCompleted> PlayersToBePurgedCompleteds { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayersToBePurged>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("PK__PlayersToBePurge");

            entity.ToTable("PlayersToBePurged", tb => tb.HasTrigger("Trig_SendToServiceBrokerQueue"));

            entity.Property(e => e.PlayerId).ValueGeneratedNever();
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.RowInsertDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<PlayersToBePurgedCompleted>(entity =>
        {
            entity.Property(e => e.RowInsertDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
