using Microsoft.AspNetCore.Http;

namespace file_upload.ViewModel
{
    public class FileViewModel
    {
        public string Name { get; set; }
        public IFormFile File { get; set; }
    }
}
