using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Services.Implementations
{
    public abstract class TransactionServiceBase
    {
        protected readonly ILogger _logger;
        protected readonly IFinancialYearRepository _financialYearRepository;
        protected readonly IAccountSettingRepository _accountSettingRepository;

        protected TransactionServiceBase(
            IFinancialYearRepository financialYearRepository,
            IAccountSettingRepository accountSettingRepository,
            ILogger logger)
        {
            _financialYearRepository = financialYearRepository ?? throw new ArgumentNullException(nameof(financialYearRepository));
            _accountSettingRepository = accountSettingRepository ?? throw new ArgumentNullException(nameof(accountSettingRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected async Task<string> GetFinancialYearID()
        {
            var financialYear = await _financialYearRepository.GetSingleActiveAsync();
            return financialYear != null ? Convert.ToString(financialYear) : string.Empty;
        }

        protected async Task AddTransactionEntry<T>(
            TransactionBuilder builder,
            string financialYearID,
            T activityType,
            int companyID,
            int branchID,
            string userID,
            string invoiceNo,
            float amount,
            string transactionTitle,
            bool isCredit)
            where T : Enum
        {
            var account = await _accountSettingRepository.GetByActivityAsync(Convert.ToInt32(activityType), companyID, branchID);
            if (account == null)
                throw new InvalidOperationException($"Account settings for {activityType} not found");

            builder.AddEntry(
                financialYearID,
                account.AccountHeadID.ToString(),
                account.AccountControlID.ToString(),
                account.AccountSubControlID.ToString(),
                invoiceNo,
                userID,
                isCredit ? amount : 0,
                isCredit ? 0 : amount,
                DateTime.Now,
                transactionTitle);
        }

        protected static string GenerateInvoiceNo(string prefix)
        {
            return $"{prefix}{DateTime.Now:yyyyMMddHHmmss}{DateTime.Now.Millisecond}";
        }

        protected static DataTable ConvertToDataTable(List<TransactionEntry> entries)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("FinancialYearID", typeof(string));
            dataTable.Columns.Add("AccountHeadID", typeof(string));
            dataTable.Columns.Add("AccountControlID", typeof(string));
            dataTable.Columns.Add("AccountSubControlID", typeof(string));
            dataTable.Columns.Add("InvoiceNo", typeof(string));
            dataTable.Columns.Add("UserID", typeof(string));
            dataTable.Columns.Add("Credit", typeof(string));
            dataTable.Columns.Add("Debit", typeof(string));
            dataTable.Columns.Add("TransactionDate", typeof(string));
            dataTable.Columns.Add("TransactionTitle", typeof(string));

            foreach (var entry in entries)
            {
                dataTable.Rows.Add(
                    entry.FinancialYearID,
                    entry.AccountHeadID,
                    entry.AccountControlID,
                    entry.AccountSubControlID,
                    entry.InvoiceNo,
                    entry.UserID,
                    entry.Credit.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    entry.Debit.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    entry.TransactionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    entry.TransactionTitle);
            }

            return dataTable;
        }
    }
}