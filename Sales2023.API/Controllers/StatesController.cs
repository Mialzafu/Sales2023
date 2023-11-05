using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales2023.API.Data;
using Sales2023.API.Helpers;
using Sales2023.Shared.DTOs;
using Sales2023.Shared.Entities;

namespace Sales2023.API.Controllers
{
    [ApiController]
    [Route("/api/states")]

    public class StatesController : ControllerBase
    {
        private readonly DataContext _context;

        public StatesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.States
                                .Include(s => s.Cities)
                                .Where(s => s.Country!.Id == pagination.Id)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));

            return Ok(await queryable
                                    .OrderBy(s => s.Name)
                                    .Paginate(pagination)
                                    .ToListAsync());
        }

        [HttpGet("totalPages")]
        public async Task<ActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.States
                                        .Where(s => s.Country!.Id == pagination.Id)
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
            var state = await _context.States
                                    .Include(x => x.Cities)
                                    .FirstOrDefaultAsync(x => x.Id == id);

            if (state == null)
                return NotFound();

            return Ok(state);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(State state)
        {
            try
            {
                _context.Add(state);
                await _context.SaveChangesAsync();
                return Ok(state);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                    return BadRequest("Ya existe un estado/provincia con el mismo nombre.");

                return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> PutAsync(State state)
        {
            try
            {
                _context.Update(state);
                await _context.SaveChangesAsync();
                return Ok(state);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                    return BadRequest("Ya existe un estado/provincia con el mismo nombre.");

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
            var state = await _context.States.FirstOrDefaultAsync(x => x.Id == id);

            if (state == null)
                return NotFound();

            _context.Remove(state);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
