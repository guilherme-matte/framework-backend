using framework_backend.Models;

namespace framework_backend.DTOs
{
    public class NewsLetterDTO
    {
        public string Data { get; set; }
        public List<NewsLetterImagesDTO>? Files { get; set; }
    }
    public class NewsLetterImagesDTO
    {
        public IFormFile File { get; set; }
        public bool First { get; set; } = false;//flag que informa qual a imagem principal da noticia
    }
}
