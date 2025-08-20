using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System.Services
{
    public interface IPrescriptionItemService
    {
        Task<(IEnumerable<PrescriptionItem> items, int totalPages)> GetPrescriptionItemsAsync(PrescriptionITemFilterRequest? filter, int page);
        Task<PrescriptionItem?> GetPrescriptionItemByIdAsync(int id);
        Task<bool> CreatePrescriptionItemAsync(PrescriptionItem item);
        Task<bool> UpdatePrescriptionItemAsync(PrescriptionItem item);
        Task<bool> DeletePrescriptionItemAsync(int id);
    }
}
