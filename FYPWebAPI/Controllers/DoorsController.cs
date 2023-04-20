using FYPWebAPI.Data;
using FYPWebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

// For more information on enabling Web API for empty doors, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FYPWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DoorsController : ControllerBase
    {
        private readonly ApplicationDbContext2 _context;

        public DoorsController(ApplicationDbContext2 context)
        {
            _context = context;
        }

        // GET: api/Doors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Door>>> GetDoors()
        {
            return await _context.Doors.Include(d => d.Vertex).Include(d => d.Section).ToListAsync();
        }

        // GET: api/Doors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Door>> GetDoor(int id)
        {
            var door = await _context.Doors.FindAsync(id);

            if (door == null)
            {
                return NotFound();
            }

            return door;
        }

        // PUT: api/Doors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoor(int id, Door door)
        {
            if (id != door.id)
            {
                return BadRequest();
            }

            _context.Entry(door).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpGet("getBySectionId/{id}")]
        public async Task<ActionResult<IEnumerable<Door>>> getBySiteId(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            var DoorToSites = await _context.Doors.Include(e => e.Vertex).Include(e => e.Section).Where(s => s.Section == section).ToListAsync();
            return DoorToSites;
        }


        // DELETE: api/Doors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoor(int id)
        {
            var door = await _context.Doors.FindAsync(id);
            if (door == null)
            {
                return NotFound();
            }

            _context.Doors.Remove(door);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DoorExists(int id)
        {
            return _context.Doors.Any(e => e.id == id);
        }
    }
}
