using DatabaseAccess.Code;
using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class TrialBalanceRepository : ITrialBalanceRepository
    {
        private readonly DatabaseQuery _query;

        public TrialBalanceRepository(DatabaseQuery query)
        {
            _query = query;
        }

        public async Task<List<TrialBalanceModel>> GetTrialBalanceAsync(int branchId, int companyId, int financialYearId)
        {
            var trialBalance = new List<TrialBalanceModel>();

            try
            {
                using (SqlConnection connection = await _query.ConnOpen())
                {
                    using (SqlCommand command = new SqlCommand("GetTrialBalance", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@BranchID", branchId));
                        command.Parameters.Add(new SqlParameter("@CompanyID", companyId));
                        command.Parameters.Add(new SqlParameter("@FinancialYearID", financialYearId));

                        var dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(command))
                        {
                            da.Fill(dt);
                        }

                        double totalDebit = 0;
                        double totalCredit = 0;

                        foreach (DataRow row in dt.Rows)
                        {
                            var balance = new TrialBalanceModel
                            {
                                FinancialYearID = Convert.ToInt32(row["FinancialYearID"]),
                                AccountSubControl = Convert.ToString(row["AccountSubControl"]),
                                AccountSubControlID = Convert.ToInt32(row["AccountSubControlID"]),
                                Debit = Convert.ToDouble(row["Debit"] == DBNull.Value ? 0 : row["Debit"]),
                                Credit = Convert.ToDouble(row["Credit"] == DBNull.Value ? 0 : row["Credit"]),
                                BranchID = Convert.ToInt32(row["BranchID"]),
                                CompanyID = Convert.ToInt32(row["CompanyID"])
                            };

                            totalDebit += balance.Debit;
                            totalCredit += balance.Credit;

                            if (balance.Debit > 0 || balance.Credit > 0)
                            {
                                trialBalance.Add(balance);
                            }
                        }

                        var totalBalance = new TrialBalanceModel
                        {
                            Credit = totalCredit,
                            Debit = totalDebit,
                            AccountSubControl = Localization.Localization.Total
                        };
                        trialBalance.Add(totalBalance);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return trialBalance;
        }
    }
}
