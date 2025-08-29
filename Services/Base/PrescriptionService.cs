using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.ViewModels;
using Microsoft.EntityFrameworkCore;

public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _repository;
    private readonly AppDbContext _context;

    public PrescriptionService(IPrescriptionRepository repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<(IEnumerable<Prescription> prescriptions, int totalPages)> GetPrescriptionsAsync(PrescriptionFilterRequest? filter, int page, string? role, int? doctorId, int? patientId)
    {
        return await _repository.GetPrescriptionsAsync(filter, page, role, doctorId, patientId);
    }

    public async Task<Prescription?> GetPrescriptionByIdAsync(int id, string? role, int? doctorId, int? patientId)
    {
        var prescription = await _repository.GetPrescriptionByIdAsync(id);

        if (prescription == null)
            return null;

        // Authorization checks
        if (role == "Patient" && patientId != prescription.PatientId)
            return null;
        if (role == "Doctor" && doctorId != prescription.DoctorId)
            return null;

        return prescription;
    }

    public async Task<bool> CreatePrescriptionAsync(PrescriptionWithItemsVM model, string? role, int? doctorId)
    {
        var appointment = await _context.Appointments.FindAsync(model.AppointmentId);
        if (appointment == null)
            return false;

        if (role == "Doctor" && doctorId != appointment.DoctorId)
            return false;

        var prescription = new Prescription
        {
            Diagnosis = model.Diagnosis,
            Notes = model.Notes,
            DateIssued = model.DateIssued,
            AppointmentId = model.AppointmentId,
            DoctorId = appointment.DoctorId,
            PatientId = appointment.PatientId,
            PrescriptionItems = model.Items.Select(i => new PrescriptionItem
            {
                MedicineName = i.MedicationName,
                Dosage = i.Dosage,
                Instructions = i.Instructions
            }).ToList()
        };

        await _repository.AddPrescriptionAsync(prescription);

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdatePrescriptionAsync(int id, PrescriptionWithItemsVM model, string? role, int? doctorId)
    {
        var prescription = await _repository.GetPrescriptionByIdAsync(id);
        if (prescription == null)
            return false;

        if (role == "Doctor" && prescription.Appointment.DoctorId != doctorId)
            return false;

        prescription.Diagnosis = model.Diagnosis;
        prescription.Notes = model.Notes;
        prescription.DateIssued = model.DateIssued;

        if (prescription.AppointmentId != model.AppointmentId)
        {
            var appt = await _context.Appointments.FindAsync(model.AppointmentId);
            if (appt == null)
                return false;
            if (role == "Doctor" && appt.DoctorId != doctorId)
                return false;

            prescription.AppointmentId = appt.Id;
            prescription.DoctorId = appt.DoctorId;
            prescription.PatientId = appt.PatientId;
        }

        _context.PrescriptionItems.RemoveRange(prescription.PrescriptionItems);
        prescription.PrescriptionItems = model.Items.Select(i => new PrescriptionItem
        {
            MedicineName = i.MedicationName,
            Dosage = i.Dosage,
            Instructions = i.Instructions
        }).ToList();

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeletePrescriptionAsync(int id, string? role, int? doctorId)
    {
        var prescription = await _repository.GetPrescriptionByIdAsync(id);
        if (prescription == null)
            return false;

        if (role == "Doctor" && prescription.Appointment.DoctorId != doctorId)
            return false;

        _context.PrescriptionItems.RemoveRange(prescription.PrescriptionItems);
        _repository.DeletePrescriptionAsync(prescription);

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<Prescription>> GetPrescriptionsByPatientAsync(int patientId, string? role, int? currentPatientId)
    {
        if (role == "Patient" && currentPatientId != patientId)
            return new List<Prescription>();
        if (role == "Doctor")
            return new List<Prescription>();

        return await _context.Prescriptions
            .Where(p => p.PatientId == patientId)
            .Include(p => p.PrescriptionItems)
            .ToListAsync();
    }

    public async Task<IEnumerable<Prescription>> GetPrescriptionsByDoctorAsync(int doctorId, string? role, int? currentDoctorId)
    {
        if (role == "Doctor" && currentDoctorId != doctorId)
            return new List<Prescription>();
        if (role == "Patient")
            return new List<Prescription>();

        return await _context.Prescriptions
            .Where(p => p.DoctorId == doctorId)
            .Include(p => p.PrescriptionItems)
            .ToListAsync();
    }
}
