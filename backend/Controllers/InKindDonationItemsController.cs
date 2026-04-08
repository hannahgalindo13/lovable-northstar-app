using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/inkinddonationitems")]
public class InKindDonationItemsController : ControllerBase
{
    private readonly AppDbContext _db;

    public InKindDonationItemsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<InKindDonationItem>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.InKindDonationItems
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<InKindDonationItem>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<InKindDonationItem>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<InKindDonationItem>>> GetById(int id)
    {
        try
        {
            var item = await _db.InKindDonationItems.AsNoTracking().FirstOrDefaultAsync(x => x.ItemId == id);
            if (item is null)
                return NotFound(ApiResponse<InKindDonationItem>.Fail("Not found"));

            return Ok(ApiResponse<InKindDonationItem>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<InKindDonationItem>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<InKindDonationItem>>> Create([FromBody] InKindDonationItem item)
    {
        try
        {
            _db.InKindDonationItems.Add(item);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = item.ItemId }, ApiResponse<InKindDonationItem>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<InKindDonationItem>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<InKindDonationItem>>> Update(int id, [FromBody] InKindDonationItem item)
    {
        try
        {
            if (item.ItemId != id)
                return BadRequest(ApiResponse<InKindDonationItem>.Fail("ID in route does not match body"));

            var existing = await _db.InKindDonationItems.FirstOrDefaultAsync(x => x.ItemId == id);
            if (existing is null)
                return NotFound(ApiResponse<InKindDonationItem>.Fail("Not found"));

            _db.Entry(existing).CurrentValues.SetValues(item);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<InKindDonationItem>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<InKindDonationItem>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.InKindDonationItems.FirstOrDefaultAsync(x => x.ItemId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.InKindDonationItems.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

