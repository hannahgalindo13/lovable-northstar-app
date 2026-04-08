using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/healthwellbeingrecords")]
public class HealthWellbeingRecordsController : ControllerBase
{
    private readonly AppDbContext _db;

    public HealthWellbeingRecordsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<HealthWellbeingRecord>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.HealthWellbeingRecords
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<HealthWellbeingRecord>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<HealthWellbeingRecord>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<HealthWellbeingRecord>>> GetById(int id)
    {
        try
        {
            var item = await _db.HealthWellbeingRecords.AsNoTracking().FirstOrDefaultAsync(x => x.HealthRecordId == id);
            if (item is null)
                return NotFound(ApiResponse<HealthWellbeingRecord>.Fail("Not found"));

            return Ok(ApiResponse<HealthWellbeingRecord>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<HealthWellbeingRecord>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<HealthWellbeingRecord>>> Create([FromBody] HealthWellbeingRecord record)
    {
        try
        {
            _db.HealthWellbeingRecords.Add(record);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = record.HealthRecordId }, ApiResponse<HealthWellbeingRecord>.Ok(record));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<HealthWellbeingRecord>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<HealthWellbeingRecord>>> Update(int id, [FromBody] HealthWellbeingRecord record)
    {
        try
        {
            if (record.HealthRecordId != id)
                return BadRequest(ApiResponse<HealthWellbeingRecord>.Fail("ID in route does not match body"));

            var existing = await _db.HealthWellbeingRecords.FirstOrDefaultAsync(x => x.HealthRecordId == id);
            if (existing is null)
                return NotFound(ApiResponse<HealthWellbeingRecord>.Fail("Not found"));

            _db.Entry(existing).CurrentValues.SetValues(record);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<HealthWellbeingRecord>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<HealthWellbeingRecord>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.HealthWellbeingRecords.FirstOrDefaultAsync(x => x.HealthRecordId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.HealthWellbeingRecords.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

