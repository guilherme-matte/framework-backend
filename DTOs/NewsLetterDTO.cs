using framework_backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace framework_backend.DTOs
{
    public class NewsLetterDTO
    {

        public string Data { get; set; }

        public List<IFormFile> Files { get; set; }

        public List<bool> FirstFlags { get; set; }

    }
    public class NewsLetterImagesDTO
    {
        public IFormFile File { get; set; }
        public bool First { get; set; } = false;//flag que informa qual a imagem principal da noticia
    }
}
