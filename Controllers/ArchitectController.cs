using framework_backend.Data;
using framework_backend.DTOs;
using framework_backend.Models;
using framework_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
            var architects = await _context.Architects.ToListAsync();

            if (architects != null && architects.Count != 0)
            {
                return architects;
            }

            return NoContent();
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
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UpdateArchitect(int id, [FromForm] ArchitectForm form)
        {

            if (string.IsNullOrWhiteSpace(form.data)) return BadRequest("Dados do arquiteto não enviados.");
            var architect = JsonSerializer.Deserialize<ArchitectDTO>(form.data, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var existingArchitect = await _context.Architects.FindAsync(id);

            if (existingArchitect == null) return NotFound();

            if (architect == null) return BadRequest("Dados do arquiteto inválidos.");

            ImageDTO imageDTO = new ImageDTO
            {
                SourceId = existingArchitect.Id,
                Source = ImageSource.Architects.ToString(),
                Images = new List<IFormFile> { form.file }
            };

            if (form.file == null || form.file.Length == 0) return BadRequest("Imagem não enviada");

            existingArchitect.Name = architect.Name;
            existingArchitect.Nationality = architect.Nationality;
            existingArchitect.Subtitle = architect.Subtitle;
            existingArchitect.BirthDate = architect.BirthDate;
            existingArchitect.Biography = architect.Biography;
            var imagePath = await _imageService.SaveImageAsync(imageDTO);
            existingArchitect.Picture = imagePath.FirstOrDefault();
            existingArchitect.Training = architect.Training;
            existingArchitect.SocialMedia = architect.SocialMedia;
            existingArchitect.Location = architect.Location;
            existingArchitect.Speciality = architect.Speciality;

            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteArchitect(int id)
        {
            var architect = await _context.Architects.FindAsync(id);
            if (architect == null) return NotFound();
            _context.Architects.Remove(architect);
            await _context.SaveChangesAsync();
            _context.ProjectContributors.RemoveRange(_context.ProjectContributors.Where(pc => pc.ArchitectId == id));
            _imageService.DeleteAllImages(ImageSource.Architects, id);
            return NoContent();
        }
        [HttpDelete("{id}/picture")]
        public async Task<ActionResult> RemoveProfilePicture(int id)
        {

            try
            {
                var architect = await _context.Architects.FindAsync(id);

                if (architect == null) return NotFound();

                await _imageService.DeleteImageAsync(new ImageDTO
                {
                    SourceId = id,
                    Source = ImageSource.Architects.ToString()
                });

                architect.Picture = null;

                _context.Architects.Update(architect);

                await _context.SaveChangesAsync();

                return Ok("Imagem removida com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao remover a imagem: " + ex.Message);
            }

        }
        [HttpGet("/api/architect/{id}/projects")]
        public async Task<ActionResult<IEnumerable<List<ProjectDTO>>>> GetProjectsByUserId(int id)
        {

            var projectContributors = await _context.ProjectContributors
                .Where(pc => pc.ArchitectId == id)
                .ToListAsync();

            var projects = new List<ProjectDTO>();
            foreach (var pc in projectContributors)
            {
                var project = await _context.Projects
                    .Include(p => p.Contributors)
                    .ThenInclude(c => c.Architect)
                    .FirstOrDefaultAsync(p => p.Id == pc.ProjectId);

                if (project == null) continue;

                var dto = await _projectResponseService.ProjectResponse(project);

                projects.Add(dto);
            }

            return Ok(projects);
        }

    }
}
