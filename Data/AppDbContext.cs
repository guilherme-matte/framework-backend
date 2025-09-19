using framework_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace framework_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ArchitectModel> Architects { get; set; }
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<ProjectContributors> ProjectContributors { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ArchitectModel>()
     .Property(a => a.Speciality)
     .HasConversion(
         v => JsonSerializer.Serialize(v ?? new List<string>(), (JsonSerializerOptions)null),
         v => string.IsNullOrEmpty(v)
             ? new List<string>()
             : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null));
       

            // Serializar lista de especialidades do arquiteto
            modelBuilder.Entity<ArchitectModel>()
                .Property(a => a.Speciality)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
                );
            modelBuilder.Entity<ProjectModel>()
                .Property(p => p.Images)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
                );

            // Owned types do arquiteto
            modelBuilder.Entity<ArchitectModel>().OwnsOne(a => a.Stats);
            modelBuilder.Entity<ArchitectModel>().OwnsOne(a => a.Training);
            modelBuilder.Entity<ArchitectModel>().OwnsOne(a => a.SocialMedia);
            modelBuilder.Entity<ArchitectModel>().OwnsOne(a => a.Location);

            // Owned types do projeto
            modelBuilder.Entity<ProjectModel>().OwnsOne(p => p.Stats);
            modelBuilder.Entity<ProjectModel>().OwnsOne(p => p.Location);

            // Chave composta e relacionamento de ProjectContributors
            modelBuilder.Entity<ProjectContributors>()
                .HasKey(pc => new { pc.ProjectId, pc.ArchitectId });

            modelBuilder.Entity<ProjectContributors>()
                .HasOne(pc => pc.ProjectModel)
                .WithMany(p => p.Contributors)
                .HasForeignKey(pc => pc.ProjectId);

            modelBuilder.Entity<ProjectContributors>()
                .HasOne(pc => pc.Architect)
                .WithMany() 
                .HasForeignKey(pc => pc.ArchitectId);

            base.OnModelCreating(modelBuilder);

        }

    }
}
