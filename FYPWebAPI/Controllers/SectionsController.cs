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
    public class SectionsController : ControllerBase
    {
        private readonly ApplicationDbContext2 _context;
         

        public SectionsController(ApplicationDbContext2 context)
        {
            _context = context;
             
        }

        // GET: api/Sections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Section>>> GetSections()
        {
            return await _context.Sections.ToListAsync();
        }
        
        // GET: api/Sections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Section>> GetSection(int id)
        {
            var section = await _context.Sections.FindAsync(id);

            if (section == null)
            {
                return NotFound();
            }

            return section;
        }

        // PUT: api/Sections/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSection(int id, Section section)
        {
            if (id != section.id)
            {
                return BadRequest();
            }

            _context.Entry(section).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SectionExists(id))
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

        // POST: api/Sections
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Section>> PostSection([FromBody] Object post)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(post.ToString());
            if (body["adminPassword"] == null || (body["adminPassword"].ToString() != Globals.AdminPassword))
            {
                return BadRequest();
            }
            int sectionId = 0;
            if (body["id"] != null)
            {
                sectionId = Int32.Parse(body["id"].ToString());
            }

            Section section = await _context.Sections.FindAsync(sectionId);
            if (section == null)
            {
                section = new Section();
            }

            int innerMapId = Int32.Parse(body["innerMapId"].ToString());
            int outerMapId = Int32.Parse(body["outerMapId"].ToString());
            Map innerMap = await _context.Maps.FindAsync(innerMapId);
            Map outerMap = await _context.Maps.FindAsync(outerMapId);
            section.InnerMap = innerMap;
            section.OuterMap = outerMap;
            section.SectionName = body["name"].ToString();
            section.bottomRightX = body["bottomRightX"];
            section.bottomRightY = body["bottomRightY"];
            section.topLeftX = body["topLeftX"];
            section.topLeftY = body["topLeftY"];
            _context.Sections.Update(section);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSection", new { id = section.id }, section);
        }
        
        [HttpGet("getSectionsOfSection/{id}")]
        public async Task<ActionResult<IEnumerable<Section>>> getSectionsOfSection(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            var sections = await _context.Sections.Where(s => s.OuterMap == section.InnerMap).ToListAsync();
            return sections;
        }

        [HttpGet("getSectionsBySiteId/{id}")]
        public async Task<ActionResult<IEnumerable<Section>>> GetSectionsBySiteId(int id)
        {
            var site = await _context.Sites.Include(s => s.StartSection).FirstOrDefaultAsync(s => s.id == id);
            if (site == null)
            {
                return NotFound();
            }

            var sectionsToSite = _context.SectionToSites.Include(s => s.Section).Include(s => s.Section.InnerMap).Include(s => s.Section.OuterMap).Where(s => s.Site.id == id);
            List<Section> sections = new();
            foreach (SectionToSite sectionToSite in sectionsToSite)
            {
                if (!sections.Any(s => s.Equals(sectionToSite.Site)))
                {
                    sections.Add(sectionToSite.Section);
                }
            }
            return sections;
        }

        private async Task<ActionResult<IEnumerable<Section>>> getSections(Section section, int depth)
        {
            if (depth > 5 || section == null)
            {
                return null;
            }
            var sections = await _context.Sections.Where(s => s.OuterMap == section.InnerMap).ToListAsync();
            var returnSections = new List<Section>();
            foreach (var s in sections)
            {
                returnSections.Add(s);
                ActionResult<IEnumerable<Section>> innerSections = await getSections(s, depth + 1);
                if (innerSections != null && innerSections != null)
                {
                    returnSections.AddRange(innerSections.Value);
                }
            }
            return returnSections;
        }

        // DELETE: api/Sections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSection(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section == null)
            {
                return NotFound();
            }

            _context.Sections.Remove(section);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SectionExists(int id)
        {
            return _context.Sections.Any(e => e.id == id);
        }
    }
}
