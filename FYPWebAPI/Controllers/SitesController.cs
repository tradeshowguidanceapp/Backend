using FYPWebAPI.Data;
using FYPWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FYPWebAPI.Controllers
{
    [EnableCors("AllowAll")]
    [Route("[controller]")]
    [ApiController]
    public class SitesController : ControllerBase
    {
        private readonly ApplicationDbContext2 _context;
         

        public SitesController(ApplicationDbContext2 context)
        {
            _context = context;
             
        }

        // GET: api/Sites
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Site>>> GetSites()
        {
            return await _context.Sites.ToListAsync();
        }

        [HttpGet("prod")]
        public async Task<ActionResult<IEnumerable<Site>>> GetSitesProd()
        {
            return await _context.Sites.Where(s => s.id == 8).ToListAsync();
        }   

        // GET: api/Sites/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Site>> GetSite(int id)
        {
            var site = await _context.Sites.Include(s => s.StartSection).Include(s => s.StartSection.OuterMap).Include(s => s.StartSection.InnerMap).FirstOrDefaultAsync((site1 => site1.id == id));

            if (site == null)
            {
                return NotFound();
            }

             return site;
        }


        // PUT: api/Sites/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSite(int id, Site site)
        {
            if (id != site.id)
            {
                return BadRequest();
            }

            _context.Entry(site).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SiteExists(id))
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

        // POST: api/Sites
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Site>> PostSite([FromBody] Object post)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(post.ToString());
            if (body["adminPassword"] == null || (body["adminPassword"].ToString() != Globals.AdminPassword))
            {
                return BadRequest("no password");
            }
            int siteId = 0;
            if (body["id"] != null)
            {
                siteId = Int32.Parse(body["id"].ToString());
            }

            Site site = await _context.Sites.Include(s => s.StartSection).Include(s => s.StartSection.OuterMap)
                .Include(s => s.StartSection.InnerMap).FirstOrDefaultAsync((site1 => site1.id == siteId));
            if (site == null)
            {
                site = new Site();
            }


            site.SiteName = body["SiteName"];
            site.SiteWebsite = body["SiteWebsite"];
            site.SiteDescription = body["SiteDescription"];

            List<string> projectIds = new List<string>();
            foreach (var pId in body["ProjectIds"])
            {
                projectIds.Add(pId.ToString());
            }
            List<ProjectToSite> projectsToRemove =
                await _context.ProjectToSites.Where(ps => ps.Site == site).ToListAsync();
            projectsToRemove.ForEach(ps =>
                _context.ProjectToSites.Remove(ps)
                );

            _context.Sites.Update(site);

            List<Project> projectsToAdd = await _context.Projects.Where(p => projectIds.Contains(p.id.ToString())).ToListAsync();
            projectsToAdd.ForEach(p =>
            {
                ProjectToSite newPS = new ProjectToSite();
                newPS.Project = p;
                newPS.Site = site;
                _context.ProjectToSites.Update(newPS);
            });


            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSite", new { id = site.id }, site);
        }
        

        // DELETE: api/Sites/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSite(int id)
        {
            var site = await _context.Sites.FindAsync(id);
            if (site == null)
            {
                return NotFound();
            }

            _context.Sites.Remove(site);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SiteExists(int id)
        {
            return _context.Sites.Any(e => e.id == id);
        }

        [HttpPost("saveAll")]
        public async Task<ActionResult<Site>> SaveAll([FromBody] Object post)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(post.ToString());
            if (body["adminPassword"] == null || (body["adminPassword"].ToString() != Globals.AdminPassword))
            {
                return BadRequest();
            }
            int siteId = 0;
            if (body["siteId"] != null)
            {
                siteId = Int32.Parse(body["siteId"].ToString());
            }
            if  (siteId <= 0)
            {
                return BadRequest();
            }
            Site site = await _context.Sites.FindAsync(siteId);
            
            Dictionary<int, Map> globalMaps = new();

            String responseMessage = "";


            Dictionary<string, Section> sectionsKeyValues = new();

            List<Project> projectsOfSite = new List<Project>();
            (await _context.ProjectToSites.Include(p2s => p2s.Project)
                .Where(p2s => p2s.Site == site).ToListAsync())
                .ForEach(p2s => projectsOfSite.Add(p2s.Project));

            _context.SectionToSites.RemoveRange(
                _context.SectionToSites.Where(s2s => s2s.Site == site)
                );

            Dictionary<string, Vertex> globalVertices = new();

            var usedMapIds = new List<int>();


            var mapIdsToSection = new Dictionary<int, Section>();


            foreach (var section in body["sections"])
            {
                try
                {
                    Section sectionObject = getSectionOrNew(section).Result;
                    int innerSectionId = 0;
                    if (section["innerMap"] != null && section["innerMap"].ToString() != "")
                    {
                        innerSectionId = Int32.Parse(section["innerMap"]["id"].ToString());
                    }
                    else
                    {
                        continue;
                    }
                    Map innerMap = await _context.Maps.FindAsync(innerSectionId);
                    if (innerMap != null)
                    {
                        innerMap.MapName = section["innerMap"]["mapName"];
                        innerMap.sizingLocation1X = section["innerMap"]["sizingLocation1X"];
                        innerMap.sizingLocation1Y = section["innerMap"]["sizingLocation1Y"];
                        innerMap.sizingLocation2X = section["innerMap"]["sizingLocation2X"];
                        innerMap.sizingLocation2Y = section["innerMap"]["sizingLocation2Y"];
                        innerMap.metresBetweenPoints = section["innerMap"]["metresBetweenPoints"];
                        if (!usedMapIds.Contains(innerSectionId))
                        {
                            usedMapIds.Add(innerSectionId);
                        }
                    }

                    int outerSectionId = 0;
                    if (section["outerMap"] != null && section["outerMap"].ToString() != "")
                    {
                        outerSectionId = Int32.Parse(section["outerMap"]["id"].ToString());
                    }
                    Map outerMap = await _context.Maps.FindAsync(outerSectionId);
                    sectionObject.InnerMap = innerMap;
                    sectionObject.OuterMap = outerMap;
                    sectionObject.topLeftX = section["topLeftX"];
                    sectionObject.topLeftY = section["topLeftY"];
                    sectionObject.bottomRightX = section["bottomRightX"];
                    sectionObject.bottomRightY = section["bottomRightY"];
                    sectionObject.SectionName = section["sectionName"];
                    _context.Sections.Update(sectionObject);

                    mapIdsToSection.Add(innerMap.id, sectionObject);

                    List<MapPointToSection> mapPointToSectionsToRemove = await _context.MapPointToSections
                        .Where(mpts => mpts.Section == sectionObject).ToListAsync();
                    _context.MapPointToSections.RemoveRange(mapPointToSectionsToRemove);

                    SectionToSite newS2S = new();
                    newS2S.Section = sectionObject;
                    newS2S.Site = site;
                    _context.SectionToSites.Update(newS2S);

                    var mapPoints = section["mapPoints"];

                    foreach (var mapPoint in mapPoints)
                    {
                        try
                        {
                            MapPoint mapPointObject = getMapPointOrNew(mapPoint).Result;
                            var networkDeviceId = 0;
                            if (mapPoint["networkDeviceId"] != null && mapPoint["networkDeviceId"].ToString() != "")
                            {
                                networkDeviceId = Int32.Parse(mapPoint["networkDeviceId"].ToString());
                            }
                            else
                            {
                                continue;
                            }

                            NetworkDevice networkDevice =
                                await _context.NetworkDevices.FindAsync(networkDeviceId);
                            var mapId = 0;
                            if (mapPoint["mapId"] != null && mapPoint["mapId"].ToString() != "")
                            {
                                mapId = Int32.Parse(mapPoint["mapId"].ToString());
                            }
                            else
                            {
                                continue;
                            }

                            Map? map;
                            if (globalMaps.ContainsKey(mapId))
                            {
                                map = globalMaps[mapId];
                            }
                            else
                            {
                                map = await _context.Maps.FindAsync(mapId);
                                globalMaps.Add(mapId, map);
                            }

                            List<MapPointToSection> mp2s2R = _context.MapPointToSections.Include(mp => mp.Section).Include(mp => mp.MapPoint).Include(mp => mp.MapPoint.Map).Where(mp => mp.MapPoint.Map.id == map.id && sectionObject.id == mp.Section.id).ToList();
                            //_context.MapPointToSections.RemoveRange(_context.MapPointToSections.Include(mp => mp.Section).Include(mp => mp.MapPoint).Include(mp => mp.MapPoint.Map).Where(mp => mp.MapPoint.Map.id == map.id && sectionObject.id == mp.Section.id).ToList());

                            foreach (var mp2s in mp2s2R)
                            {
                                if (mp2s.id > 0)
                                {
                                    _context.MapPointToSections.Remove(mp2s);
                                }
                            }

                            mapPointObject.NetworkDevice = networkDevice;
                            mapPointObject.Map = map;
                            mapPointObject.X = mapPoint["x"];
                            mapPointObject.Y = mapPoint["y"];
                            mapPointObject.Range = mapPoint["range"];
                            mapPointObject.Name = mapPoint["name"];

                            MapPointToSection mapPointToSection = new();
                            mapPointToSection.MapPoint = mapPointObject;
                            mapPointToSection.Section = sectionObject;

                            _context.MapPointToSections.Update(mapPointToSection);
                            _context.Entry(mapPointObject).State = EntityState.Modified;
                            _context.MapPoints.Update(mapPointObject);
                        }
                        catch {
                            Console.WriteLine("problem with mappoint");
                        }
                    }

                    int updateProjectLocations = 0;

                    try
                    {
                        updateProjectLocations = section["updateProjectLocations"];

                    } catch {
                        Console.WriteLine("Error with project location");
                    }

                    updateProjectLocations = 0;


                    if (updateProjectLocations == 1 && section["projectLocations"] != null)
                    {
                        int added = 0;
                        foreach (var project in section["projectLocations"])
                        {
                            try
                            {
                                ProjectLocation newProjectLocation = new();
                                Project? projectObject = projectsOfSite.Find(p => p.id.ToString() == project["projectId"].ToString());
                                if (projectObject == null)
                                {
                                    continue;
                                }
                                newProjectLocation.Project = projectObject;
                                newProjectLocation.Section = sectionObject;
                                newProjectLocation.X = project["x"];
                                newProjectLocation.Y = project["y"];
                                _context.ProjectLocations.Update(newProjectLocation);
                                added++;
                            }
                            catch
                            {
                                Console.WriteLine("ERROR");
                            }
                        }
                        if (added > 5)
                        {
                            _context.ProjectLocations.RemoveRange(
                            _context.ProjectLocations.Where(pl => pl.Section.Equals(sectionObject)));
                        }
                    }


                    int updateHints = section["updateHints"];

                    if (updateHints == 1)
                    {
                        _context.Hints.RemoveRange(_context.Hints.Where(h => h.Map.Equals(innerMap)));

                        foreach (var hint in section["hints"])
                        {
                            Hint theHint = new Hint();
                            theHint.Map = innerMap;
                            theHint.ContentType = hint["contentType"];
                            theHint.HintImage = hint["hintImage"];
                            theHint.Name = hint["name"];
                            theHint.X = hint["x"];
                            theHint.Y = hint["y"];
                            theHint.HintText = hint["hintText"];
                            _context.Hints.Update(theHint);
                        }
                    }

                    int updateProjects = section["updateProjects"];
                    if (updateProjects == 1)
                    {

                        _context.ProjectToSections.RemoveRange(
                            _context.ProjectToSections.Where(p2s => p2s.Section == sectionObject));

                        foreach (var projectId in section["projects"])
                        {
                            Project theProject = null;
                            foreach (var p in projectsOfSite)
                            {
                                if (p.id == Int32.Parse(projectId.ToString()))
                                {
                                    theProject = p;
                                    break;
                                }
                            }

                            if (theProject != null)
                            {
                                ProjectToSection newP2S = new();
                                newP2S.Section = sectionObject;
                                newP2S.Project = theProject;
                                _context.ProjectToSections.Update(newP2S);
                            }
                        }
                    }

                    int updateRouting = section["updateRouting"];
                    if (updateRouting == 1)
                    {
                        List<Edge> edgesToRemove = await _context.Edges.Where(v => v.Section == sectionObject).ToListAsync();
                        List<Vertex> verticesToRemove = await _context.Vertices.Where(v => v.Section == sectionObject).ToListAsync();

                        bool updateVerticesAndEdges = true;

                        Dictionary<string, Vertex> verticesKeyValues = new();
                        List<Vertex> verticesToAdd = new();
                        foreach (var vertex in section["vertices"])
                        {
                            try
                            {
                                Vertex vertexObject = getVertexOrNew(vertex).Result;
                                vertexObject.X = vertex["x"];
                                vertexObject.Y = vertex["y"];
                                vertexObject.VertexName = vertex["vertexName"];
                                vertexObject.Section = sectionObject;
                                verticesToAdd.Add(vertexObject);
                                verticesKeyValues.Add(vertex["fakeId"].ToString(), vertexObject);
                                globalVertices.Add(vertex["fakeId"].ToString(), vertexObject);

                            }
                            catch (Exception e)
                            {
                                updateVerticesAndEdges = false;
                                String error = e.Message;
                                responseMessage += "Error with vertex: " + vertex["fakeId"] + "\n";
                            }
                        }

                        List<Edge> edgesToAdd = new();

                        foreach (var edge in section["edges"])
                        {
                            try
                            {
                                Edge edgeObject = getEdgeOrNew(edge).Result;
                                edgeObject.VertexA = verticesKeyValues[edge["vertexAfakeId"].ToString()];
                                edgeObject.VertexB = verticesKeyValues[edge["vertexBfakeId"].ToString()];
                                edgeObject.Section = sectionObject;
                                edgesToAdd.Add(edgeObject);
                            }
                            catch (Exception e)
                            {
                                updateVerticesAndEdges = false;
                                responseMessage += "Error with edge: " + e.Message + "\n";
                            }
                        }

                        int lengthToRemoveVertices = verticesToRemove.Count;
                        int lengthToRemoveEdges = edgesToRemove.Count;
                        int lengthToAddVertices = verticesToAdd.Count;
                        int lengthToAddEdges = edgesToAdd.Count;

                        if (lengthToRemoveVertices > 0)
                        {
                            if (lengthToRemoveVertices > lengthToAddVertices)
                            {
                                if ((lengthToRemoveVertices - 10) > lengthToAddVertices)
                                {
                                    updateVerticesAndEdges = false;
                                    responseMessage = "Too many vertices deleted";
                                }
                            }
                        }

                        if (lengthToRemoveEdges > 0)
                        {
                            if ((lengthToRemoveEdges / 2) > lengthToAddEdges)
                            {
                                updateVerticesAndEdges = false;
                                responseMessage = "Too many edges to removed" + lengthToRemoveEdges + ", " + lengthToAddEdges;
                            }
                        }

                        if (section["sectionId"] == 7)
                        {
                            Console.WriteLine(responseMessage);
                        }

                        if (updateVerticesAndEdges)
                        {
                            _context.Edges.RemoveRange(edgesToRemove);
                            _context.Vertices.RemoveRange(verticesToRemove);
                            _context.Edges.UpdateRange(edgesToAdd);
                            for (var i = 0; i < verticesToAdd.Count; i++)
                            {
                                try
                                {
                                    _context.Entry(verticesToAdd[i]).State = EntityState.Added;
                                    _context.Vertices.Update(verticesToAdd[i]);
                                } catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                            }
                            //_context.Vertices.UpdateRange(verticesToAdd);
                        }   
                    }

                    int updateDoors = section["updateDoors"];
                    if (updateDoors > 0)
                    {

                        List<Door> doorsToRemove = _context.Doors.Where(d => d.Section == sectionObject).ToList();

                        _context.Doors.RemoveRange(doorsToRemove);

                        foreach (var door in section["doors"])
                        {
                            try
                            {
                                Door doorObject = getDoorOrNew(door).Result;
                                doorObject.Vertex = globalVertices[door["vertexFakeId"].ToString()];
                                doorObject.Section = sectionObject;
                                doorObject.DoorType = door["doorType"];
                                _context.Doors.Update(doorObject);
                            }
                            catch
                            {
                            }
                        }
                    }


                    sectionsKeyValues.Add(section["fakeId"].ToString(), sectionObject);
                }
                catch { }  
            }

            List<int> sectionsKeyValuesIds = new();
            foreach (var section in sectionsKeyValues)
            {
                sectionsKeyValuesIds.Add(section.Value.id);
            }

            var mapPointsToRemove = await _context.MapPointToSections.Include(mp => mp.Section).Where(m => sectionsKeyValuesIds.Contains(m.Section.id)).ToListAsync();
            _context.MapPointToSections.RemoveRange(mapPointsToRemove);

            site.StartSection = sectionsKeyValues[body["startSectionFakeId"].ToString()];
            _context.Sites.Update(site);
            await _context.SaveChangesAsync();
            //
            // return CreatedAtAction("GetSite", new { id = site.id }, site);
            return Ok(responseMessage);
        }

        public async Task<Edge> getEdgeOrNew(dynamic body)
        {
            var edgeId = 0;
            if (body["id"] != null && body["id"].ToString() != "")
            {
                edgeId = Int32.Parse(body["id"].ToString());
            }
            Edge edgeObject = await _context.Edges.FindAsync(edgeId);
            edgeObject ??= new Edge();

            return edgeObject;
        }

        public async Task<Hint> getHintOrNew(dynamic body)
        {
            var hintId = 0;
            if (body["id"] != null && body["id"].ToString() != "")
            {
                hintId = Int32.Parse(body["id"].ToString());
            }
            Hint hintObject = await _context.Hints.FindAsync(hintId);
            hintObject ??= new Hint();

            return hintObject;
        }

        public async Task<Door> getDoorOrNew(dynamic body)
        {
            var doorId = 0;
            if (body["id"] != null && body["id"].ToString() != "")
            {
                doorId = Int32.Parse(body["id"].ToString());
            }
            Door doorObject = await _context.Doors.FindAsync(doorId);
            doorObject ??= new Door();

            return doorObject;
        }

        public async Task<Vertex> getVertexOrNew(dynamic body)
        {
            var vertexId = 0;
            if (body["id"] != null && body["id"].ToString() != "")
            {
                vertexId = Int32.Parse(body["id"].ToString());
            }
            Vertex vertexObject = await _context.Vertices.FindAsync(vertexId);
            vertexObject ??= new Vertex();

            return vertexObject;
        }

        public async Task<MapPoint> getMapPointOrNew(dynamic body)
        {
            var mapPointId = 0;
            try
            {
                if (body["mapPointId"] != null && body["mapPointId"].ToString() != "")
                {
                    mapPointId = Int32.Parse(body["mapPointId"].ToString());
                }
            }
            catch
            {
            }

            MapPoint mapPointObject = await _context.MapPoints.FindAsync(mapPointId);
            mapPointObject ??= new MapPoint();

            return mapPointObject;
        }

        public async Task<NetworkDevice> getNetworkDeviceOrNew(dynamic body)
        {
            var networkDeviceId = 0;
            if (body["networkDeviceId"] != null)
            {
                networkDeviceId = Int32.Parse(body["networkDeviceId"].ToString());
            }
            NetworkDevice networkDeviceObject = await _context.NetworkDevices.FindAsync(networkDeviceId);
            networkDeviceObject ??= new NetworkDevice();

            return networkDeviceObject;
        }

        public async Task<Section> getSectionOrNew(dynamic body)
        {
            var sectionId = 0;
            if (body["sectionId"] != null)
            {
                sectionId = Int32.Parse(body["sectionId"].ToString());
            }
            Section sectionObject = await _context.Sections.FindAsync(sectionId);
            sectionObject ??= new Section();

            return sectionObject;
        }
    }
}
