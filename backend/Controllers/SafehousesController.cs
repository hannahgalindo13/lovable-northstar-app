using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/safehouses")]
public class SafehousesController : ControllerBase
{
    private readonly AppDbContext _db;

    public SafehousesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Safehouse>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.Safehouses
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<Safehouse>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<Safehouse>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<Safehouse>>> GetById(int id)
    {
        try
        {
            var item = await _db.Safehouses.AsNoTracking().FirstOrDefaultAsync(x => x.SafehouseId == id);
            if (item is null)
                return NotFound(ApiResponse<Safehouse>.Fail("Not found"));

            return Ok(ApiResponse<Safehouse>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Safehouse>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Safehouse>>> Create([FromBody] Safehouse safehouse)
    {
        try
        {
            _db.Safehouses.Add(safehouse);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = safehouse.SafehouseId }, ApiResponse<Safehouse>.Ok(safehouse));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Safehouse>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<Safehouse>>> Update(int id, [FromBody] Safehouse safehouse)
    {
        try
        {
            if (safehouse.SafehouseId != id)
                return BadRequest(ApiResponse<Safehouse>.Fail("ID in route does not match body"));

            var existing = await _db.Safehouses.FirstOrDefaultAsync(x => x.SafehouseId == id);
            if (existing is null)
                return NotFound(ApiResponse<Safehouse>.Fail("Not found"));

            _db.Entry(existing).CurrentValues.SetValues(safehouse);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<Safehouse>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Safehouse>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.Safehouses.FirstOrDefaultAsync(x => x.SafehouseId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.Safehouses.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

