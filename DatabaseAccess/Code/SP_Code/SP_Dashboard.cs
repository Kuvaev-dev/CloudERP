using System;
using System.Data.SqlClient;
using System.Data;
using DatabaseAccess.Models;

namespace DatabaseAccess.Code.SP_Code
{
    public class SP_Dashboard
    {
        private readonly CloudDBEntities _db;

        public SP_Dashboard(CloudDBEntities db)
        {
            _db = db;
        }

        public DashboardModel GetDashboardValues(string fromDate, string toDate, int branchID, int companyID)
        {
            DashboardModel dashboardValues = new DashboardModel();

            try 
            { 
                SqlCommand command = new SqlCommand("GetDashboardValues", DatabaseQuery.ConnOpen())
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);
                command.Parameters.AddWithValue("@BranchID", branchID);
                command.Parameters.AddWithValue("@CompanyID", companyID);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        dashboardValues.CurrentMonthRevenue = Convert.ToDouble(reader["Current Month Revenue"]);
                        dashboardValues.CurrentMonthExpenses = Convert.ToDouble(reader["Current Month Expenses"]);
                        dashboardValues.NetIncome = Convert.ToDouble(reader["Net Income"]);
                        dashboardValues.Capital = Convert.ToDouble(reader["Capital"]);
                        dashboardValues.CurrentMonthRecovery = Convert.ToDouble(reader["Current Month Recovery"]);
                        dashboardValues.CashPlusBankAccountBalance = Convert.ToDouble(reader["Cash/Bank Balance"]);
                        dashboardValues.TotalReceivable = Convert.ToDouble(reader["Total Receivable"]);
                        dashboardValues.TotalPayable = Convert.ToDouble(reader["Total Payable"]);

                        dashboardValues.TotalSalesToday = Convert.ToDouble(reader["Total Sales Today"]);
                        dashboardValues.SalesPaymentPendingToday = Convert.ToDouble(reader["Sales Payment Pending Today"]);
                        dashboardValues.SalesPaymentPaidToday = Convert.ToDouble(reader["Sales Payment Paid Today"]);
                        dashboardValues.TotalPurchasesToday = Convert.ToDouble(reader["Total Purchases Today"]);
                        dashboardValues.PurchasesPaymentPendingToday = Convert.ToDouble(reader["Purchases Payment Pending Today"]);
                        dashboardValues.PurchasesPaymentPaidToday = Convert.ToDouble(reader["Purchases Payment Paid Today"]);

                        dashboardValues.TotalReturnSalesToday = Convert.ToDouble(reader["Total Return Sales Today"]);
                        dashboardValues.ReturnSalesPaymentPendingToday = Convert.ToDouble(reader["Return Sales Payment Pending Today"]);
                        dashboardValues.ReturnSalesPaymentPaidToday = Convert.ToDouble(reader["Return Sales Payment Paid Today"]);
                        dashboardValues.TotalReturnPurchasesToday = Convert.ToDouble(reader["Total Return Purchases Today"]);
                        dashboardValues.ReturnPurchasesPaymentPendingToday = Convert.ToDouble(reader["Return Purchases Payment Pending Today"]);
                        dashboardValues.ReturnPurchasesPaymentPaidToday = Convert.ToDouble(reader["Return Purchases Payment Paid Today"]);

                        dashboardValues.TotalSalesCurrentMonth = Convert.ToDouble(reader["Total Sales Current Month"]);
                        dashboardValues.SalesPaymentPendingCurrentMonth = Convert.ToDouble(reader["Sales Payment Pending Current Month"]);
                        dashboardValues.SalesPaymentPaidCurrentMonth = Convert.ToDouble(reader["Sales Payment Paid Current Month"]);
                        dashboardValues.TotalPurchasesCurrentMonth = Convert.ToDouble(reader["Total Purchases Current Month"]);
                        dashboardValues.PurchasesPaymentPendingCurrentMonth = Convert.ToDouble(reader["Purchases Payment Pending Current Month"]);
                        dashboardValues.PurchasesPaymentPaidCurrentMonth = Convert.ToDouble(reader["Purchases Payment Paid Current Month"]);

                        dashboardValues.TotalReturnSalesCurrentMonth = Convert.ToDouble(reader["Total Return Sales Current Month"]);
                        dashboardValues.ReturnSalesPaymentPendingCurrentMonth = Convert.ToDouble(reader["Return Sales Payment Pending Current Month"]);
                        dashboardValues.ReturnSalesPaymentPaidCurrentMonth = Convert.ToDouble(reader["Return Sales Payment Paid Current Month"]);
                        dashboardValues.TotalReturnPurchasesCurrentMonth = Convert.ToDouble(reader["Total Return Purchases Current Month"]);
                        dashboardValues.ReturnPurchasesPaymentPendingCurrentMonth = Convert.ToDouble(reader["Return Purchases Payment Pending Current Month"]);
                        dashboardValues.ReturnPurchasesPaymentPaidCurrentMonth = Convert.ToDouble(reader["Return Purchases Payment Paid Current Month"]);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return dashboardValues;
        }
    }
}
