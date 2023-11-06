using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code.SP_Code
{
    public class SP_GeneralTransaction
    {
        private CloudDBEntities db = new CloudDBEntities();

        public List<AllAccountModel> GetAllTransactions(int CompanyID, int BranchID)
        {
            var accountsList = new List<AllAccountModel>();
            SqlCommand command = new SqlCommand("GetAllAccounts", DatabaseQuery.ConnOpen())
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.Parameters.AddWithValue("@CompanyID", CompanyID);
            var dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                var account = new AllAccountModel();
                account.AccountHeadID = Convert.ToInt32(row[0].ToString());
                account.AccountHeadName = Convert.ToString(row[1]);
                account.AccountControlID = Convert.ToInt32(row[2].ToString());
                account.AccountControlName = Convert.ToString(row[3]);
                account.BranchID = Convert.ToInt32(row[4].ToString());
                account.CompanyID = Convert.ToInt32(row[5].ToString());
                account.AccountSubControlID = Convert.ToInt32(row[6].ToString());
                account.AccountSubControl = Convert.ToString(row[7]);

                accountsList.Add(account);
            }
            return accountsList;
        }
    }
}
