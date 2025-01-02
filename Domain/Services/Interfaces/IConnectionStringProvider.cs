namespace Domain.Services.Interfaces
{
    public interface IConnectionStringProvider
    {
        string GetConnectionString(string name);
    }
}
