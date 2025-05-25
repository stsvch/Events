using Events.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Events.Infrastructure.Services.Caching
{
    public class CachedImageService : IImageStorageService
    {
        private const string CacheKeyTemplate = "image:data:{0}";
        private readonly IImageStorageService _inner;
        private readonly IDistributedCache _cache;
        private readonly TimeSpan _ttl = TimeSpan.FromHours(1);

        public CachedImageService(
            IImageStorageService inner,
            IDistributedCache cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public Task<string> UploadAsync(Stream stream, string fileName, CancellationToken ct = default)
            => _inner.UploadAsync(stream, fileName, ct);

        public async Task DeleteAsync(string publicId, CancellationToken ct = default)
        {
            await _inner.DeleteAsync(publicId, ct);
            var key = string.Format(CacheKeyTemplate, publicId);
            await _cache.RemoveAsync(key, ct);
        }

        public async Task<Stream> GetAsync(string publicId, CancellationToken ct = default)
        {
            var key = string.Format(CacheKeyTemplate, publicId);

            var cached = await _cache.GetAsync(key, ct);
            if (cached != null)
            {
                return new MemoryStream(cached);
            }

            using var originalStream = await _inner.GetAsync(publicId, ct);
            using var ms = new MemoryStream();
            await originalStream.CopyToAsync(ms, ct);

            var bytes = ms.ToArray();

            await _cache.SetAsync(
                key,
                bytes,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _ttl },
                ct);

            ms.Position = 0;
            return new MemoryStream(bytes);
        }
    }

}
