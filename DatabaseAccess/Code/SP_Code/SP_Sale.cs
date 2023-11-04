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
    public class SP_Sale
    {
        private CloudDBEntities db = new CloudDBEntities();

        public List<SalePaymentModel> RemainingPaymentList(int CompanyID, int BranchID)
        {
            var remainingPaymentList = new List<SalePaymentModel>();
            SqlCommand command = new SqlCommand("GetCustomerRemainingPaymentRecord", DatabaseQuery.ConnOpen())
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
                var customerID = Convert.ToInt32(Convert.ToString(row[4]));
                var customer = db.tblCustomer.Find(customerID);
                var payment = new SalePaymentModel();
                payment.CustomerInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
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
                payment.CustomerContactNo = customer.CustomerContact;
                payment.CustomerAddress = customer.CustomerAddress;
                payment.CustomerID = customer.CustomerID;
                payment.CustomerName = customer.Customername;
                payment.TotalAmount = totalAmount;

                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }

        public List<SalePaymentModel> CustomSalesList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate)
        {
            var remainingPaymentList = new List<SalePaymentModel>();
            SqlCommand command = new SqlCommand("GetSalesHistory", DatabaseQuery.ConnOpen())
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
                var customerID = Convert.ToInt32(Convert.ToString(row[4]));
                var customer = db.tblCustomer.Find(customerID);
                var payment = new SalePaymentModel();
                payment.CustomerInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
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
                payment.CustomerContactNo = customer.CustomerContact;
                payment.CustomerAddress = customer.CustomerAddress;
                payment.CustomerID = customer.CustomerID;
                payment.CustomerName = customer.Customername;
                payment.TotalAmount = totalAmount;
                payment.ReturnProductAmount = returnTotalAmount;
                payment.ReturnPaymentAmount = returnPayAmount;
                payment.TotalAmount = afterReturnTotalAmount;

                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }

        public List<SalePaymentModel> SalePaymentHistory(int CustomerInvoiceID)
        {
            var remainingPaymentList = new List<SalePaymentModel>();
            SqlCommand command = new SqlCommand("GetCustomerPaymentHistory", DatabaseQuery.ConnOpen())
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@CustomerInvoiceID", CustomerInvoiceID);
            var dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                var customerID = Convert.ToInt32(Convert.ToString(row[4]));
                var userID = Convert.ToInt32(Convert.ToString(row[9]));
                var customer = db.tblCustomer.Find(customerID);
                var user = db.tblUser.Find(userID);
                var payment = new SalePaymentModel();
                payment.CustomerInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
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
                payment.CustomerContactNo = customer.CustomerContact;
                payment.CustomerAddress = customer.CustomerAddress;
                payment.CustomerID = customer.CustomerID;
                payment.CustomerName = customer.Customername;
                payment.TotalAmount = totalAmount;
                payment.UserID = user.UserID;
                payment.UserName = user.UserName;

                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }
    }
}
