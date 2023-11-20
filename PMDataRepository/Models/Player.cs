using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMDataRepository.Models;

[Table("Player")]
[Index("DateEnrolled", "Status", Name = "IDX_Player_DateEnrolled_Status")]
[Index("Status", "PlayerId", Name = "IDX_Player_Status_PlayerID_SSN_DateEnroll_BDay_XLastUpdate")]
[Index("Birthday", Name = "Idx_Player_Birthday")]
[Index("Ssn", Name = "idx_Player_SSN")]
public partial class Player
{
    [Key]
    [Column("PlayerID")]
    public int PlayerId { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [Column("SSN")]
    [StringLength(16)]
    [Unicode(false)]
    public string? Ssn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateEnrolled { get; set; }

    public int? AttractionNumber { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? Exempt { get; set; }

    public int? EnrollmentSource { get; set; }

    public int? PinNumber { get; set; }

    [Column("PINLocked")]
    [StringLength(1)]
    [Unicode(false)]
    public string? Pinlocked { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Birthday { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? AnniversaryDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CurrentDay { get; set; }

    public int CurrentTrip { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CurrentDayBeginDate { get; set; }

    [Column("XLastUpdated", TypeName = "datetime")]
    public DateTime XlastUpdated { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string AbandonedCard { get; set; } = null!;

    [Column("ID1")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Id1 { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? Gender { get; set; }

    [Column("SiteID")]
    public short SiteId { get; set; }

    [Column("PINDigest")]
    [StringLength(60)]
    [Unicode(false)]
    public string? Pindigest { get; set; }

    public int? Sentinel { get; set; }

    public int? EncryptIndex { get; set; }

    [Column("FailedPINAttempts")]
    public short? FailedPinattempts { get; set; }

    [StringLength(21)]
    [Unicode(false)]
    public string? Seed { get; set; }

    [Column("LanguageID")]
    public short? LanguageId { get; set; }

    [Column("AffiliationID")]
    public short? AffiliationId { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? Mosaic1 { get; set; }

    public int? Mosaic2 { get; set; }

    [Column("SourceID")]
    public int? SourceId { get; set; }

    [Column("SuffixID")]
    public byte? SuffixId { get; set; }

    [Column("GenerationID")]
    public byte? GenerationId { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string WebEnabled { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? WebLastVisitDate { get; set; }

    public int WebLoginCount { get; set; }

    [StringLength(64)]
    public string? CompanyName { get; set; }

    [StringLength(40)]
    public string? JobTitle { get; set; }

    [Column("DPID")]
    public int? Dpid { get; set; }

    [Column("RefusedID")]
    [StringLength(1)]
    [Unicode(false)]
    public string? RefusedId { get; set; }

    public bool? SmartCardEnabled { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? YearsAtCompany { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? YearsInIndustry { get; set; }

    [Column("BusinessTypeID")]
    public int? BusinessTypeId { get; set; }

    public int? Nationality { get; set; }

    [Column("PIP_PEP")]
    [StringLength(1)]
    [Unicode(false)]
    public string? PipPep { get; set; }

    [InverseProperty("Player")]
    public virtual ICollection<PlayerGroup> PlayerGroups { get; set; } = new List<PlayerGroup>();
}
