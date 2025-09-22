using framework_backend.Data;
using framework_backend.Models;
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
    }
    
}
