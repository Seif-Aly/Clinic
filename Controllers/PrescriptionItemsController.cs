using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTOs.PrescriptionItem;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PrescriptionItemsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrescriptionItemDto>>> GetPrescriptionItems()
        {
            var items = await _context.PrescriptionItems.Include(p => p.Prescription).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<PrescriptionItemDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PrescriptionItemDto>> GetPrescriptionItem(int id)
        {
            var item = await _context.PrescriptionItems
                .Include(p => p.Prescription)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (item == null)
                return NotFound();

            return Ok(_mapper.Map<PrescriptionItemDto>(item));
        }

        [HttpPost]
        public async Task<ActionResult<PrescriptionItemDto>> PostPrescriptionItem(CreatePrescriptionItemDto dto)
        {
            var item = _mapper.Map<PrescriptionItem>(dto);

            _context.PrescriptionItems.Add(item);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<PrescriptionItemDto>(item);
            return CreatedAtAction(nameof(GetPrescriptionItem), new { id = item.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrescriptionItem(int id, UpdatePrescriptionItemDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var item = await _context.PrescriptionItems.FindAsync(id);
            if (item == null)
                return NotFound();

            _mapper.Map(dto, item);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.PrescriptionItems.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescriptionItem(int id)
        {
            var item = await _context.PrescriptionItems.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.PrescriptionItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}