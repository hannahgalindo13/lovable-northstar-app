using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Donation
{
    public int DonationId { get; set; }

    public int SupporterId { get; set; }

    public string DonationType { get; set; } = null!;

    public DateOnly DonationDate { get; set; }

    public string ChannelSource { get; set; } = null!;

    public string? CurrencyCode { get; set; }

    public decimal? Amount { get; set; }

    public decimal? EstimatedValue { get; set; }

    public string? ImpactUnit { get; set; }

    public int IsRecurring { get; set; }

    public string? CampaignName { get; set; }

    public string? Notes { get; set; }

    public int? CreatedByPartnerId { get; set; }

    public int? ReferralPostId { get; set; }

    public virtual ICollection<DonationAllocation> DonationAllocations { get; set; } = new List<DonationAllocation>();

    public virtual ICollection<InKindDonationItem> InKindDonationItems { get; set; } = new List<InKindDonationItem>();

    public virtual Supporter? Supporter { get; set; }
}
