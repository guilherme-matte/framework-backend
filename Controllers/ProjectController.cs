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
    [Route("/api/project")]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ProjectResponseService _projectResponseService;
        private readonly ImageService _imageService;
        public ProjectController(AppDbContext context)
        {
            _context = context;
            _projectResponseService = new ProjectResponseService(_context);
            _imageService = new ImageService();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetAllProjects([FromQuery] List<int> contributorIds)
        {

            var projects = await _context.Projects
                .Include(p => p.Contributors)
                .ThenInclude(c => c.Architect)
                .ToListAsync();


            var projectsDto = new List<ProjectDTO>();
            foreach (var project in projects)
            {
                var dto = await _projectResponseService.ProjectResponse(project);
                if (dto != null)
                    projectsDto.Add(dto);
            }

            return Ok(projectsDto);
        }
        [HttpPost]
        public async Task<ActionResult<CreateProjectDTO>> CreateProject(CreateProjectDTO form)
        {
            string error = validateForm(form);
            if (error != null)
            {
                return BadRequest(error);
            }
            var projectDto = JsonSerializer.Deserialize<CreateProjectDataDTO>(form.data, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var architectIds = projectDto.Architects.Select(a => a.ArchitectId).ToList();
            var existingArchitects = await _context.Architects
                                               .Where(a => architectIds.Contains(a.Id))
                                               .ToListAsync();

            var projectModel = new ProjectModel
            {
                Title = projectDto.Title,
                ShortDescription = projectDto.ShortDescription,
                LongDescription = projectDto.LongDescription,
                Area = projectDto.Area,
                Location = new LocationDTO
                {
                    City = projectDto.Location.City,
                    State = projectDto.Location.State,
                    Country = projectDto.Location.Country,
                    Address = projectDto.Location.Address,
                    Coordinates = new Coordinates
                    {
                        Latitude = projectDto.Location?.Coordinates?.Latitude ?? 0,
                        Longitude = projectDto.Location?.Coordinates?.Longitude ?? 0
                    }
                },

                ESG = projectDto.ESG,
                StartDate = projectDto.StartDate,
                OnGoing = projectDto.Ongoing,


            };
            if (!projectDto.Ongoing)
            {
                projectModel.EndDate = projectDto.EndDate;
            }

            projectModel.Contributors ??= new List<ProjectContributors>();

            foreach (var ar in projectDto.Architects)
            {
                var architect = existingArchitects.FirstOrDefault(a => a.Id == ar.ArchitectId);
                if (architect != null)
                {
                    projectModel.Contributors.Add(new ProjectContributors
                    {
                        Architect = architect,
                        Role = ar.Role
                    });
                }
            }

            await _context.Projects.AddAsync(projectModel);
            await _context.SaveChangesAsync();

            var imageDTO = new ImageDTO
            {
                SourceId = projectModel.Id,
                Source = ImageSource.Projects.ToString(),
                Images = form.files
            };

            projectDto.Images = await _imageService.SaveImageAsync(imageDTO);
            projectModel.Images = projectDto.Images;
            await _context.SaveChangesAsync();

            var projectResponse = await _projectResponseService.ProjectResponse(projectModel);

            return CreatedAtAction(nameof(GetProjectById), new { projectId = projectModel.Id }, projectDto);
        }

        private string validateForm(CreateProjectDTO form)
        {
            if (form == null)
                return "Dados não enviados";

            if (form.files == null)
                return "Imagens não enviadas";

            return null;
        }

        [HttpGet("{projectId}")]
        public async Task<ActionResult<ProjectDTO>> GetProjectById(int projectId, [FromQuery] List<int> contributorIds)
        {
            var project = await _context.Projects
                .Include(p => p.Contributors)
                .ThenInclude(c => c.Architect)
                .FirstOrDefaultAsync(p => p.Id == projectId);
            var projectDto = await _projectResponseService.ProjectResponse(project);
            if (projectDto == null) return NotFound();
            return Ok(projectDto);
        }

    }

}
