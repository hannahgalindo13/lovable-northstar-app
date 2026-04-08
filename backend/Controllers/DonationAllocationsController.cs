using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/donationallocations")]
public class DonationAllocationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public DonationAllocationsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<DonationAllocation>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.DonationAllocations
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<DonationAllocation>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<DonationAllocation>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<DonationAllocation>>> GetById(int id)
    {
        try
        {
            var item = await _db.DonationAllocations.AsNoTracking().FirstOrDefaultAsync(x => x.AllocationId == id);
            if (item is null)
                return NotFound(ApiResponse<DonationAllocation>.Fail("Not found"));

            return Ok(ApiResponse<DonationAllocation>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<DonationAllocation>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DonationAllocation>>> Create([FromBody] DonationAllocation allocation)
    {
        try
        {
            _db.DonationAllocations.Add(allocation);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = allocation.AllocationId }, ApiResponse<DonationAllocation>.Ok(allocation));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<DonationAllocation>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<DonationAllocation>>> Update(int id, [FromBody] DonationAllocation allocation)
    {
        try
        {
            if (allocation.AllocationId != id)
                return BadRequest(ApiResponse<DonationAllocation>.Fail("ID in route does not match body"));

            var existing = await _db.DonationAllocations.FirstOrDefaultAsync(x => x.AllocationId == id);
            if (existing is null)
                return NotFound(ApiResponse<DonationAllocation>.Fail("Not found"));

            _db.Entry(existing).CurrentValues.SetValues(allocation);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<DonationAllocation>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<DonationAllocation>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.DonationAllocations.FirstOrDefaultAsync(x => x.AllocationId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.DonationAllocations.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

