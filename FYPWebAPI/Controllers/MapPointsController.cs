using FYPWebAPI.Data;
using FYPWebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FYPWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MapPointsController : ControllerBase
    {
        private readonly ApplicationDbContext2 _context;

        public MapPointsController(ApplicationDbContext2 context)
        {
            _context = context;
        }

        // GET: api/MapPoints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MapPoint>>> GetMapPoints()
        {
            return await _context.MapPoints.Include(mp => mp.Map).Include(mp => mp.NetworkDevice).ToListAsync();
        }

        [HttpGet("getBySectionId/{id}")]
        public async Task<ActionResult<IEnumerable<MapPoint>>> getBySiteId(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            List<MapPointToSection> mapPointToSections = await _context.MapPointToSections
                .Include(mpts => mpts.MapPoint).Include(mpts => mpts.MapPoint.Map).Include(mpts => mpts.MapPoint.NetworkDevice)
                .Where(mpts => mpts.Section.id == section.id)
                .ToListAsync();
            List<MapPoint> mapPoints = new();
            foreach (var mapPointToSection in mapPointToSections)
            {
                mapPoints.Add(mapPointToSection.MapPoint);
            }

            var mapPointIds = new List<int>();
            var networkDeviceIds = new List<int>();
            var mapPointsToSend = new List<MapPoint>();
            foreach (var mapPoint in mapPoints)
            {
                int duplicate = 0;
                if (mapPointIds.Contains(mapPoint.Map.id))
                {
                    duplicate += 1;
                }
                else
                {
                    mapPointIds.Add(mapPoint.Map.id);
                }

                if (networkDeviceIds.Contains(mapPoint.NetworkDevice.id))
                {
                    duplicate += 1;
                }
                else
                {
                    networkDeviceIds.Add(mapPoint.NetworkDevice.id);
                }

                if (duplicate < 2)
                {
                    mapPointsToSend.Add(mapPoint);
                }
            }

            return mapPointsToSend;
        }

        [HttpPost("getAll")]
        public async Task<ActionResult<IEnumerable<MapPoint>>> GetAllByMaps([FromBody] Object maps)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(maps.ToString());
            List<string> mapIds = new();
            foreach (var mapId in body["mapIds"])
            {
                mapIds.Add(mapId.ToString());
            }
            var mapPoints = await _context.MapPoints
                .Include(mp => mp.NetworkDevice)
                .Include(mp => mp.Map)
                .Where(mp => mapIds.Contains(mp.Map.id.ToString()))
                .ToListAsync();

            var mapPointIds = new List<int>();
            var networkDeviceIds = new List<int>();
            var mapPointsToSend = new List<MapPoint>();
            foreach (var mapPoint in mapPoints)
            {
                bool duplicate = false;
                if (mapPointIds.Contains(mapPoint.id))
                {
                    duplicate = true;
                }
                else
                {
                    mapPointIds.Add(mapPoint.id);
                }

                if (networkDeviceIds.Contains(mapPoint.id))
                {
                    duplicate = true;
                }
                else
                {
                    networkDeviceIds.Add(mapPoint.id);
                }

                if (!duplicate)
                {
                    mapPointsToSend.Add(mapPoint);
                }
            }

            return mapPointsToSend;
        }

        // GET: api/MapPoints/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MapPoint>> GetMapPoint(int id)
        {
            var mapPoint = await _context.MapPoints.FindAsync(id);

            if (mapPoint == null)
            {
                return NotFound();
            }

            return mapPoint;
        }

        // PUT: api/MapPoints/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMapPoint(int id, MapPoint mapPoint)
        {
            if (id != mapPoint.id)
            {
                return BadRequest();
            }

            _context.Entry(mapPoint).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MapPointExists(id))
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

        // POST: api/MapPoints
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MapPoint>> PostMapPoint([FromBody] Object post)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(post.ToString());
            if (body["adminPassword"] == null || (body["adminPassword"].ToString() != Globals.AdminPassword))
            {
                return BadRequest();
            }
            var mapPoint = new MapPoint();
            Map map = await _context.Maps.FindAsync(body["mapId"]);
            NetworkDevice networkDevice = await _context.NetworkDevices.FindAsync(body["networkDeviceId"]);
            mapPoint.Map = map;
            mapPoint.NetworkDevice = networkDevice;
            mapPoint.X = Int32.Parse(body["x"]);
            mapPoint.Y = Int32.Parse(body["y"]);
            mapPoint.Range = Int32.Parse(body["range"]);
            _context.MapPoints.Update(mapPoint);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMapPoint", new { id = mapPoint.id }, mapPoint);
        }

        // DELETE: api/MapPoints/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMapPoint(int id)
        {
            var mapPoint = await _context.MapPoints.FindAsync(id);
            if (mapPoint == null)
            {
                return NotFound();
            }

            _context.MapPoints.Remove(mapPoint);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MapPointExists(int id)
        {
            return _context.MapPoints.Any(e => e.id == id);
        }
    }
}
