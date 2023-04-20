using FYPWebAPI.Data;
using FYPWebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty hints, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FYPWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HintsController : ControllerBase
    {
        private readonly ApplicationDbContext2 _context;

        public HintsController(ApplicationDbContext2 context)
        {
            _context = context;
        }

        // GET: api/Hints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hint>>> GetHints()
        {
            return await _context.Hints.Include(h => h.Map).ToListAsync();
        }

        // GET: api/Hints/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hint>> GetHint(int id)
        {
            var hint = await _context.Hints.FindAsync(id);

            if (hint == null)
            {
                return NotFound();
            }

            return hint;
        }

        // PUT: api/Hints/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHint(int id, Hint hint)
        {
            if (id != hint.id)
            {
                return BadRequest();
            }

            _context.Entry(hint).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HintExists(id))
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
        public async Task<ActionResult<IEnumerable<Hint>>> getBySiteId(int id)
        {
            var section = await _context.Sections.Include(s => s.InnerMap).FirstAsync(s => s.id == id);
            var HintToSites = await _context.Hints.Include(e => e.Map).Where(s => s.Map == section.InnerMap).ToListAsync();
            return HintToSites;
        }


        // DELETE: api/Hints/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHint(int id)
        {
            var hint = await _context.Hints.FindAsync(id);
            if (hint == null)
            {
                return NotFound();
            }

            _context.Hints.Remove(hint);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HintExists(int id)
        {
            return _context.Hints.Any(e => e.id == id);
        }
    }
}
