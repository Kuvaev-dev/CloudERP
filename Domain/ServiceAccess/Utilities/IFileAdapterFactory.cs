using Domain.UtilsAccess;

namespace Domain.ServiceAccess
{
    public interface IFileAdapterFactory
    {
        IFile Create(object file);
    }
}
