using Utils.Interfaces;

namespace API.Adapters
{
    public class FormFileAdapter : IFile
    {
        private readonly IFormFile _file;

        public FormFileAdapter(IFormFile file)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));
        }

        public string FileName => _file.FileName;
        public long ContentLength => (long)_file.Length;
        public Stream InputStream => _file.OpenReadStream();

        public void SaveAs(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                _file.CopyTo(stream);
            }
        }
    }
}
