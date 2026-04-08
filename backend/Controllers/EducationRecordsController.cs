using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/educationrecords")]
[Authorize]
public class EducationRecordsController : ControllerBase
{
    private readonly AppDbContext _db;

    public EducationRecordsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<EducationRecord>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.EducationRecords
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<EducationRecord>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<EducationRecord>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<EducationRecord>>> GetById(int id)
    {
        try
        {
            var item = await _db.EducationRecords.AsNoTracking().FirstOrDefaultAsync(x => x.EducationRecordId == id);
            if (item is null)
                return NotFound(ApiResponse<EducationRecord>.Fail("Not found"));

            return Ok(ApiResponse<EducationRecord>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<EducationRecord>.Fail(ex.Message));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<EducationRecord>>> Create([FromBody] EducationRecord record)
    {
        try
        {
            _db.EducationRecords.Add(record);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = record.EducationRecordId }, ApiResponse<EducationRecord>.Ok(record));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<EducationRecord>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<EducationRecord>>> Update(int id, [FromBody] EducationRecord record)
    {
        try
        {
            if (record.EducationRecordId != id)
                return BadRequest(ApiResponse<EducationRecord>.Fail("ID in route does not match body"));

            var existing = await _db.EducationRecords.FirstOrDefaultAsync(x => x.EducationRecordId == id);
            if (existing is null)
                return NotFound(ApiResponse<EducationRecord>.Fail("Not found"));

            _db.Entry(existing).CurrentValues.SetValues(record);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<EducationRecord>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<EducationRecord>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.EducationRecords.FirstOrDefaultAsync(x => x.EducationRecordId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.EducationRecords.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

