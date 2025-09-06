using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Appointment;
using Clinic_Complex_Management_System1.DTOs.Appointment;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Repositories.Interfaces;
using Clinic_Complex_Management_System1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System1.Services.Base
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public AppointmentService(IAppointmentRepository repository, IMapper mapper, AppDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<GetAppointmentsResult> GetAppointments(AppointmantFilterReqest? filter, string role, int? doctorId, int? patientId, int page)
        {
            var totalCount = await _repository.CountFilteredAppointmentsAsync(filter, role, doctorId, patientId);
            var data = await _repository.GetFilteredAppointmentsAsync(filter, role, doctorId, patientId, page);
            var dtoList = _mapper.Map<IEnumerable<AppointmentDto>>(data);
            var result = new GetAppointmentsResult()
            {
                Appointments = dtoList.ToList(),
                TotalCount = (int)Math.Ceiling(totalCount / 6.0)
            };
            return result;
        }

        public async Task<AppointmentDto?> GetAppointment(int id, string role, int? doctorId, int? patientId)
        {
            var appointment = await _repository.GetByIdAsync(id);
            if (appointment == null)
                return null;

            if (role == "Doctor" && appointment.DoctorId != doctorId) return null;
            if (role == "Patient" && appointment.PatientId != patientId) return null;

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<AppointmentDto?> CreateAppointment(CreateAppointmentDto dto, string role, int? doctorId, int? patientId)
        {
            if (role == "Doctor")
            {
                if (doctorId == null)
                    return null;
                dto.DoctorId = doctorId;
            }
            if (role == "Patient")
            {
                if (patientId == null)
                    return null;
                dto.PatientId = patientId;
            }

            if (!await _context.Patients.AnyAsync(p => p.Id == dto.PatientId) ||
                !await _context.Doctors.AnyAsync(d => d.Id == dto.DoctorId))
                return null;

            var entity = _mapper.Map<Appointment>(dto);
            await _repository.AddAsync(entity);

            var saved = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == entity.Id);

            return _mapper.Map<AppointmentDto>(saved);
        }

        public async Task<bool> UpdateAppointment(int id, UpdateAppointmentDto dto, string role, int? doctorId, int? patientId)
        {
            var appointment = await _repository.GetByIdAsync(id);
            if (appointment == null) return false;
            if (role == "Doctor")
            {
                if (doctorId == null)
                    return false;
                if (appointment.DoctorId != doctorId)
                    return false;
            }
            if (role == "Patient")
            {
                if (patientId == null)
                    return false;
                if (appointment.PatientId != patientId)
                    return false;
            }
            appointment.Status = dto.Status ?? appointment.Status;
            appointment.AppointmentDateTime = dto.AppointmentDateTime.HasValue ? dto.AppointmentDateTime.Value : appointment.AppointmentDateTime;
            appointment.DoctorId = dto.DoctorId ?? appointment.DoctorId;
            appointment.PatientId = dto.PatientId.HasValue ? dto.PatientId.Value : appointment.PatientId;

            await _repository.UpdateAsync(appointment);
            return true;
        }

        public async Task<bool> DeleteAppointment(int id)
        {
            var appointment = await _repository.GetByIdAsync(id);
            if (appointment == null) return false;

            await _repository.DeleteAsync(appointment);
            return true;
        }
    }

}
