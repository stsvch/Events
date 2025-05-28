using Events.Application.Interfaces;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure.Services.Images
{
    public class ImageProxyService : IImageProxyService
    {
        private readonly IImageStorageService _storage;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider
            = new FileExtensionContentTypeProvider();

        public ImageProxyService(IImageStorageService storage)
            => _storage = storage;

        public async Task<(Stream Stream, string ContentType)> GetByUrlAsync(
            string url, CancellationToken ct = default)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                throw new ArgumentException("Invalid URL.", nameof(url));

            var filename = Path.GetFileName(uri.LocalPath);
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("Cannot extract filename from URL.", nameof(url));

            var publicId = Path.GetFileNameWithoutExtension(filename);

            var stream = await _storage.GetAsync(publicId, ct);

            if (!_contentTypeProvider.TryGetContentType(filename, out var contentType))
                contentType = "application/octet-stream";

            return (stream, contentType);
        }
    }
}
