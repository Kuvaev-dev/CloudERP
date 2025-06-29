﻿using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using DatabaseAccess.Context;
using Domain.UtilsAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Financial
{
    public class GeneralTransactionRepository : IGeneralTransactionRepository
    {
        private readonly CloudDBEntities _dbContext;
        private readonly IDatabaseQuery _query;
        private readonly IAccountSubControlRepository _accountSubControlRepository;
        private readonly IConfiguration _configuration;

        private DataTable _dtEntries;

        public GeneralTransactionRepository(
            CloudDBEntities dbContext,
            IDatabaseQuery query,
            IAccountSubControlRepository accountSubControlRepository,
            IConfiguration configuration)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _query = query ?? throw new ArgumentNullException(nameof(query));
            _accountSubControlRepository = accountSubControlRepository ?? throw new ArgumentNullException(nameof(accountSubControlRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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

        public async Task<string> ConfirmGeneralTransaction(
            float transferAmount,
            int userId,
            int branchId,
            int companyId,
            string invoiceNo,
            int debitAccountControlId,
            int creditAccountControlId,
            string reason)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                InitializeDataTable();

                string transactionTitle = reason;
                var activeFinancialYear = await _dbContext.tblFinancialYear.FirstOrDefaultAsync(f => f.IsActive == true);
                int financialYearId = activeFinancialYear.FinancialYearID;

                if (financialYearId <= 0)
                {
                    return Localization.CloudERP.Messages.Messages.CompanyFinancialYearNotSet;
                }

                var debitAccount = await _accountSubControlRepository.GetBySettingAsync(debitAccountControlId, companyId, branchId);
                if (debitAccount == null)
                {
                    return Localization.Services.Localization.DebitAccountNotFound;
                }

                SetEntries(Convert.ToString(financialYearId),
                    debitAccount.AccountHeadID.ToString(),
                    debitAccount.AccountControlID.ToString(),
                    debitAccount.AccountSubControlID.ToString(),
                    invoiceNo,
                    userId.ToString(),
                    "0",
                    transferAmount.ToString(),
                    DateTime.Now, transactionTitle);

                var creditAccount = await _accountSubControlRepository.GetBySettingAsync(creditAccountControlId, companyId, branchId);
                if (creditAccount == null)
                {
                    return Localization.Services.Localization.CreditAccountNotFound;
                }

                transactionTitle = Localization.Services.Localization.GeneralTransactionSucceed;

                SetEntries(Convert.ToString(financialYearId),
                    creditAccount.AccountHeadID.ToString(),
                    creditAccount.AccountControlID.ToString(),
                    creditAccount.AccountSubControlID.ToString(),
                    invoiceNo,
                    userId.ToString(),
                    transferAmount.ToString(),
                    "0",
                    DateTime.Now, transactionTitle);

                foreach (DataRow entryRow in _dtEntries.Rows)
                {
                    string entryDate = Convert.ToDateTime(entryRow["TransectionDate"]).ToString("yyyy-MM-dd HH:mm:ss");
                    string entryQuery = "INSERT INTO tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID, Credit, Debit, TransectionDate, TransectionTitle, CompanyID, BranchID) " +
                                        "VALUES (@FinancialYearID, @AccountHeadID, @AccountControlID, @AccountSubControlID, @InvoiceNo, @UserID, @Credit, @Debit, @TransectionDate, @TransectionTitle, @CompanyID, @BranchID)";

                    var entryParams = new[]
                    {
                        new SqlParameter("@FinancialYearID", Convert.ToString(entryRow["FinancialYearID"])),
                        new SqlParameter("@AccountHeadID", Convert.ToString(entryRow["AccountHeadID"])),
                        new SqlParameter("@AccountControlID", Convert.ToString(entryRow["AccountControlID"])),
                        new SqlParameter("@AccountSubControlID", Convert.ToString(entryRow["AccountSubControlID"])),
                        new SqlParameter("@InvoiceNo", Convert.ToString(entryRow["InvoiceNo"])),
                        new SqlParameter("@UserID", Convert.ToString(entryRow["UserID"])),
                        new SqlParameter("@Credit", Convert.ToDecimal(entryRow["Credit"])),
                        new SqlParameter("@Debit", Convert.ToDecimal(entryRow["Debit"])),
                        new SqlParameter("@TransectionDate", DateTime.Parse(entryDate)),
                        new SqlParameter("@TransectionTitle", Convert.ToString(entryRow["TransectionTitle"])),
                        new SqlParameter("@CompanyID", companyId),
                        new SqlParameter("@BranchID", branchId)
                    };

                    await _query.ExecuteNonQueryAsync(entryQuery, entryParams);
                }

                transaction.Commit();
                return Localization.Services.Localization.GeneralTransactionSucceed;
            }
        }

        public async Task<List<AllAccountModel>> GetAllAccounts(int CompanyID, int BranchID)
        {
            var accountsList = new List<AllAccountModel>();

            using (SqlConnection connection = await _query.ConnOpenAsync() as SqlConnection)
            {
                using (SqlCommand command = new("GetAllAccounts", connection))
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
                        var account = new AllAccountModel
                        {
                            AccountHeadID = Convert.ToInt32(row["AccountHeadID"]),
                            AccountHeadName = Convert.ToString(row["AccountHeadName"]),
                            AccountControlID = Convert.ToInt32(row["AccountControlID"]),
                            AccountControlName = Convert.ToString(row["AccountControlName"]),
                            BranchID = Convert.ToInt32(row["BranchID"]),
                            CompanyID = Convert.ToInt32(row["CompanyID"]),
                            AccountSubControlID = Convert.ToInt32(row["AccountSubControlID"]),
                            AccountSubControl = Convert.ToString(row["AccountSubControl"])
                        };

                        accountsList.Add(account);
                    }
                }
            }

            return accountsList;
        }

        public async Task<List<JournalModel>> GetJournal(int CompanyID, int? BranchID, DateTime FromDate, DateTime ToDate)
        {
            var journalEntries = new List<JournalModel>();

            using (SqlConnection connection = await _query.ConnOpenAsync() as SqlConnection)
            {
                using (SqlCommand command = new("GetJournal", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@BranchID", BranchID));
                    command.Parameters.Add(new SqlParameter("@CompanyID", CompanyID));
                    command.Parameters.Add(new SqlParameter("@FromDate", FromDate));
                    command.Parameters.Add(new SqlParameter("@ToDate", ToDate));

                    var dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        da.Fill(dt);
                    }

                    int no = 1; // №
                    foreach (DataRow row in dt.Rows)
                    {
                        var entry = new JournalModel
                        {
                            TransectionDate = Convert.ToDateTime(row["TransectionDate"]),
                            AccountSubControl = Convert.ToString(row["AccountSubControl"]),
                            TransectionTitle = Convert.ToString(row["TransectionTitle"]),
                            AccountSubControlID = Convert.ToInt32(row["AccountSubControlID"]),
                            InvoiceNo = Convert.ToString(row["InvoiceNo"]),
                            Debit = Convert.ToDouble(row["Debit"]),
                            Credit = Convert.ToDouble(row["Credit"]),
                            SNO = no
                        };
                        journalEntries.Add(entry);
                        no += 1;
                    }
                }
            }

            return journalEntries;
        }

        private void SetEntries(string financialYearId, string accountHeadId, string accountControlId, string accountSubControlId, string invoiceNo, string userId, string credit, string debit, DateTime transactionDate, string transactionTitle)
        {
            InitializeDataTable();

            _dtEntries.Rows.Add(
                financialYearId,
                accountHeadId,
                accountControlId,
                accountSubControlId,
                invoiceNo,
                userId,
                credit,
                debit,
                transactionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                transactionTitle);
        }
    }
}
