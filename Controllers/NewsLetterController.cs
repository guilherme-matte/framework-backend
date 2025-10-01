using framework_backend.Data;
using framework_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace framework_backend.Controllers
{
    [ApiController]
    [Route("/api/news")]
    public class NewsLetterController : ControllerBase
    {
        private readonly AppDbContext _context;
        public NewsLetterController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsLetterModel>>> GetNews()
        {
            return await _context.NewsLetter.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<NewsLetterModel>> GetNewsById(int id)
        {
            var news = await _context.NewsLetter.FindAsync(id);

            if (news != null)
            {
                return news;
            }
            return NotFound("Noticia não encontrada");
        }

    }
}
