using Backend.Data;
using Backend.Models;
using Backend.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/donations")]
public class DonationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public DonationsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Donation>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var items = await _db.Donations
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<List<Donation>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<Donation>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<Donation>>> GetById(int id)
    {
        try
        {
            var item = await _db.Donations.AsNoTracking().FirstOrDefaultAsync(x => x.DonationId == id);
            if (item is null)
                return NotFound(ApiResponse<Donation>.Fail("Not found"));

            return Ok(ApiResponse<Donation>.Ok(item));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Donation>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Donation>>> Create([FromBody] DonationUpsertDto dto)
    {
        try
        {
            var donation = new Donation
            {
                SupporterId = dto.SupporterId,
                DonationType = dto.DonationType,
                DonationDate = dto.DonationDate,
                ChannelSource = dto.ChannelSource,
                CurrencyCode = dto.CurrencyCode,
                Amount = dto.Amount,
                EstimatedValue = dto.EstimatedValue,
                ImpactUnit = dto.ImpactUnit,
                IsRecurring = dto.IsRecurring,
                CampaignName = dto.CampaignName,
                Notes = dto.Notes,
                CreatedByPartnerId = dto.CreatedByPartnerId,
                ReferralPostId = dto.ReferralPostId
            };

            _db.Donations.Add(donation);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = donation.DonationId }, ApiResponse<Donation>.Ok(donation));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Donation>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<Donation>>> Update(int id, [FromBody] DonationUpsertDto dto)
    {
        try
        {
            var existing = await _db.Donations.FirstOrDefaultAsync(x => x.DonationId == id);
            if (existing is null)
                return NotFound(ApiResponse<Donation>.Fail("Not found"));

            existing.SupporterId = dto.SupporterId;
            existing.DonationType = dto.DonationType;
            existing.DonationDate = dto.DonationDate;
            existing.ChannelSource = dto.ChannelSource;
            existing.CurrencyCode = dto.CurrencyCode;
            existing.Amount = dto.Amount;
            existing.EstimatedValue = dto.EstimatedValue;
            existing.ImpactUnit = dto.ImpactUnit;
            existing.IsRecurring = dto.IsRecurring;
            existing.CampaignName = dto.CampaignName;
            existing.Notes = dto.Notes;
            existing.CreatedByPartnerId = dto.CreatedByPartnerId;
            existing.ReferralPostId = dto.ReferralPostId;
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<Donation>.Ok(existing));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Donation>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var existing = await _db.Donations.FirstOrDefaultAsync(x => x.DonationId == id);
            if (existing is null)
                return NotFound(ApiResponse<object>.Fail("Not found"));

            _db.Donations.Remove(existing);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

