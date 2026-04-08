using Backend.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Description).ToArray() });
        }

        await _userManager.AddToRoleAsync(user, "Donor");
        var token = await BuildJwtTokenAsync(user);
        return Ok(new AuthResponse
        {
            Message = "Registered successfully.",
            AccessToken = token,
            Email = user.Email ?? request.Email,
            Roles = new[] { "Donor" }
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var valid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!valid)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = await BuildJwtTokenAsync(user);

        return Ok(new AuthResponse
        {
            Message = "Logged in.",
            AccessToken = token,
            Email = user.Email ?? request.Email,
            Roles = roles
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        await Task.CompletedTask;
        return Ok(new { message = "Logged out." });
    }

    [HttpGet("me")]
    [AllowAnonymous]
    public async Task<ActionResult> Me()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Ok(new { isAuthenticated = false });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Ok(new { isAuthenticated = false });
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new
        {
            isAuthenticated = true,
            email = user.Email,
            roles
        });
    }

    private async Task<string> BuildJwtTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var issuer = _configuration["Auth:Issuer"] ?? "NorthstarBackend";
        var audience = _configuration["Auth:Audience"] ?? "NorthstarFrontend";
        var secret = _configuration["Auth:JwtSecret"] ?? throw new InvalidOperationException("Auth:JwtSecret missing.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    public class AuthResponse
    {
        public string Message { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    }
}
