using System;
using System.Data;
using System.Linq;

namespace DatabaseAccess.Code
{
    public class PurchaseEntry
    {
        private CloudDBEntities _db;
        public string selectsupplierid = string.Empty;
        private DataTable _dtEntries = null;

        public PurchaseEntry(CloudDBEntities db)
        {
            _db = db;
        }

        public string ConfirmPurchase(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, float Amount, string SupplierID, string SupplierName, bool isPayment)
        {
            try
            {
                _dtEntries = null;

                string pruchasetitle = "Purchase From " + SupplierName.Trim();

                var financialYearCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                
                string FinancialYearID = string.Empty;

                if (financialYearCheck != null && financialYearCheck.Rows.Count > 0)
                {
                    FinancialYearID = Convert.ToString(financialYearCheck.Rows[0][0]);
                }

                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set! Please Contact to Administrator!";
                }

                string successmessage = "Purchase Success";
                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;

                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)

                var purchaseAccount = _db.tblAccountSetting.Where(a => a.AccountActivityID == 3 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); // 3 - Purchase
                
                // Debit Entry Purchase                                                                                                                                            
                AccountHeadID = Convert.ToString(purchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(purchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(purchaseAccount.AccountSubControlID);
                string transectiontitle = string.Empty;
                transectiontitle = "Purchase From " + SupplierName.Trim();
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);
                
                // Credit Entry Purchase
                purchaseAccount = _db.tblAccountSetting.Where(a => a.AccountActivityID == 8 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); // 8 - Purchase Payment Pending
                AccountHeadID = Convert.ToString(purchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(purchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(purchaseAccount.AccountSubControlID);
                transectiontitle = SupplierName + ", Purchase Payment is Pending!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);

                if (isPayment == true)
                {
                    string payinvoicenno = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                    purchaseAccount = _db.tblAccountSetting.Where(a => a.AccountActivityID == 8 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); ; // 8 - Purchase Payment Pending
                    AccountHeadID = Convert.ToString(purchaseAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(purchaseAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(purchaseAccount.AccountSubControlID);
                    transectiontitle = "Payment Paid to " + SupplierName;
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, payinvoicenno, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);
                    
                    purchaseAccount = _db.tblAccountSetting.Where(a => a.AccountActivityID == 9 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); ; // 9 - Purchase Payment Succed
                    AccountHeadID = Convert.ToString(purchaseAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(purchaseAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(purchaseAccount.AccountSubControlID);
                    transectiontitle = SupplierName + ", Purchase Payment is Succeed!";
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, payinvoicenno, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);

                    string paymentquery = string.Format("insert into tblSupplierPayment(SupplierID,SupplierInvoiceID,UserID,InvoiceNo,TotalAmount,PaymentAmount,RemainingBalance,CompanyID,BranchID,InvoiceDate) " +
                    "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}', '{9}')",
                    SupplierID, SupplierInvoiceID, UserID, payinvoicenno, Amount, Amount, "0", CompanyID, BranchID, DateTime.Now.ToString("yyyy/MM/dd"));
                    DatabaseQuery.Insert(paymentquery);

                    successmessage += " with Payment.";
                }

                foreach (DataRow entryRow in _dtEntries.Rows)
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
                return "Unexpected Error is Occured. Please Try Again!" + ex.Message;
            }
        }

        public string PurchasePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, float TotalAmount, float Amount, string SupplierID, string SupplierName, float RemainingBalance)
        {
            try
            {
                _dtEntries = null;
                string pruchasetitle = "Purchase From " + SupplierName.Trim();
                var financialYearCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = string.Empty;

                if (financialYearCheck != null && financialYearCheck.Rows.Count > 0)
                {
                    FinancialYearID = Convert.ToString(financialYearCheck.Rows[0][0]);
                }

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
                
                var purchaseAccount = _db.tblAccountSetting.Where(a => a.AccountActivityID == 3 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); // 3 - Purchase
                purchaseAccount = _db.tblAccountSetting.Where(a => a.AccountActivityID == 8 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); ; // 8 - Purchase Payment Pending
                AccountHeadID = Convert.ToString(purchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(purchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(purchaseAccount.AccountSubControlID);
                transectiontitle = "Payment Paid to " + SupplierName;
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);
                
                purchaseAccount = _db.tblAccountSetting.Where(a => a.AccountActivityID == 9 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); ; // 9 - Purchase Payment Succed
                AccountHeadID = Convert.ToString(purchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(purchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(purchaseAccount.AccountSubControlID);
                transectiontitle = SupplierName + ", Purchase Payment is Succeed!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);

                string paymentquery = string.Format("insert into tblSupplierPayment(SupplierID,SupplierInvoiceID,UserID,InvoiceNo,TotalAmount,PaymentAmount,RemainingBalance,CompanyID,BranchID,InvoiceDate) " +
                "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                SupplierID, SupplierInvoiceID, UserID, InvoiceNo, TotalAmount, Amount, Convert.ToString(RemainingBalance), CompanyID, BranchID, DateTime.Now.ToString("yyyy/MM/dd"));
                DatabaseQuery.Insert(paymentquery);

                foreach (DataRow entryRow in _dtEntries.Rows)
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

                return "Payment Is Paid";
            }
            catch (Exception ex)
            {
                return "Unexpected Error is Occured. Please Try Again!" + ex.Message;
            }
        }

        // Purchase Return
        public string ReturnPurchase(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, int SupplierReturnInvoiceID, float Amount, string SupplierID, string SupplierName, bool isPayment)
        {
            try
            {
                _dtEntries = null;
                string returnpruchasetitle = "Return Purchase to " + SupplierName.Trim();
                var financialYearCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = string.Empty;
                
                if (financialYearCheck != null && financialYearCheck.Rows.Count > 0)
                {
                    FinancialYearID = Convert.ToString(financialYearCheck.Rows[0][0]);
                }

                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set! Please Contact to Administrator!";
                }

                string successmessage = "Return Purchase Success";
                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;

                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)

                var returnPurchaseAccount = _db.tblAccountSetting.Where(a => a.AccountActivityID == 4 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); // 4 - Return Purchase
                
                // Credit Entry Return Purchase                                                                                                                                            
                AccountHeadID = Convert.ToString(returnPurchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnPurchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnPurchaseAccount.AccountSubControlID);
                string transectiontitle = string.Empty;
                transectiontitle = "Return Purchase to " + SupplierName.Trim();
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);
                
                // Debit Entry Return Purchase
                returnPurchaseAccount = _db.tblAccountSetting.Where(a => a.AccountActivityID == 12 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); // 12 - Purchase Return Payment Pending
                AccountHeadID = Convert.ToString(returnPurchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnPurchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnPurchaseAccount.AccountSubControlID);
                transectiontitle = SupplierName + ", Return Purchase Payment is Pending!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);

                if (isPayment == true)
                {
                    string payinvoicenno = "RPP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                    returnPurchaseAccount = _db.tblAccountSetting.Where(a => a.AccountActivityID == 12 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); ; // 12 - Purchase Return Payment Pending
                    AccountHeadID = Convert.ToString(returnPurchaseAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(returnPurchaseAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(returnPurchaseAccount.AccountSubControlID);
                    transectiontitle = "Return Payment from " + SupplierName;
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, payinvoicenno, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);
                    
                    returnPurchaseAccount = _db.tblAccountSetting.Where(a => a.AccountActivityID == 13 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); ; // 13 - Purchase Return Payment Succeed
                    AccountHeadID = Convert.ToString(returnPurchaseAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(returnPurchaseAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(returnPurchaseAccount.AccountSubControlID);
                    transectiontitle = SupplierName + ", Return Purchase Payment is Succeed!";
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, payinvoicenno, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);

                    string paymentquery = string.Format("insert into tblSupplierReturnPayment(SupplierID,SupplierInvoiceID,UserID,InvoiceNo,TotalAmount,PaymentAmount,RemainingBalance,CompanyID,BranchID,SupplierReturnInvoiceID,InvoiceDate) " +
                    "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                    SupplierID, SupplierInvoiceID, UserID, payinvoicenno, Amount, Amount, "0", CompanyID, BranchID, SupplierReturnInvoiceID, DateTime.Now.ToString("yyyy/MM/dd"));
                    DatabaseQuery.Insert(paymentquery);

                    successmessage += " with Payment.";
                }

                foreach (DataRow entryRow in _dtEntries.Rows)
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
                return "Unexpected Error is Occured. Please Try Again!" + ex.Message;
            }
        }

        public string ReturnPurchasePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, int SupplierReturnInvoiceID, float TotalAmount, float Amount, string SupplierID, string SupplierName, float RemainingBalance)
        {
            try
            {
                _dtEntries = null;
                string returnpruchasetitle = "Return Purchase to " + SupplierName.Trim();
                var financialYearCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = string.Empty;

                if (financialYearCheck != null && financialYearCheck.Rows.Count > 0)
                {
                    FinancialYearID = Convert.ToString(financialYearCheck.Rows[0][0]);
                }

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

                var returnPurchaseAccount = _db.tblAccountSetting.Where(a => a.AccountActivityID == 12 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); ; // 12 - Purchase Return Payment Pending
                AccountHeadID = Convert.ToString(returnPurchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnPurchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnPurchaseAccount.AccountSubControlID);
                transectiontitle = "Return Payment from " + SupplierName;
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);
                
                returnPurchaseAccount = _db.tblAccountSetting.Where(a => a.AccountActivityID == 13 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault(); ; // 13 - Purchase Return Payment Succeed
                AccountHeadID = Convert.ToString(returnPurchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnPurchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnPurchaseAccount.AccountSubControlID);
                transectiontitle = SupplierName + ", Return Purchase Payment is Succeed!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);

                string paymentquery = string.Format("insert into tblSupplierReturnPayment(SupplierID,SupplierInvoiceID,UserID,InvoiceNo,TotalAmount,PaymentAmount,RemainingBalance,CompanyID,BranchID,SupplierReturnInvoiceID,InvoiceDate) " +
                "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                SupplierID, SupplierInvoiceID, UserID, InvoiceNo, TotalAmount, Amount, Convert.ToString(RemainingBalance), CompanyID, BranchID, SupplierReturnInvoiceID, DateTime.Now.ToString("yyyy/MM/dd"));
                DatabaseQuery.Insert(paymentquery);

                foreach (DataRow entryRow in _dtEntries.Rows)
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

                return "Return Purchase Payment is Paid";
            }
            catch (Exception ex)
            {
                return "Unexpected Error is Occured. Please Try Again!" + ex.Message;
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
            if (_dtEntries == null)
            {
                _dtEntries = new DataTable();
                _dtEntries.Columns.Add("FinancialYearID");
                _dtEntries.Columns.Add("AccountHeadID");
                _dtEntries.Columns.Add("AccountControlID");
                _dtEntries.Columns.Add("AccountSubControlID");
                _dtEntries.Columns.Add("InvoiceNo");
                _dtEntries.Columns.Add("UserID");
                _dtEntries.Columns.Add("Credit");
                _dtEntries.Columns.Add("Debit");
                _dtEntries.Columns.Add("TransactionDate");
                _dtEntries.Columns.Add("TransectionTitle");
            }

            if (_dtEntries != null)
            {
                _dtEntries.Rows.Add(
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
