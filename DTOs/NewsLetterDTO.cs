using framework_backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace framework_backend.DTOs
{
    public class NewsLetterDTO
    {
        [FromForm] public string Title { get; set; }
        [FromForm] public DateOnly Date { get; set; }
        [FromForm] public string Excerpt { get; set; }
        [FromForm] public string Category { get; set; }
        [FromForm] public string Tags { get; set; }
        [FromForm] public string BulletPoint { get; set; }


        public List<IFormFile> Files { get; set; }

        public List<bool> FirstFlags { get; set; }

    }
    public class NewsLetterImagesDTO
    {
        public IFormFile File { get; set; }
        public bool First { get; set; } = false;//flag que informa qual a imagem principal da noticia
    }
}
