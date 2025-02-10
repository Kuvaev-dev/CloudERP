using DatabaseAccess.Adapters;
using System.Web;
using Utils.Interfaces;

namespace DatabaseAccess.Factories
{
    public interface IFileAdapterFactory
    {
        IFile Create(HttpPostedFileBase file);
    }

    public class FileAdapterFactory : IFileAdapterFactory
    {
        public IFile Create(HttpPostedFileBase file)
        {
            return new HttpPostedFileAdapter(file);
        }
    }
}