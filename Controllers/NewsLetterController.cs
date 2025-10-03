using framework_backend.Data;
using framework_backend.DTOs;
using framework_backend.Filter;
using framework_backend.Models;
using framework_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace framework_backend.Controllers
{
    [ApiController]
    [Route("/api/news")]
    public class NewsLetterController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ImageService _imageService;


        public NewsLetterController(AppDbContext context, ImageService imageService)
        {
            _imageService = new ImageService();
            _context = context;
        }

        private string ValidateForm(NewsLetterDTO dto)
        {
            if (dto==null)
                return "Dados não enviados";
                        
            if (dto.File == null)
                return "Imagens não enviadas";

            return null; 

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsLetterModel>>> GetNews()
        {
            var news = await _context.NewsLetter.ToListAsync();

            if (news != null && news.Any())
            {
                return news;
            }

            return NoContent();
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
        [HttpPost]
        [Consumes("multipart/form-data")]

        public async Task<ActionResult> CreateNewsLetter([FromForm] NewsLetterDTO form)
        {
            string error = ValidateForm(form);
            if (error != null) return BadRequest(error);
           
            var news = JsonSerializer.Deserialize<NewsLetterModel>(form.Data);

            await _context.NewsLetter.AddAsync(news);

            ImageDTO imageDTO = new ImageDTO
            {
                SourceId = news.Id,
                Source = ImageSource.Architects.ToString(),
                Images = new List<IFormFile> { form.File }
            };
            var imagePath = await _imageService.SaveImageAsync(imageDTO);

            news.Images = imagePath.FirstOrDefault();
            
            
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNewsById), new { id = news.Id }, news);

        }
    }
}
