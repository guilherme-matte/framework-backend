using framework_backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace framework_backend.Models
{
    public class ProjectModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Area { get; set; }//em m²
        public LocationDTO Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Ongoing { get; set; }//se o projeto ainda está em andamento
        public List<string> Images { get; set; } = new();//Url das imagens do projeto
        public bool ESG { get; set; }
        public bool Featured { get; set; }
        public ProjectStats Stats { get; set; } = new();
        public List<ProjectContributors> Contributors { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
    [Owned]
    public class Coordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    [Owned]
    public class ProjectStats
    {
        public int Views { get; set; }
        public int Likes { get; set; }

    }

    [Owned]
    public class ProjectContributors
    {
        public int ProjectId { get; set; }
        public ProjectModel ProjectModel { get; set; }
        public int ArchitectId { get; set; }
        public ArchitectModel Architect { get; set; }
        public string Role { get; set; }
    }
}
