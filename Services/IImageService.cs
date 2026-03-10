namespace RecetasAPINet.Services
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile file);
    }
}