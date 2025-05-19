using Domain.Facades;
using Domain.ServiceAccess;
using System.Data;

namespace Services.Implementations
{
    public class SalaryTransactionService : ISalaryTransactionService
    {
        private DataTable _dtEntries;
        private readonly SalaryTransactionFacade _salaryTransactionFacade;

        const int SALE_RETURN_PAYMENT_PENDING_ACTIVITY_ID = 8;

        public SalaryTransactionService(SalaryTransactionFacade salaryTransactionFacade)
        {
            _salaryTransactionFacade = salaryTransactionFacade ?? throw new ArgumentNullException(nameof(salaryTransactionFacade));
        }

        private void InitializeDataTable()
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
                _dtEntries.Columns.Add("TransectionDate");
                _dtEntries.Columns.Add("TransectionTitle");
            }
        }

        public async Task<string> Confirm(
            int EmployeeID,
            double TransferAmount,
            int UserID,
            int BranchID,
            int CompanyID,
            string InvoiceNo,
            string SalaryMonth,
            string SalaryYear)
        {
            try
            {
                string transectiontitle = Localization.Services.Localization.SalaryIsPending;

                var employee = await _salaryTransactionFacade.EmployeeRepository.GetByIdAsync(EmployeeID);
                string employeename = string.Empty;

                if (employee != null)
                {
                    employeename = Localization.Services.Localization.To + employee.FullName;
                    transectiontitle += employeename;
                    transectiontitle = $"{Localization.Services.Localization.SalarySucceed} {employee.FullName}";
                }

                var financialYearCheck = await _salaryTransactionFacade.FinancialYearRepository.GetSingleActiveAsync();
                string? FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return Localization.CloudERP.Messages.Messages.CompanyFinancialYearNotSet;
                }

                var account = await _salaryTransactionFacade.AccountSettingRepository.GetByActivityAsync(SALE_RETURN_PAYMENT_PENDING_ACTIVITY_ID, CompanyID, BranchID);
                if (account == null)
                {
                    return Localization.Services.Localization.AccountSettingsNotFoundForTheProvidedCompanyIDAndBranchID;
                }

                SetEntries(FinancialYearID,
                    Convert.ToString(account.AccountHeadID),
                    Convert.ToString(account.AccountControlID),
                    Convert.ToString(account.AccountSubControlID),
                    InvoiceNo,
                    UserID.ToString(),
                    "0",
                    Convert.ToString(TransferAmount),
                    DateTime.Now,
                    transectiontitle);

                transectiontitle = $"{Localization.Services.Localization.SalarySucceed} {employee.FullName}";

                SetEntries(FinancialYearID,
                    Convert.ToString(account.AccountHeadID),
                    Convert.ToString(account.AccountControlID),
                    Convert.ToString(account.AccountSubControlID),
                    InvoiceNo,
                    UserID.ToString(),
                    Convert.ToString(TransferAmount),
                    "0",
                    DateTime.Now,
                    transectiontitle);

                await _salaryTransactionFacade.SalaryTransactionRepository.Confirm(EmployeeID, TransferAmount, UserID, BranchID, CompanyID, InvoiceNo, SalaryMonth, SalaryYear);
                await _salaryTransactionFacade.SalaryTransactionRepository.InsertTransaction(CompanyID, BranchID);

                return Localization.Services.Localization.SalarySucceed;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return Localization.Services.Localization.UnexpectedErrorOccurred;
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
            InitializeDataTable();

            int columnCount = _dtEntries.Columns.Count;
            int itemCount = new object[]
            {
                FinancialYearID,
                AccountHeadID,
                AccountControlID,
                AccountSubControlID,
                InvoiceNo,
                UserID,
                Credit,
                Debit,
                TransactionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                TransectionTitle
            }.Length;

            if (itemCount == columnCount)
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
                    TransactionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    TransectionTitle);
            }
        }
    }
}
