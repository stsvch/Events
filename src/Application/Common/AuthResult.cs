using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Common
{
    public class AuthResult
    {
        public bool Succeeded { get; }
        public string AccessToken { get; }
        public string RefreshToken { get; }

        private AuthResult(bool ok, string access, string refresh)
        {
            Succeeded = ok;
            AccessToken = access;
            RefreshToken = refresh;
        }

        public static AuthResult Success(string access, string refresh)
            => new AuthResult(true, access, refresh);

        public static AuthResult Failure()
            => new AuthResult(false, "", "");
    }
}
