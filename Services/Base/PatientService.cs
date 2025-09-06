using AutoMapper;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Patient;
using Clinic_Complex_Management_System1.DTOs.Patient;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Identity;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    private const int DefaultPageSize = 6;

    public PatientService(IPatientRepository repository,
        IMapper mapper,
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<User> userManager
        )
    {
        _repository = repository;
        _mapper = mapper;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<GetPatientsResult> GetPatientsAsync(PatientFilterRequest? filter, int page, int pageSize = DefaultPageSize)
    {
        var patients = (await _repository.GetAllAsync()).AsQueryable();

        if (filter != null)
        {
            if (!string.IsNullOrEmpty(filter.NamePationt))
                patients = patients.Where(p => p.FullName.Contains(filter.NamePationt));
            if (!string.IsNullOrEmpty(filter.National))
                patients = patients.Where(p => p.NationalId == filter.National);
            if (filter.dateOfBrith.HasValue)
                patients = patients.Where(p => p.DateOfBirth.Date == filter.dateOfBrith.Value.Date);
            if (!string.IsNullOrEmpty(filter.gender))
                patients = patients.Where(p => p.Gender == filter.gender);
        }

        if (page < 1) page = 1;

        var totalCount = patients.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var pagedPatients = patients
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var patientsResult = new GetPatientsResult();
        patientsResult.Patients = pagedPatients;
        patientsResult.TotalPages = totalPages;
        return patientsResult;
    }

    public async Task<PatientDto?> GetPatientByIdAsync(int id)
    {
        var patientEntity = await _repository.GetByIdAsync(id);
        var patientResult = _mapper.Map<PatientDto>(patientEntity);
        return patientResult;
    }


    public async Task<PatientDto> CreatePatientAsync(CreatePatientDto patientDto)
    {

        var patientEntity = _mapper.Map<Patient>(patientDto);
        await _repository.AddAsync(patientEntity);
        var patientResult = _mapper.Map<PatientDto>(patientEntity);
        return patientResult;
    }

    public async Task<bool> UpdatePatientAsync(Patient patient)
    {
        return await _repository.Update(patient);
    }

    //public async Task<bool> DeletePatientAsync(int id)
    //{
    //    var patient = await _repository.GetByIdAsync(id);
    //    if (patient == null)
    //        return false;

    //    _repository.Remove(patient);
    //    //remove user for deleted patient
    //    if (patient.UserId != null)
    //    {
    //        var user = await _userManager.FindByIdAsync(patient.UserId.ToString());
    //        if (user != null)
    //        {
    //            var roles = await _userManager.GetRolesAsync(user);
    //            await _userManager.RemoveFromRolesAsync(user, roles);
    //            await _userManager.DeleteAsync(user);
    //        }
    //    }
    //    return true;

    //}
    public async Task<bool> DeletePatientAsync(int id)
    {
        var patient = await _repository.GetByIdAsync(id);
        if (patient == null) return false;

        // 1) Finish EF removal first (await SaveChangesAsync inside the repo)
        var removed = await _repository.Remove(patient); // <-- implement & await
        if (!removed) return false;

        // 2) Then Identity work (sequentially, awaited)
        if (patient.UserId is Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles?.Count > 0)
                    await _userManager.RemoveFromRolesAsync(user, roles);

                await _userManager.DeleteAsync(user);
            }
        }

        return true;
    }

    public async Task<int> GetPatientIdByUserIdAsync(Guid userId)
    {
        return await _repository.GetPatientIdByUserIdAsync(userId);
    }

    //public async Task<bool> UpdatePatientAsync(UpdatePatientDto patientDto, int id)
    //{
    //    try
    //    {
    //        var patientInDb = await _repository.GetByIdAsync(id);
    //        if (patientInDb == null)
    //            return false;

    //        patientInDb.FullName = patientDto.FullName ?? patientInDb.FullName;
    //        patientInDb.NationalId = patientDto.NationalId ?? patientInDb.NationalId;
    //        patientInDb.Phone = patientDto.Phone ?? patientInDb.Phone;
    //        patientInDb.Gender = patientDto.Gender ?? patientInDb.Gender;
    //        patientInDb.DateOfBirth = patientDto.DateOfBirth ?? patientInDb.DateOfBirth;
    //        patientInDb.Email = patientDto.Email ?? patientInDb.Email;
    //        if (!string.IsNullOrEmpty(patientDto.Email))
    //        {
    //            if (patientInDb.UserId != null)
    //            {
    //                var user = await _userManager.FindByIdAsync(patientInDb.UserId.ToString());
    //                if (user != null)
    //                {
    //                    await _userManager.SetEmailAsync(user, patientDto.Email);
    //                    await _userManager.SetUserNameAsync(user, patientDto.Email);
    //                    _userManager.Dispose();
    //                }
    //            }
    //        }
    //        return await _repository.Update(patientInDb);
    //    }
    //    catch (Exception ex)
    //    {
    //        return false;
    //    }
    //}
    public async Task<bool> UpdatePatientAsync(UpdatePatientDto patientDto, int id)
    {
        var patientInDb = await _repository.GetByIdAsync(id);
        if (patientInDb == null)
            return false;

        patientInDb.FullName = patientDto.FullName ?? patientInDb.FullName;
        patientInDb.NationalId = patientDto.NationalId ?? patientInDb.NationalId;
        patientInDb.Phone = patientDto.Phone ?? patientInDb.Phone;
        patientInDb.Gender = patientDto.Gender ?? patientInDb.Gender;
        patientInDb.DateOfBirth = patientDto.DateOfBirth ?? patientInDb.DateOfBirth;
        patientInDb.Email = patientDto.Email ?? patientInDb.Email;

        // 1) Save patient changes first
        var saved = await _repository.Update(patientInDb); // Ensure this awaits SaveChangesAsync
        if (!saved) return false;

        // 2) Then update Identity user (sequentially)
        if (!string.IsNullOrEmpty(patientDto.Email) && patientInDb.UserId != null)
        {
            var user = await _userManager.FindByIdAsync(patientInDb.UserId.ToString());
            if (user != null)
            {
                // Both are async and awaited, one after the other
                var setEmailResult = await _userManager.SetEmailAsync(user, patientDto.Email);
                if (!setEmailResult.Succeeded) return false;

                var setUserNameResult = await _userManager.SetUserNameAsync(user, patientDto.Email);
                if (!setUserNameResult.Succeeded) return false;
            }
        }

        return true;
    }

}
