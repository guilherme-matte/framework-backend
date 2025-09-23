using framework_backend.Data;
using framework_backend.DTOs;
using framework_backend.Models;
using framework_backend.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<CreateProjectDTO>> CreateProject(CreateProjectDTO dto)
        {
            if (dto.Project == null || dto.Architects == null || !dto.Architects.Any())
                return BadRequest("Projeto e arquitetos são obrigatórios.");
            var architectIds = dto.Architects.Select(a => a.ArchitectId).ToList();
            var existingArchitects = await _context.Architects
                                               .Where(a => architectIds.Contains(a.Id))
                                               .ToListAsync();

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

                ESG = dto.Project.ESG,
                StartDate = dto.Project.StartDate,
                OnGoing = dto.Project.Ongoing,


            };
            if (!dto.Project.Ongoing)
            {
                projectModel.EndDate = dto.Project.EndDate;
            }

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

            //using var transaction = await _context.Database.BeginTransactionAsync();

            //try
            //{
            //    _context.Projects.Add(projectModel);
            //    await _context.SaveChangesAsync();

            //    ImageDTO imageDTO = new ImageDTO
            //    {
            //        Id = projectModel.Id,
            //        Source = ImageSource.Projects.ToString(),
            //        Images = dto.Images
            //    };

            //    projectModel.Images.AddRange(await _imageService.SaveImageAsync(imageDTO));
            //    await _context.SaveChangesAsync();

            //    await transaction.CommitAsync();
            //}
            //catch
            //{
            //    await transaction.RollbackAsync();
            //    throw;
            //}

            _context.Projects.Add(projectModel);
            await _context.SaveChangesAsync();
            Console.WriteLine("Project saved with ID: " + projectModel.Id);
            Console.WriteLine("Architect IDs: " + string.Join(", ", architectIds));
            var projectDto = await _projectResponseService.ProjectResponse(projectModel);
            return CreatedAtAction(nameof(GetProjectById), new { projectId = projectModel.Id }, projectDto);
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
