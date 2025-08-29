using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.Repositories;
using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System.Services
{
    public class PrescriptionItemService : IPrescriptionItemService
    {
        private readonly IPrescriptionItemRepository _repository;
        private const int PageSize = 6;

        public PrescriptionItemService(IPrescriptionItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<PrescriptionItem> items, int totalPages)> GetPrescriptionItemsAsync(PrescriptionITemFilterRequest? filter, int page)
        {
            var allItems = await _repository.GetAllAsync();

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.MedicalName))
                    allItems = allItems.Where(i => i.MedicineName.Contains(filter.MedicalName, StringComparison.OrdinalIgnoreCase));

                if (filter.PresctiptionId.HasValue)
                    allItems = allItems.Where(i => i.PrescriptionId == filter.PresctiptionId.Value);
            }

            var count = allItems.Count();
            var totalPages = (int)Math.Ceiling(count / (double)PageSize);

            var itemsPaged = allItems.Skip((page - 1) * PageSize).Take(PageSize);

            return (itemsPaged, totalPages);
        }

        public async Task<PrescriptionItem?> GetPrescriptionItemByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> CreatePrescriptionItemAsync(PrescriptionItem item)
        {
            await _repository.AddAsync(item);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> UpdatePrescriptionItemAsync(PrescriptionItem item)
        {
            try
            {
                _repository.Update(item);
                return await _repository.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeletePrescriptionItemAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                return false;

            _repository.Remove(item);
            return await _repository.SaveChangesAsync();
        }
    }
}
