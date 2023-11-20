using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMDataRepository.Models;

[Table("CasinoGroup")]
[Index("SiteId", "Status", Name = "IDX_CasinoGroup_SiteID_Status")]
public partial class CasinoGroup
{
    [Key]
    [Column("GroupID")]
    public int GroupId { get; set; }

    [Column("SiteID")]
    public short SiteId { get; set; }

    [StringLength(32)]
    [Unicode(false)]
    public string GroupName { get; set; } = null!;

    [StringLength(128)]
    [Unicode(false)]
    public string Description { get; set; } = null!;

    [StringLength(1)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [Column("PM_Create_Date", TypeName = "datetime")]
    public DateTime? PmCreateDate { get; set; }

    [Column("PM_Last_Update", TypeName = "datetime")]
    public DateTime? PmLastUpdate { get; set; }

    [InverseProperty("Group")]
    public virtual ICollection<PlayerGroup> PlayerGroups { get; set; } = new List<PlayerGroup>();
}
