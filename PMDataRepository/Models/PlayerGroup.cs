using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMDataRepository.Models;

[PrimaryKey("PlayerId", "GroupId")]
[Table("PlayerGroup")]
[Index("PmCreateDate", "PmLastUpdate", Name = "IDX_PlayerGroup_Create_Update_Cover")]
[Index("GroupId", "PlayerId", "UserId", "ExtractionId", Name = "IDX_PlayerGroup_GroupID_Cover")]
public partial class PlayerGroup
{
    [Key]
    [Column("PlayerID")]
    public int PlayerId { get; set; }

    [Key]
    [Column("GroupID")]
    public int GroupId { get; set; }

    [Column("SiteID")]
    public short? SiteId { get; set; }

    [Column("ExtractionID")]
    public int? ExtractionId { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [Column("PM_Create_Date", TypeName = "datetime")]
    public DateTime? PmCreateDate { get; set; }

    [Column("PM_Last_Update", TypeName = "datetime")]
    public DateTime? PmLastUpdate { get; set; }

    [ForeignKey("GroupId")]
    [InverseProperty("PlayerGroups")]
    public virtual CasinoGroup Group { get; set; } = null!;

    [ForeignKey("PlayerId")]
    [InverseProperty("PlayerGroups")]
    public virtual Player Player { get; set; } = null!;
}
