using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System1.Models;
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
    private const string RoleAdmin = "Admin";
    private const string RoleDoctor = "Doctor";
    private const string RolePatient = "Patient";

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }
    public class RegisterDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
    }

    public class LoginDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.Role))
            return BadRequest("Username, Password and Role are required.");

        var roleNorm = dto.Role.Trim();
        if (roleNorm != RoleAdmin && roleNorm != RoleDoctor && roleNorm != RolePatient)
            return BadRequest("Role must be Admin, Doctor, or Patient.");

        var existingUser = _context.Users.FirstOrDefault(u => u.Username == dto.Username);
        if (existingUser != null)
            return BadRequest("Username already exists.");

        if (roleNorm == RoleDoctor && dto.DoctorId is null)
            return BadRequest("Doctor role requires DoctorId.");
        if (roleNorm == RolePatient && dto.PatientId is null)
            return BadRequest("Patient role requires PatientId.");

        if (dto.DoctorId is not null && !_context.Doctors.Any(d => d.Id == dto.DoctorId))
            return BadRequest("DoctorId not found.");
        if (dto.PatientId is not null && !_context.Patients.Any(p => p.Id == dto.PatientId))
            return BadRequest("PatientId not found.");

        var user = new User
        {
            Username = dto.Username,
            Password = dto.Password,
            Role = roleNorm,
            DoctorId = dto.DoctorId,
            PatientId = dto.PatientId
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto login)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == login.Username && u.Password == login.Password);
        if (user == null)
            return Unauthorized("Invalid credentials.");

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token, user.Id, user.Username, user.Role, user.DoctorId, user.PatientId });
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role ?? string.Empty)
        };

        if (user.DoctorId is not null)
            claims.Add(new Claim("doctorId", user.DoctorId.Value.ToString()));
        if (user.PatientId is not null)
            claims.Add(new Claim("patientId", user.PatientId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
