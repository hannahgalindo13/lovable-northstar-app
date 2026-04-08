using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/incidentreports")]
public class IncidentReportsController : ControllerBase
{
    private readonly AppDbContext _db;

    public IncidentReportsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<IncidentReport>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.IncidentReports
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<IncidentReport>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<IncidentReport>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<IncidentReport>>> GetById(int id)
    {
        try
        {
            var item = await _db.IncidentReports.AsNoTracking().FirstOrDefaultAsync(x => x.IncidentId == id);
            if (item is null)
                return NotFound(ApiResponse<IncidentReport>.Fail("Not found"));

            return Ok(ApiResponse<IncidentReport>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IncidentReport>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IncidentReport>>> Create([FromBody] IncidentReport report)
    {
        try
        {
            _db.IncidentReports.Add(report);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = report.IncidentId }, ApiResponse<IncidentReport>.Ok(report));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IncidentReport>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<IncidentReport>>> Update(int id, [FromBody] IncidentReport report)
    {
        try
        {
            if (report.IncidentId != id)
                return BadRequest(ApiResponse<IncidentReport>.Fail("ID in route does not match body"));

            var existing = await _db.IncidentReports.FirstOrDefaultAsync(x => x.IncidentId == id);
            if (existing is null)
                return NotFound(ApiResponse<IncidentReport>.Fail("Not found"));

            _db.Entry(existing).CurrentValues.SetValues(report);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<IncidentReport>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IncidentReport>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.IncidentReports.FirstOrDefaultAsync(x => x.IncidentId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.IncidentReports.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

