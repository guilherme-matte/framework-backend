using framework_backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace framework_backend.DTOs
{
    public class ArchitectDTO
    {
        public string Name { get; set; }
        public string Nationality { get; set; }
        public string Subtitle { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Biography { get; set; }
        public List<string> Speciality { get; set; } = new();
        public ArchitectTraining Training { get; set; }
        public ArchitectSocialMedia SocialMedia { get; set; }
        public Location Location { get; set; }
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
    }
    public class ArchitectUpdateForm
    {
        [FromForm(Name = "Data")]
        public string Data { get; set; } = null!;

        [FromForm(Name = "File")]
        public IFormFile? Img { get; set; }
    }

}
