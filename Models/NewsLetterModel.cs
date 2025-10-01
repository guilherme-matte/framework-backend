namespace framework_backend.Models
{
    public class NewsLetterModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateOnly Date {  get; set; }
        public string Excerpt { get; set; }
        public string Category { get; set; }

        public List<string> Tags { get; set; }
        public List<string> BulletPoint { get; set; }

        public int Views { get; set; }
        public int Likes { get; set; }
    }
}
