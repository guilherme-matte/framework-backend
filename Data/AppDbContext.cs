using framework_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace framework_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Architect> Architects { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Architect>()
     .Property(a => a.Speciality)
     .HasConversion(
         v => JsonSerializer.Serialize(v ?? new List<string>(), (JsonSerializerOptions)null),
         v => string.IsNullOrEmpty(v)
             ? new List<string>()
             : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null));
       
        }

    }
}
