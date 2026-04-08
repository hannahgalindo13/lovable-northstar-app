using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/publicimpactsnapshots")]
public class PublicImpactSnapshotsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PublicImpactSnapshotsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<PublicImpactSnapshot>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.PublicImpactSnapshots
                .AsNoTracking()
                .OrderByDescending(x => x.SnapshotId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<PublicImpactSnapshot>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<PublicImpactSnapshot>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PublicImpactSnapshot>>> GetById(int id)
    {
        try
        {
            var item = await _db.PublicImpactSnapshots.AsNoTracking().FirstOrDefaultAsync(x => x.SnapshotId == id);
            if (item is null)
                return NotFound(ApiResponse<PublicImpactSnapshot>.Fail("Not found"));

            return Ok(ApiResponse<PublicImpactSnapshot>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PublicImpactSnapshot>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PublicImpactSnapshot>>> Create([FromBody] PublicImpactSnapshot snapshot)
    {
        try
        {
            _db.PublicImpactSnapshots.Add(snapshot);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = snapshot.SnapshotId }, ApiResponse<PublicImpactSnapshot>.Ok(snapshot));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PublicImpactSnapshot>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<PublicImpactSnapshot>>> Update(int id, [FromBody] PublicImpactSnapshot snapshot)
    {
        try
        {
            if (snapshot.SnapshotId != id)
                return BadRequest(ApiResponse<PublicImpactSnapshot>.Fail("ID in route does not match body"));

            var existing = await _db.PublicImpactSnapshots.FirstOrDefaultAsync(x => x.SnapshotId == id);
            if (existing is null)
                return NotFound(ApiResponse<PublicImpactSnapshot>.Fail("Not found"));

            _db.Entry(existing).CurrentValues.SetValues(snapshot);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<PublicImpactSnapshot>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PublicImpactSnapshot>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.PublicImpactSnapshots.FirstOrDefaultAsync(x => x.SnapshotId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.PublicImpactSnapshots.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

