using System.Security.Claims;
using Backend.Data;
using Backend.Models;
using Backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/public")]
public class PublicDonationsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<PublicDonationsController> _logger;

    public PublicDonationsController(AppDbContext db, ILogger<PublicDonationsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpPost("donate")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> Donate([FromBody] PublicDonationRequestDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (dto.Amount <= 0)
        {
            return BadRequest(ApiResponse<object>.Fail("Amount must be greater than 0."));
        }

        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var supporterId = await GetOrCreateSupporterIdAsync(dto.DonorName.Trim(), dto.Email?.Trim());

            var donation = new Donation
            {
                SupporterId = supporterId,
                DonationType = "Monetary",
                DonationDate = DateOnly.FromDateTime(DateTime.UtcNow),
                ChannelSource = "PublicDonationPage",
                CurrencyCode = "USD",
                Amount = dto.Amount,
                EstimatedValue = null,
                ImpactUnit = null,
                IsRecurring = 0,
                CampaignName = null,
                Notes = null,
                CreatedByPartnerId = null,
                ReferralPostId = null,
                UserId = currentUserId,
                DonorName = dto.DonorName.Trim(),
                Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim()
            };

            _db.Donations.Add(donation);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new
            {
                donationId = donation.DonationId,
                amount = donation.Amount,
                donationDate = donation.DonationDate,
                donorName = donation.DonorName
            }, "Thank you for your donation!"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Public donation failed.");
            return StatusCode(500, ApiResponse<object>.Fail("Unable to process donation right now."));
        }
    }

    private async Task<int> GetOrCreateSupporterIdAsync(string donorName, string? email)
    {
        var normalizedEmail = string.IsNullOrWhiteSpace(email) ? null : email.ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(normalizedEmail))
        {
            var existingByEmail = await _db.Supporters.FirstOrDefaultAsync(s => s.Email != null && s.Email.ToLower() == normalizedEmail);
            if (existingByEmail is not null)
            {
                if (!string.Equals(existingByEmail.DisplayName, donorName, StringComparison.Ordinal))
                {
                    existingByEmail.DisplayName = donorName;
                }
                return existingByEmail.SupporterId;
            }
        }

        var supporter = new Supporter
        {
            SupporterType = "Individual",
            DisplayName = donorName,
            RelationshipType = "Donor",
            Email = string.IsNullOrWhiteSpace(email) ? null : email,
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            AcquisitionChannel = "PublicDonationPage"
        };

        _db.Supporters.Add(supporter);
        await _db.SaveChangesAsync();
        return supporter.SupporterId;
    }
}
