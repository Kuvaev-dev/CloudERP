using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code
{
    public class PurchaseEntry
    {
        public string selectsupplierid = string.Empty;
        DataTable dtEntries = null;
        public string Purchase(int CompanyID, int BranchID, int UserID, string InvoiceNo, float Amount, tblSupplier supplier)
        {
            dtEntries = null;
            string pruchasetitle = "Purchase From " + supplier.SupplierName.Trim();
            float totalpurchaseamount = 0;
            var financialYearCheck = (DatabaseQuery.Retrive("select top 1 FinancialYearID from FinancialYearTable where IsActive = 1"));
            string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
            if (string.IsNullOrEmpty(FinancialYearID))
            {
                return "Your Company Financial Year is not Set! Please Contact to Administrator!";
            }

            // Add Invoice Header Details
            string insertsupplierinvoice = string.Format("insert into SupplierInvoiceTable(Supplier_ID, User_ID, InvoiceNo,Title,TotalAmount,InvoiceDate,Description) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                supplierid, CurrentUser.UserID, invoiceno, pruchasetitle, totalpurchaseamount, DateTime.Now, "");
            bool result = DatabaseAccess.Insert(insertsupplierinvoice);
            if (result == false)
            {
                ep.SetError(btnFinilize, "Please Check Purchase Info in finilizeing some issue!");
                btnFinilize.Focus();
                return;
            }

            // Get Invoice Purchase ID
            string SupplierInvoiceID = Convert.ToString(DatabaseAccess.Retrive("select Max(SupplierInvoiceID) from SupplierInvoiceTable").Rows[0][0]);
            if (string.IsNullOrEmpty(SupplierInvoiceID))
            {
                ep.SetError(btnFinilize, "Please Check Purchase Info in finilizeing some issue!");
                btnFinilize.Focus();
                return;
            }
            string successmessage = "Purchase Success";
            foreach (DataGridViewRow productrow in dgvPurchaseCart.Rows)
            {
                // Add Invoice Details by Products
                string invoicedetailsquery = string.Format("insert into SupplierInvoiceDetailsTable(SupplierInvoice_ID, Product_ID, PurchaseQty, PurchaseUnitPrice) values('{0}','{1}','{2}','{3}')",
                SupplierInvoiceID, productrow.Cells[2].Value, productrow.Cells[5].Value, productrow.Cells[6].Value);
                bool invoicedetailsresult = DatabaseAccess.Insert(invoicedetailsquery);
                if (invoicedetailsresult == false)
                {
                    break;
                }


                string purchaseqty = Convert.ToString(productrow.Cells[5].Value);

                string AccountHead_ID = string.Empty;
                string AccountControl_ID = string.Empty;
                string AccountSubControl_ID = string.Empty;
                Accounts.GetAccountDetails(Convert.ToString(productrow.Cells[4].Value), out AccountHead_ID, out AccountControl_ID, out AccountSubControl_ID);
                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)
                string transectiontitle = string.Empty;
                if (AccountControl_ID == "13" || AccountHead_ID == "3")  // Direct Costs
                {
                    transectiontitle = "Purchase From " + lblSupplier.Text.Trim();
                    SetEntries(FinancialYearID, "2", "3", "0", invoiceno, CurrentUser.UserID.ToString(), Convert.ToString(productrow.Cells[9].Value), "0", DateTime.Now, transectiontitle);

                    transectiontitle = lblSupplier.Text.Trim() + " , Purchase Payment is Pending!";
                    SetEntries(FinancialYearID, AccountHead_ID, AccountControl_ID, AccountSubControl_ID, invoiceno, CurrentUser.UserID.ToString(), "0", Convert.ToString(productrow.Cells[9].Value), DateTime.Now, transectiontitle);

                }
                else if (AccountControl_ID == "3" || AccountHead_ID == "2") // Account payable
                {
                    transectiontitle = "Payement Paid to " + lblSupplier.Text.Trim();
                    SetEntries(FinancialYearID, "1", "1", "0", invoiceno, CurrentUser.UserID.ToString(), Convert.ToString(productrow.Cells[9].Value), "0", DateTime.Now, transectiontitle);

                    transectiontitle = lblSupplier.Text.Trim() + " , Purchase Payment is Succesed!";
                    SetEntries(FinancialYearID, AccountHead_ID, AccountControl_ID, AccountSubControl_ID, invoiceno, CurrentUser.UserID.ToString(), "0", Convert.ToString(productrow.Cells[9].Value), DateTime.Now, transectiontitle);
                }

                else if (AccountControl_ID == "2" || AccountHead_ID == "1") // Account Recevible
                {
                    transectiontitle = "Get Products in Payments from " + lblSupplier.Text.Trim();
                    SetEntries(FinancialYearID, AccountHead_ID, AccountControl_ID, AccountSubControl_ID, invoiceno, CurrentUser.UserID.ToString(), Convert.ToString(productrow.Cells[9].Value), "0", DateTime.Now, transectiontitle);

                    transectiontitle = lblSupplier.Text.Trim() + " , Get Product in Payments!";
                    SetEntries(FinancialYearID, "3", "13", AccountSubControl_ID, invoiceno, CurrentUser.UserID.ToString(), "0", Convert.ToString(productrow.Cells[9].Value), DateTime.Now, transectiontitle);
                }
                else if (AccountControl_ID == "1016" || AccountHead_ID == "5") // Return Purchase
                {
                    transectiontitle = "Return Product to " + lblSupplier.Text.Trim();
                    SetEntries(FinancialYearID, AccountHead_ID, AccountControl_ID, AccountSubControl_ID, invoiceno, CurrentUser.UserID.ToString(), Convert.ToString(productrow.Cells[9].Value), "0", DateTime.Now, transectiontitle);

                    transectiontitle = lblSupplier.Text.Trim() + " , Get Product in Payments!";
                    SetEntries(FinancialYearID, "1", "2", "0", invoiceno, CurrentUser.UserID.ToString(), "0", Convert.ToString(productrow.Cells[9].Value), DateTime.Now, transectiontitle);
                    purchaseqty = "-" + purchaseqty;
                }
                else if (AccountControl_ID == "1015" || AccountHead_ID == "4") // Products from Capital
                {
                    transectiontitle = "Owner Investment in Products Format" + lblSupplier.Text.Trim();
                    SetEntries(FinancialYearID, AccountHead_ID, AccountControl_ID, AccountSubControl_ID, invoiceno, CurrentUser.UserID.ToString(), Convert.ToString(productrow.Cells[9].Value), "0", DateTime.Now, transectiontitle);

                    transectiontitle = lblSupplier.Text.Trim() + " , Get Product From Owner!";
                    SetEntries(FinancialYearID, "3", "13", AccountSubControl_ID, invoiceno, CurrentUser.UserID.ToString(), "0", Convert.ToString(productrow.Cells[9].Value), DateTime.Now, transectiontitle);
                }


                // Stock Updated
                string updatestockquery = string.Format("update StockTable set Quantity = Quantity + '{0}', SaleUnitPrice = '{1}', PurchaseUnitPrice = '{2}', ExpiryDate= '{3}', MfturDate = '{4}', StockTrasholdQty = '{5}' where ProductID = '{6}'",
                purchaseqty, Convert.ToString(productrow.Cells[7].Value).Trim(), Convert.ToString(productrow.Cells[6].Value).Trim(), Convert.ToString(productrow.Cells[11].Value).Trim(), Convert.ToString(productrow.Cells[10].Value).Trim(), Convert.ToString(productrow.Cells[8].Value).Trim(), Convert.ToString(productrow.Cells[2].Value).Trim());
                bool productupdateresult = DatabaseAccess.Update(updatestockquery);
                if (productupdateresult == false)
                {
                    Accounts.DeletePurchaseInvoiceDetails(SupplierInvoiceID);
                    Accounts.DeleteTransectionDetails(invoiceno);
                    break;
                }


            }

            if (dtEntries != null)
            {
                foreach (DataRow entryrow in dtEntries.Rows)
                {
                    string entryquery = string.Format("insert into TransactionsTable (FinancialYearID, AccountHead_ID, AccountControl_ID, AccountSubControl_ID,InvoiceNo, User_ID,Credit,Debit,TransactionDate,TransectionTitle) values " +
                 " ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                 Convert.ToString(entryrow[0]), Convert.ToString(entryrow[1]), Convert.ToString(entryrow[2]), Convert.ToString(entryrow[3]), Convert.ToString(entryrow[4]), Convert.ToString(entryrow[5]), Convert.ToString(entryrow[6]), Convert.ToString(entryrow[7]), Convert.ToString(entryrow[8]), Convert.ToString(entryrow[9]));
                    DatabaseAccess.Insert(entryquery);
                }

            }

            if (chkSetPayment.Checked == true)
            {
                string payinvoicenno = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmssmm");

                string transectiontitle = "Payement Paid to " + lblSupplier.Text.Trim();
                string creditquery = string.Format("insert into TransactionsTable (FinancialYearID, AccountHead_ID, AccountControl_ID, AccountSubControl_ID,InvoiceNo, User_ID,Credit,Debit,TransactionDate,TransectionTitle) values " +
                    " ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                    FinancialYearID, "1", "1", "0", payinvoicenno, CurrentUser.UserID, totalpurchaseamount, "0", DateTime.Now, transectiontitle);

                transectiontitle = lblSupplier.Text.Trim() + " , Purchase Payment is Succesed!";
                string debitquery = string.Format("insert into TransactionsTable (FinancialYearID, AccountHead_ID, AccountControl_ID, AccountSubControl_ID,InvoiceNo, User_ID,Credit,Debit,TransactionDate,TransectionTitle) values " +
                    " ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                    FinancialYearID, "2", "3", "0", payinvoicenno, CurrentUser.UserID, "0", totalpurchaseamount, DateTime.Now, transectiontitle);
                DatabaseAccess.Insert(creditquery);
                DatabaseAccess.Insert(debitquery);
                string paymentquery = string.Format("insert into SupplierPaymentTable(Supplier_ID,SupplierInvoice_ID,User_ID,InvoiceNo,TotalAmount,PaymentAmount,RemainingBalance) " +
                "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                supplierid, SupplierInvoiceID, CurrentUser.UserID, payinvoicenno, totalpurchaseamount, totalpurchaseamount, "0");
                DatabaseAccess.Insert(paymentquery);

                successmessage = successmessage + " with Payment.";

            }
            frmPurchaseInvoice frmInvoce = new frmPurchaseInvoice(invoiceno);
            frmInvoce.ShowDialog();
            MessageBox.Show(successmessage);
            this.Close();


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
                if (dtEntries.Rows.Count == 0)
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
                    TransactionDate,
                    TransectionTitle);
                }
                else
                {
                    bool isupdated = false;
                    foreach (DataRow item in dtEntries.Rows)
                    {
                        decimal creditvalue = 0;
                        decimal debetvalue = 0;
                        decimal.TryParse(Convert.ToString(item[6]).Trim(), out creditvalue);
                        decimal.TryParse(Convert.ToString(item[7]).Trim(), out debetvalue);

                        if (Convert.ToString(item[1]).Trim() == AccountHeadID.Trim() &&
                           Convert.ToString(item[2]).Trim() == AccountControlID.Trim() &&
                           Convert.ToString(item[3]).Trim() == AccountSubControlID.Trim() &&
                           creditvalue > 0)
                        {
                            item[6] = (creditvalue + Convert.ToDecimal(Credit));
                            isupdated = true;

                        }
                        else if (Convert.ToString(item[1]).Trim() == AccountHeadID.Trim() &&
                          Convert.ToString(item[2]).Trim() == AccountControlID.Trim() &&
                          Convert.ToString(item[3]).Trim() == AccountSubControlID.Trim() &&
                          debetvalue > 0)
                        {
                            item[7] = (debetvalue + Convert.ToDecimal(Debit));
                            isupdated = true;
                        }
                    }

                    if (isupdated == false)
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
                        TransactionDate,
                        TransectionTitle);

                    }
                }
            }
        }
    }
}
