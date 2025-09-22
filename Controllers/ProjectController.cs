using framework_backend.Data;
using framework_backend.DTOs;
using framework_backend.Models;
using framework_backend.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace framework_backend.Controllers
{
    [DisableCors]
    [ApiController]
    [Route("/api/project")]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ProjectResponseService _projectResponseService;
        public ProjectController(AppDbContext context)
        {
            _context = context;
            _projectResponseService = new ProjectResponseService(context);

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
                var dto = await _projectResponseService.ProjectResponse(project.Id, contributorIds);
                if (dto != null)
                    projectsDto.Add(dto);
            }

            return Ok(projectsDto);
        }
        [HttpPost]
        public async Task<ActionResult<CreateProjectDTO>> CreateProject(CreateProjectDTO dto)
        {
            if (dto.Project == null || dto.Architects == null || !dto.Architects.Any())
                return BadRequest("Projeto e arquitetos são obrigatórios.");

            var projectModel = new ProjectModel
            {
                Title = dto.Project.Title,
                ShortDescription = dto.Project.ShortDescription,
                LongDescription = dto.Project.LongDescription,
                Area = dto.Project.Area,
                Location = new LocationDTO
                {
                    City = dto.Project.Location.City,
                    State = dto.Project.Location.State,
                    Country = dto.Project.Location.Country,
                    Address = dto.Project.Location.Address,
                    Coordinates = new Coordinates
                    {
                        Latitude = dto.Project.Location?.Coordinates?.Latitude ?? 0,
                        Longitude = dto.Project.Location?.Coordinates?.Longitude ?? 0
                    }
                },
                Images = dto.Project.Images,
                ESG = dto.Project.ESG
                
                

            };

            var architectIds = dto.Architects.Select(a => a.ArchitectId).ToList();
            var existingArchitects = await _context.Architects
                                                   .Where(a => architectIds.Contains(a.Id))
                                                   .ToListAsync();

            projectModel.Contributors ??= new List<ProjectContributors>();
            foreach (var ar in dto.Architects)
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

            _context.Projects.Add(projectModel);
            await _context.SaveChangesAsync();

            var projectDto = await _projectResponseService.ProjectResponse(projectModel.Id, architectIds);
            return CreatedAtAction(nameof(GetProjectById), new { id = projectModel.Id }, projectDto);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDTO>> GetProjectById(int id, [FromQuery] List<int> contributorIds)
        {
            var projectDto = await _projectResponseService.ProjectResponse(id, contributorIds);
            if (projectDto == null) return NotFound();
            return Ok(projectDto);
        }
    }

}
