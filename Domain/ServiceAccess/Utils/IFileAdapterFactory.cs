using Utils.Interfaces;

namespace Domain.ServiceAccess
{
    public interface IFileAdapterFactory
    {
        IFile Create(object file);
    }
}
