using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace framework_backend.Models
{
    public class ArchitectModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }
        public string Subtitle { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Biography { get; set; }
        public List<String> Speciality { get; set; } = new();
        public string Picture { get; set; }//armazena URL da foto de perfil

        public bool Verified { get; set; }
        public bool Trending { get; set; }//não encontrei nome melhor para destaques
        [JsonIgnore]
       
        public ArchitectTraining Training { get; set; }
        public ArchitectSocialMedia SocialMedia { get; set; }
        public ArchitectStats Stats { get; set; }
        public Location Location { get; set; }
    }
    [Owned]
    public class ArchitectSocialMedia
    {
        public string Linkedin { get; set; }
        public string Instagram { get; set; }
        public string Portfolio { get; set; }
    }
    [Owned]
    public class ArchitectStats //likes, followers, projetos, etc..
    {
        public int TotalProjects { get; set; }
        public int ESGProjects { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public int Followers { get; set; }
    }
    [Owned]
    public class ArchitectTraining//formação academica
    {
        public string Name { get; set; }
        public int Year { get; set; }
    }

}
