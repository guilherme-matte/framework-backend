using framework_backend.DTOs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;


namespace framework_backend.Services
{
    public enum ImageSource
    {
        Projects,
        Architects,
        Other
    }
    public class ImageService
    {
        public async Task SaveCompressedImageAsync(Stream input, Stream output)
        {
            input.Position = 0;
            using var image = await Image.LoadAsync(input);

            var encoder = new JpegEncoder
            {
                Quality = 90, // qualidade visual
                SkipMetadata = true
            };

            await image.SaveAsync(output, encoder);
        }
        public bool ContainsImage(string source, int id)
        {
            var path = Path.Combine("wwwroot/img", source, id.ToString());
            return Directory.Exists(path) && Directory.GetFiles(path).Length > 0;
        }
        private bool IsValidImage(IFormFile file)
        {
            long maxFileSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxFileSize || file.Length < 8) return false;

            try
            {
                using var stream = file.OpenReadStream();
                var result = Image.Load<Rgba32>(stream);
                return result != null;
            }
            catch
            {
                return false;
            }

        }
        public async Task<List<string>> SaveImageAsync(ImageDTO dto)
        {

            if (dto.Images == null || !dto.Images.Any()) throw new ArgumentException("Imagem inexistente");
            string baseDirectory = Path.Combine("wwwroot/img", dto.Source, dto.SourceId.ToString());

            

            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }
            
            var urls = new List<string>();
            foreach (var img in dto.Images)
            {
                if (!IsValidImage(img)) throw new ArgumentException("Imagem inválida");
                if (img.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(img.FileName)}";
                    var filePath = Path.Combine(baseDirectory, fileName);

                    using var inputStream = img.OpenReadStream();
                    using var outputStream = new FileStream(filePath, FileMode.Create);
                    await SaveCompressedImageAsync(inputStream, outputStream);

                    urls.Add($"/img/{dto.Source}/{dto.SourceId}/{fileName}");
                }
            }
            return urls;
        }
        public void DeleteImage(string filePath)
        {
            try
            {
                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");// Define o diretório raiz permitido

                if (!Path.GetFullPath(filePath).StartsWith(rootPath)) throw new UnauthorizedAccessException("Caminho inválido.");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                else
                {
                    throw new FileNotFoundException("Imagem não encontrada.");
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Erro ao deletar imagem.", ex);
            }

        }
    }
}
