using Backend.Data;
using Backend.Models;
using Backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/donations")]
[Authorize]
public class DonationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public DonationsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<Donation>>> Create([FromBody] DonationUpsertDto dto)
    {
        try
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var supporterId = dto.SupporterId > 0 ? dto.SupporterId : await GetOrCreateAnonymousSupporterIdAsync();

            var donation = new Donation
            {
                SupporterId = supporterId,
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
                ReferralPostId = dto.ReferralPostId,
                // SHOW IN VIDEO: Anonymous donations allowed (UserId = null)
                // SHOW IN VIDEO: Logged-in donations tied to user account
                UserId = userId
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
    [Authorize(Roles = "Admin")]
    // SHOW IN VIDEO: This endpoint requires Admin role
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
    [Authorize(Roles = "Admin")]
    // SHOW IN VIDEO: This endpoint requires Admin role
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

    [HttpGet("my")]
    [Authorize(Roles = "Donor")]
    // SHOW IN VIDEO: Donor endpoint restricted to logged-in user
    // SHOW IN VIDEO: User ID pulled from claims, not request
    // SHOW IN VIDEO: Prevents users from accessing others' data
    // SHOW IN VIDEO: Donors only see their own donations, not anonymous or others
    public async Task<ActionResult<ApiResponse<List<MyDonationResponse>>>> GetMyDonations()
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized(ApiResponse<List<MyDonationResponse>>.Fail("Unauthorized"));
            }

            var items = await _db.Donations
                .AsNoTracking()
                .Where(x => x.UserId == currentUserId)
                .OrderByDescending(x => x.DonationDate)
                .Select(x => new MyDonationResponse
                {
                    DonationId = x.DonationId,
                    Amount = x.Amount,
                    EstimatedValue = x.EstimatedValue,
                    DonationDate = x.DonationDate,
                    ImpactUnit = x.ImpactUnit,
                    DonationType = x.DonationType,
                    Notes = x.Notes
                })
                .ToListAsync();

            return Ok(ApiResponse<List<MyDonationResponse>>.Ok(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<MyDonationResponse>>.Fail(ex.Message));
        }
    }

    private async Task<int> GetOrCreateAnonymousSupporterIdAsync()
    {
        const string anonymousDisplayName = "Anonymous Donor";

        var existing = await _db.Supporters.FirstOrDefaultAsync(s => s.DisplayName == anonymousDisplayName);
        if (existing is not null)
        {
            return existing.SupporterId;
        }

        var supporter = new Supporter
        {
            SupporterType = "Anonymous",
            DisplayName = anonymousDisplayName,
            RelationshipType = "Anonymous",
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };

        _db.Supporters.Add(supporter);
        await _db.SaveChangesAsync();
        return supporter.SupporterId;
    }

    public class MyDonationResponse
    {
        public int DonationId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? EstimatedValue { get; set; }
        public DateOnly DonationDate { get; set; }
        public string? ImpactUnit { get; set; }
        public string DonationType { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}

