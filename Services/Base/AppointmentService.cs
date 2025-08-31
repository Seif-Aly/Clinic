using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Appointment;
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

        public async Task<(IEnumerable<AppointmentDto>, int totalPages)> GetAppointments(AppointmantFilterReqest? filter, string role, int? doctorId, int? patientId, int page)
        {
            var totalCount = await _repository.CountFilteredAppointmentsAsync(filter, role, doctorId, patientId);
            var data = await _repository.GetFilteredAppointmentsAsync(filter, role, doctorId, patientId, page);
            var dtoList = _mapper.Map<IEnumerable<AppointmentDto>>(data);
            return (dtoList, (int)Math.Ceiling(totalCount / 6.0));
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

        public async Task<AppointmentDto?> CreateAppointment(CreateAppointmentDto dto, string role, int? doctorId)
        {
            if (role == "Doctor" && dto.DoctorId != doctorId) return null;

            if (!await _context.Patients.AnyAsync(p => p.Id == dto.PatientId) ||
                !await _context.Doctors.AnyAsync(d => d.Id == dto.DoctorId))
                return null;

            var entity = _mapper.Map<Appointment>(dto);
            await _repository.AddAsync(entity);
<<<<<<< HEAD
            return _mapper.Map<AppointmentDto>(entity);
=======

            var saved = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == entity.Id);

            return _mapper.Map<AppointmentDto>(saved);
>>>>>>> main
        }

        public async Task<bool> UpdateAppointment(UpdateAppointmentDto dto)
        {
            var appointment = await _repository.GetByIdAsync(dto.Id);
            if (appointment == null) return false;

            appointment.Status = dto.Status;
            appointment.AppointmentDateTime = dto.AppointmentDateTime;
            appointment.DoctorId = dto.DoctorId;
            appointment.PatientId = dto.PatientId;

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
