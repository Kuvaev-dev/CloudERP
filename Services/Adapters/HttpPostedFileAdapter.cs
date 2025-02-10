using System;
using System.IO;
using System.Web;
using Utils.Interfaces;

namespace DatabaseAccess.Adapters
{
    public class HttpPostedFileAdapter : IFile
    {
        private readonly HttpPostedFileBase _file;

        public HttpPostedFileAdapter(HttpPostedFileBase file)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));
        }

        public string FileName => _file.FileName;
        public int ContentLength => _file.ContentLength;
        public Stream InputStream => _file.InputStream;

        public void SaveAs(string path)
        {
            _file.SaveAs(path);
        }
    }
}
