using framework_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace framework_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Architect> Architects { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Architect>()
                .Property(a => a.BirthDate)
                .HasColumnType("date");

            base.OnModelCreating(modelBuilder);
        }
    }
}
