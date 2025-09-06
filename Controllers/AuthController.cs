using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTOs.Patient;
using Clinic_Complex_Management_System1.DTOs.Doctor;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IMapper _mapper;
    private readonly IDoctorService _doctorService;
    private readonly IPatientService _patientService;
    private const string RoleAdmin = "Admin";
    private const string RoleDoctor = "Doctor";
    private const string RolePatient = "Patient";

    public AuthController(AppDbContext context,
        IConfiguration config,
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IMapper mapper,
        IDoctorService doctorService,
        IPatientService patientService
        )
    {
        _context = context;
        _config = config;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _doctorService = doctorService;
        _patientService = patientService;
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null)
            return BadRequest(new { message = "User not found." });

        var result = await _userManager.AddToRoleAsync(user, dto.Role);
        if (result.Succeeded)
            return Ok(new { message = "Role assigned successfully." });

        return BadRequest(new { message = "Failed to assign role." });
    }
    // [HttpPost("register")]
    // public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    // {
    //     if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
    //          return BadRequest("Username and password are required.");

    //[HttpPost("register")]
    //// [Authorize(Roles = "Admin")]
    //public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    //{
    //    if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
    //        return BadRequest(new { message = "Email and password are required." });

    //    var role = "Admin";

    //    var existingUser = await _userManager.FindByEmailAsync(dto.Email.Trim().ToLower());
    //    if (existingUser != null)
    //        return BadRequest(new { message = "Email is already registered." });

    //    // Ensure role exists
    //    if (!await _roleManager.RoleExistsAsync(role))
    //        await _roleManager.CreateAsync(new IdentityRole<Guid>(role));

    //    var user = new User
    //    {
    //        UserName = string.IsNullOrWhiteSpace(dto.Username) ? dto.Email.Trim().ToLower() : dto.Username.Trim(),
    //        Email = dto.Email.Trim().ToLower(),
    //        EmailConfirmed = true
    //    };

    //    var create = await _userManager.CreateAsync(user, dto.Password);
    //    if (!create.Succeeded)
    //        return BadRequest(new
    //        {
    //            message = "Failed to create user.",
    //            errors = create.Errors.Select(e => e.Description)
    //        });

    //    var addRole = await _userManager.AddToRoleAsync(user, role);


    //    if (!addRole.Succeeded)
    //        return BadRequest(new
    //        {
    //            message = "Failed to assign role.",
    //            errors = addRole.Errors.Select(e => e.Description)
    //        });

    //    var roles = await _userManager.GetRolesAsync(user);
    //    Console.WriteLine("Roles at login: " + string.Join(",", roles));

    //    var token = GenerateJwtToken(user, roles);
    //    return Ok(new { token });
    //}

    [HttpPost("register-patient")]
    public async Task<IActionResult> RegisterPatient([FromForm] RegisterPatientDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(new { message = "Email is required." });
        if (string.IsNullOrWhiteSpace(dto.Password))
            dto.Password = GenerateMasterPassword();
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
        var createPatientDto = new CreatePatientDto
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            NationalId = dto.NationalId,
            Gender = dto.Gender,
            UserId = user.Id,
        };
        var patientProfile = await _patientService.CreatePatientAsync(createPatientDto);
        var patientResult = new RegisterPatientResult
        {
            Id = patientProfile.Id,
            FullName = patientProfile.FullName,
            Email = patientProfile.Email,
            Phone = patientProfile.Phone,
            NationalId = patientProfile.NationalId,
            Gender = patientProfile.Gender,
            Password = dto.Password // return the generated or provided password
        };
        return Ok(patientResult);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "Email and password are required." });

        var existingUser = await _userManager.FindByEmailAsync(dto.Email.Trim().ToLower());
        if (existingUser != null)
            return BadRequest(new { message = "Email is already registered." });

        // ensure Admin role exists
        if (!await _roleManager.RoleExistsAsync("Admin"))
            await _roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));

        var user = new User
        {
            UserName = dto.Email.Trim().ToLower(),
            Email = dto.Email.Trim().ToLower(),
            EmailConfirmed = true
        };
        var create = await _userManager.CreateAsync(user, dto.Password);
        if (!create.Succeeded)
            return BadRequest(new { message = "Failed to create user.", errors = create.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(user, "Admin");

        var roles = await _userManager.GetRolesAsync(user);
        var token = await GenerateJwtToken(user, roles);
        return Ok(new { token });
    }
    [HttpPost("register-doctor")]
    public async Task<IActionResult> RegisterDoctor([FromForm] RegisterDoctorDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(new { message = "Email is required." });
        if (string.IsNullOrWhiteSpace(dto.Password))
            dto.Password = GenerateMasterPassword();
        var existingUser = await _userManager.FindByEmailAsync(dto.Email.Trim().ToLower());
        if (existingUser != null)
            return BadRequest(new { message = "Email is already registered." });

        // ensure Doctor role exists
        if (!await _roleManager.RoleExistsAsync("Doctor"))
            await _roleManager.CreateAsync(new IdentityRole<Guid>("Doctor"));

        var user = new User
        {
            UserName = dto.Email.Trim().ToLower(),
            Email = dto.Email.Trim().ToLower(),
            EmailConfirmed = true
        };
        var create = await _userManager.CreateAsync(user, dto.Password);
        if (!create.Succeeded)
            return BadRequest(new { message = "Failed to create user.", errors = create.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(user, "Doctor");
        var createDoctorDto = new CreateDoctorDto
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            Specialization = dto.Specialization,
            ClinicId = dto.ClinicId,
            UserId = user.Id,
            Image = dto.Image
        };
        var doctorProfile = await _doctorService.AddDoctorAsync(createDoctorDto);
        var doctorResult = new RegisterDoctorResult
        {
            Id = doctorProfile.Id,
            FullName = doctorProfile.FullName,
            Email = doctorProfile.Email,
            Phone = doctorProfile.Phone,
            Specialization = doctorProfile.Specialization,
            Image = doctorProfile.Image,
            ClinicName = doctorProfile.ClinicName,
            Password = dto.Password // return the generated or provided password
        };
        return Ok(doctorResult);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "Email and password are required." });

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            Console.WriteLine("User not found for email: " + dto.Email);
            return Unauthorized(new { message = "Invalid email or password." });

        }
        else
        {
            Console.WriteLine("User found: " + user.UserName);
        }


        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValid)
        {
            Console.WriteLine("Password invalid for user: " + user.UserName);
            return Unauthorized(new { message = "Invalid email or password." });

        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = await GenerateJwtToken(user, roles);

        return Ok(new { token });
    }

    private string GenerateMasterPassword()
    {
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "@#$%^&*!";

        var random = new Random();

        // Ensure each category is represented
        var passwordChars = new List<char>
    {
        upper[random.Next(upper.Length)],
        lower[random.Next(lower.Length)],
        digits[random.Next(digits.Length)],
        special[random.Next(special.Length)]
    };

        // Fill the rest with random mix
        string allChars = upper + lower + digits + special;
        while (passwordChars.Count < 12)
        {
            passwordChars.Add(allChars[random.Next(allChars.Length)]);
        }

        // Shuffle to avoid predictable pattern (e.g. always starts with uppercase/lower/etc.)
        return new string(passwordChars.OrderBy(_ => random.Next()).ToArray());
    }

    private async Task<string> GenerateJwtToken(User user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
        };
        // Add both the standard Role claim type and a plain "role" claim.
        foreach (var role in roles)
        {
            if (role == "Doctor")
            {
                var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(user.Id);
                claims.Add(new Claim("doctorId", doctorId.ToString()));
            }

            if (role == "Patient")
            {
                var patientId = await _patientService.GetPatientIdByUserIdAsync(user.Id);
                claims.Add(new Claim("patientId", patientId.ToString()));
            }
            claims.Add(new Claim(ClaimTypes.Role, role));
            claims.Add(new Claim("role", role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
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
    public class AssignRoleDto
    {
        public string UserId { get; set; }
        public string Role { get; set; }
    }

    public class RegisterDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
    }

    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
