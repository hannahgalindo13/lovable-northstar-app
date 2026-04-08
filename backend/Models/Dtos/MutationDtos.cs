namespace Backend.Models.Dtos;

public class SupporterUpsertDto
{
    public string SupporterType { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? OrganizationName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string RelationshipType { get; set; } = null!;
    public string? Region { get; set; }
    public string? Country { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Status { get; set; } = null!;
    public DateOnly? FirstDonationDate { get; set; }
    public string? AcquisitionChannel { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DonationUpsertDto
{
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
}

public class ResidentUpsertDto
{
    public string CaseControlNo { get; set; } = null!;
    public string InternalCode { get; set; } = null!;
    public int SafehouseId { get; set; }
    public string CaseStatus { get; set; } = null!;
    public string Sex { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string? BirthStatus { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? Religion { get; set; }
    public string CaseCategory { get; set; } = null!;
    public int SubCatOrphaned { get; set; }
    public int SubCatTrafficked { get; set; }
    public int SubCatChildLabor { get; set; }
    public int SubCatPhysicalAbuse { get; set; }
    public int SubCatSexualAbuse { get; set; }
    public int SubCatOsaec { get; set; }
    public int SubCatCicl { get; set; }
    public int SubCatAtRisk { get; set; }
    public int SubCatStreetChild { get; set; }
    public int SubCatChildWithHiv { get; set; }
    public int IsPwd { get; set; }
    public string? PwdType { get; set; }
    public int HasSpecialNeeds { get; set; }
    public string? SpecialNeedsDiagnosis { get; set; }
    public int FamilyIs4ps { get; set; }
    public int FamilySoloParent { get; set; }
    public int FamilyIndigenous { get; set; }
    public int FamilyParentPwd { get; set; }
    public int FamilyInformalSettler { get; set; }
    public DateOnly DateOfAdmission { get; set; }
    public string? AgeUponAdmission { get; set; }
    public string? PresentAge { get; set; }
    public string? LengthOfStay { get; set; }
    public string? ReferralSource { get; set; }
    public string? ReferringAgencyPerson { get; set; }
    public DateOnly? DateColbRegistered { get; set; }
    public DateOnly? DateColbObtained { get; set; }
    public string? AssignedSocialWorker { get; set; }
    public string? InitialCaseAssessment { get; set; }
    public DateOnly? DateCaseStudyPrepared { get; set; }
    public string? ReintegrationType { get; set; }
    public string? ReintegrationStatus { get; set; }
    public string InitialRiskLevel { get; set; } = null!;
    public string CurrentRiskLevel { get; set; } = null!;
    public DateOnly DateEnrolled { get; set; }
    public DateOnly? DateClosed { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? NotesRestricted { get; set; }
}

public class ProcessRecordingUpsertDto
{
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
}

public class HomeVisitationUpsertDto
{
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
    public string CoordinationKind { get; set; } = "HomeVisit";
    public string? VisitTime { get; set; }
}
