using Clinic_Complex_Management_System1.Models;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hospital>()
                .HasMany(h => h.Clinics)
                .WithOne(c => c.Hospital)
                .HasForeignKey(c => c.HospitalId);

            modelBuilder.Entity<Clinic>()
                .HasMany(c => c.Doctors)
                .WithOne(d => d.Clinic)
                .HasForeignKey(d => d.ClinicId);

            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Appointments)
                .WithOne(a => a.Doctor)
                .HasForeignKey(a => a.DoctorId);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Appointments)
                .WithOne(a => a.Patient)
                .HasForeignKey(a => a.PatientId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Prescription)
                .WithOne(p => p.Appointment)
                .HasForeignKey<Prescription>(p => p.AppointmentId);

            modelBuilder.Entity<Prescription>()
                .HasMany(p => p.PrescriptionItems)
                .WithOne(pi => pi.Prescription)
                .HasForeignKey(pi => pi.PrescriptionId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}
