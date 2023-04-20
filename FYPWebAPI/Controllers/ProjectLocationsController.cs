using FYPWebAPI.Data;
using FYPWebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using static System.Collections.Specialized.BitVector32;
using Section = FYPWebAPI.Models.Section;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FYPWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProjectLocationsController : ControllerBase
    {
        private readonly ApplicationDbContext2 _context;
         

        public ProjectLocationsController(ApplicationDbContext2 context)
        {
            _context = context;
             
        }

        // GET: api/ProjectLocations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectLocation>>> GetProjectLocations()
        {
            return await _context.ProjectLocations.ToListAsync();
        }

        // GET: api/ProjectLocations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectLocation>> GetProjectLocation(int id)
        {
            var projectLocation = await _context.ProjectLocations.FindAsync(id);

            if (projectLocation == null)
            {
                return NotFound();
            }

            return projectLocation;
        }

        // PUT: api/ProjectLocations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectLocation(int id, ProjectLocation projectLocation)
        {
            if (id != projectLocation.id)
            {
                return BadRequest();
            }

            _context.Entry(projectLocation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectLocationExists(id))
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

        // POST: api/ProjectLocations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectLocation>> PostProjectLocation([FromBody] Object post)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(post.ToString());
            if (body["adminPassword"] == null || (body["adminPassword"].ToString() != Globals.AdminPassword))
            {
                return BadRequest();
            }
            ProjectLocation projectLocation = new ProjectLocation();
            Project project = await _context.Maps.FindAsync(body["projectId"]);
            Section section = await _context.Maps.FindAsync(body["sectionId"]);
            projectLocation.Section = section;
            projectLocation.Project = project;
            projectLocation.X = Int32.Parse(body["x"]);
            projectLocation.Y = Int32.Parse(body["y"]);
            _context.ProjectLocations.Update(projectLocation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProjectLocation", new { id = projectLocation.id }, projectLocation);
        }

        [HttpPost("getAll")]
        public async Task<ActionResult<IEnumerable<ProjectLocation>>> GetAllBySections([FromBody] Object sections){
        
            var body = JsonConvert.DeserializeObject<dynamic>(sections.ToString());
            List<string> sectionIds = new();
            foreach (var sectionId in body["sectionIds"])
            {
                sectionIds.Add(sectionId.ToString());
            }
            var projectLocations = await _context.ProjectLocations.Include(mp => mp.Section)
            .Include(mp => mp.Project)
                .Where(mp => sectionIds.Contains(mp.Section.id.ToString()))
                .ToListAsync();

            return projectLocations;
        }


        // DELETE: api/ProjectLocations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectLocation(int id)
        {
            var projectLocation = await _context.ProjectLocations.FindAsync(id);
            if (projectLocation == null)
            {
                return NotFound();
            }

            _context.ProjectLocations.Remove(projectLocation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectLocationExists(int id)
        {
            return _context.ProjectLocations.Any(e => e.id == id);
        }
    }
}
