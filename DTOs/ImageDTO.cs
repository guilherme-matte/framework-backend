namespace framework_backend.DTOs
{
    public class ImageDTO
    {
        public int Id { get; set; }
        public List<IFormFile> Images { get; set; }
        public string Source { get; set; }//de onde vem a imagem (projeto, arquiteto, etc), é passado como parametro dentro do controller
    }
}
