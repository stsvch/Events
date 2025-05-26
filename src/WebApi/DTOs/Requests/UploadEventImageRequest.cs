namespace Events.WebApi.DTOs.Requests
{
    public class UploadEventImageRequest
    {
        public IFormFile? File { get; set; }
        public string? Url { get; set; }
    }
}
