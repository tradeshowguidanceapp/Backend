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
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext2 _context;
         

        public ProjectsController(ApplicationDbContext2 context)
        {
            _context = context;
             
        }

        // GET: api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // PUT: api/Projects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.id)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // POST: api/Projects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject([FromBody] Object post)
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

            Project project = await _context.Projects.FindAsync(sectionId);
            if (!((project != null)))
            {
                project = new Project();
            }

            string projectId = body["projectId"];
            if (projectId == null)
            {
                projectId = "0";
            }
            project.ProjectName = body["projectName"];
            project.ProjectDescription = body["projectDescription"];
            project.ProjectID = projectId;
            project.ContactEmail = body["contactEmail"];
            project.ContentType = body["contentType"];
            project.ProjectImage = body["projectImage"];
            project.ProjectOwner = body["projectOwner"];
            project.ProjectWebsite = body["projectWebsite"];
            project.Supervisor = body["supervisor"];
            project.Tags = body["tags"];
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.id }, project);
        }
        
        [HttpGet("getBySiteId/{id}")]
        public async Task<ActionResult<IEnumerable<Project>>> getBySiteId(int id)
        {
            var site = await _context.Sites.FindAsync(id);
            var ProjectToSites = await _context.ProjectToSites.Include(p => p.Project).Where(s => s.Site == site).ToListAsync();
            List<Project> projects = new List<Project>();
            ProjectToSites.ForEach(p => projects.Add(p.Project));
            return projects;
        }

        [HttpGet("getBySectionId/{id}")]
        public async Task<ActionResult<IEnumerable<Project>>> getBySectionId(int id)
        {
            var site = await _context.Sections.FindAsync(id);
            var ProjectToSections = await _context.ProjectToSections.Include(p => p.Project).Where(s => s.Section == site).ToListAsync();
            List<Project> projects = new List<Project>();
            ProjectToSections.ForEach(p => projects.Add(p.Project));
            return projects;
        }


        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.id == id);
        }
    }
}
