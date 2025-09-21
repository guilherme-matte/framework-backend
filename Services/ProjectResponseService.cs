using framework_backend.Data;
using framework_backend.DTOs;
using framework_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace framework_backend.Services
{
    public class ProjectResponseService
    {
        public readonly AppDbContext _context;
        public ProjectResponseService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ProjectDTO> ProjectResponse(int projectId, List<int> contributorIds)
        {
            var project = await _context.Projects
                .Include(p => p.Contributors)
                .ThenInclude(c => c.Architect)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) return null;


            var dto = new ProjectDTO
            {
                Id = project.Id.ToString(),
                Title = project.Title,
                ShortDescription = project.ShortDescription,
                Area = project.Area,
                Location = new LocationDTO
                {
                    City = project.Location?.City,
                    Country = project.Location?.Country,
                    State = project.Location?.State,
                    Address = project.Location?.Address,
                    Coordinates = project.Location?.Coordinates != null
                    ? new Coordinates
                    {
                        Latitude = project.Location.Coordinates.Latitude,
                        Longitude = project.Location.Coordinates.Longitude
                    }
                    : new Coordinates()
                },
                Images = project.Images,
                Stats = new StatsDTO
                {
                    Likes = project.Stats?.Likes ?? 0,
                    Views = project.Stats?.Views ?? 0
                },
                LongDescription = project.LongDescription,
                ESG = project.ESG,
                Featured = project.Featured,
                Contributors = project.Contributors
                    .Select(c => new ContributorsDTO
                    {
                        Id = c.ArchitectId.ToString(),
                        Role = c.Role,
                        Name = c.Architect?.Name,
                        Subtitle = c.Architect?.Subtitle,
                        Picture = c.Architect?.Picture,
                        Trending = c.Architect?.Trending ?? false
                    })
                    .ToList()
            };

            return dto;
        }
    }

}
