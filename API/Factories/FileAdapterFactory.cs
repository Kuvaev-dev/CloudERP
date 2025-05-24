using API.Adapters;
using Domain.ServiceAccess;
using Domain.UtilsAccess;

namespace API.Factories
{
    public class FileAdapterFactory : IFileAdapterFactory
    {
        public IFile Create(object file)
        {
            return new FormFileAdapter((IFormFile)file);
        }
    }
}
