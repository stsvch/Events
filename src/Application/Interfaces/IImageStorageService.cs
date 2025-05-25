using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, CancellationToken ct = default);
        Task DeleteAsync(string publicId, CancellationToken ct = default);
    }
}
