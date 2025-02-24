namespace Utils.Models
{
    public class ForecastInputModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<ForecastData> ForecastData { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
    }
}