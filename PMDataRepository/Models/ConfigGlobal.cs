using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMDataRepository.Models;

[Table("ConfigGlobal")]
public partial class ConfigGlobal
{
    [Key]
    [Column("ItemID")]
    public int ItemId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? ConfigValue { get; set; }

    [ForeignKey("ItemId")]
    [InverseProperty("ConfigGlobal")]
    public virtual ConfigItem Item { get; set; } = null!;
}
