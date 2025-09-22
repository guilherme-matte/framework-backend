using framework_backend.DTOs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;


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
        public async Task CompressImageAsync(Stream input, Stream output)
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

        public bool IsValidImage(IFormFile file)
        {
            long maxFileSize = 25 * 1024 * 1024; // 25MB

            if (file.Length > maxFileSize) return false;
            
            if (file.Length < 8) return false;

            byte[] buffer = new byte[8];
            using (var stream = file.OpenReadStream())
            {
                stream.Read(buffer, 0, buffer.Length);
            }

            // PNG
            if (buffer[0] == 0x89 &&
                buffer[1] == 0x50 &&
                buffer[2] == 0x4E &&
                buffer[3] == 0x47 &&
                buffer[4] == 0x0D &&
                buffer[5] == 0x0A &&
                buffer[6] == 0x1A &&
                buffer[7] == 0x0A)
                return true;

            // JPEG
            if (buffer[0] == 0xFF &&
                buffer[1] == 0xD8 &&
                buffer[2] == 0xFF)
                return true;



            return false;
        }
        public async Task<List<string>> SaveImageAsync(ImageDTO dto)
        {

            if (dto.Images == null || !dto.Images.Any()) throw new ArgumentException("Imagem inexistente");
            string baseDirectory = Path.Combine("wwwroot/img", dto.Source, dto.Id.ToString());

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
                    await CompressImageAsync(inputStream, outputStream);

                    urls.Add($"/img/{dto.Source}/{dto.Id}/{fileName}");
                }
            }
            return urls;

        }
    }
}
