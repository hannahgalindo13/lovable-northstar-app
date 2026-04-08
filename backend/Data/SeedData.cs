using Backend.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public static class SeedData
{
    public static async Task SeedUsersAndRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var roles = new[] { "Admin", "Donor", "Staff" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = configuration["SeedUsers:Admin:Email"] ?? "admin@example.local";
        var adminPassword = configuration["SeedUsers:Admin:Password"] ?? "AdminPassword1234!";
        var donorEmail = configuration["SeedUsers:Donor:Email"] ?? "donor@example.local";
        var donorPassword = configuration["SeedUsers:Donor:Password"] ?? "DonorPassword1234!";

        await EnsureUserAsync(userManager, adminEmail, adminPassword, "Admin");
        await EnsureUserAsync(userManager, donorEmail, donorPassword, "Donor");
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create seeded user {email}: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
