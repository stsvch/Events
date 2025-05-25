using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Events.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace Events.Infrastructure.Services.Images
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }

    public class CloudinaryImageService : IImageStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageService(IOptions<CloudinarySettings> opts)
        {
            var s = opts.Value;
            _cloudinary = new Cloudinary(new Account(s.CloudName, s.ApiKey, s.ApiSecret));
        }

        public async Task<string> UploadAsync(Stream stream, string fileName, CancellationToken ct = default)
        {
            var upload = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                PublicId = Path.GetFileNameWithoutExtension(fileName),
                Overwrite = true
            };
            var result = await _cloudinary.UploadAsync(upload, ct);
            return result.SecureUrl.ToString();
        }

        public async Task DeleteAsync(string publicId, CancellationToken ct = default)
        {
            var deletionParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deletionParams); 
        }
    }
}
