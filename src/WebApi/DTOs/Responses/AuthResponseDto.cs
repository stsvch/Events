namespace Events.WebApi.DTOs.Responses
{
    public class AuthResponseDto
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
