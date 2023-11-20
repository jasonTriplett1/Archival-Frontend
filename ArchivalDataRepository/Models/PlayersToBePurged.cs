using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchivalDataRepository.Models;

[Table("PlayersToBePurged")]
[Index("Processed", Name = "IX_Processed_PlayersToBePurged")]
public partial class PlayersToBePurged
{
    [Column("ID")]
    public int Id { get; set; }

    [Key]
    [Column("PlayerID")]
    public int PlayerId { get; set; }

    public byte Processed { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime RowInsertDate { get; set; }

    public long? ArchiveSessionId { get; set; }

    public long? DataArchivalQueueId { get; set; }
}
