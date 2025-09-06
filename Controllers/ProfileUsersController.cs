using Clinic_Complex_Management_System1.DTOs.profileuser;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Repositories.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Complex_Management_System1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileUsersController : ControllerBase
    {
        private readonly IProfileUserRepository _profileUserRepository;

        public ProfileUsersController(IProfileUserRepository profileUserRepository)
        {
            _profileUserRepository = profileUserRepository;
        }
        [HttpGet("GetAllProfiles")]
        public async Task<IActionResult> GetAllProfiles()
        {
            var profiles = await _profileUserRepository.GetAllAsync();
            return Ok(profiles.Adapt<IEnumerable<ProfileDto>>());
        }
        [HttpGet("GetProfileById/{id}")]
        public async Task<IActionResult> GetProfileById(int id)
        {
            var profile = await _profileUserRepository.GetByIdAsync(id);
            if (profile == null) return NotFound();
            return Ok(profile.Adapt<ProfileDto>());
        }
        [HttpPost]
        public async Task<IActionResult> CreateProfile([FromBody] CreateProfileDto dto)
        {
            var profile = dto.Adapt<Profileuser>();
            await _profileUserRepository.AddAsync(profile);
            await _profileUserRepository.SaveChangesAsync();
            return Ok("Successfull Add Profiles User");
        }

        [HttpPut("UpdateProfile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");

            var profilesindb = await _profileUserRepository.GetByIdAsync(id);
            if (profilesindb == null) return NotFound();

            var profiles =dto.Adapt(profilesindb); 
            _profileUserRepository.Update(profiles);
            await _profileUserRepository.SaveChangesAsync();

            return Ok("Successfull Update Profiles user");
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete( int id)
        {
            var profiles = await _profileUserRepository.GetByIdAsync(id);

            if (profiles is not null)
            {
                _profileUserRepository.Delete(profiles);
                await _profileUserRepository.SaveChangesAsync();

                return Ok("Delete Profiles User Successfully");
            }

            return NotFound();

        }

    }
}
