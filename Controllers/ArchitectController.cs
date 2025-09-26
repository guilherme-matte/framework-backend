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
        private readonly ImageService _imageService;

        public ArchitectController(AppDbContext context, ImageService imageService)
        {
            _context = context;
            _projectResponseService = new ProjectResponseService(_context);
            _imageService = new ImageService();

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
        public async Task<ActionResult> UpdateArchitect(int id, [FromForm]ArchitectDTO architect)
        {

            var existingArchitect = await _context.Architects.FindAsync(id);

            if (existingArchitect == null) return NotFound();
            ImageDTO imageDTO = new ImageDTO
            {
                Id = existingArchitect.Id,
                Source = ImageSource.Architects.ToString(),
                Images = new List<IFormFile> { architect.Image }
            };

            if (imageDTO == null) return BadRequest("Imagem inexistente");
            existingArchitect.Name = architect.Name;
            existingArchitect.Nationality = architect.Nationality;
            existingArchitect.Subtitle = architect.Subtitle;
            existingArchitect.BirthDate = architect.BirthDate;
            existingArchitect.Biography = architect.Biography;
            var imagePath = await _imageService.SaveImageAsync(imageDTO);
            existingArchitect.Picture = imagePath.FirstOrDefault()??null;
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
        public async Task<ActionResult<IEnumerable<List<ProjectDTO>>>> GetProjectsByUserId(int id)
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
                    continue; 
                }


                var dto = await _projectResponseService.ProjectResponse(project);

                projects.Add(dto);
            }

            return Ok(projects);
        }
        
    }
}
