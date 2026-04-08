using Backend.Data;
using Backend.Models;
using Backend.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/supporters")]
public class SupportersController : ControllerBase
{
    private readonly AppDbContext _db;

    public SupportersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Supporter>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.Supporters
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<Supporter>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<Supporter>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<Supporter>>> GetById(int id)
    {
        try
        {
            var item = await _db.Supporters.AsNoTracking().FirstOrDefaultAsync(x => x.SupporterId == id);
            if (item is null)
                return NotFound(ApiResponse<Supporter>.Fail("Not found"));

            return Ok(ApiResponse<Supporter>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Supporter>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Supporter>>> Create([FromBody] SupporterUpsertDto dto)
    {
        try
        {
            var supporter = new Supporter
            {
                SupporterType = dto.SupporterType,
                DisplayName = dto.DisplayName,
                OrganizationName = dto.OrganizationName,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                RelationshipType = dto.RelationshipType,
                Region = dto.Region,
                Country = dto.Country,
                Email = dto.Email,
                Phone = dto.Phone,
                Status = dto.Status,
                FirstDonationDate = dto.FirstDonationDate,
                AcquisitionChannel = dto.AcquisitionChannel,
                CreatedAt = dto.CreatedAt
            };

            _db.Supporters.Add(supporter);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = supporter.SupporterId }, ApiResponse<Supporter>.Ok(supporter));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Supporter>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<Supporter>>> Update(int id, [FromBody] SupporterUpsertDto dto)
    {
        try
        {
            var existing = await _db.Supporters.FirstOrDefaultAsync(x => x.SupporterId == id);
            if (existing is null)
                return NotFound(ApiResponse<Supporter>.Fail("Not found"));

            existing.SupporterType = dto.SupporterType;
            existing.DisplayName = dto.DisplayName;
            existing.OrganizationName = dto.OrganizationName;
            existing.FirstName = dto.FirstName;
            existing.LastName = dto.LastName;
            existing.RelationshipType = dto.RelationshipType;
            existing.Region = dto.Region;
            existing.Country = dto.Country;
            existing.Email = dto.Email;
            existing.Phone = dto.Phone;
            existing.Status = dto.Status;
            existing.FirstDonationDate = dto.FirstDonationDate;
            existing.AcquisitionChannel = dto.AcquisitionChannel;
            existing.CreatedAt = dto.CreatedAt;
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<Supporter>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Supporter>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.Supporters.FirstOrDefaultAsync(x => x.SupporterId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.Supporters.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

