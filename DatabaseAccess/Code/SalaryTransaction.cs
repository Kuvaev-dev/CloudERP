using System;
using System.Data;
using System.Threading.Tasks;
using DatabaseAccess.Localization;
using Domain.RepositoryAccess;

public class SalaryTransaction
{
    private DataTable _dtEntries = null;
    private readonly ISalaryTransactionRepository _salaryTransactionRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IFinancialYearRepository _financialYearRepository;
    private readonly IAccountSettingRepository _accountSettingRepository;

    public SalaryTransaction(ISalaryTransactionRepository salaryTransactionRepository, IEmployeeRepository employeeRepository, IFinancialYearRepository financialYearRepository, IAccountSettingRepository accountSettingRepository)
    {
        _salaryTransactionRepository = salaryTransactionRepository;
        _employeeRepository = employeeRepository;
        _financialYearRepository = financialYearRepository;
        _accountSettingRepository = accountSettingRepository;
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
            string transectiontitle = Localization.SalaryIsPending;

            var employee = await _employeeRepository.GetByIdAsync(EmployeeID);
            string employeename = string.Empty;

            if (employee != null)
            {
                employeename = Localization.To + employee.FullName;
                transectiontitle += employeename;
            }

            var financialYearCheck = await _financialYearRepository.GetSingleActiveAsync();
            string FinancialYearID = financialYearCheck != null ? Convert.ToString(financialYearCheck) : string.Empty;
            if (string.IsNullOrEmpty(FinancialYearID))
            {
                return Localization.CompanyFinancialYearNotSet;
            }

            // 8 - Sale Return Payment Pending
            var account = await _accountSettingRepository.GetByActivityAsync(8, CompanyID, BranchID);
            if (account == null)
            {
                return Localization.AccountSettingsNotFoundForTheProvidedCompanyIDAndBranchID;
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

            transectiontitle = Localization.SalarySucceed + employee.FullName;

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

            await _salaryTransactionRepository.Confirm(EmployeeID, TransferAmount, UserID, BranchID, CompanyID, InvoiceNo, SalaryMonth, SalaryYear);
            await _salaryTransactionRepository.InsertTransaction(CompanyID, BranchID);
                
            return Localization.SalarySucceed;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            return Localization.UnexpectedErrorOccurred;
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