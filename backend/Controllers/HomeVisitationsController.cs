using Backend.Data;
using Backend.Models;
using Backend.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/homevisitations")]
public class HomeVisitationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public HomeVisitationsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<HomeVisitation>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.HomeVisitations
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<HomeVisitation>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<HomeVisitation>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<HomeVisitation>>> GetById(int id)
    {
        try
        {
            var item = await _db.HomeVisitations.AsNoTracking().FirstOrDefaultAsync(x => x.VisitationId == id);
            if (item is null)
                return NotFound(ApiResponse<HomeVisitation>.Fail("Not found"));

            return Ok(ApiResponse<HomeVisitation>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<HomeVisitation>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<HomeVisitation>>> Create([FromBody] HomeVisitationUpsertDto dto)
    {
        try
        {
            var visitation = new HomeVisitation
            {
                ResidentId = dto.ResidentId,
                VisitDate = dto.VisitDate,
                SocialWorker = dto.SocialWorker,
                VisitType = dto.VisitType,
                LocationVisited = dto.LocationVisited,
                FamilyMembersPresent = dto.FamilyMembersPresent,
                Purpose = dto.Purpose,
                Observations = dto.Observations,
                FamilyCooperationLevel = dto.FamilyCooperationLevel,
                SafetyConcernsNoted = dto.SafetyConcernsNoted,
                FollowUpNeeded = dto.FollowUpNeeded,
                FollowUpNotes = dto.FollowUpNotes,
                VisitOutcome = dto.VisitOutcome,
                CoordinationKind = dto.CoordinationKind,
                VisitTime = dto.VisitTime
            };

            _db.HomeVisitations.Add(visitation);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = visitation.VisitationId }, ApiResponse<HomeVisitation>.Ok(visitation));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<HomeVisitation>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<HomeVisitation>>> Update(int id, [FromBody] HomeVisitationUpsertDto dto)
    {
        try
        {
            var existing = await _db.HomeVisitations.FirstOrDefaultAsync(x => x.VisitationId == id);
            if (existing is null)
                return NotFound(ApiResponse<HomeVisitation>.Fail("Not found"));

            existing.ResidentId = dto.ResidentId;
            existing.VisitDate = dto.VisitDate;
            existing.SocialWorker = dto.SocialWorker;
            existing.VisitType = dto.VisitType;
            existing.LocationVisited = dto.LocationVisited;
            existing.FamilyMembersPresent = dto.FamilyMembersPresent;
            existing.Purpose = dto.Purpose;
            existing.Observations = dto.Observations;
            existing.FamilyCooperationLevel = dto.FamilyCooperationLevel;
            existing.SafetyConcernsNoted = dto.SafetyConcernsNoted;
            existing.FollowUpNeeded = dto.FollowUpNeeded;
            existing.FollowUpNotes = dto.FollowUpNotes;
            existing.VisitOutcome = dto.VisitOutcome;
            existing.CoordinationKind = dto.CoordinationKind;
            existing.VisitTime = dto.VisitTime;
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<HomeVisitation>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<HomeVisitation>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.HomeVisitations.FirstOrDefaultAsync(x => x.VisitationId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.HomeVisitations.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

