using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class PartnerAssignment
{
    public int AssignmentId { get; set; }

    public int PartnerId { get; set; }

    public int? SafehouseId { get; set; }

    public string ProgramArea { get; set; } = null!;

    public DateOnly AssignmentStart { get; set; }

    public DateOnly? AssignmentEnd { get; set; }

    public string? ResponsibilityNotes { get; set; }

    public int IsPrimary { get; set; }

    public string Status { get; set; } = null!;

    public virtual Partner Partner { get; set; } = null!;

    public virtual Safehouse? Safehouse { get; set; }
}
