using Microsoft.AspNetCore.Http;

namespace backend.Models.Dtos
{
    public class CreateChapterFromFileDto
    {
        public string Title { get; set; }
        public int VolumeId { get; set; }
        public int Order { get; set; }
        public IFormFile File { get; set; }
    }
}
