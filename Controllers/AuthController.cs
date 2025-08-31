using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.DTOs.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
<<<<<<< HEAD
=======
using Microsoft.AspNetCore.Authorization;
>>>>>>> main


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly UserManager<User> _userManager;
<<<<<<< HEAD
=======
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
>>>>>>> main
    private const string RoleAdmin = "Admin";
    private const string RoleDoctor = "Doctor";
    private const string RolePatient = "Patient";

<<<<<<< HEAD
    public AuthController(AppDbContext context, IConfiguration config, UserManager<User> userManager)
=======
    public AuthController(AppDbContext context, IConfiguration config, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
>>>>>>> main
    {
        _context = context;
        _config = config;
        _userManager = userManager;
<<<<<<< HEAD
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



=======
        _roleManager = roleManager;
    }
>>>>>>> main

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null)
<<<<<<< HEAD
            return BadRequest("User not found.");

        var result = await _userManager.AddToRoleAsync(user, dto.Role);
        if (result.Succeeded)
            return Ok("Role assigned successfully.");

        return BadRequest("Failed to assign role.");
    }
    // [HttpPost("register")]
    // public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    // {
    //     if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
    //          return BadRequest("Username and password are required.");

    //     var user = new User
    //     {
    //         UserName = dto.Username,
    //         Email = dto.Username // Assuming email is used as username
    //     };
    //
    //       var result = await _userManager.CreateAsync(user, dto.Password);
    //     if (!result.Succeeded)
    //       return BadRequest(result.Errors);

    // Assign default role if provided
    // if (!string.IsNullOrWhiteSpace(dto.Role))
    //{
    //  await _userManager.AddToRoleAsync(user, dto.Role);
    //}

    // var roles = await _userManager.GetRolesAsync(user);
    // var token = GenerateJwtToken(user, roles);

    //     return Ok(new { Token = token });
    //  }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Email and password are required.");

        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return BadRequest("Email is already registered.");

        var user = new User
        {
            UserName = dto.Email, 
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        return Ok(new { Token = token });
=======
            return BadRequest(new { message = "User not found." });

        var result = await _userManager.AddToRoleAsync(user, dto.Role);
        if (result.Succeeded)
            return Ok(new { message = "Role assigned successfully." });

        return BadRequest(new { message = "Failed to assign role." });
    }

    [HttpPost("register")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "Email and password are required." });

        var role = "Admin";

        var existingUser = await _userManager.FindByEmailAsync(dto.Email.Trim().ToLower());
        if (existingUser != null)
            return BadRequest(new { message = "Email is already registered." });

        // Ensure role exists
        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole<Guid>(role));

        var user = new User
        {
            UserName = string.IsNullOrWhiteSpace(dto.Username) ? dto.Email.Trim().ToLower() : dto.Username.Trim(),
            Email = dto.Email.Trim().ToLower(),
            EmailConfirmed = true
        };

        var create = await _userManager.CreateAsync(user, dto.Password);
        if (!create.Succeeded)
            return BadRequest(new
            {
                message = "Failed to create user.",
                errors = create.Errors.Select(e => e.Description)
            });

        var addRole = await _userManager.AddToRoleAsync(user, role);

        
        if (!addRole.Succeeded)
            return BadRequest(new
            {
                message = "Failed to assign role.",
                errors = addRole.Errors.Select(e => e.Description)
            });

        var roles = await _userManager.GetRolesAsync(user);
        Console.WriteLine("Roles at login: " + string.Join(",", roles));

        var token = GenerateJwtToken(user, roles);
        return Ok(new { token });
    }

    [AllowAnonymous]
    [HttpPost("register-patient")]
    public async Task<IActionResult> RegisterPatient([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "Email and password are required." });

        var existingUser = await _userManager.FindByEmailAsync(dto.Email.Trim().ToLower());
        if (existingUser != null)
            return BadRequest(new { message = "Email is already registered." });

        // ensure Patient role exists
        if (!await _roleManager.RoleExistsAsync("Patient"))
            await _roleManager.CreateAsync(new IdentityRole<Guid>("Patient"));

        var user = new User
        {
            UserName = dto.Email.Trim().ToLower(),
            Email = dto.Email.Trim().ToLower(),
            EmailConfirmed = true
        };
        var create = await _userManager.CreateAsync(user, dto.Password);
        if (!create.Succeeded)
            return BadRequest(new { message = "Failed to create user.", errors = create.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(user, "Patient");

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);
        return Ok(new { token });
>>>>>>> main
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
<<<<<<< HEAD
            return BadRequest("Email and password are required.");
=======
            return BadRequest(new { message = "Email and password are required." });
>>>>>>> main

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            Console.WriteLine("User not found for email: " + dto.Email);
<<<<<<< HEAD
            return Unauthorized("Invalid email or password.");
=======
            return Unauthorized(new { message = "Invalid email or password." });

>>>>>>> main
        }
        else
        {
            Console.WriteLine("User found: " + user.UserName);
        }


        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValid)
        {
            Console.WriteLine("Password invalid for user: " + user.UserName);
<<<<<<< HEAD
            return Unauthorized("Invalid email or password.");
=======
            return Unauthorized(new { message = "Invalid email or password." });

>>>>>>> main
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

<<<<<<< HEAD
        return Ok(new { Token = token });
=======
        return Ok(new { token });
>>>>>>> main
    }


    private string GenerateJwtToken(User user, IList<string> roles)
    {
        var claims = new List<Claim>
<<<<<<< HEAD
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
=======
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
        };

        // Add both the standard Role claim type and a plain "role" claim.
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));         
            claims.Add(new Claim("role", role));                 
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
>>>>>>> main
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
<<<<<<< HEAD
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
=======
>>>>>>> main
    public class AssignRoleDto
    {
        public string UserId { get; set; }
        public string Role { get; set; }
    }

    public class RegisterDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
<<<<<<< HEAD
        public string? Role { get; set; }
=======
        // public string? Role { get; set; }
>>>>>>> main
        public string? Email { get; set; } 
    }

    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
