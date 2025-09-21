using framework_backend.Models;

namespace framework_backend.DTOs
{
    public class ProjectDTO
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Area { get; set; }
        public LocationDTO Location { get; set; }
        public List<string> Images { get; set; } = new();
        public StatsDTO Stats { get; set; }
        public string LongDescription { get; set; }
        public bool ESG { get; set; }
        public bool Featured { get; set; }
        public List<ContributorsDTO> Contributors { get; set; } = new();
    }

    public class LocationDTO
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public Coordinates Coordinates { get; set; } = new();
    }

    public class StatsDTO
    {
        public int Likes { get; set; }
        public int Views { get; set; }
    }

    public class ContributorsDTO
    {
        public string Id { get; set; }
        public bool Trending { get; set; }
        public string Name { get; set; }
        public string Subtitle { get; set; }
        public string Role { get; set; }
        public string Picture { get; set; }
    }

}
