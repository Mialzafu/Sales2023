using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales2023.API.Data;
using Sales2023.API.Helpers;
using Sales2023.Shared.DTOs;
using Sales2023.Shared.Entities;

namespace Sales2023.API.Controllers
{
    [ApiController]
    [Route("/api/cities")]

    public class CitiesController : ControllerBase
    {
        private readonly DataContext _context;

        public CitiesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Cities
                                .Where(c => c.State!.Id == pagination.Id)
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
            var queryable = _context.Cities
                                        .Where(c => c.State!.Id == pagination.Id)
                                        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);

            return Ok(totalPages);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(x => x.Id == id);

            if (city == null)
                return NotFound();

            return Ok(city);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(City city)
        {
            try
            {
                _context.Add(city);
                await _context.SaveChangesAsync();
                return Ok(city);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                    return BadRequest("Ya existe una ciudad con el mismo nombre.");

                return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> PutAsync(City city)
        {
            try
            {
                _context.Update(city);
                await _context.SaveChangesAsync();
                return Ok(city);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                    return BadRequest("Ya existe una ciudad con el mismo nombre.");

                return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(x => x.Id == id);

            if (city == null)
                return NotFound();

            _context.Remove(city);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
