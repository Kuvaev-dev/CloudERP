using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code.SP_Code
{
    public class SP_Purchase
    {
        private CloudDBEntities db = new CloudDBEntities();

        public List<PurchasePaymentModel> RemainingPaymentList(int CompanyID, int BranchID)
        {
            var remainingPaymentList = new List<PurchasePaymentModel>();
            SqlCommand command = new SqlCommand("GetSupplierRemainingPaymentRecord", DatabaseQuery.ConnOpen())
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
                var supplierID = Convert.ToInt32(Convert.ToString(row[4]));
                var supplier = db.tblSupplier.Find(supplierID);
                var payment = new PurchasePaymentModel();
                payment.SupplierInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
                payment.BranchID = Convert.ToInt32(Convert.ToString(row[1]));
                payment.CompanyID = Convert.ToInt32(Convert.ToString(row[2]));
                payment.InvoiceDate = Convert.ToDateTime(Convert.ToString(row[3]));
                payment.InvoiceNo = Convert.ToString(row[5]);
                double payAmount = 0;
                double.TryParse(Convert.ToString(row[7]), out payAmount);
                double remainingBalance = 0;
                double.TryParse(Convert.ToString(row[8]), out remainingBalance);
                double totalAmount = 0;
                double.TryParse(Convert.ToString(row[6]), out totalAmount);
                payment.PaymentAmount = payAmount;
                payment.RemainingBalance = remainingBalance;
                payment.SupplierContactNo = supplier.SupplierConatctNo;
                payment.SupplierAddress = supplier.SupplierAddress;
                payment.SupplierID = supplier.SupplierID;
                payment.SupplierName = supplier.SupplierName;
                payment.TotalAmount = totalAmount;

                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }
    }
}
