using System.Numerics;

namespace Clinic_Complex_Management_System1.Models
{
    public class Clinic
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Specialization { get; set; }

      
        public int HospitalId { get; set; }
        public Hospital? Hospital { get; set; }

        public ICollection<Doctor> ?Doctors { get; set; }
    }

}
