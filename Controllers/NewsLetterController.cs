using framework_backend.Data;
using framework_backend.DTOs;
using framework_backend.Models;
using framework_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace framework_backend.Controllers
{
    //[ApiController]
    //[Route("/api/news")]
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
            if (string.IsNullOrEmpty(dto.Data))
                return "Dados não enviados";

            if (dto.FirstFlags == null || !dto.FirstFlags.Any())
                return "Flag de imagem principal não enviada";

            if (dto.FirstFlags.Count(f => f) > 1)
                return "Apenas uma imagem pode ser marcada como principal";

            if (dto.Files == null || !dto.Files.Any())
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

            var news = JsonSerializer.Deserialize<NewsLetterModel>(form.Data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _context.NewsLetter.Add(news);
            await _context.SaveChangesAsync();

            var images = form.Files.Select(f => f).ToList();

            var urls = await _imageService.SaveImageAsync(new ImageDTO
            {
                Source = ImageSource.News.ToString(),
                SourceId = news.Id,
                Images = images
            });

            for (int i = 0; i < urls.Count; i++)
            {
                NewsLetterImages img = new NewsLetterImages();
                img.first = form.FirstFlags[i];
                img.Image = urls[i];

                news.Images.Add(img);
            }

            _context.NewsLetter.Update(news);
            await _context.SaveChangesAsync();
            return Ok(news);

        }
    }
}
