namespace Domain.Interfaces
{
    public interface IConnectionStringProvider
    {
        string GetConnectionString(string name);
    }
}
