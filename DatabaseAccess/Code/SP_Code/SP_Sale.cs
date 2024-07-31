using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseAccess.Code.SP_Code
{
    public class SP_Sale
    {
        private readonly CloudDBEntities _db;

        public SP_Sale(CloudDBEntities db)
        {
            _db = db;
        }

        public List<SalePaymentModel> RemainingPaymentList(int CompanyID, int BranchID)
        {
            var remainingPaymentList = new List<SalePaymentModel>();

            try
            {
                using (SqlConnection connection = DatabaseQuery.ConnOpen())
                {
                    using (SqlCommand command = new SqlCommand("GetCustomerRemainingPaymentRecord", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@BranchID", BranchID));
                        command.Parameters.Add(new SqlParameter("@CompanyID", CompanyID));

                        var dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(command))
                        {
                            da.Fill(dt);
                        }

                        foreach (DataRow row in dt.Rows)
                        {
                            var customerID = Convert.ToInt32(row["CustomerID"]);
                            var customer = _db.tblCustomer.Find(customerID);

                            if (customer == null)
                            {
                                Console.WriteLine($"Warning: Customer with ID {customerID} not found.");
                                continue;
                            }

                            var payment = new SalePaymentModel
                            {
                                CustomerInvoiceID = Convert.ToInt32(row["CustomerInvoiceID"]),
                                BranchID = Convert.ToInt32(row["BranchID"]),
                                CompanyID = Convert.ToInt32(row["CompanyID"]),
                                InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                                InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                                PaymentAmount = Convert.ToDouble(row["Payment"] == DBNull.Value ? 0 : row["Payment"]),
                                RemainingBalance = Convert.ToDouble(row["RemainingBalance"] == DBNull.Value ? 0 : row["RemainingBalance"]),
                                CustomerContactNo = customer.CustomerContact,
                                CustomerAddress = customer.CustomerAddress,
                                CustomerID = customer.CustomerID,
                                CustomerName = customer.Customername,
                                TotalAmount = Convert.ToDouble(row["TotalAmount"] == DBNull.Value ? 0 : row["TotalAmount"])
                            };

                            remainingPaymentList.Add(payment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return remainingPaymentList;
        }

        public List<SalePaymentModel> CustomSalesList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate)
        {
            var remainingPaymentList = new List<SalePaymentModel>();

            try
            {
                using (SqlConnection connection = DatabaseQuery.ConnOpen())
                {
                    using (SqlCommand command = new SqlCommand("GetSalesHistory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@BranchID", BranchID));
                        command.Parameters.Add(new SqlParameter("@CompanyID", CompanyID));
                        command.Parameters.Add(new SqlParameter("@FromDate", FromDate.ToString("yyyy-MM-dd")));
                        command.Parameters.Add(new SqlParameter("@ToDate", ToDate.ToString("yyyy-MM-dd")));

                        var dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(command))
                        {
                            da.Fill(dt);
                        }

                        foreach (DataRow row in dt.Rows)
                        {
                            var customerID = Convert.ToInt32(row["CustomerID"]);
                            var customer = _db.tblCustomer.Find(customerID);

                            if (customer == null)
                            {
                                Console.WriteLine($"Warning: Customer with ID {customerID} not found.");
                                continue;
                            }

                            var payment = new SalePaymentModel
                            {
                                CustomerInvoiceID = Convert.ToInt32(row["CustomerInvoiceID"]),
                                BranchID = Convert.ToInt32(row["BranchID"]),
                                CompanyID = Convert.ToInt32(row["CompanyID"]),
                                InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                                InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                                TotalAmount = Convert.ToDouble(row["BeforeReturnTotal"] == DBNull.Value ? 0 : row["BeforeReturnTotal"]),
                                ReturnProductAmount = Convert.ToDouble(row["ReturnTotal"] == DBNull.Value ? 0 : row["ReturnTotal"]),
                                AfterReturnTotalAmount = Convert.ToDouble(row["AfterReturnTotal"] == DBNull.Value ? 0 : row["AfterReturnTotal"]),
                                PaymentAmount = Convert.ToDouble(row["PaidAmount"] == DBNull.Value ? 0 : row["PaidAmount"]),
                                ReturnPaymentAmount = Convert.ToDouble(row["ReturnPayment"] == DBNull.Value ? 0 : row["ReturnPayment"]),
                                RemainingBalance = Convert.ToDouble(row["RemainingBalance"] == DBNull.Value ? 0 : row["RemainingBalance"]),
                                CustomerContactNo = customer.CustomerContact,
                                CustomerAddress = customer.CustomerAddress,
                                CustomerID = customer.CustomerID,
                                CustomerName = customer.Customername
                            };

                            remainingPaymentList.Add(payment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return remainingPaymentList;
        }

        public List<SalePaymentModel> SalePaymentHistory(int CustomerInvoiceID)
        {
            var remainingPaymentList = new List<SalePaymentModel>();

            try
            {
                using (SqlConnection connection = DatabaseQuery.ConnOpen())
                {
                    using (SqlCommand command = new SqlCommand("GetCustomerPaymentHistory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@CustomerInvoiceID", CustomerInvoiceID));

                        var dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(command))
                        {
                            da.Fill(dt);
                        }

                        foreach (DataRow row in dt.Rows)
                        {
                            var customerID = Convert.ToInt32(row["CustomerID"]);
                            var userID = Convert.ToInt32(row["UserID"]);
                            var customer = _db.tblCustomer.Find(customerID);
                            var user = _db.tblUser.Find(userID);

                            if (customer == null || user == null)
                            {
                                Console.WriteLine($"Warning: Customer with ID {customerID} or User with ID {userID} not found.");
                                continue;
                            }

                            var payment = new SalePaymentModel
                            {
                                CustomerInvoiceID = Convert.ToInt32(row["CustomerInvoiceID"]),
                                BranchID = Convert.ToInt32(row["BranchID"]),
                                CompanyID = Convert.ToInt32(row["CompanyID"]),
                                InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                                InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                                TotalAmount = Convert.ToDouble(row["TotalAmount"] == DBNull.Value ? 0 : row["TotalAmount"]),
                                PaymentAmount = Convert.ToDouble(row["PaidAmount"] == DBNull.Value ? 0 : row["PaidAmount"]),
                                RemainingBalance = Convert.ToDouble(row["RemainingBalance"] == DBNull.Value ? 0 : row["RemainingBalance"]),
                                CustomerContactNo = customer.CustomerContact,
                                CustomerAddress = customer.CustomerAddress,
                                CustomerID = customer.CustomerID,
                                CustomerName = customer.Customername,
                                UserID = user.UserID,
                                UserName = user.UserName
                            };

                            remainingPaymentList.Add(payment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return remainingPaymentList;
        }

        public List<SalePaymentModel> GetReturnSaleAmountPending(int CompanyID, int BranchID)
        {
            var remainingPaymentList = new List<SalePaymentModel>();

            try
            {
                using (SqlConnection connection = DatabaseQuery.ConnOpen())
                {
                    using (SqlCommand command = new SqlCommand("GetReturnSaleAmountPending", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@BranchID", BranchID));
                        command.Parameters.Add(new SqlParameter("@CompanyID", CompanyID));

                        var dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(command))
                        {
                            da.Fill(dt);
                        }

                        foreach (DataRow row in dt.Rows)
                        {
                            var customerID = Convert.ToInt32(row["CustomerID"]);
                            var customer = _db.tblCustomer.Find(customerID);

                            if (customer == null)
                            {
                                Console.WriteLine($"Warning: Customer with ID {customerID} not found.");
                                continue;
                            }

                            var payment = new SalePaymentModel
                            {
                                CustomerInvoiceID = Convert.ToInt32(row["CustomerInvoiceID"]),
                                BranchID = Convert.ToInt32(row["BranchID"]),
                                CompanyID = Convert.ToInt32(row["CompanyID"]),
                                InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                                InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                                TotalAmount = Convert.ToDouble(row["BeforeReturnTotal"] == DBNull.Value ? 0 : row["BeforeReturnTotal"]),
                                ReturnProductAmount = Convert.ToDouble(row["ReturnTotal"] == DBNull.Value ? 0 : row["ReturnTotal"]),
                                AfterReturnTotalAmount = Convert.ToDouble(row["AfterReturnTotal"] == DBNull.Value ? 0 : row["AfterReturnTotal"]),
                                PaymentAmount = Convert.ToDouble(row["PaidAmount"] == DBNull.Value ? 0 : row["PaidAmount"]),
                                ReturnPaymentAmount = Convert.ToDouble(row["ReturnPayment"] == DBNull.Value ? 0 : row["ReturnPayment"]),
                                RemainingBalance = Convert.ToDouble(row["RemainingBalance"] == DBNull.Value ? 0 : row["RemainingBalance"]),
                                CustomerContactNo = customer.CustomerContact,
                                CustomerAddress = customer.CustomerAddress,
                                CustomerID = customer.CustomerID,
                                CustomerName = customer.Customername
                            };

                            remainingPaymentList.Add(payment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return remainingPaymentList;
        }
    }
}