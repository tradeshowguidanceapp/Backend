
using FYPWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FYPWebAPI.Data
{
    public class ApplicationDbContext2 : DbContext
    {
        public ApplicationDbContext2(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<NetworkDevice> NetworkDevices { get; set; }

        public DbSet<Section> Rooms { get; set; }

        public DbSet<Map> Maps { get; set; }

        public DbSet<MapPoint> MapPoints { get; set; }
        
        public DbSet<Project> Projects { get; set; }

        public DbSet<ProjectLocation> ProjectLocations { get; set; }

        public DbSet<Site> Sites { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<SectionToSite> SectionToSites { get; set; }

        public DbSet<ProjectToSite> ProjectToSites { get; set; }

        public DbSet<ProjectToSection> ProjectToSections { get; set; }
        public DbSet<Vertex> Vertices { get; set; }
        public DbSet<Edge> Edges { get; set; }

        public DbSet<Door> Doors { get; set; }

        public DbSet<Hint> Hints { get; set; }

        public DbSet<FeatureUsage> FeatureUsage { get; set; }

        public DbSet<MapPointToSection> MapPointToSections { get; set; }

    }
}