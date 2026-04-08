using Backend.Data;
using Backend.Models.Identity;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
const string CorsPolicyName = "AllowFrontend";

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // SHOW IN VIDEO: Password policy set to 14 characters
        // SHOW IN VIDEO: Identity automatically configures cookie authentication (no duplicate schemes)
        options.Password.RequiredLength = 14;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "northstar.auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.SlidingExpiration = true;
    options.LoginPath = "/login";
});

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        // SHOW IN VIDEO: CORS configured with credentials for secure cookie auth
        policy.WithOrigins("http://localhost:8080", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

await SeedData.SeedUsersAndRolesAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions
{
    CheckConsentNeeded = _ => true,
    MinimumSameSitePolicy = SameSiteMode.Lax,
    Secure = CookieSecurePolicy.Always
});

app.Use(async (context, next) =>
{
    // SHOW IN VIDEO: CSP header visible in browser dev tools
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; connect-src 'self';";

    if (HttpMethods.IsDelete(context.Request.Method))
    {
        var confirm = context.Request.Query["confirm"].ToString();
        if (!string.Equals(confirm, "true", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = "DELETE requires confirm=true query parameter." });
            return;
        }
    }

    await next();
});

app.UseRouting();
app.UseCors(CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SHOW IN VIDEO: Backend runs on fixed port to ensure frontend connectivity
const string BackendUrl = "https://localhost:5048";
Console.WriteLine("Starting backend on https://localhost:5048");

try
{
    await app.RunAsync(BackendUrl);
}
catch (IOException ex) when (ex.InnerException is AddressInUseException || ex.Message.Contains("address already in use", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("Port 5048 is in use. Please stop other instances.");
}
