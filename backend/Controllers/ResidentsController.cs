using Backend.Data;
using Backend.Models;
using Backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/residents")]
[Authorize]
public class ResidentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ResidentsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Resident>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.Residents
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<Resident>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<Resident>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<Resident>>> GetById(int id)
    {
        try
        {
            var item = await _db.Residents.AsNoTracking().FirstOrDefaultAsync(x => x.ResidentId == id);
            if (item is null)
                return NotFound(ApiResponse<Resident>.Fail("Not found"));

            return Ok(ApiResponse<Resident>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Resident>.Fail(ex.Message));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<Resident>>> Create([FromBody] ResidentUpsertDto dto)
    {
        try
        {
            var resident = new Resident
            {
                CaseControlNo = dto.CaseControlNo,
                InternalCode = dto.InternalCode,
                SafehouseId = dto.SafehouseId,
                CaseStatus = dto.CaseStatus,
                Sex = dto.Sex,
                DateOfBirth = dto.DateOfBirth,
                BirthStatus = dto.BirthStatus,
                PlaceOfBirth = dto.PlaceOfBirth,
                Religion = dto.Religion,
                CaseCategory = dto.CaseCategory,
                SubCatOrphaned = dto.SubCatOrphaned,
                SubCatTrafficked = dto.SubCatTrafficked,
                SubCatChildLabor = dto.SubCatChildLabor,
                SubCatPhysicalAbuse = dto.SubCatPhysicalAbuse,
                SubCatSexualAbuse = dto.SubCatSexualAbuse,
                SubCatOsaec = dto.SubCatOsaec,
                SubCatCicl = dto.SubCatCicl,
                SubCatAtRisk = dto.SubCatAtRisk,
                SubCatStreetChild = dto.SubCatStreetChild,
                SubCatChildWithHiv = dto.SubCatChildWithHiv,
                IsPwd = dto.IsPwd,
                PwdType = dto.PwdType,
                HasSpecialNeeds = dto.HasSpecialNeeds,
                SpecialNeedsDiagnosis = dto.SpecialNeedsDiagnosis,
                FamilyIs4ps = dto.FamilyIs4ps,
                FamilySoloParent = dto.FamilySoloParent,
                FamilyIndigenous = dto.FamilyIndigenous,
                FamilyParentPwd = dto.FamilyParentPwd,
                FamilyInformalSettler = dto.FamilyInformalSettler,
                DateOfAdmission = dto.DateOfAdmission,
                AgeUponAdmission = dto.AgeUponAdmission,
                PresentAge = dto.PresentAge,
                LengthOfStay = dto.LengthOfStay,
                ReferralSource = dto.ReferralSource,
                ReferringAgencyPerson = dto.ReferringAgencyPerson,
                DateColbRegistered = dto.DateColbRegistered,
                DateColbObtained = dto.DateColbObtained,
                AssignedSocialWorker = dto.AssignedSocialWorker,
                InitialCaseAssessment = dto.InitialCaseAssessment,
                DateCaseStudyPrepared = dto.DateCaseStudyPrepared,
                ReintegrationType = dto.ReintegrationType,
                ReintegrationStatus = dto.ReintegrationStatus,
                InitialRiskLevel = dto.InitialRiskLevel,
                CurrentRiskLevel = dto.CurrentRiskLevel,
                DateEnrolled = dto.DateEnrolled,
                DateClosed = dto.DateClosed,
                CreatedAt = dto.CreatedAt,
                NotesRestricted = dto.NotesRestricted
            };

            _db.Residents.Add(resident);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = resident.ResidentId }, ApiResponse<Resident>.Ok(resident));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Resident>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<Resident>>> Update(int id, [FromBody] ResidentUpsertDto dto)
    {
        try
        {
            var existing = await _db.Residents.FirstOrDefaultAsync(x => x.ResidentId == id);
            if (existing is null)
                return NotFound(ApiResponse<Resident>.Fail("Not found"));

            existing.CaseControlNo = dto.CaseControlNo;
            existing.InternalCode = dto.InternalCode;
            existing.SafehouseId = dto.SafehouseId;
            existing.CaseStatus = dto.CaseStatus;
            existing.Sex = dto.Sex;
            existing.DateOfBirth = dto.DateOfBirth;
            existing.BirthStatus = dto.BirthStatus;
            existing.PlaceOfBirth = dto.PlaceOfBirth;
            existing.Religion = dto.Religion;
            existing.CaseCategory = dto.CaseCategory;
            existing.SubCatOrphaned = dto.SubCatOrphaned;
            existing.SubCatTrafficked = dto.SubCatTrafficked;
            existing.SubCatChildLabor = dto.SubCatChildLabor;
            existing.SubCatPhysicalAbuse = dto.SubCatPhysicalAbuse;
            existing.SubCatSexualAbuse = dto.SubCatSexualAbuse;
            existing.SubCatOsaec = dto.SubCatOsaec;
            existing.SubCatCicl = dto.SubCatCicl;
            existing.SubCatAtRisk = dto.SubCatAtRisk;
            existing.SubCatStreetChild = dto.SubCatStreetChild;
            existing.SubCatChildWithHiv = dto.SubCatChildWithHiv;
            existing.IsPwd = dto.IsPwd;
            existing.PwdType = dto.PwdType;
            existing.HasSpecialNeeds = dto.HasSpecialNeeds;
            existing.SpecialNeedsDiagnosis = dto.SpecialNeedsDiagnosis;
            existing.FamilyIs4ps = dto.FamilyIs4ps;
            existing.FamilySoloParent = dto.FamilySoloParent;
            existing.FamilyIndigenous = dto.FamilyIndigenous;
            existing.FamilyParentPwd = dto.FamilyParentPwd;
            existing.FamilyInformalSettler = dto.FamilyInformalSettler;
            existing.DateOfAdmission = dto.DateOfAdmission;
            existing.AgeUponAdmission = dto.AgeUponAdmission;
            existing.PresentAge = dto.PresentAge;
            existing.LengthOfStay = dto.LengthOfStay;
            existing.ReferralSource = dto.ReferralSource;
            existing.ReferringAgencyPerson = dto.ReferringAgencyPerson;
            existing.DateColbRegistered = dto.DateColbRegistered;
            existing.DateColbObtained = dto.DateColbObtained;
            existing.AssignedSocialWorker = dto.AssignedSocialWorker;
            existing.InitialCaseAssessment = dto.InitialCaseAssessment;
            existing.DateCaseStudyPrepared = dto.DateCaseStudyPrepared;
            existing.ReintegrationType = dto.ReintegrationType;
            existing.ReintegrationStatus = dto.ReintegrationStatus;
            existing.InitialRiskLevel = dto.InitialRiskLevel;
            existing.CurrentRiskLevel = dto.CurrentRiskLevel;
            existing.DateEnrolled = dto.DateEnrolled;
            existing.DateClosed = dto.DateClosed;
            existing.CreatedAt = dto.CreatedAt;
            existing.NotesRestricted = dto.NotesRestricted;
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<Resident>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Resident>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.Residents.FirstOrDefaultAsync(x => x.ResidentId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.Residents.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

