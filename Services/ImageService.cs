namespace RecetasAPINet.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded");

            var folderPath = Path.Combine(_env.WebRootPath, "ImageRecipes");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var request = _httpContextAccessor.HttpContext!.Request;
            var url = $"{request.Scheme}://{request.Host}/ImageRecipes/{fileName}";

            return url;
        }

        public void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return;

            try
            {
                var uri = new Uri(imageUrl);
                var fileName = Path.GetFileName(uri.LocalPath);
                var filePath = Path.Combine(_env.WebRootPath, "ImageRecipes", fileName);

                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch
            {
                // Ignorar errores en el borrado para no interrumpir el flujo principal
            }
        }

    }
}