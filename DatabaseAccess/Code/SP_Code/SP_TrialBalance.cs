using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseAccess.Code.SP_Code
{
    public class SP_TrialBalance
    {
        public List<TrialBalanceModel> TriaBalance(int BranchID, int CompanyID, int FinancialYearID)
        {
            var triaBalance = new List<TrialBalanceModel>();

            try
            {
                using (SqlConnection connection = DatabaseQuery.ConnOpen())
                {
                    using (SqlCommand command = new SqlCommand("GetTrialBalance", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@BranchID", BranchID));
                        command.Parameters.Add(new SqlParameter("@CompanyID", CompanyID));
                        command.Parameters.Add(new SqlParameter("@FinancialYearID", FinancialYearID));

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
                                FinancialYearID = Convert.ToInt32(row[0]),
                                AccountSubControl = Convert.ToString(row[1]),
                                AccountSubControlID = Convert.ToInt32(row[2]),
                                Debit = Convert.ToDouble(row[3] == DBNull.Value ? 0 : row[3]),
                                Credit = Convert.ToDouble(row[4] == DBNull.Value ? 0 : row[4]),
                                BranchID = Convert.ToInt32(row[5]),
                                CompanyID = Convert.ToInt32(row[6])
                            };

                            totalDebit += balance.Debit;
                            totalCredit += balance.Credit;

                            if (balance.Debit > 0 || balance.Credit > 0)
                            {
                                triaBalance.Add(balance);
                            }
                        }

                        var totalBalance = new TrialBalanceModel
                        {
                            Credit = totalCredit,
                            Debit = totalDebit,
                            AccountSubControl = "Total"
                        };
                        triaBalance.Add(totalBalance);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return triaBalance;
        }
    }
}