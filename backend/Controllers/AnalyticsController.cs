using Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api")]
[AllowAnonymous]
public class AnalyticsController : ControllerBase
{
    private readonly AppDbContext _db;
    private const string AchievedPlanStatus = "Achieved";

    public AnalyticsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<object>> GetDashboardStats()
    {
        try
        {
            var totalResidents = await _db.Residents.CountAsync();
            var activeResidents = await _db.Residents.CountAsync(x => x.CaseStatus == "Active");
            var safehouseCount = await _db.Safehouses.CountAsync();
            var totalDonations = await _db.Donations.SumAsync(x => x.Amount ?? 0m);

            return Ok(new
            {
                totalResidents,
                activeResidents,
                safehouseCount,
                totalDonations
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("impact")]
    public async Task<ActionResult<object>> GetImpactData()
    {
        try
        {
            var latestSnapshot = await _db.PublicImpactSnapshots
                .AsNoTracking()
                .Where(x => x.IsPublished == 1)
                .OrderByDescending(x => x.SnapshotDate)
                .FirstOrDefaultAsync();

            var totalResidents = await _db.Residents.CountAsync();
            var activeResidents = await _db.Residents.CountAsync(x => x.CaseStatus == "Active");
            var safehouseCount = await _db.Safehouses.CountAsync();
            var totalDonations = await _db.Donations.SumAsync(x => x.Amount ?? 0m);

            var metricsJson = latestSnapshot?.MetricPayloadJson;
            if (string.IsNullOrWhiteSpace(metricsJson))
            {
                metricsJson = $$"""
                {"totalResidents":{{totalResidents}},"activeResidents":{{activeResidents}},"safehouseCount":{{safehouseCount}},"totalDonations":{{totalDonations}}}
                """;
            }

            var donationRows = await _db.Donations
                .AsNoTracking()
                .Select(x => new
                {
                    x.DonationDate,
                    Amount = x.Amount ?? 0m,
                    x.CampaignName
                })
                .ToListAsync();

            var monthlyDonations = donationRows
                .GroupBy(x => new { x.DonationDate.Year, x.DonationDate.Month })
                .OrderBy(x => x.Key.Year)
                .ThenBy(x => x.Key.Month)
                .TakeLast(9)
                .Select(x => new
                {
                    month = new DateTime(x.Key.Year, x.Key.Month, 1).ToString("MMM"),
                    amount = decimal.ToInt32(x.Sum(v => v.Amount))
                })
                .ToList();

            var latestMonthAmount = monthlyDonations.Count > 0 ? monthlyDonations[^1].amount : 0;
            var previousMonthAmount = monthlyDonations.Count > 1 ? monthlyDonations[^2].amount : 0;
            var donationsGrowthPercent = previousMonthAmount > 0
                ? (int)Math.Round(((latestMonthAmount - previousMonthAmount) / (double)previousMonthAmount) * 100)
                : 0;

            var programOutcomes = await _db.InterventionPlans
                .AsNoTracking()
                .GroupBy(x => x.PlanCategory)
                .Select(x => new
                {
                    program = x.Key,
                    rate = x.Count() == 0 ? 0 : (int)Math.Round(100.0 * x.Count(p => p.Status == AchievedPlanStatus) / x.Count())
                })
                .OrderByDescending(x => x.rate)
                .Take(6)
                .ToListAsync();

            var campaigns = donationRows
                .Where(x => !string.IsNullOrWhiteSpace(x.CampaignName))
                .GroupBy(x => x.CampaignName!)
                .Select(x => new
                {
                    name = x.Key,
                    raised = decimal.ToInt32(x.Sum(v => v.Amount))
                })
                .OrderByDescending(x => x.raised)
                .Take(5)
                .ToList();

            var allocationTotals = await _db.DonationAllocations
                .AsNoTracking()
                .GroupBy(x => x.ProgramArea)
                .Select(x => new
                {
                    label = x.Key,
                    amount = x.Sum(v => v.AmountAllocated)
                })
                .OrderByDescending(x => x.amount)
                .Take(4)
                .ToListAsync();

            var totalAllocated = allocationTotals.Sum(x => x.amount);
            var allocationBreakdown = allocationTotals
                .Select(x => new
                {
                    x.label,
                    amount = decimal.ToInt32(x.amount),
                    percent = totalAllocated > 0 ? (int)Math.Round((x.amount / totalAllocated) * 100) : 0
                })
                .ToList();

            return Ok(new
            {
                headline = latestSnapshot?.Headline ?? "Monthly impact update",
                summary = latestSnapshot?.SummaryText ?? "Live metrics reflect current database values.",
                metrics = metricsJson,
                monthlyDonations,
                donationsGrowthPercent,
                programOutcomes,
                campaigns,
                allocationBreakdown
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
