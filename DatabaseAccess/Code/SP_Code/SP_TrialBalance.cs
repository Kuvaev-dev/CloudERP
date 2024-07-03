using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace DatabaseAccess.Code.SP_Code
{
    public class SP_TrialBalance
    {
        public List<TrialBalanceModel> TriaBalance(int BranchID, int CompanyID, int FinancialYearID)
        {
            var triaBalance = new List<TrialBalanceModel>();

            SqlCommand command = new SqlCommand("GetTrialBalance", DatabaseQuery.ConnOpen())
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.Parameters.AddWithValue("@CompanyID", CompanyID);
            command.Parameters.AddWithValue("@FinancialYearID", FinancialYearID);

            var dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);

            double totalDebit = 0;
            double totalCredit = 0;

            foreach (DataRow row in dt.Rows)
            {
                var balance = new TrialBalanceModel();
                balance.FinancialYearID = Convert.ToInt32(Convert.ToString(row[0]));
                balance.AccountSubControl = Convert.ToString(row[1]);
                balance.AccountSubControlID = Convert.ToInt32(row[2]);
                balance.Debit = Convert.ToDouble(row[3] == DBNull.Value ? 0 : row[3]);
                balance.Credit = Convert.ToDouble(row[4] == DBNull.Value ? 0 : row[4]);
                balance.BranchID = Convert.ToInt32(row[5]);
                balance.CompanyID = Convert.ToInt32(row[6]);

                totalDebit += Convert.ToDouble(row[3] == DBNull.Value ? 0 : row[3]);
                totalCredit += Convert.ToDouble(row[4] == DBNull.Value ? 0 : row[4]);

                if (balance.Debit > 0 || balance.Credit > 0)
                {
                    triaBalance.Add(balance);
                }
            }

            var totalBalance = new TrialBalanceModel();
            totalBalance.Credit = totalCredit;
            totalBalance.Debit = totalDebit;
            totalBalance.AccountSubControl = "Total";
            triaBalance.Add(totalBalance);

            return triaBalance;
        }
    }
}
