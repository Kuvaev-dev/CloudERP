using DatabaseAccess.Adapters;
using Domain.Interfaces;
using System.Web;

namespace CloudERP.Factories
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