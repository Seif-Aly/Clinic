using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
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
        public DbSet<Profileuser> profileusers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



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

            // Seed roles
            var adminRoleId = new Guid("7dbc8218-e029-4644-a1ca-b27f30fb8a03");
            var doctorRoleId = new Guid("bdc3d729-461f-4f56-8670-07f4e7174854");
            var patientRoleId = new Guid("f9a500aa-4587-4aea-8795-f52fd4fb8581");
            var adminUserId = new Guid("efd76860-e9b8-4a2e-a008-4d0bd4b6bf34");

            modelBuilder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid>
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole<Guid>
                {
                    Id = doctorRoleId,
                    Name = "Doctor",
                    NormalizedName = "DOCTOR"
                },
                new IdentityRole<Guid>
                {
                    Id = patientRoleId,
                    Name = "Patient",
                    NormalizedName = "PATIENT"
                }
            );

            // Seed default Admin user
            var adminUser = new User
            {
                Id = adminUserId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@clinic.com",
                NormalizedEmail = "ADMIN@CLINIC.COM",
                EmailConfirmed = true,
                SecurityStamp = "admin-security-stamp",
                ConcurrencyStamp = "admin-concurrency-stamp",
                PasswordHash = "AQAAAAIAAYagAAAAEE9kuPBZ2JrMW2m6pBLqmawlspU09L0WKUg5hetgTNTIBMMFtLcMM7Kwd8ABzw6uIg=="
            };
            modelBuilder.Entity<User>().HasData(adminUser);

            // Assign Admin role to Admin user
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid>
                {
                    UserId = adminUserId,
                    RoleId = adminRoleId
                }
            );
            // one to one
            modelBuilder.Entity<User>()
              .HasOne<Profileuser>()
              .WithOne(p => p.User)
             .HasForeignKey<Profileuser>(p => p.UserId);
             
             // Doctor <-> User
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithMany() // a User can be linked to many Doctors if needed, usually 1:1
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Patient <-> User
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
