using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/safehousemonthlymetrics")]
[Authorize]
public class SafehouseMonthlyMetricsController : ControllerBase
{
    private readonly AppDbContext _db;

    public SafehouseMonthlyMetricsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<SafehouseMonthlyMetric>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.SafehouseMonthlyMetrics
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<SafehouseMonthlyMetric>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<SafehouseMonthlyMetric>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<SafehouseMonthlyMetric>>> GetById(int id)
    {
        try
        {
            var item = await _db.SafehouseMonthlyMetrics.AsNoTracking().FirstOrDefaultAsync(x => x.MetricId == id);
            if (item is null)
                return NotFound(ApiResponse<SafehouseMonthlyMetric>.Fail("Not found"));

            return Ok(ApiResponse<SafehouseMonthlyMetric>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SafehouseMonthlyMetric>.Fail(ex.Message));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SafehouseMonthlyMetric>>> Create([FromBody] SafehouseMonthlyMetric metric)
    {
        try
        {
            _db.SafehouseMonthlyMetrics.Add(metric);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = metric.MetricId }, ApiResponse<SafehouseMonthlyMetric>.Ok(metric));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SafehouseMonthlyMetric>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SafehouseMonthlyMetric>>> Update(int id, [FromBody] SafehouseMonthlyMetric metric)
    {
        try
        {
            if (metric.MetricId != id)
                return BadRequest(ApiResponse<SafehouseMonthlyMetric>.Fail("ID in route does not match body"));

            var existing = await _db.SafehouseMonthlyMetrics.FirstOrDefaultAsync(x => x.MetricId == id);
            if (existing is null)
                return NotFound(ApiResponse<SafehouseMonthlyMetric>.Fail("Not found"));

            _db.Entry(existing).CurrentValues.SetValues(metric);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<SafehouseMonthlyMetric>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SafehouseMonthlyMetric>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.SafehouseMonthlyMetrics.FirstOrDefaultAsync(x => x.MetricId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.SafehouseMonthlyMetrics.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

