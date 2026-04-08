using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class HomeVisitation
{
    public int VisitationId { get; set; }

    public int ResidentId { get; set; }

    public DateOnly VisitDate { get; set; }

    public string SocialWorker { get; set; } = null!;

    public string VisitType { get; set; } = null!;

    public string? LocationVisited { get; set; }

    public string? FamilyMembersPresent { get; set; }

    public string? Purpose { get; set; }

    public string? Observations { get; set; }

    public string? FamilyCooperationLevel { get; set; }

    public int SafetyConcernsNoted { get; set; }

    public int FollowUpNeeded { get; set; }

    public string? FollowUpNotes { get; set; }

    public string? VisitOutcome { get; set; }

    public string CoordinationKind { get; set; } = null!;

    public string? VisitTime { get; set; }

    public virtual Resident? Resident { get; set; }
}
