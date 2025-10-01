using framework_backend.Data;
using framework_backend.DTOs;
using framework_backend.Models;
using framework_backend.Services;
using Humanizer;
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
        [HttpPost]
        [Consumes("multipart/form-data")]

        public async Task<ActionResult> CreateNewsLetter([FromForm] NewsLetterDTO form)
        {
            if (string.IsNullOrEmpty(form.Data)) return BadRequest("Dados não enviados");
            var news = JsonSerializer.Deserialize<NewsLetterModel>(form.Data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine(form.Data);

            Console.WriteLine(form.Files);
            _context.NewsLetter.Add(news);
            await _context.SaveChangesAsync();
            var images = form.Files.Select(f => f.File).ToList();
            var urls = await _imageService.SaveImageAsync(new ImageDTO
            {
                Source = ImageSource.News.ToString(),
                SourceId = news.Id,
                Images = images
            });
            Console.WriteLine(urls[0].ToString());
            Console.WriteLine(form.Files[0].First);
            for (int i = 0; i < urls.Count; i++)
            {
                NewsLetterImages img = new NewsLetterImages();
                img.first = form.Files[i].First;
                img.Image = urls[i];

                news.Images.Add(img);
            }

            _context.NewsLetter.Update(news);
            await _context.SaveChangesAsync();
            return Ok(news);

        }
    }
}
