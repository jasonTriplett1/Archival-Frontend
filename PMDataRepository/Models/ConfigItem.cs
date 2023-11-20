using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMDataRepository.Models;

[Table("ConfigItem")]
[Index("CategoryId", "PermissionNum", "Visible", Name = "IDX_ConfigItem_CategoryID")]
[Index("InternalDescription", "ItemId", "ConfigType", Name = "IDX_ConfigItem_Cover")]
[Index("InternalDescription", Name = "UC_ConfigItem_InternalDescription", IsUnique = true)]
public partial class ConfigItem
{
    [Key]
    [Column("ItemID")]
    public int ItemId { get; set; }

    [StringLength(64)]
    [Unicode(false)]
    public string Description { get; set; } = null!;

    [StringLength(2000)]
    [Unicode(false)]
    public string? LongDescription { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? DefaultValue { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string ConfigType { get; set; } = null!;

    public byte FillType { get; set; }

    [StringLength(2000)]
    [Unicode(false)]
    public string? FillValue { get; set; }

    public bool InsertBlank { get; set; }

    [StringLength(32)]
    [Unicode(false)]
    public string? BlankLabel { get; set; }

    public byte ValueType { get; set; }

    public byte SpecialControl { get; set; }

    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [Column("OrderID")]
    public int? OrderId { get; set; }

    public int ValidationMask { get; set; }

    [Column(TypeName = "money")]
    public decimal? RangeLow { get; set; }

    [Column(TypeName = "money")]
    public decimal? RangeHigh { get; set; }

    public bool Visible { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? CustomGetStatement { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? CustomUpdateStatement { get; set; }

    [Column("GroupID")]
    public int? GroupId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? DisableIf { get; set; }

    [StringLength(64)]
    [Unicode(false)]
    public string InternalDescription { get; set; } = null!;

    public short? PermissionNum { get; set; }

    [Column(TypeName = "decimal(9, 2)")]
    public decimal? TranslateStoreFactor { get; set; }

    [Column(TypeName = "decimal(9, 2)")]
    public decimal? TranslateDisplayFactor { get; set; }

    [InverseProperty("Item")]
    public virtual ConfigGlobal? ConfigGlobal { get; set; }

    [ForeignKey("GroupId")]
    [InverseProperty("InverseGroup")]
    public virtual ConfigItem? Group { get; set; }

    [InverseProperty("Group")]
    public virtual ICollection<ConfigItem> InverseGroup { get; set; } = new List<ConfigItem>();
}
