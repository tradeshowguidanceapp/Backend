using FYPWebAPI.Data;
using FYPWebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

// For more information on enabling Web API for empty vertices, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FYPWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VerticesController : ControllerBase
    {
        private readonly ApplicationDbContext2 _context;
         

        public VerticesController(ApplicationDbContext2 context)
        {
            _context = context;
             
        }

        // GET: api/Vertices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vertex>>> GetVertices()
        {
            return await _context.Vertices.ToListAsync();
        }

        // GET: api/Vertices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vertex>> GetVertex(int id)
        {
            var vertex = await _context.Vertices.FindAsync(id);

            if (vertex == null)
            {
                return NotFound();
            }

            return vertex;
        }

        // PUT: api/Vertices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVertex(int id, Vertex vertex)
        {
            if (id != vertex.id)
            {
                return BadRequest();
            }

            _context.Entry(vertex).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VertexExists(id))
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
        public async Task<ActionResult<IEnumerable<Vertex>>> getBySiteId(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            var VertexToSites = await _context.Vertices.Where(s => s.Section == section).ToListAsync();
            return VertexToSites;
        }


        // DELETE: api/Vertices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVertex(int id)
        {
            var vertex = await _context.Vertices.FindAsync(id);
            if (vertex == null)
            {
                return NotFound();
            }

            _context.Vertices.Remove(vertex);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VertexExists(int id)
        {
            return _context.Vertices.Any(e => e.id == id);
        }
    }
}
