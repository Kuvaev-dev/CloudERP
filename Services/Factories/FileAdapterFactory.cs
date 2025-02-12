using DatabaseAccess.Adapters;
using System.Web;
using Utils.Interfaces;

namespace DatabaseAccess.Factories
{
    public interface IFileAdapterFactory
    {
        IFile Create(HttpPostedFile file);
    }

    public class FileAdapterFactory : IFileAdapterFactory
    {
        public IFile Create(HttpPostedFile file)
        {
            return new HttpPostedFileAdapter(file);
        }
    }
}