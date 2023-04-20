using FYPWebAPI.Data;
using FYPWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FYPWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MapsController : ControllerBase
    {
        private readonly ApplicationDbContext2 _context;
         

        public MapsController(ApplicationDbContext2 context)
        {
            _context = context;
             
        }

        // GET: api/Maps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Map>>> GetMaps()
        {
            return await _context.Maps.ToListAsync();
        }

        // GET: api/Maps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Map>> GetMap(int id)
        {
            var map = await _context.Maps.FindAsync(id);

            if (map == null)
            {
                return NotFound();
            }

            return map;
        }

        // PUT: api/Maps/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMap(int id, Map map)
        {
            if (id != map.id)
            {
                return BadRequest();
            }

            _context.Entry(map).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MapExists(id))
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

        // POST: api/Maps
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Map>> CreateMap([FromBody] Object post)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(post.ToString());
            if (body["adminPassword"] == null || (body["adminPassword"].ToString() != Globals.AdminPassword))
            {
                return BadRequest();
            }
            int mapId = 0;
            if (body["id"] != null)
            {
                mapId = Int32.Parse(body["id"].ToString());
            }
            Map map = await _context.Maps.FindAsync(mapId);
            if (map == null)
                {
                map = new Map();
                }

            map.MapImage = body["mapImage"];
            map.MapName = body["mapName"];
            map.ContentType = body["contentType"];
            _context.Maps.Update(map);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMap), new { id = map.id }, map);
        }

        // DELETE: api/Maps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMap(int id)
        {
            var map = await _context.Maps.FindAsync(id);
            if (map == null)
            {
                return NotFound();
            }

            _context.Maps.Remove(map);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MapExists(int id)
        {
            return _context.Maps.Any(e => e.id == id);
        }
    }
}
