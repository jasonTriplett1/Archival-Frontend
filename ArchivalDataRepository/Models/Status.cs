using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchivalDataRepository.Models;

[Table("Status")]
public partial class Status
{
    [Key]
    public short Id { get; set; }

    [StringLength(100)]
    public string StatusName { get; set; } = null!;
}
