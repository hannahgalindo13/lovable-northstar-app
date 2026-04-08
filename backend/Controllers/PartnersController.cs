using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/partners")]
[Authorize]
public class PartnersController : ControllerBase
{
    private readonly AppDbContext _db;

    public PartnersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Partner>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.Partners
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<Partner>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<Partner>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<Partner>>> GetById(int id)
    {
        try
        {
            var item = await _db.Partners.AsNoTracking().FirstOrDefaultAsync(x => x.PartnerId == id);
            if (item is null)
                return NotFound(ApiResponse<Partner>.Fail("Not found"));

            return Ok(ApiResponse<Partner>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Partner>.Fail(ex.Message));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<Partner>>> Create([FromBody] Partner partner)
    {
        try
        {
            _db.Partners.Add(partner);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = partner.PartnerId }, ApiResponse<Partner>.Ok(partner));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Partner>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<Partner>>> Update(int id, [FromBody] Partner partner)
    {
        try
        {
            if (partner.PartnerId != id)
                return BadRequest(ApiResponse<Partner>.Fail("ID in route does not match body"));

            var existing = await _db.Partners.FirstOrDefaultAsync(x => x.PartnerId == id);
            if (existing is null)
                return NotFound(ApiResponse<Partner>.Fail("Not found"));

            _db.Entry(existing).CurrentValues.SetValues(partner);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<Partner>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Partner>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.Partners.FirstOrDefaultAsync(x => x.PartnerId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.Partners.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

