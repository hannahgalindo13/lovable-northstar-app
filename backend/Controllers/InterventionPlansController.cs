using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/interventionplans")]
public class InterventionPlansController : ControllerBase
{
    private readonly AppDbContext _db;

    public InterventionPlansController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<InterventionPlan>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.InterventionPlans
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<InterventionPlan>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<InterventionPlan>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<InterventionPlan>>> GetById(int id)
    {
        try
        {
            var item = await _db.InterventionPlans.AsNoTracking().FirstOrDefaultAsync(x => x.PlanId == id);
            if (item is null)
                return NotFound(ApiResponse<InterventionPlan>.Fail("Not found"));

            return Ok(ApiResponse<InterventionPlan>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<InterventionPlan>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<InterventionPlan>>> Create([FromBody] InterventionPlan plan)
    {
        try
        {
            _db.InterventionPlans.Add(plan);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = plan.PlanId }, ApiResponse<InterventionPlan>.Ok(plan));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<InterventionPlan>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<InterventionPlan>>> Update(int id, [FromBody] InterventionPlan plan)
    {
        try
        {
            if (plan.PlanId != id)
                return BadRequest(ApiResponse<InterventionPlan>.Fail("ID in route does not match body"));

            var existing = await _db.InterventionPlans.FirstOrDefaultAsync(x => x.PlanId == id);
            if (existing is null)
                return NotFound(ApiResponse<InterventionPlan>.Fail("Not found"));

            _db.Entry(existing).CurrentValues.SetValues(plan);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<InterventionPlan>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<InterventionPlan>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.InterventionPlans.FirstOrDefaultAsync(x => x.PlanId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.InterventionPlans.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

