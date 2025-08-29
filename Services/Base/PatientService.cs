using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.DTOs.Patient;
using Clinic_Complex_Management_System1.Models;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;
    private const int DefaultPageSize = 6;

    public PatientService(IPatientRepository repository)
    {
        _repository = repository;
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

    public async Task<Patient?> GetPatientByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> CreatePatientAsync(Patient patient)
    {
        await _repository.AddAsync(patient);
        return await _repository.SaveChangesAsync();
    }

    public async Task<bool> UpdatePatientAsync(Patient patient)
    {
        _repository.Update(patient);
        return await _repository.SaveChangesAsync();
    }

    public async Task<bool> DeletePatientAsync(int id)
    {
        var patient = await _repository.GetByIdAsync(id);
        if (patient == null)
            return false;

        _repository.Remove(patient);
        return await _repository.SaveChangesAsync();
    }


}
