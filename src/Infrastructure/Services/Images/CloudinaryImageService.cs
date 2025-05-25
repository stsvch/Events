using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Events.Application.Interfaces;
using Events.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Events.Infrastructure.Services.Images
{

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

        public async Task<Stream> GetAsync(string publicId, CancellationToken ct = default)
        {
            var url = _cloudinary.Api.UrlImgUp.BuildUrl(publicId);
            var http = new HttpClient();
            var response = await http.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync(ct);
        }
    }
}
