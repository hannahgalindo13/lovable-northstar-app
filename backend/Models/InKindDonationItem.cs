using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class InKindDonationItem
{
    public int ItemId { get; set; }

    public int DonationId { get; set; }

    public string ItemName { get; set; } = null!;

    public string ItemCategory { get; set; } = null!;

    public int Quantity { get; set; }

    public string UnitOfMeasure { get; set; } = null!;

    public decimal EstimatedUnitValue { get; set; }

    public string? IntendedUse { get; set; }

    public string? ReceivedCondition { get; set; }

    public virtual Donation Donation { get; set; } = null!;
}
