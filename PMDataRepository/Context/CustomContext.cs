using Microsoft.EntityFrameworkCore;
using PMDataRepository.CustomModels;
using PMDataRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMDataRepository.Context
{
    public partial class PlayerManagementContext : DbContext
    {
        public virtual DbSet<IndexesWithPageAndRowLocksDisabledReturnValue> IndexesWithPageAndRowLocksDisabledReturnValues { get; set; }
        public virtual DbSet<PlayerGroupInfoReport> PlayerGroupInfoReports { get; set; }
        public virtual DbSet<DataArchivalStatus> DataArchivalStatuses { get; set; }
        public virtual DbSet<PmDataArchivalJobReport> PmDataArchivalJobReports { get; set; }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IndexesWithPageAndRowLocksDisabledReturnValue>()
                   .HasNoKey();
            modelBuilder.Entity<PlayerGroupInfoReport>()
                 .HasNoKey();
            modelBuilder.Entity<DataArchivalStatus>()
                .HasNoKey();
            modelBuilder.Entity<PmDataArchivalJobReport>()
                .HasNoKey();
        }

    }
}
