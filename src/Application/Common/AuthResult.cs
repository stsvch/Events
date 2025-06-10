

namespace Events.Application.Common
{
    public class AuthResult
    {
        public bool Succeeded { get; }
        public string AccessToken { get; }
        public string RefreshToken { get; }
        public IEnumerable<string> Errors { get; }
        public string? UserId { get; init; }

        private AuthResult(bool succeeded, string accessToken, string refreshToken, IEnumerable<string> errors, string userId)
        {
            Succeeded = succeeded;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Errors = errors;
            UserId = userId;
        }

        public static AuthResult Success(string accessToken, string refreshToken, string userId)
            => new AuthResult(true, accessToken, refreshToken, Array.Empty<string>(), userId);

        public static AuthResult Failure(params string[] errors)
            => new AuthResult(false, string.Empty, string.Empty, errors ?? new[] { "Authentication failed." }, string.Empty);


        public static AuthResult Failure()
            => Failure(new[] { "Authentication failed." });
    }
}
