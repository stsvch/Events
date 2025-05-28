using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Interfaces
{
    public interface IImageProxyService
    {
        Task<(Stream Stream, string ContentType)> GetByUrlAsync(string url, CancellationToken ct = default);
    }
}
