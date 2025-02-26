namespace Utils.Interfaces
{
    public interface IForecastingService
    {
        double GenerateForecast(int companyID, int branchID, DateTime startDate, DateTime endDate);
    }
}
