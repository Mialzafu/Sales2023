using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales2023.API.Data;
using Sales2023.API.Helpers;
using Sales2023.Shared.DTOs;
using Sales2023.Shared.Entities;

namespace Sales2023.API.Controllers
{
    [ApiController]
    [Route("/api/countries")]

    public class CountriesController : ControllerBase
    {
        private readonly DataContext _context;

        public CountriesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Countries
                                    .Include(c => c.States)
                                    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));

                return Ok(await queryable
                                    .OrderBy(c => c.Name)
                                    .Paginate(pagination)
                                    .ToListAsync());
        }

        [HttpGet("totalPages")]
        public async Task<ActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Countries.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            
            return Ok(totalPages);
        }

        [HttpGet("full")]
        public async Task<ActionResult> GetFullAsync()
        {
            return Ok(await _context.Countries
                                .Include(c => c.States!)
                                .ThenInclude(s => s.Cities)
                                .ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetAsync(int id)
        {
            var country = await _context.Countries
                                    .Include(c => c.States!)
                                    .ThenInclude(s => s.Cities)
                                    .FirstOrDefaultAsync(x => x.Id == id);
            
            if (country is null)
                return NotFound();

            return Ok(country);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(Country country)
        {
            try
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return Ok(country);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                    return BadRequest("Ya existe un país con el mismo nombre.");
                else
                    return BadRequest(dbUpdateException.InnerException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> PutAsync(Country country)
        {
            try
            {
                _context.Update(country);
                await _context.SaveChangesAsync();
                return Ok(country);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                    return BadRequest("Ya existe un país con el mismo nombre.");
                else
                    return BadRequest(dbUpdateException.InnerException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var afectedRows = await _context.Countries
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync();

            if (afectedRows == 0)
                return NotFound();

            return NoContent();
        }
    }
}
