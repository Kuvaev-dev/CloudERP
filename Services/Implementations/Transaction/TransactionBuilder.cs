using Domain.Models;

namespace Services.Implementations
{
    public class TransactionBuilder
    {
        private readonly List<TransactionEntry> _entries = new();

        public TransactionBuilder AddEntry(
            string financialYearID,
            string accountHeadID,
            string accountControlID,
            string accountSubControlID,
            string invoiceNo,
            string userID,
            float credit,
            float debit,
            DateTime transactionDate,
            string transactionTitle)
        {
            _entries.Add(new TransactionEntry
            {
                FinancialYearID = financialYearID,
                AccountHeadID = accountHeadID,
                AccountControlID = accountControlID,
                AccountSubControlID = accountSubControlID,
                InvoiceNo = invoiceNo,
                UserID = userID,
                Credit = credit,
                Debit = debit,
                TransactionDate = transactionDate,
                TransactionTitle = transactionTitle
            });
            return this;
        }

        public List<TransactionEntry> Build() => _entries;
    }
}
