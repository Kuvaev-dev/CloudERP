using DatabaseAccess.Code;
using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface IPurchaseRepository
    {
        Task<List<PurchasePaymentModel>> RemainingPaymentList(int CompanyID, int BranchID);
        Task<List<PurchasePaymentModel>> CustomPurchasesList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate);
        Task<List<PurchasePaymentModel>> PurchasePaymentHistory(int SupplierInvoiceID);
        Task<List<SupplierReturnInvoiceModel>> PurchaseReturnPaymentPending(int? SupplierInvoiceID);
        Task<List<PurchasePaymentModel>> GetReturnPurchasesPaymentPending(int CompanyID, int BranchID);
    }

    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly DatabaseQuery _query;
        private readonly ISupplierRepository _suppliers;
        private readonly IUserRepository _users;

        public PurchaseRepository(DatabaseQuery query, ISupplierRepository suppliers, IUserRepository users)
        {
            _query = query;
            _suppliers = suppliers;
            _users = users;
        }

        public async Task<List<PurchasePaymentModel>> RemainingPaymentList(int CompanyID, int BranchID)
        {
            var remainingPaymentList = new List<PurchasePaymentModel>();

            try
            {
                using (SqlCommand command = new SqlCommand("GetSupplierRemainingPaymentRecord", await _query.ConnOpen()))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@BranchID", BranchID);
                    command.Parameters.AddWithValue("@CompanyID", CompanyID);

                    var dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        da.Fill(dt);
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        var supplierID = Convert.ToInt32(row["SupplierID"]);
                        var supplier = await _suppliers.GetByIdAsync(supplierID);

                        var payment = new PurchasePaymentModel
                        {
                            SupplierInvoiceID = Convert.ToInt32(row["SupplierInvoiceID"]),
                            BranchID = Convert.ToInt32(row["BranchID"]),
                            CompanyID = Convert.ToInt32(row["CompanyID"]),
                            InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                            InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                            TotalAmount = Convert.ToDouble(row["BeforeReturnTotal"]),
                            ReturnProductAmount = Convert.ToDouble(row["ReturnTotal"]),
                            PaymentAmount = Convert.ToDouble(row["PaidAmount"]),
                            ReturnPaymentAmount = Convert.ToDouble(row["ReturnPayment"]),
                            RemainingBalance = Convert.ToDouble(row["RemainingBalance"]),
                            SupplierContactNo = supplier.SupplierConatctNo,
                            SupplierAddress = supplier.SupplierAddress,
                            SupplierID = supplier.SupplierID,
                            SupplierName = supplier.SupplierName
                        };

                        remainingPaymentList.Add(payment);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return remainingPaymentList;
        }

        public async Task<List<PurchasePaymentModel>> CustomPurchasesList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate)
        {
            var remainingPaymentList = new List<PurchasePaymentModel>();

            try
            {
                using (SqlCommand command = new SqlCommand("GetPurchasesHistory", await _query.ConnOpen()))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@BranchID", BranchID);
                    command.Parameters.AddWithValue("@CompanyID", CompanyID);
                    command.Parameters.AddWithValue("@FromDate", FromDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@ToDate", ToDate.ToString("yyyy-MM-dd"));

                    var dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        da.Fill(dt);
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        var supplierID = Convert.ToInt32(row["SupplierID"]);
                        var supplier = await _suppliers.GetByIdAsync(supplierID);

                        var payment = new PurchasePaymentModel
                        {
                            SupplierInvoiceID = Convert.ToInt32(row["SupplierInvoiceID"]),
                            BranchID = Convert.ToInt32(row["BranchID"]),
                            CompanyID = Convert.ToInt32(row["CompanyID"]),
                            InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                            InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                            TotalAmount = Convert.ToDouble(row["BeforeReturnTotal"]),
                            ReturnProductAmount = Convert.ToDouble(row["ReturnTotal"]),
                            PaymentAmount = Convert.ToDouble(row["PaidAmount"]),
                            ReturnPaymentAmount = Convert.ToDouble(row["ReturnPayment"]),
                            RemainingBalance = Convert.ToDouble(row["RemainingBalance"]),
                            SupplierContactNo = supplier.SupplierConatctNo,
                            SupplierAddress = supplier.SupplierAddress,
                            SupplierID = supplier.SupplierID,
                            SupplierName = supplier.SupplierName
                        };

                        remainingPaymentList.Add(payment);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return remainingPaymentList;
        }

        public async Task<List<PurchasePaymentModel>> PurchasePaymentHistory(int SupplierInvoiceID)
        {
            var remainingPaymentList = new List<PurchasePaymentModel>();

            try
            {
                using (SqlCommand command = new SqlCommand("GetSupplierPaymentHistory", await _query.ConnOpen()))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SupplierInvoiceID", SupplierInvoiceID);

                    var dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        da.Fill(dt);
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        var supplier = await _suppliers.GetByIdAsync(Convert.ToInt32(row["SupplierID"]));
                        var user = await _users.GetByIdAsync(Convert.ToInt32(row["UserID"]));

                        var payment = new PurchasePaymentModel
                        {
                            SupplierInvoiceID = Convert.ToInt32(row["SupplierInvoiceID"]),
                            BranchID = Convert.ToInt32(row["BranchID"]),
                            CompanyID = Convert.ToInt32(row["CompanyID"]),
                            InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                            InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                            TotalAmount = Convert.ToDouble(row["TotalAmount"]),
                            PaymentAmount = Convert.ToDouble(row["PaymentAmount"]),
                            RemainingBalance = Convert.ToDouble(row["RemainingBalance"]),
                            SupplierContactNo = supplier.SupplierConatctNo,
                            SupplierAddress = supplier.SupplierAddress,
                            SupplierID = supplier.SupplierID,
                            SupplierName = supplier.SupplierName,
                            UserID = user.UserID,
                            UserName = user.UserName
                        };

                        remainingPaymentList.Add(payment);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return remainingPaymentList;
        }

        public async Task<List<SupplierReturnInvoiceModel>> PurchaseReturnPaymentPending(int? SupplierInvoiceID)
        {
            var remainingPaymentList = new List<SupplierReturnInvoiceModel>();

            try
            {
                using (SqlCommand command = new SqlCommand("GetSupplierReturnPurchasePaymentPending", await _query.ConnOpen()))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SupplierInvoiceID", SupplierInvoiceID.HasValue ? (object)SupplierInvoiceID.Value : DBNull.Value);

                    var dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        da.Fill(dt);
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        var supplier = await _suppliers.GetByIdAsync(Convert.ToInt32(row["SupplierID"]));
                        var user = await _users.GetByIdAsync(Convert.ToInt32(row["UserID"]));

                        var payment = new SupplierReturnInvoiceModel
                        {
                            SupplierReturnInvoiceID = Convert.ToInt32(row["SupplierReturnInvoiceID"]),
                            SupplierInvoiceID = Convert.ToInt32(row["SupplierInvoiceID"]),
                            BranchID = Convert.ToInt32(row["BranchID"]),
                            CompanyID = Convert.ToInt32(row["CompanyID"]),
                            InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                            InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                            ReturnTotalAmount = Convert.ToDouble(row["ReturnTotal"]),
                            ReturnPaymentAmount = Convert.ToDouble(row["ReturnPayment"]),
                            RemainingBalance = Convert.ToDouble(row["ReturnRemainingPayment"]),
                            SupplierContactNo = supplier.SupplierConatctNo,
                            SupplierAddress = supplier.SupplierAddress,
                            SupplierID = supplier.SupplierID,
                            SupplierName = supplier.SupplierName,
                            UserID = user.UserID,
                            UserName = user.UserName
                        };

                        remainingPaymentList.Add(payment);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            return remainingPaymentList;
        }

        public async Task<List<PurchasePaymentModel>> GetReturnPurchasesPaymentPending(int CompanyID, int BranchID)
        {
            var remainingPaymentList = new List<PurchasePaymentModel>();

            try
            {
                using (SqlCommand command = new SqlCommand("GetReturnPurchasePaymentPending", await _query.ConnOpen()))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@BranchID", BranchID);
                    command.Parameters.AddWithValue("@CompanyID", CompanyID);

                    var dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        da.Fill(dt);
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        var supplier = await _suppliers.GetByIdAsync(Convert.ToInt32(row["SupplierID"]));

                        var payment = new PurchasePaymentModel
                        {
                            SupplierInvoiceID = Convert.ToInt32(row["SupplierInvoiceID"]),
                            BranchID = Convert.ToInt32(row["BranchID"]),
                            CompanyID = Convert.ToInt32(row["CompanyID"]),
                            InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                            InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                            TotalAmount = Convert.ToDouble(row["BeforeReturnTotal"]),
                            ReturnProductAmount = Convert.ToDouble(row["ReturnTotal"]),
                            PaymentAmount = Convert.ToDouble(row["PaidAmount"]),
                            ReturnPaymentAmount = Convert.ToDouble(row["ReturnPayment"]),
                            RemainingBalance = Convert.ToDouble(row["RemainingBalance"]),
                            SupplierContactNo = supplier.SupplierConatctNo,
                            SupplierAddress = supplier.SupplierAddress,
                            SupplierID = supplier.SupplierID,
                            SupplierName = supplier.SupplierName
                        };

                        remainingPaymentList.Add(payment);
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