using framework_backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace framework_backend.DTOs
{
    public class NewsLetterDTO
    {
        public string Data { get; set; }

        public IFormFile File { get; set; }


    }
   
}
