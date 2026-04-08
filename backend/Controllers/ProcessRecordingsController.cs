using Backend.Data;
using Backend.Models;
using Backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/processrecordings")]
[Authorize]
public class ProcessRecordingsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProcessRecordingsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProcessRecording>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.ProcessRecordings
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<ProcessRecording>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<ProcessRecording>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProcessRecording>>> GetById(int id)
    {
        try
        {
            var item = await _db.ProcessRecordings.AsNoTracking().FirstOrDefaultAsync(x => x.RecordingId == id);
            if (item is null)
                return NotFound(ApiResponse<ProcessRecording>.Fail("Not found"));

            return Ok(ApiResponse<ProcessRecording>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProcessRecording>.Fail(ex.Message));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ProcessRecording>>> Create([FromBody] ProcessRecordingUpsertDto dto)
    {
        try
        {
            var recording = new ProcessRecording
            {
                ResidentId = dto.ResidentId,
                SessionDate = dto.SessionDate,
                SocialWorker = dto.SocialWorker,
                SessionType = dto.SessionType,
                SessionDurationMinutes = dto.SessionDurationMinutes,
                EmotionalStateObserved = dto.EmotionalStateObserved,
                EmotionalStateEnd = dto.EmotionalStateEnd,
                SessionNarrative = dto.SessionNarrative,
                InterventionsApplied = dto.InterventionsApplied,
                FollowUpActions = dto.FollowUpActions,
                ProgressNoted = dto.ProgressNoted,
                ConcernsFlagged = dto.ConcernsFlagged,
                ReferralMade = dto.ReferralMade,
                NotesRestricted = dto.NotesRestricted
            };

            _db.ProcessRecordings.Add(recording);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = recording.RecordingId }, ApiResponse<ProcessRecording>.Ok(recording));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProcessRecording>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ProcessRecording>>> Update(int id, [FromBody] ProcessRecordingUpsertDto dto)
    {
        try
        {
            var existing = await _db.ProcessRecordings.FirstOrDefaultAsync(x => x.RecordingId == id);
            if (existing is null)
                return NotFound(ApiResponse<ProcessRecording>.Fail("Not found"));

            existing.ResidentId = dto.ResidentId;
            existing.SessionDate = dto.SessionDate;
            existing.SocialWorker = dto.SocialWorker;
            existing.SessionType = dto.SessionType;
            existing.SessionDurationMinutes = dto.SessionDurationMinutes;
            existing.EmotionalStateObserved = dto.EmotionalStateObserved;
            existing.EmotionalStateEnd = dto.EmotionalStateEnd;
            existing.SessionNarrative = dto.SessionNarrative;
            existing.InterventionsApplied = dto.InterventionsApplied;
            existing.FollowUpActions = dto.FollowUpActions;
            existing.ProgressNoted = dto.ProgressNoted;
            existing.ConcernsFlagged = dto.ConcernsFlagged;
            existing.ReferralMade = dto.ReferralMade;
            existing.NotesRestricted = dto.NotesRestricted;
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<ProcessRecording>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProcessRecording>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.ProcessRecordings.FirstOrDefaultAsync(x => x.RecordingId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.ProcessRecordings.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

