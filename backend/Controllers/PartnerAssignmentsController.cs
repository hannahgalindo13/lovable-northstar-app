using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/partnerassignments")]
public class PartnerAssignmentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PartnerAssignmentsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<PartnerAssignment>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.PartnerAssignments
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<PartnerAssignment>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<PartnerAssignment>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PartnerAssignment>>> GetById(int id)
    {
        try
        {
            var item = await _db.PartnerAssignments.AsNoTracking().FirstOrDefaultAsync(x => x.AssignmentId == id);
            if (item is null)
                return NotFound(ApiResponse<PartnerAssignment>.Fail("Not found"));

            return Ok(ApiResponse<PartnerAssignment>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PartnerAssignment>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PartnerAssignment>>> Create([FromBody] PartnerAssignment assignment)
    {
        try
        {
            _db.PartnerAssignments.Add(assignment);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = assignment.AssignmentId }, ApiResponse<PartnerAssignment>.Ok(assignment));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PartnerAssignment>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<PartnerAssignment>>> Update(int id, [FromBody] PartnerAssignment assignment)
    {
        try
        {
            if (assignment.AssignmentId != id)
                return BadRequest(ApiResponse<PartnerAssignment>.Fail("ID in route does not match body"));

            var existing = await _db.PartnerAssignments.FirstOrDefaultAsync(x => x.AssignmentId == id);
            if (existing is null)
                return NotFound(ApiResponse<PartnerAssignment>.Fail("Not found"));

            _db.Entry(existing).CurrentValues.SetValues(assignment);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<PartnerAssignment>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PartnerAssignment>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.PartnerAssignments.FirstOrDefaultAsync(x => x.AssignmentId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.PartnerAssignments.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

