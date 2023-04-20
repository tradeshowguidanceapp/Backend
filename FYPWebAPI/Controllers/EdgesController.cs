using FYPWebAPI.Data;
using FYPWebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

// For more information on enabling Web API for empty edges, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FYPWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EdgesController : ControllerBase
    {
        private readonly ApplicationDbContext2 _context;

        public EdgesController(ApplicationDbContext2 context)
        {
            _context = context;
        }

        // GET: api/Edges
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Edge>>> GetEdges()
        {
            return await _context.Edges.ToListAsync();
        }

        // GET: api/Edges/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Edge>> GetEdge(int id)
        {
            var edge = await _context.Edges.FindAsync(id);

            if (edge == null)
            {
                return NotFound();
            }

            return edge;
        }

        // PUT: api/Edges/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEdge(int id, Edge edge)
        {
            if (id != edge.id)
            {
                return BadRequest();
            }

            _context.Entry(edge).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EdgeExists(id))
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
        public async Task<ActionResult<IEnumerable<Edge>>> getBySiteId(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            var EdgeToSites = await _context.Edges.Include(e => e.VertexA).Include(e => e.VertexB).Where(s => s.Section == section).ToListAsync();
            return EdgeToSites;
        }


        // DELETE: api/Edges/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEdge(int id)
        {
            var edge = await _context.Edges.FindAsync(id);
            if (edge == null)
            {
                return NotFound();
            }

            _context.Edges.Remove(edge);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EdgeExists(int id)
        {
            return _context.Edges.Any(e => e.id == id);
        }
    }
}
