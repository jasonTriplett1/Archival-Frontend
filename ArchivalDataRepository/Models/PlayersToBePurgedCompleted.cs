using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchivalDataRepository.Models;

[Table("PlayersToBePurged_Completed")]
public partial class PlayersToBePurgedCompleted
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [Column("PlayerID")]
    public int PlayerId { get; set; }

    public byte Processed { get; set; }

    [Column("PlayersToBePurged_RowInsertDate", TypeName = "datetime")]
    public DateTime? PlayersToBePurgedRowInsertDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime RowInsertDate { get; set; }

    public long? ArchiveSessionid { get; set; }

    public long? DataArchivalQueueId { get; set; }
}
