namespace RecetasAPINet.Services
{
    public class ImageService : IImageService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public ImageService(CloudinaryDotNet.Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded");

            using var stream = file.OpenReadStream();
            var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams
            {
                File = new CloudinaryDotNet.FileDescription(file.FileName, stream),
                Folder = "ImageRecipes"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                 throw new Exception(uploadResult.Error.Message);

            return uploadResult.SecureUrl.ToString();
        }

        public void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return;

            try
            {
                var uri = new Uri(imageUrl);
                var segments = uri.Segments;
                var fileNameWithExtension = segments.Last();
                var publicId = Path.GetFileNameWithoutExtension(fileNameWithExtension);

                // Asumimos que están en la carpeta "ImageRecipes" en Cloudinary
                var deletionParams = new CloudinaryDotNet.Actions.DeletionParams($"ImageRecipes/{publicId}");
                _cloudinary.Destroy(deletionParams);
            }
            catch
            {
                // Ignorar errores en el borrado para no interrumpir el flujo principal
            }
        }

    }
}