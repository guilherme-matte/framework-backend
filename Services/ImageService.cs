using framework_backend.DTOs;
using Imagekit.Sdk;
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
        News
    }
    public class ImageService
    {
        private readonly ImagekitClient _imageKit;

        public ImageService()
        {
            var publicKey = Environment.GetEnvironmentVariable("IMAGEKIT_PUBLIC_KEY");
            var privateKey = Environment.GetEnvironmentVariable("IMAGEKIT_PRIVATE_KEY");
            var urlEndpoint = Environment.GetEnvironmentVariable("IMAGEKIT_URL_ENDPOINT");

            if (string.IsNullOrWhiteSpace(publicKey) ||
                string.IsNullOrWhiteSpace(privateKey) ||
                string.IsNullOrWhiteSpace(urlEndpoint))
            {
                throw new InvalidOperationException("As variáveis de ambiente do ImageKit não estão configuradas.");
            }

            _imageKit = new ImagekitClient(publicKey, privateKey, urlEndpoint);
        }
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
            if (dto.Images == null || !dto.Images.Any())
                throw new ArgumentException("Imagem inexistente");

            var urls = new List<string>();

            foreach (var img in dto.Images)
            {
                if (!IsValidImage(img))
                    throw new ArgumentException("Imagem inválida");

                using var inputStream = img.OpenReadStream();
                using var compressedStream = new MemoryStream();

                await SaveCompressedImageAsync(inputStream, compressedStream);
                await compressedStream.FlushAsync();

                compressedStream.Position = 0;

                if (compressedStream.Length < 100)
                    throw new IOException($"Compressão falhou: stream muito pequeno ({compressedStream.Length} bytes)");

                var imageBytes = compressedStream.ToArray();

                var uploadRequest = new FileCreateRequest
                {
                    file = imageBytes,
                    fileName = $"{Guid.NewGuid()}_{Path.GetFileName(img.FileName)}",
                    folder = $"/{dto.Source}/{dto.SourceId}"
                };

                var result = await _imageKit.UploadAsync(uploadRequest);

                if (result == null || string.IsNullOrEmpty(result.url))
                {
                    throw new IOException("Falha ao enviar imagem para o ImageKit (resposta nula).");
                }

                urls.Add(result.url.Replace("https://ik.imagekit.io/framework", ""));
            }

            return urls;
        }

        public async Task DeleteImageAsync(string fileId)
        {
            var result = await _imageKit.DeleteFileAsync(fileId);
            if (result == null)
                throw new IOException("Erro ao deletar imagem do ImageKit.");
        }
    }
}
