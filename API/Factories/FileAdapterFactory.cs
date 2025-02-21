using API.Adapters;
using Utils.Interfaces;

namespace API.Factories
{
    public interface IFileAdapterFactory
    {
        IFile Create(IFormFile file);
    }

    public class FileAdapterFactory : IFileAdapterFactory
    {
        public IFile Create(IFormFile file)
        {
            return new FormFileAdapter(file);
        }
    }
}
