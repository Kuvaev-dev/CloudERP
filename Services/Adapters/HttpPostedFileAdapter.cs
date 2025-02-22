using Microsoft.AspNetCore.Http;
using Utils.Interfaces;

namespace Services.Adapters
{
    public class HttpPostedFileAdapter : IFile
    {
        private readonly IFormFile _file;

        public HttpPostedFileAdapter(IFormFile file)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));
        }

        public string FileName => _file.FileName;
        public long ContentLength => _file.Length;
        public Stream InputStream => _file.OpenReadStream();

        public void SaveAs(string path)
        {
            using var stream = new FileStream(path, FileMode.Create);
            _file.CopyTo(stream);
        }
    }
}
