using framework_backend.Data;
using framework_backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace framework_backend.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ArchitectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArchitectController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Architect>>> GetArchitects()
        {
            return await _context.Architects.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Architect>> GetArchitect(int id)
        {
            var architect = await _context.Architects.FirstOrDefaultAsync(a => a.Id == id);

            if (architect == null)
            {
                return NotFound();
            }
            return architect;
        }
        [HttpPost]
        public async Task<ActionResult<Architect>> CreateArchitect(Architect architect)
        {
            if (architect == null) return BadRequest();

            _context.Architects.Add(architect);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArchitect), new { id = architect.Id }, architect);
        }
    }
}
