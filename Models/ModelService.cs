using Microsoft.EntityFrameworkCore;

namespace framework_backend.Models
{
    
    
    
    [Owned]
    public class Location
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
    }
}
