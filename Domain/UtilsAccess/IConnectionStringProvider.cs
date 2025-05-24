namespace Domain.UtilsAccess
{
    public interface IConnectionStringProvider
    {
        string GetConnectionString(string name);
    }
}
