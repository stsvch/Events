using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Extensions
{
    public class JwtAuthenticationResult
    {
        public bool Succeeded { get; }
        public string AccessToken { get; }
        public string RefreshToken { get; }

        private JwtAuthenticationResult(bool succeeded, string accessToken, string refreshToken)
        {
            Succeeded = succeeded;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public static JwtAuthenticationResult Failure()
            => new JwtAuthenticationResult(false, string.Empty, string.Empty);

        public static JwtAuthenticationResult Success(string accessToken, string refreshToken)
            => new JwtAuthenticationResult(true, accessToken, refreshToken);
    }
}
