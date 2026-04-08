using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class ProcessRecording
{
    public int RecordingId { get; set; }

    public int ResidentId { get; set; }

    public DateOnly SessionDate { get; set; }

    public string SocialWorker { get; set; } = null!;

    public string SessionType { get; set; } = null!;

    public int? SessionDurationMinutes { get; set; }

    public string EmotionalStateObserved { get; set; } = null!;

    public string? EmotionalStateEnd { get; set; }

    public string? SessionNarrative { get; set; }

    public string? InterventionsApplied { get; set; }

    public string? FollowUpActions { get; set; }

    public int ProgressNoted { get; set; }

    public int ConcernsFlagged { get; set; }

    public int ReferralMade { get; set; }

    public string? NotesRestricted { get; set; }

    public virtual Resident? Resident { get; set; }
}
