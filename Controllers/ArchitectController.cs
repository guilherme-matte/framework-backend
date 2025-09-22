using framework_backend.Data;
using framework_backend.DTOs;
using framework_backend.Models;
using framework_backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace framework_backend.Controllers
{
    [ApiController]
    [Route("/api/architect")]
    public class ArchitectController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ProjectResponseService _projectResponseService;

        public ArchitectController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArchitectModel>>> GetArchitects()
        {
            return await _context.Architects.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ArchitectModel>> GetArchitect(int id)
        {
            var architect = await _context.Architects.FirstOrDefaultAsync(a => a.Id == id);
            if (architect == null)
            {
                return NotFound();
            }

            return architect;
        }
        [HttpPost]

        public async Task<ActionResult<IEnumerable<ArchitectModel>>> CreateArchitect(ArchitectModel architect)
        {
            if (architect == null) return BadRequest();

            _context.Architects.Add(architect);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArchitect), new { id = architect.Id }, architect);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateArchitect(int id, ArchitectModel architect)
        {

            var existingArchitect = await _context.Architects.FindAsync(id);

            if (existingArchitect == null) return NotFound();

            existingArchitect.Name = architect.Name;
            existingArchitect.Nationality = architect.Nationality;
            existingArchitect.Subtitle = architect.Subtitle;
            existingArchitect.BirthDate = architect.BirthDate;
            existingArchitect.Biography = architect.Biography;
            existingArchitect.Picture = architect.Picture;
            existingArchitect.Verified = architect.Verified;
            existingArchitect.Trending = architect.Trending;
            existingArchitect.Training = architect.Training;
            existingArchitect.SocialMedia = architect.SocialMedia;
            existingArchitect.Stats = architect.Stats;
            existingArchitect.Location = architect.Location;
            existingArchitect.Speciality = architect.Speciality;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteArchitect(int id)
        {
            var architect = await _context.Architects.FindAsync(id);
            if (architect == null) return NotFound();
            _context.Architects.Remove(architect);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("/api/architect/{id}/projects")]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjectsByUserId(int id)
        {
            

            var projectContributors = await _context.ProjectContributors
                .Where(pc => pc.ArchitectId == id)
                .ToListAsync();

            Console.WriteLine($"Found {projectContributors.Count} project contributors for architect ID {id}");
            Console.WriteLine($"Project Contributors: {System.Text.Json.JsonSerializer.Serialize(projectContributors)}");
            var projects = new List<ProjectDTO>();
            foreach (var pc in projectContributors)
            {
                var project = await _context.Projects
                    .Include(p => p.Contributors)
                    .ThenInclude(c => c.Architect)
                    .FirstOrDefaultAsync(p => p.Id == pc.ProjectId);

                if (project == null)
                {
                    Console.WriteLine($"Project not found for ProjectId {pc.ProjectId}");
                    continue; // ignora se o projeto não existir
                }

                var dto = new ProjectDTO
                {
                    Id = project.Id.ToString(),
                    Title = project.Title,
                    ShortDescription = project.ShortDescription,
                    Area = project.Area,
                    Location = project.Location != null ? new LocationDTO
                    {
                        City = project.Location.City,
                        State = project.Location.State,
                        Country = project.Location.Country,
                        Address = project.Location.Address,
                        Coordinates = project.Location.Coordinates != null
                            ? new Coordinates
                            {
                                Latitude = project.Location.Coordinates.Latitude,
                                Longitude = project.Location.Coordinates.Longitude
                            }
                            : new Coordinates()
                    } : null,
                    Images = project.Images ?? new List<string>(),
                    Stats = new StatsDTO
                    {
                        Likes = project.Stats?.Likes ?? 0,
                        Views = project.Stats?.Views ?? 0
                    },
                    LongDescription = project.LongDescription,
                    ESG = project.ESG,
                    Featured = project.Featured,
                    Contributors = project.Contributors?
                        .Select(c => new ContributorsDTO
                        {
                            Id = c.ArchitectId.ToString(),
                            Role = c.Role,
                            Name = c.Architect?.Name,
                            Subtitle = c.Architect?.Subtitle,
                            Picture = c.Architect?.Picture,
                            Trending = c.Architect?.Trending ?? false
                        })
                        .ToList() ?? new List<ContributorsDTO>()
                };

                projects.Add(dto);
            }

            return Ok(projects);
        }

    }
}
