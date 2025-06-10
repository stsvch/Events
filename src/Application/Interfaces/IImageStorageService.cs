

namespace Events.Application.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, CancellationToken ct = default);
        Task DeleteAsync(string publicId, CancellationToken ct = default);

        Task<Stream> GetAsync(string publicId, CancellationToken ct = default);
    }
}
