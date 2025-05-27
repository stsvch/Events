using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Common
{
    public class LogoutResult
    {
        public bool Succeeded { get; init; }
        public IEnumerable<string> Errors { get; init; } = Array.Empty<string>();
    }
}
