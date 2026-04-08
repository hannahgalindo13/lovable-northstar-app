using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class PublicImpactSnapshot
{
    public int SnapshotId { get; set; }

    public DateOnly SnapshotDate { get; set; }

    public string? Headline { get; set; }

    public string? SummaryText { get; set; }

    public string? MetricPayloadJson { get; set; }

    public int IsPublished { get; set; }

    public DateOnly? PublishedAt { get; set; }
}
