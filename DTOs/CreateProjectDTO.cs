using Microsoft.AspNetCore.Mvc;

public class CreateProjectDTO
{
    //[FromForm]
    public CreateProjectDataDTO Project { get; set; }
    
    //[FromForm]
    public List<ArchitectAndRole> Architects { get; set; }
    
//    [FromForm]
//    public List<IFormFile> Images { get; set; }
}

public class CreateProjectDataDTO
{
    public string Title { get; set; }
    public string ShortDescription { get; set; }
    public string LongDescription { get; set; }
    public string Area { get; set; }
    public CreateLocationDTO Location { get; set; }
    public bool ESG { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool Ongoing { get; set; }
}
public class CreateCoordinates
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
public class CreateLocationDTO
{
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string Address { get; set; }
    public CreateCoordinates Coordinates { get; set; } = new();
}

public class ArchitectAndRole
{
    public int ArchitectId { get; set; }
    public string Role { get; set; }
}
public class CreateStatsDTO
{
    public int Likes { get; set; }
    public int Views { get; set; }
}
