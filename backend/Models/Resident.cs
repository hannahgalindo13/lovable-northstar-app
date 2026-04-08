using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Resident
{
    public int ResidentId { get; set; }

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

    public virtual ICollection<EducationRecord> EducationRecords { get; set; } = new List<EducationRecord>();

    public virtual ICollection<HealthWellbeingRecord> HealthWellbeingRecords { get; set; } = new List<HealthWellbeingRecord>();

    public virtual ICollection<HomeVisitation> HomeVisitations { get; set; } = new List<HomeVisitation>();

    public virtual ICollection<IncidentReport> IncidentReports { get; set; } = new List<IncidentReport>();

    public virtual ICollection<InterventionPlan> InterventionPlans { get; set; } = new List<InterventionPlan>();

    public virtual ICollection<ProcessRecording> ProcessRecordings { get; set; } = new List<ProcessRecording>();

    public virtual Safehouse? Safehouse { get; set; }
}
