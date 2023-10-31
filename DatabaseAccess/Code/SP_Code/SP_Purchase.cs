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
                double totalAmount = 0;
                double.TryParse(Convert.ToString(row[6]), out totalAmount);
                double returnTotalAmount = 0;
                double.TryParse(Convert.ToString(row[7]), out returnTotalAmount);
                double afterReturnTotalAmount = 0;
                double.TryParse(Convert.ToString(row[8]), out afterReturnTotalAmount);
                double payAmount = 0;
                double.TryParse(Convert.ToString(row[9]), out payAmount);
                double returnPayAmount = 0;
                double.TryParse(Convert.ToString(row[10]), out returnPayAmount);
                double remainingBalance = 0;
                double.TryParse(Convert.ToString(row[11]), out remainingBalance);
                payment.PaymentAmount = payAmount;
                payment.RemainingBalance = remainingBalance;
                payment.SupplierContactNo = supplier.SupplierConatctNo;
                payment.SupplierAddress = supplier.SupplierAddress;
                payment.SupplierID = supplier.SupplierID;
                payment.SupplierName = supplier.SupplierName;
                payment.TotalAmount = totalAmount;
                payment.ReturnProductAmount = returnTotalAmount;
                payment.ReturnPaymentAmount = returnPayAmount;
                payment.TotalAmount = afterReturnTotalAmount;

                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }

        public List<PurchasePaymentModel> CustomPurchasesList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate)
        {
            var remainingPaymentList = new List<PurchasePaymentModel>();
            SqlCommand command = new SqlCommand("GetPurchasesHistory", DatabaseQuery.ConnOpen())
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.Parameters.AddWithValue("@CompanyID", CompanyID);
            command.Parameters.AddWithValue("@FromDate", FromDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@ToDate", ToDate.ToString("yyyy-MM-dd"));
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
                double totalAmount = 0;
                double.TryParse(Convert.ToString(row[6]), out totalAmount);
                double returnTotalAmount = 0;
                double.TryParse(Convert.ToString(row[7]), out returnTotalAmount);
                double afterReturnTotalAmount = 0;
                double.TryParse(Convert.ToString(row[8]), out afterReturnTotalAmount);
                double payAmount = 0;
                double.TryParse(Convert.ToString(row[9]), out payAmount);
                double returnPayAmount = 0;
                double.TryParse(Convert.ToString(row[10]), out returnPayAmount);
                double remainingBalance = 0;
                double.TryParse(Convert.ToString(row[11]), out remainingBalance);
                payment.PaymentAmount = payAmount;
                payment.RemainingBalance = remainingBalance;
                payment.SupplierContactNo = supplier.SupplierConatctNo;
                payment.SupplierAddress = supplier.SupplierAddress;
                payment.SupplierID = supplier.SupplierID;
                payment.SupplierName = supplier.SupplierName;
                payment.TotalAmount = totalAmount;
                payment.ReturnProductAmount = returnTotalAmount;
                payment.ReturnPaymentAmount = returnPayAmount;
                payment.TotalAmount = afterReturnTotalAmount;

                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }

        public List<PurchasePaymentModel> PurchasePaymentHistory(int SupplierInvoiceID)
        {
            var remainingPaymentList = new List<PurchasePaymentModel>();
            SqlCommand command = new SqlCommand("GetSupplierPaymentHistory", DatabaseQuery.ConnOpen())
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@SupplierInvoiceID", SupplierInvoiceID);
            var dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                var supplierID = Convert.ToInt32(Convert.ToString(row[4]));
                var userID = Convert.ToInt32(Convert.ToString(row[9]));
                var supplier = db.tblSupplier.Find(supplierID);
                var user = db.tblUser.Find(userID);
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
                payment.UserID = user.UserID;
                payment.UserName = user.UserName;

                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }

        public List<SupplierReturnInvoiceModel> PurchaseReturnPaymenPending(int? SupplierInvoiceID)
        {
            var remainingPaymentList = new List<SupplierReturnInvoiceModel>();
            SqlCommand command = new SqlCommand("GetSupplierReturnPurchasePaymentPending", DatabaseQuery.ConnOpen())
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@SupplierInvoiceID", (int)SupplierInvoiceID);
            var dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                var supplierID = Convert.ToInt32(Convert.ToString(row[5]));
                var userID = Convert.ToInt32(Convert.ToString(row[10]));
                var supplier = db.tblSupplier.Find(supplierID);
                var user = db.tblUser.Find(userID);
                var payment = new SupplierReturnInvoiceModel();
                payment.SupplierReturnInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
                payment.SupplierInvoiceID = Convert.ToInt32(Convert.ToString(row[1]));
                payment.BranchID = Convert.ToInt32(Convert.ToString(row[2]));
                payment.CompanyID = Convert.ToInt32(Convert.ToString(row[3]));
                payment.InvoiceDate = Convert.ToDateTime(Convert.ToString(row[4]));
                payment.InvoiceNo = Convert.ToString(row[6]);
                double payAmount = 0;
                double.TryParse(Convert.ToString(row[8]), out payAmount);
                double remainingBalance = 0;
                double.TryParse(Convert.ToString(row[9]), out remainingBalance);
                double totalAmount = 0;
                double.TryParse(Convert.ToString(row[7]), out totalAmount);
                payment.ReturnPaymentAmount = payAmount;
                payment.RemainingBalance = remainingBalance;
                payment.SupplierContactNo = supplier.SupplierConatctNo;
                payment.SupplierAddress = supplier.SupplierAddress;
                payment.SupplierID = supplier.SupplierID;
                payment.SupplierName = supplier.SupplierName;
                payment.ReturnTotalAmount = totalAmount;
                payment.UserID = user.UserID;
                payment.UserName = user.UserName;

                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }
    }
}
