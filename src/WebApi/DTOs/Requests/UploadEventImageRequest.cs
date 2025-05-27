using Microsoft.AspNetCore.Mvc;

namespace Events.WebApi.DTOs.Requests
{
    public class UploadEventImageRequest
    {
        [FromForm(Name = "file")]
        public IFormFile File { get; set; }

        [FromForm(Name = "url")]
        public string Url { get; set; }
    }
}
