using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class IncidentReport
{
    public int IncidentId { get; set; }

    public int ResidentId { get; set; }

    public int SafehouseId { get; set; }

    public DateOnly IncidentDate { get; set; }

    public string IncidentType { get; set; } = null!;

    public string Severity { get; set; } = null!;

    public string? Description { get; set; }

    public string? ResponseTaken { get; set; }

    public int Resolved { get; set; }

    public DateOnly? ResolutionDate { get; set; }

    public string? ReportedBy { get; set; }

    public int FollowUpRequired { get; set; }

    public virtual Resident Resident { get; set; } = null!;

    public virtual Safehouse Safehouse { get; set; } = null!;
}
