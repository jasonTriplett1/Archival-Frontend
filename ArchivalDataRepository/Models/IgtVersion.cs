using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchivalDataRepository.Models;

[Keyless]
[Table("IGT_Versions")]
public partial class IgtVersion
{
    [StringLength(3)]
    [Unicode(false)]
    public string? Type { get; set; }

    [Column("PackageID")]
    [StringLength(20)]
    [Unicode(false)]
    public string? PackageId { get; set; }

    [StringLength(128)]
    [Unicode(false)]
    public string? HotFixId { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? ProductName { get; set; }

    [StringLength(32)]
    [Unicode(false)]
    public string? Version { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? Build { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? InstallationDate { get; set; }

    [StringLength(256)]
    [Unicode(false)]
    public string? Description { get; set; }

    [StringLength(5)]
    [Unicode(false)]
    public string? Status { get; set; }

    [StringLength(256)]
    [Unicode(false)]
    public string? UserName { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? HashNumber { get; set; }
}
