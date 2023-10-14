using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code
{
    public class SaleEntry
    {
        private CloudDBEntities db = new CloudDBEntities();
        public string selectcustomerid = string.Empty;
        DataTable dtEntries = null;

        public string ConfirmSale(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, float Amount, string CustomerID, string Customername, bool isPayment)
        {
            try
            {
                dtEntries = null;
                string saletitle = "Sale To " + Customername.Trim();
                var financialYearCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set! Please Contact to Administrator!";
                }

                string successmessage = "Sale Success";
                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;
                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)
                var saleAccount = db.tblAccountSetting.Where(a => a.AccountActivityID == 1 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); // 1 - Sale
                // Credit Entry Sale                                                                                                                                            
                AccountHeadID = Convert.ToString(saleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(saleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(saleAccount.AccountSubControlID);
                string transectiontitle = string.Empty;
                transectiontitle = "Sale To " + Customername.Trim();
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);
                // Debit Entry Sale
                saleAccount = db.tblAccountSetting.Where(a => a.AccountActivityID == 10 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); // 10 - Sale Payment Pending 
                AccountHeadID = Convert.ToString(saleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(saleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(saleAccount.AccountSubControlID);
                transectiontitle = Customername + ", Sale Payment is Pending!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);

                if (isPayment == true)
                {
                    string payinvoicenno = "INP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                    saleAccount = db.tblAccountSetting.Where(a => a.AccountActivityID == 10 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); ; // 8 - Purchase Payment Pending
                    AccountHeadID = Convert.ToString(saleAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(saleAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(saleAccount.AccountSubControlID);
                    transectiontitle = "Sale Payment Paid By " + Customername;
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);
                    saleAccount = db.tblAccountSetting.Where(a => a.AccountActivityID == 11 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); ; // 9 - Purchase Payment Succed
                    AccountHeadID = Convert.ToString(saleAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(saleAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(saleAccount.AccountSubControlID);
                    transectiontitle = Customername + ", Sale Payment is Succeed!";
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);

                    string paymentquery = string.Format("insert into tblCustomerPayment(CustomerID,CustomerInvoiceID,UserID,InvoiceNo,TotalAmount,PaidAmount,RemainingBalance,CompanyID,BranchID) " +
                    "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                    CustomerID, CustomerInvoiceID, UserID, payinvoicenno, Amount, Amount, "0", CompanyID, BranchID);
                    DatabaseQuery.Insert(paymentquery);

                    successmessage += " with Payment.";
                }

                foreach (DataRow entryRow in dtEntries.Rows)
                {
                    string entryQuery = string.Format("insert into tblTransaction(FinancialYearID,AccountHeadID,AccountControlID,AccountSubControlID,InvoiceNo,UserID,Credit,Debit,TransectionDate,TransectionTitle,CompanyID,BranchID) " +
                        "values {'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}'}",
                        Convert.ToString(entryRow[0]),
                        Convert.ToString(entryRow[1]),
                        Convert.ToString(entryRow[2]),
                        Convert.ToString(entryRow[3]),
                        Convert.ToString(entryRow[4]),
                        Convert.ToString(entryRow[5]),
                        Convert.ToString(entryRow[6]),
                        Convert.ToString(entryRow[7]),
                        Convert.ToDateTime(Convert.ToString(entryRow[8])).ToString("yyyy/MM/dd HH:mm"),
                        Convert.ToString(entryRow[9]),
                        CompanyID, BranchID);
                    DatabaseQuery.Insert(entryQuery);
                }

                return successmessage;
            }
            catch (Exception ex)
            {
                return "Unexpected Error is Occured. Please Try Again!";
            }
        }

        public string SalePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, float TotalAmount, float Amount, string CustomerID, string Customername, float RemainingBalance)
        {
            try
            {
                dtEntries = null;
                string saletitle = "Sale To " + Customername.Trim();
                var financialYearCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set! Please Contact to Administrator!";
                }

                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;
                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)
                string transectiontitle = string.Empty;

                var saleAccount = db.tblAccountSetting.Where(a => a.AccountActivityID == 10 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); ; // 8 - Purchase Payment Pending
                AccountHeadID = Convert.ToString(saleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(saleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(saleAccount.AccountSubControlID);
                transectiontitle = "Sale Payment Paid By " + Customername;
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);
                saleAccount = db.tblAccountSetting.Where(a => a.AccountActivityID == 11 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); ; // 9 - Purchase Payment Succed
                AccountHeadID = Convert.ToString(saleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(saleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(saleAccount.AccountSubControlID);
                transectiontitle = Customername + ", Sale Payment is Succeed!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);

                string paymentquery = string.Format("insert into tblCustomerPayment(CustomerID,CustomerInvoiceID,UserID,InvoiceNo,TotalAmount,PaidAmount,RemainingBalance,CompanyID,BranchID) " +
                "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                CustomerID, CustomerInvoiceID, UserID, InvoiceNo, TotalAmount, Amount, Convert.ToString(RemainingBalance), CompanyID, BranchID);
                DatabaseQuery.Insert(paymentquery);

                foreach (DataRow entryRow in dtEntries.Rows)
                {
                    string entryQuery = string.Format("insert into tblTransaction(FinancialYearID,AccountHeadID,AccountControlID,AccountSubControlID,InvoiceNo,UserID,Credit,Debit,TransectionDate,TransectionTitle,CompanyID,BranchID) " +
                        "values {'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}'}",
                        Convert.ToString(entryRow[0]),
                        Convert.ToString(entryRow[1]),
                        Convert.ToString(entryRow[2]),
                        Convert.ToString(entryRow[3]),
                        Convert.ToString(entryRow[4]),
                        Convert.ToString(entryRow[5]),
                        Convert.ToString(entryRow[6]),
                        Convert.ToString(entryRow[7]),
                        Convert.ToDateTime(Convert.ToString(entryRow[8])).ToString("yyyy/MM/dd HH:mm"),
                        Convert.ToString(entryRow[9]),
                        CompanyID, BranchID);
                    DatabaseQuery.Insert(entryQuery);
                }

                return "Paid Successfully";
            }
            catch (Exception ex)
            {
                return "Unexpected Error is Occured. Please Try Again!";
            }
        }

        private void SetEntries(
       string FinancialYearID,
       string AccountHeadID,
       string AccountControlID,
       string AccountSubControlID,
       string InvoiceNo,
       string UserID,
       string Credit,
       string Debit,
       DateTime TransactionDate,
       string TransectionTitle)
        {
            if (dtEntries == null)
            {
                dtEntries = new DataTable();
                dtEntries.Columns.Add("FinancialYearID");
                dtEntries.Columns.Add("AccountHeadID");
                dtEntries.Columns.Add("AccountControlID");
                dtEntries.Columns.Add("AccountSubControlID");
                dtEntries.Columns.Add("InvoiceNo");
                dtEntries.Columns.Add("UserID");
                dtEntries.Columns.Add("Credit");
                dtEntries.Columns.Add("Debit");
                dtEntries.Columns.Add("TransactionDate");
                dtEntries.Columns.Add("TransectionTitle");
            }

            if (dtEntries != null)
            {
                dtEntries.Rows.Add(
                FinancialYearID,
                AccountHeadID,
                AccountControlID,
                AccountSubControlID,
                InvoiceNo,
                UserID,
                Credit,
                Debit,
                TransactionDate.ToString("yyyy/MM/dd HH:mm"),
                TransectionTitle);
            }
        }
    }
}
