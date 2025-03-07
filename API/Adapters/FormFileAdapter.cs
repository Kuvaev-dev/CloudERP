using Utils.Interfaces;

namespace API.Adapters
{
    public class FormFileAdapter : IFile
    {
        private readonly byte[] _fileBytes;
        private readonly string _fileName;

        public FormFileAdapter(IFormFile file)
        {
            ArgumentNullException.ThrowIfNull(file);

            _fileName = file.FileName;
            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            _fileBytes = memoryStream.ToArray();
        }

        public string FileName => _fileName;
        public long ContentLength => _fileBytes.Length;
        public Stream InputStream => new MemoryStream(_fileBytes);

        public void SaveAs(string path)
        {
            File.WriteAllBytes(path, _fileBytes);
        }
    }
}