using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly UserManager<User> _userManager;
    private const string RoleAdmin = "Admin";
    private const string RoleDoctor = "Doctor";
    private const string RolePatient = "Patient";

    public AuthController(AppDbContext context, IConfiguration config, UserManager<User> userManager)
    {
        _context = context;
        _config = config;
        _userManager = userManager;
    }
    //public class RegisterDto
    //{
    //    public string? Username { get; set; }
    //    public string? Password { get; set; }
    //    public Guid? Role { get; set; } = null;
    //}

    //public class LoginDto
    //{
    //    public string? Username { get; set; }
    //    public string? Password { get; set; }
    //}




    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null)
            return BadRequest("User not found.");

        var result = await _userManager.AddToRoleAsync(user, dto.Role);
        if (result.Succeeded)
            return Ok("Role assigned successfully.");

        return BadRequest("Failed to assign role.");
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Username and password are required.");

        var user = new User
        {
            UserName = dto.Username,
            Email = dto.Username // Assuming email is used as username
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Assign default role if provided
        if (!string.IsNullOrWhiteSpace(dto.Role))
        {
            await _userManager.AddToRoleAsync(user, dto.Role);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        return Ok(new { Token = token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Username and password are required.");

        var user = await _userManager.FindByNameAsync(dto.Username);
        if (user == null)
            return Unauthorized("Invalid username or password.");

        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValid)
            return Unauthorized("Invalid username or password.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        return Ok(new { Token = token });
    }
    private string GenerateJwtToken(User user, IList<string> roles)
    {
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
            };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    //private async  Task<string> GenerateJwtToken(User user)
    //{
    //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
    //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    var claims = new List<Claim>
    //        {
    //            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    //            new Claim(ClaimTypes.Name, user.UserName!),
    //            new Claim(ClaimTypes.Email, user.Email ?? "")
    //        };

    //    // Add roles as claims
    //    var roles = await _userManager.GetRolesAsync(user);
    //    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    //    var token = new JwtSecurityToken(
    //        issuer: _config["Jwt:Issuer"],
    //        audience: _config["Jwt:Audience"],
    //        claims: claims,
    //        expires: DateTime.UtcNow.AddDays(7),
    //        signingCredentials: creds
    //    );

    //    return new JwtSecurityTokenHandler().WriteToken(token);
    //}
    public class AssignRoleDto
    {
        public string UserId { get; set; }
        public string Role { get; set; }
    }

    public class RegisterDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }

    public class LoginDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
