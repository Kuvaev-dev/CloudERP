SELECT        dbo.tblTransaction.TransactionID, dbo.tblTransaction.FinancialYearID, dbo.tblTransaction.AccountHeadID, dbo.tblTransaction.AccountControlID, dbo.tblTransaction.AccountSubControlID, 
                         dbo.tblAccountHead.AccountHeadName + '-/-' + dbo.tblAccountControl.AccountControlName + '-/-' + dbo.tblAccountSubControl.AccountSubControlName AS AccountTitle, dbo.tblTransaction.InvoiceNo, 
                         dbo.tblTransaction.CompanyID, dbo.tblTransaction.BranchID, dbo.tblTransaction.Debit, dbo.tblTransaction.Credit, dbo.tblTransaction.TransectionDate, dbo.tblTransaction.TransectionTitle, dbo.tblTransaction.UserID
FROM            dbo.tblTransaction INNER JOIN
                         dbo.tblAccountSubControl ON dbo.tblTransaction.AccountSubControlID = dbo.tblAccountSubControl.AccountSubControlID INNER JOIN
                         dbo.tblAccountHead ON dbo.tblTransaction.AccountHeadID = dbo.tblAccountHead.AccountHeadID AND dbo.tblTransaction.AccountHeadID = dbo.tblAccountHead.AccountHeadID INNER JOIN
                         dbo.tblAccountControl ON dbo.tblTransaction.AccountControlID = dbo.tblAccountControl.AccountControlID
/****** Object:  StoredProcedure [dbo].[GetAccountHeadDetails]    Script Date: 01.05.2024 22:58:01 ******/
CREATE proc [dbo].[GetAccountHeadDetails](@HeadID as int, @FinancialYearID as int, @BranchID as int, @CompanyID as int)
as
begin
	select [v_Transaction].[AccountTitle],
	case when sum([v_Transaction].[Debit]) > sum([v_Transaction].[Credit]) then isnull(sum([v_Transaction].[Debit]) - sum([v_Transaction].[Credit]), 0)
		 when sum([v_Transaction].[Debit]) < sum([v_Transaction].[Credit]) then isnull(sum([v_Transaction].[Credit]) - sum([v_Transaction].[Debit]), 0) else 0 end as [Total],
	case when sum([v_Transaction].[Debit]) > sum([v_Transaction].[Credit]) then 'Debit'
		 when sum([v_Transaction].[Debit]) < sum([v_Transaction].[Credit]) then 'Credit' else '0' end as [Status]
	from [v_Transaction] 
	where [v_Transaction].[AccountHeadID] = isnull(@HeadID, 0) 
						   and [v_Transaction].[FinancialYearID] = isnull(@FinancialYearID, 0) 
						   and [v_Transaction].[BranchID] = @BranchID 
						   and [v_Transaction].[CompanyID] = @CompanyID 
    group by [v_Transaction].[AccountTitle]
end
GO
/****** Object:  StoredProcedure [dbo].[GetAccountTotalAmount]    Script Date: 01.05.2024 22:58:01 ******/
CREATE proc [dbo].[GetAccountTotalAmount](@FromDate as DATE, @ToDate as DATE, @HeadID as int, @BranchID as int, @CompanyID as int)
as
begin
select [v_Transaction].[AccountTitle],
	case when sum([v_Transaction].[Debit]) > sum([v_Transaction].[Credit]) then isnull(sum([v_Transaction].[Debit]) - sum([v_Transaction].[Credit]), 0)
		 when sum([v_Transaction].[Debit]) < sum([v_Transaction].[Credit]) then isnull(sum([v_Transaction].[Credit]) - sum([v_Transaction].[Debit]), 0) else 0 end as [Total],
	case when sum([v_Transaction].[Debit]) > sum([v_Transaction].[Credit]) then 'Debit'
		 when sum([v_Transaction].[Debit]) < sum([v_Transaction].[Credit]) then 'Credit' else '0' end as [Status]
	from [v_Transaction] 
	where [v_Transaction].[TransectionDate] > dateadd(day, -1, @FromDate) 
		  and [v_Transaction].[TransectionDate] < dateadd(day, -1, @ToDate)
		  and [v_Transaction].[AccountHeadID] = isnull(@HeadID, 0) 
		  and [v_Transaction].[BranchID] = @BranchID 
		  and [v_Transaction].[CompanyID] = @CompanyID 
    group by [v_Transaction].[AccountTitle]
end
GO
/****** Object:  StoredProcedure [dbo].[GetAllAccounts]    Script Date: 01.05.2024 22:58:01 ******/
create proc [dbo].[GetAllAccounts](@BranchID as int, @CompanyID as int)
as
begin
select [AHC].[AccountHeadID],
[AHC].[AccountHeadName],
[AHC].[AccountControlID],
[AHC].[AccountControlName],
[AHC].[BranchID],
[AHC].[CompanyID],
[ACS].[AccountSubControlID],
[AHC].[AccountControl] + '-/- ' + [ACS].[AccountSubControlName] [AccountSubControl]
from [dbo].[tblAccountSubControl] [ACS]
inner join
(select [AH].[AccountHeadID],
[AH].[AccountHeadName],
[AC].[AccountControlID],
[AC].[AccountControlName],
[AH].[AccountHeadName] + '-/-' + [AC].[AccountControlName] [AccountControl],
[AC].[BranchID],
[AC].[CompanyID]
from [dbo].[tblAccountHead] [AH]
inner join [dbo].[tblAccountControl] [AC]
on [AH].[AccountHeadID] = [AC].[AccountHeadID]) [AHC]
on [ACS].[AccountControlID] = [AHC].[AccountControlID]
where [AHC].[BranchID] = @BranchID and [AHC].[CompanyID] = @CompanyID
order by [AHC].[AccountHeadID]
end
GO
/****** Object:  StoredProcedure [dbo].[GetCustomerPaymentHistory]    Script Date: 01.05.2024 22:58:01 ******/
CREATE proc [dbo].[GetCustomerPaymentHistory](@CustomerInvoiceID as int)
as begin
select [CI].[CustomerInvoiceID], [CI].[BranchID], [CI].[CompanyID], [CI].[InvoiceDate], [CI].[CustomerID],
[CP].[InvoiceNo], cast([CI].[TotalAmount] as decimal(10, 2)), [CP].[PaidAmount], [CP].[RemainingBalance], [CP].[UserID]
from [dbo].[tblCustomerInvoice] [CI] 
inner join 
(select [dbo].[tblCustomerPayment].[CustomerInvoiceID],
[dbo].[tblCustomerPayment].[BranchID], 
[dbo].[tblCustomerPayment].[CompanyID], 
[dbo].[tblCustomerPayment].[PaidAmount], 
[dbo].[tblCustomerPayment].[RemainingBalance],
[dbo].[tblCustomerPayment].[UserID],
[dbo].[tblCustomerPayment].[InvoiceNo]
from [dbo].[tblCustomerPayment]) [CP]
on [CI].[CustomerInvoiceID] = [CP].[CustomerInvoiceID] 
where [CI].[CustomerInvoiceID] = ISNULL(@CustomerInvoiceID, 0)
end;
GO
/****** Object:  StoredProcedure [dbo].[GetCustomerRemainingPaymentRecord]    Script Date: 01.05.2024 22:58:01 ******/
CREATE proc [dbo].[GetCustomerRemainingPaymentRecord](@BranchID as int, @CompanyID as int)
as begin
select [CI].[CustomerInvoiceID], [CI].[BranchID], [CI].[CompanyID], [CI].[InvoiceDate], [CI].[CustomerID],
[CI].[InvoiceNo], cast([CI].[TotalAmount] as decimal(10, 2)), [CP].[Payment], (cast([CI].[TotalAmount] as decimal(10, 2)) - [CP].[Payment]) [RemainingBalance] from [dbo].[tblCustomerInvoice] [CI] 
full join (select [CustomerInvoiceID], [BranchID], [CompanyID], sum([PaidAmount]) as [Payment]
from [dbo].[tblCustomerPayment] group by [CustomerInvoiceID], [BranchID], [CompanyID]) [CP]
on [CI].[CustomerInvoiceID] = [CP].[CustomerInvoiceID] where [CI].[CompanyID] = @CompanyID and [CI].[BranchID] = @BranchID
and cast([CI].[TotalAmount] as decimal(10, 2)) > ISNULL([CP].[Payment], 0)
end;
GO
/****** Object:  StoredProcedure [dbo].[GetCustomerReturnSalePaidPending]    Script Date: 01.05.2024 22:58:01 ******/
create proc [dbo].[GetCustomerReturnSalePaidPending](@CustomerInvoiceID as int)
as begin
select [RCI].[CustomerReturnInvoiceID], [RCI].[CustomerInvoiceID], [RCI].[BranchID], [RCI].[CompanyID], [RCI].[InvoiceDate], [RCI].[CustomerID],
[RCI].[InvoiceNo], isnull(cast([RCI].[TotalAmount] as decimal(10, 2)), 0) [ReturnTotal], isnull([CP].[SPayment], 0) [ReturnPayment],
isnull(cast([RCI].[TotalAmount] as decimal(10, 2)), 0) - isnull([CP].[SPayment], 0) [ReturnRemainingPayment], [RCI].[UserID]
from [dbo].[tblCustomerReturnInvoice] [RCI]
full join 
(select [CustomerReturnInvoiceID],  
sum([PaidAmount]) [SPayment]
from [dbo].[tblCustomerReturnPayment]
group by [CustomerReturnInvoiceID]) [CP]
on [RCI].[CustomerReturnInvoiceID] = [CP].[CustomerReturnInvoiceID] 
where [RCI].[CustomerInvoiceID] = ISNULL(@CustomerInvoiceID, 0)
and isnull(cast([RCI].[TotalAmount] as decimal(10, 2)), 0) - isnull([CP].[SPayment], 0) > 0
end;
GO
/****** Object:  StoredProcedure [dbo].[GetDashboardValues]    Script Date: 01.05.2024 22:58:01 ******/
CREATE proc [dbo].[GetDashboardValues](@FromDate as date, @ToDate as date, @BranchID as int, @CompanyID as int)
as
begin
	declare @CurrentMonthRevenue as float = 0;
	declare @CurrentMonthExpenses as float = 0;
	declare @NetIncome as float = 0;
	declare @CurrentMonthRecovery as float = 0;
	declare @CashplusBankaccountBalance as float = 0;
	declare @TotalReceivable as float = 0;
	declare @TotalPayable as float = 0;
	declare @Capital as float = 0;

    DECLARE @TotalSalesToday FLOAT = 0;
    DECLARE @SalesPaymentPendingToday FLOAT = 0;
    DECLARE @SalesPaymentPaidToday FLOAT = 0;
    DECLARE @TotalPurchasesToday FLOAT = 0;
    DECLARE @PurchasesPaymentPendingToday FLOAT = 0;
    DECLARE @PurchasesPaymentPaidToday FLOAT = 0;
    DECLARE @TotalReturnSalesToday FLOAT = 0;
    DECLARE @ReturnSalesPaymentPendingToday FLOAT = 0;
    DECLARE @ReturnSalesPaymentPaidToday FLOAT = 0;
    DECLARE @TotalReturnPurchasesToday FLOAT = 0;
    DECLARE @ReturnPurchasesPaymentPendingToday FLOAT = 0;
    DECLARE @ReturnPurchasesPaymentPaidToday FLOAT = 0;

    DECLARE @TotalSalesCurrentMonth FLOAT = 0;
    DECLARE @SalesPaymentPendingCurrentMonth FLOAT = 0;
    DECLARE @SalesPaymentPaidCurrentMonth FLOAT = 0;
    DECLARE @TotalPurchasesCurrentMonth FLOAT = 0;
    DECLARE @PurchasesPaymentPendingCurrentMonth FLOAT = 0;
    DECLARE @PurchasesPaymentPaidCurrentMonth FLOAT = 0;
    DECLARE @TotalReturnSalesCurrentMonth FLOAT = 0;
    DECLARE @ReturnSalesPaymentPendingCurrentMonth FLOAT = 0;
    DECLARE @ReturnSalesPaymentPaidCurrentMonth FLOAT = 0;
    DECLARE @TotalReturnPurchasesCurrentMonth FLOAT = 0;
    DECLARE @ReturnPurchasesPaymentPendingCurrentMonth FLOAT = 0;
    DECLARE @ReturnPurchasesPaymentPaidCurrentMonth FLOAT = 0;

	-- Current Month Revenue
	-- 6 - Revenue
	SET @CurrentMonthRevenue = ISNULL((
		SELECT SUM([Credit]) - SUM([Debit]) AS [TotalAmount]
		FROM [v_Transaction]
		WHERE [TransectionDate] >= @FromDate 
		  AND [TransectionDate] <= @ToDate
		  AND [AccountHeadID] = 6
		  AND [BranchID] = @BranchID 
		  AND [CompanyID] = @CompanyID
	), 0);

	-- Current Month Expenses
	-- 5 - Expenses
	SET @CurrentMonthExpenses = ISNULL((
		SELECT SUM([Debit]) - SUM([Credit]) AS [TotalAmount]
		FROM [v_Transaction]
		WHERE [TransectionDate] >= @FromDate 
		  AND [TransectionDate] <= @ToDate
		  AND [AccountHeadID] = 5
		  AND [BranchID] = @BranchID 
		  AND [CompanyID] = @CompanyID
	), 0);

	-- Current Month Net Income/Loss
	SET @NetIncome = @CurrentMonthRevenue - @CurrentMonthExpenses;

	-- Total Capital
	-- 4 - Capital
	SET @Capital = ISNULL((
		SELECT CASE 
			WHEN SUM([TA].[Debit]) > SUM([TA].[Credit]) THEN SUM([TA].[Debit]) - SUM([TA].[Credit])
			WHEN SUM([TA].[Debit]) < SUM([TA].[Credit]) THEN SUM([TA].[Credit]) - SUM([TA].[Debit])
			ELSE 0 
		END AS TOTAL
		FROM (
			SELECT 
				ISNULL(SUM([v_Transaction].[Debit]) - SUM([v_Transaction].[Credit]), 0) AS [Debit],
				ISNULL(SUM([v_Transaction].[Credit]) - SUM([v_Transaction].[Debit]), 0) AS [Credit]
			FROM [v_Transaction]
			WHERE [AccountHeadID] = 4
			  AND [BranchID] = @BranchID
			  AND [CompanyID] = @CompanyID
			GROUP BY [AccountTitle]
		) AS [TA]
	), 0);

	-- Current Month Recovery
	-- 16 - Account Receivable, 17 - Note Receivable
	DECLARE @TotalAccountR FLOAT = ISNULL((
		SELECT SUM([Credit]) AS [TotalAmount]
		FROM [v_Transaction]
		WHERE [TransectionDate] >= @FromDate 
		  AND [TransectionDate] <= @ToDate
		  AND [AccountControlID] = (
			  SELECT TOP 1 [AccountControlID] 
			  FROM [tblAccountSetting] 
			  WHERE [AccountActivityID] = 16
		  )
		  AND [BranchID] = @BranchID 
		  AND [CompanyID] = @CompanyID
	), 0);

	DECLARE @TotalNoteR FLOAT = ISNULL((
		SELECT SUM([Credit]) AS [TotalAmount]
		FROM [v_Transaction]
		WHERE [TransectionDate] >= @FromDate 
		  AND [TransectionDate] <= @ToDate
		  AND [AccountControlID] = (
			  SELECT TOP 1 [AccountControlID] 
			  FROM [tblAccountSetting] 
			  WHERE [AccountActivityID] = 17
		  )
		  AND [BranchID] = @BranchID 
		  AND [CompanyID] = @CompanyID
	), 0);

	SET @CurrentMonthRecovery = @TotalAccountR + @TotalNoteR;

	-- Current Cash/Bank Balance
	-- 18 - Total Cash Balance
	SET @CashplusBankaccountBalance = ISNULL((
		SELECT SUM([Debit]) - SUM([Credit]) AS [TotalAmount]
		FROM [v_Transaction]
		WHERE [AccountHeadID] = (
			SELECT TOP 1 [AccountControlID] 
			FROM [tblAccountSetting] 
			WHERE [AccountActivityID] = 18
		)
		  AND [BranchID] = @BranchID 
		  AND [CompanyID] = @CompanyID
	), 0);

	-- Total Receivable
	-- 16 - Account Receivable, 17 - Note Receivable
	DECLARE @TotalAccountRPending FLOAT = ISNULL((
		SELECT SUM([Debit]) - SUM([Credit]) AS [TotalAmount]
		FROM [v_Transaction]
		WHERE [TransectionDate] >= @FromDate 
		  AND [TransectionDate] <= @ToDate
		  AND [AccountControlID] = (
			  SELECT TOP 1 [AccountControlID] 
			  FROM [tblAccountSetting] 
			  WHERE [AccountActivityID] = 16
		  )
		  AND [BranchID] = @BranchID 
		  AND [CompanyID] = @CompanyID
	), 0);

	DECLARE @TotalNoteRPending FLOAT = ISNULL((
		SELECT SUM([Debit]) - SUM([Credit]) AS [TotalAmount]
		FROM [v_Transaction]
		WHERE [TransectionDate] >= @FromDate 
		  AND [TransectionDate] <= @ToDate
		  AND [AccountControlID] = (
			  SELECT TOP 1 [AccountControlID] 
			  FROM [tblAccountSetting] 
			  WHERE [AccountActivityID] = 17
		  )
		  AND [BranchID] = @BranchID 
		  AND [CompanyID] = @CompanyID
	), 0);

	SET @TotalReceivable = @TotalAccountRPending + @TotalNoteRPending;

	-- Total Payable
	-- 19 - Account Payable, 20 - Note Payable
	DECLARE @TotalAccountPPending FLOAT = ISNULL((
		SELECT SUM([Credit]) - SUM([Debit]) AS [TotalAmount]
		FROM [v_Transaction]
		WHERE [TransectionDate] >= @FromDate 
		  AND [TransectionDate] <= @ToDate
		  AND [AccountControlID] = (
			  SELECT TOP 1 [AccountControlID] 
			  FROM [tblAccountSetting] 
			  WHERE [AccountActivityID] = 19
		  )
		  AND [BranchID] = @BranchID 
		  AND [CompanyID] = @CompanyID
	), 0);

	DECLARE @TotalNotePPending FLOAT = ISNULL((
		SELECT SUM([Credit]) - SUM([Debit]) AS [TotalAmount]
		FROM [v_Transaction]
		WHERE [TransectionDate] >= @FromDate 
		  AND [TransectionDate] <= @ToDate
		  AND [AccountControlID] = (
			  SELECT TOP 1 [AccountControlID] 
			  FROM [tblAccountSetting] 
			  WHERE [AccountActivityID] = 20
		  )
		  AND [BranchID] = @BranchID 
		  AND [CompanyID] = @CompanyID
	), 0);

	SET @TotalPayable = @TotalAccountPPending + @TotalNotePPending;

	-- For Today
    SELECT 
        @TotalSalesToday = ISNULL(SUM(TotalAmount), 0),
        @SalesPaymentPendingToday = ISNULL(SUM(RemainingBalance), 0),
        @SalesPaymentPaidToday = ISNULL(SUM(PaidAmount), 0)
    FROM tblCustomerPayment
    WHERE CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)
        AND BranchID = @BranchID
        AND CompanyID = @CompanyID;

    SELECT 
        @TotalPurchasesToday = ISNULL(SUM(TotalAmount), 0),
        @PurchasesPaymentPendingToday = ISNULL(SUM(RemainingBalance), 0),
        @PurchasesPaymentPaidToday = ISNULL(SUM(PaymentAmount), 0)
    FROM tblSupplierPayment
    WHERE CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)
        AND BranchID = @BranchID
        AND CompanyID = @CompanyID;

    SELECT 
        @TotalReturnSalesToday = ISNULL(SUM(TotalAmount), 0),
        @ReturnSalesPaymentPendingToday = ISNULL(SUM(RemainingBalance), 0),
        @ReturnSalesPaymentPaidToday = ISNULL(SUM(PaidAmount), 0)
    FROM tblCustomerReturnPayment
    WHERE CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)
        AND BranchID = @BranchID
        AND CompanyID = @CompanyID;

    SELECT 
        @TotalReturnPurchasesToday = ISNULL(SUM(TotalAmount), 0),
        @ReturnPurchasesPaymentPendingToday = ISNULL(SUM(RemainingBalance), 0),
        @ReturnPurchasesPaymentPaidToday = ISNULL(SUM(PaymentAmount), 0)
    FROM tblSupplierReturnPayment
    WHERE CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)
        AND BranchID = @BranchID
        AND CompanyID = @CompanyID;

    -- For A Month
    SELECT 
        @TotalSalesCurrentMonth = ISNULL(SUM(TotalAmount), 0),
        @SalesPaymentPendingCurrentMonth = ISNULL(SUM(RemainingBalance), 0),
        @SalesPaymentPaidCurrentMonth = ISNULL(SUM(PaidAmount), 0)
    FROM tblCustomerPayment
    WHERE MONTH(InvoiceDate) = MONTH(GETDATE())
        AND YEAR(InvoiceDate) = YEAR(GETDATE())
        AND BranchID = @BranchID
        AND CompanyID = @CompanyID;

    SELECT 
        @TotalPurchasesCurrentMonth = ISNULL(SUM(TotalAmount), 0),
        @PurchasesPaymentPendingCurrentMonth = ISNULL(SUM(RemainingBalance), 0),
        @PurchasesPaymentPaidCurrentMonth = ISNULL(SUM(PaymentAmount), 0)
    FROM tblSupplierPayment
    WHERE MONTH(InvoiceDate) = MONTH(GETDATE())
        AND YEAR(InvoiceDate) = YEAR(GETDATE())
        AND BranchID = @BranchID
        AND CompanyID = @CompanyID;

    SELECT 
        @TotalReturnSalesCurrentMonth = ISNULL(SUM(TotalAmount), 0),
        @ReturnSalesPaymentPendingCurrentMonth = ISNULL(SUM(RemainingBalance), 0),
        @ReturnSalesPaymentPaidCurrentMonth = ISNULL(SUM(PaidAmount), 0)
    FROM tblCustomerReturnPayment
    WHERE MONTH(InvoiceDate) = MONTH(GETDATE())
        AND YEAR(InvoiceDate) = YEAR(GETDATE())
        AND BranchID = @BranchID
        AND CompanyID = @CompanyID;

    SELECT 
        @TotalReturnPurchasesCurrentMonth = ISNULL(SUM(TotalAmount), 0),
        @ReturnPurchasesPaymentPendingCurrentMonth = ISNULL(SUM(RemainingBalance), 0),
        @ReturnPurchasesPaymentPaidCurrentMonth = ISNULL(SUM(PaymentAmount), 0)
    FROM tblSupplierReturnPayment
    WHERE MONTH(InvoiceDate) = MONTH(GETDATE())
        AND YEAR(InvoiceDate) = YEAR(GETDATE())
        AND BranchID = @BranchID
        AND CompanyID = @CompanyID;

	select 
	@CurrentMonthRevenue as [Current Month Revenue],
	@CurrentMonthExpenses as [Current Month Expenses],
	@NetIncome as [Net Income],
	@Capital as [Capital],
	@CurrentMonthRecovery as [Current Month Recovery],
	@CashplusBankaccountBalance as [Cash/Bank Balance],
	@TotalReceivable as [Total Receivable],
	@TotalPayable as [Total Payable],
	@TotalSalesToday AS [Total Sales Today],
    @SalesPaymentPendingToday AS [Sales Payment Pending Today],
    @SalesPaymentPaidToday AS [Sales Payment Paid Today],
    @TotalPurchasesToday AS [Total Purchases Today],
    @PurchasesPaymentPendingToday AS [Purchases Payment Pending Today],
    @PurchasesPaymentPaidToday AS [Purchases Payment Paid Today],
    @TotalReturnSalesToday AS [Total Return Sales Today],
    @ReturnSalesPaymentPendingToday AS [Return Sales Payment Pending Today],
    @ReturnSalesPaymentPaidToday AS [Return Sales Payment Paid Today],
    @TotalReturnPurchasesToday AS [Total Return Purchases Today],
    @ReturnPurchasesPaymentPendingToday AS [Return Purchases Payment Pending Today],
    @ReturnPurchasesPaymentPaidToday AS [Return Purchases Payment Paid Today],
    @TotalSalesCurrentMonth AS [Total Sales Current Month],
    @SalesPaymentPendingCurrentMonth AS [Sales Payment Pending Current Month],
    @SalesPaymentPaidCurrentMonth AS [Sales Payment Paid Current Month],
    @TotalPurchasesCurrentMonth AS [Total Purchases Current Month],
    @PurchasesPaymentPendingCurrentMonth AS [Purchases Payment Pending Current Month],
    @PurchasesPaymentPaidCurrentMonth AS [Purchases Payment Paid Current Month],
    @TotalReturnSalesCurrentMonth AS [Total Return Sales Current Month],
    @ReturnSalesPaymentPendingCurrentMonth AS [Return Sales Payment Pending Current Month],
    @ReturnSalesPaymentPaidCurrentMonth AS [Return Sales Payment Paid Current Month],
    @TotalReturnPurchasesCurrentMonth AS [Total Return Purchases Current Month],
    @ReturnPurchasesPaymentPendingCurrentMonth AS [Return Purchases Payment Pending Current Month],
    @ReturnPurchasesPaymentPaidCurrentMonth AS [Return Purchases Payment Paid Current Month];
end
/****** Object:  StoredProcedure [dbo].[GetJournal]    Script Date: 01.05.2024 22:58:01 ******/
CREATE proc [dbo].[GetJournal](@BranchID as int, @CompanyID as int, @FromDate as Date, @ToDate as Date)
as
begin
select [TR].[TransectionDate],
[ACTS].[AccountSubControl],
[TR].[TransectionTitle],
[ACTS].[AccountSubControlID],
[TR].[InvoiceNo],
[TR].[Debit],
[TR].[Credit]
from [dbo].[tblTransaction] [TR]
inner join 
(select [AHC].[AccountHeadID],
[AHC].[AccountHeadName],
[AHC].[AccountControlID],
[AHC].[AccountControlName],
[AHC].[BranchID],
[AHC].[CompanyID],
[ACS].[AccountSubControlID],
[AHC].[AccountControl] + '-/- ' + [ACS].[AccountSubControlName] [AccountSubControl]
from [dbo].[tblAccountSubControl] [ACS]
inner join
(select [AH].[AccountHeadID],
[AH].[AccountHeadName],
[AC].[AccountControlID],
[AC].[AccountControlName],
[AH].[AccountHeadName] + '-/-' + [AC].[AccountControlName] [AccountControl],
[AC].[BranchID],
[AC].[CompanyID]
from [dbo].[tblAccountHead] [AH]
inner join [dbo].[tblAccountControl] [AC]
on [AH].[AccountHeadID] = [AC].[AccountHeadID]) [AHC]
on [ACS].[AccountControlID] = [AHC].[AccountControlID]) [ACTS]
on [TR].[AccountSubControlID] = [ACTS].[AccountSubControlID]
where 
[TR].[BranchID] = @BranchID 
and [TR].[CompanyID] = @CompanyID
and [TR].[TransectionDate] > dateadd(day, -1, @FromDate)
and [TR].[TransectionDate] < dateadd(day, 1, @ToDate)
order by [TR].[TransactionID]
end
GO
/****** Object:  StoredProcedure [dbo].[GetLedger]    Script Date: 01.05.2024 22:58:01 ******/
create proc [dbo].[GetLedger](@BranchID as int, @CompanyID as int, @FinancialYearID as int)
as 
begin
	select [TransactionID], [FinancialYearID], [BranchID], [CompanyID],
		   [AccountHeadID], [AccountControlID], [AccountSubControlID],
		   [AccountTitle], [InvoiceNo], [TransectionDate], 
		   [TransectionTitle], [Debit], [Credit]
	from [v_Transaction]
	where
		BranchID = isnull(@BranchID, 0)
		and CompanyID = isnull(@CompanyID, 0)
		and FinancialYearID = isnull(@FinancialYearID, 0)
	order by [AccountSubControlID]
end
GO
/****** Object:  StoredProcedure [dbo].[GetPurchasesHistory]    Script Date: 01.05.2024 22:58:01 ******/
CREATE proc [dbo].[GetPurchasesHistory](@BranchID as int, @CompanyID as int, @FromDate as date, @ToDate as date)
as begin
select [SI].[SupplierInvoiceID], [SI].[BranchID], [SI].[CompanyID], [SI].[InvoiceDate], [SI].[SupplierID],
[SI].[InvoiceNo], cast([SI].[TotalAmount] as decimal(10, 2)) [BeforeReturnTotal], isnull([SR].[ReturnTotal], 0) [ReturnTotal],
isnull((cast([SI].[TotalAmount] as decimal(10, 2)) - isnull([SR].[ReturnTotal], 0)), 0) [AfterReturnTotal], isnull([SP].[Payment], 0) [PaidAmount], isnull([PR].[ReturnPayment], 0) [ReturnPayment], 
(isnull((cast([SI].[TotalAmount] as decimal(10, 2)) - isnull([SR].[ReturnTotal], 0)), 0) - isnull([SP].[Payment], 0) - isnull([PR].[ReturnPayment], 0)) [RemainingBalance] 
from [dbo].[tblSupplierInvoice] [SI] full join (select [SupplierInvoiceID], [BranchID], [CompanyID], sum([PaymentAmount]) as [Payment]
from [dbo].[tblSupplierPayment] group by [SupplierInvoiceID], [BranchID], [CompanyID]) [SP]
on [SI].[SupplierInvoiceID] = [SP].[SupplierInvoiceID] 
full join (select [SupplierInvoiceID], isnull(sum([TotalAmount]), 0) [ReturnTotal] from [dbo].[tblSupplierReturnInvoice] 
group by [SupplierInvoiceID]) [SR]
on [SR].[SupplierInvoiceID] = [SI].[SupplierInvoiceID]
full join (select [SupplierInvoiceID], isnull(sum([PaymentAmount]), 0) [ReturnPayment] from [dbo].[tblSupplierReturnPayment] group by [SupplierInvoiceID]) [PR]
on [PR].[SupplierInvoiceID] = [SI].[SupplierInvoiceID]
where [SI].[CompanyID] = @CompanyID and [SI].[BranchID] = @BranchID
and [SI].[InvoiceDate] >= @FromDate and [SI].[InvoiceDate] <= @ToDate
end;
GO
/****** Object:  StoredProcedure [dbo].[GetReturnPurchasePaymentPending]    Script Date: 01.05.2024 22:58:01 ******/
CREATE proc [dbo].[GetReturnPurchasePaymentPending](@BranchID as int, @CompanyID as int)
as begin
select [SI].[SupplierInvoiceID], [SI].[BranchID], [SI].[CompanyID], [SI].[InvoiceDate], [SI].[SupplierID],
[SI].[InvoiceNo], cast([SI].[TotalAmount] as decimal(10, 2)) [BeforeReturnTotal], isnull([SR].[ReturnTotal], 0) [ReturnTotal],
isnull((cast([SI].[TotalAmount] as decimal(10, 2)) - isnull([SR].[ReturnTotal], 0)), 0) [AfterReturnTotal], isnull([SP].[Payment], 0) [PaidAmount], isnull([PR].[ReturnPayment], 0) [ReturnPayment], 
(isnull((cast([SI].[TotalAmount] as decimal(10, 2)) - isnull([SR].[ReturnTotal], 0)), 0) - isnull([SP].[Payment], 0) - isnull([PR].[ReturnPayment], 0)) [RemainingBalance] 
from [dbo].[tblSupplierInvoice] [SI] full join (select [SupplierInvoiceID], [BranchID], [CompanyID], sum([PaymentAmount]) as [Payment]
from [dbo].[tblSupplierPayment] group by [SupplierInvoiceID], [BranchID], [CompanyID]) [SP]
on [SI].[SupplierInvoiceID] = [SP].[SupplierInvoiceID] 
full join (select [SupplierInvoiceID], isnull(sum([TotalAmount]), 0) [ReturnTotal] from [dbo].[tblSupplierReturnInvoice] 
group by [SupplierInvoiceID]) [SR]
on [SR].[SupplierInvoiceID] = [SI].[SupplierInvoiceID]
full join (select [SupplierInvoiceID], isnull(sum([PaymentAmount]), 0) [ReturnPayment] from [dbo].[tblSupplierReturnPayment] group by [SupplierInvoiceID]) [PR]
on [PR].[SupplierInvoiceID] = [SI].[SupplierInvoiceID]
where [SI].[CompanyID] = @CompanyID and [SI].[BranchID] = @BranchID
and isnull([PR].[ReturnPayment], 0) < isnull([SR].[ReturnTotal], 0)
end;
GO
/****** Object:  StoredProcedure [dbo].[GetReturnSaleAmountPending]    Script Date: 01.05.2024 22:58:01 ******/
create proc [dbo].[GetReturnSaleAmountPending](@BranchID as int, @CompanyID as int)
as begin
select [CI].[CustomerInvoiceID], [CI].[BranchID], [CI].[CompanyID], [CI].[InvoiceDate], [CI].[CustomerID],
[CI].[InvoiceNo], cast([CI].[TotalAmount] as decimal(10, 2)) [BeforeReturnTotal], isnull([CR].[ReturnTotal], 0) [ReturnTotal],
isnull((cast([CI].[TotalAmount] as decimal(10, 2)) - isnull([CR].[ReturnTotal], 0)), 0) [AfterReturnTotal], isnull([CP].[Payment], 0) [PaidAmount], isnull([PR].[ReturnPayment], 0) [ReturnPayment], 
(isnull((cast([CI].[TotalAmount] as decimal(10, 2)) - isnull([CR].[ReturnTotal], 0)), 0) - isnull([CP].[Payment], 0) - isnull([PR].[ReturnPayment], 0)) [RemainingBalance] 
from [dbo].[tblCustomerInvoice] [CI] full join (select [CustomerInvoiceID], [BranchID], [CompanyID], sum([PaidAmount]) as [Payment]
from [dbo].[tblCustomerPayment] group by [CustomerInvoiceID], [BranchID], [CompanyID]) [CP]
on [CI].[CustomerInvoiceID] = [CP].[CustomerInvoiceID] 
full join (select [CustomerInvoiceID], isnull(sum([TotalAmount]), 0) [ReturnTotal] from [dbo].[tblCustomerReturnInvoice] 
group by [CustomerInvoiceID]) [CR]
on [CR].[CustomerInvoiceID] = [CI].[CustomerInvoiceID]
full join (select [CustomerInvoiceID], isnull(sum([PaidAmount]), 0) [ReturnPayment] from [dbo].[tblCustomerReturnPayment] group by [CustomerInvoiceID]) [PR]
on [PR].[CustomerInvoiceID] = [CI].[CustomerInvoiceID]

where [CI].[CompanyID] = @CompanyID and [CI].[BranchID] = @BranchID
and isnull([PR].[ReturnPayment], 0) < isnull([CR].[ReturnTotal], 0)
end;
GO
/****** Object:  StoredProcedure [dbo].[GetSalesHistory]    Script Date: 01.05.2024 22:58:01 ******/
CREATE proc [dbo].[GetSalesHistory](@BranchID as int, @CompanyID as int, @FromDate as date, @ToDate as date)
as begin
select [CI].[CustomerInvoiceID], [CI].[BranchID], [CI].[CompanyID], [CI].[InvoiceDate], [CI].[CustomerID],
[CI].[InvoiceNo], cast([CI].[TotalAmount] as decimal(10, 2)) [BeforeReturnTotal], isnull([CR].[ReturnTotal], 0) [ReturnTotal],
isnull((cast([CI].[TotalAmount] as decimal(10, 2)) - isnull([CR].[ReturnTotal], 0)), 0) [AfterReturnTotal], isnull([CP].[Payment], 0) [PaidAmount], isnull([PR].[ReturnPayment], 0) [ReturnPayment], 
(isnull((cast([CI].[TotalAmount] as decimal(10, 2)) - isnull([CR].[ReturnTotal], 0)), 0) - isnull([CP].[Payment], 0) - isnull([PR].[ReturnPayment], 0)) [RemainingBalance] 
from [dbo].[tblCustomerInvoice] [CI] full join (select [CustomerInvoiceID], [BranchID], [CompanyID], sum([PaidAmount]) as [Payment]
from [dbo].[tblCustomerPayment] group by [CustomerInvoiceID], [BranchID], [CompanyID]) [CP]
on [CI].[CustomerInvoiceID] = [CP].[CustomerInvoiceID] 
full join (select [CustomerInvoiceID], isnull(sum([TotalAmount]), 0) [ReturnTotal] from [dbo].[tblCustomerReturnInvoice] 
group by [CustomerInvoiceID]) [CR]
on [CR].[CustomerInvoiceID] = [CI].[CustomerInvoiceID]
full join (select [CustomerInvoiceID], isnull(sum([PaidAmount]), 0) [ReturnPayment] from [dbo].[tblCustomerReturnPayment] group by [CustomerInvoiceID]) [PR]
on [PR].[CustomerInvoiceID] = [CI].[CustomerInvoiceID]

where [CI].[CompanyID] = @CompanyID and [CI].[BranchID] = @BranchID
and [CI].[InvoiceDate] >= @FromDate and [CI].[InvoiceDate] <= @ToDate

end;
GO
/****** Object:  StoredProcedure [dbo].[GetSupplierPaymentHistory]    Script Date: 01.05.2024 22:58:01 ******/
CREATE proc [dbo].[GetSupplierPaymentHistory](@SupplierInvoiceID as int)
as begin
select [SI].[SupplierInvoiceID], [SI].[BranchID], [SI].[CompanyID], [SI].[InvoiceDate], [SI].[SupplierID],
[SP].[InvoiceNo], cast([SI].[TotalAmount] as decimal(10, 2)), [SP].[PaymentAmount], [SP].[RemainingBalance], [SP].[UserID]
from [dbo].[tblSupplierInvoice] [SI] 
inner join 
(select [dbo].[tblSupplierPayment].[SupplierInvoiceID], 
[dbo].[tblSupplierPayment].[BranchID], 
[dbo].[tblSupplierPayment].[CompanyID], 
[dbo].[tblSupplierPayment].[PaymentAmount], 
[dbo].[tblSupplierPayment].[RemainingBalance],
[dbo].[tblSupplierPayment].[UserID],
[dbo].[tblSupplierPayment].[InvoiceNo]
from [dbo].[tblSupplierPayment]) [SP]
on [SI].[SupplierInvoiceID] = [SP].[SupplierInvoiceID] 
where [SI].[SupplierInvoiceID] = ISNULL(@SupplierInvoiceID, 0)
end;
GO
/****** Object:  StoredProcedure [dbo].[GetSupplierRemainingPaymentRecord]    Script Date: 01.05.2024 22:58:01 ******/
CREATE proc [dbo].[GetSupplierRemainingPaymentRecord](@BranchID as int, @CompanyID as int)
as begin
select [SI].[SupplierInvoiceID], [SI].[BranchID], [SI].[CompanyID], [SI].[InvoiceDate], [SI].[SupplierID],
[SI].[InvoiceNo], cast([SI].[TotalAmount] as decimal(10, 2)) [BeforeReturnTotal], isnull([SR].[ReturnTotal], 0) [ReturnTotal],
isnull((cast([SI].[TotalAmount] as decimal(10, 2)) - isnull([SR].[ReturnTotal], 0)), 0) [AfterReturnTotal], isnull([SP].[Payment], 0) [PaidAmount],
isnull([PR].[ReturnPayment], 0) [ReturnPayment], (isnull((cast([SI].[TotalAmount] as decimal(10, 2)) - isnull([SR].[ReturnTotal], 0)), 0) - isnull([SP].[Payment], 0) - isnull([PR].[ReturnPayment], 0)) [RemainingBalance] from [dbo].[tblSupplierInvoice] [SI] 
full join (select [SupplierInvoiceID], [BranchID], [CompanyID], sum([PaymentAmount]) as [Payment]
from [dbo].[tblSupplierPayment] group by [SupplierInvoiceID], [BranchID], [CompanyID]) [SP]
on [SI].[SupplierInvoiceID] = [SP].[SupplierInvoiceID] 
full join (select [SupplierInvoiceID], isnull(sum([TotalAmount]), 0) [ReturnTotal] from [dbo].[tblSupplierReturnInvoice] 
group by [SupplierInvoiceID]) [SR]
on [SR].[SupplierInvoiceID] = [SI].[SupplierInvoiceID]
full join (select [SupplierInvoiceID], isnull(sum([PaymentAmount]), 0) [ReturnPayment] from [dbo].[tblSupplierReturnPayment] group by [SupplierInvoiceID]) [PR]
on [PR].[SupplierInvoiceID] = [SI].[SupplierInvoiceID]
where [SI].[CompanyID] = @CompanyID and [SI].[BranchID] = @BranchID
and (isnull((cast([SI].[TotalAmount] as decimal(10, 2)) - isnull([SR].[ReturnTotal], 0)), 0) - isnull([SP].[Payment], 0) - isnull([PR].[ReturnPayment], 0)) > 0
end;
GO
/****** Object:  StoredProcedure [dbo].[GetSupplierReturnPurchasePaymentPending]    Script Date: 01.05.2024 22:58:01 ******/
CREATE proc [dbo].[GetSupplierReturnPurchasePaymentPending](@SupplierInvoiceID as int)
as begin
select [RSI].[SupplierReturnInvoiceID], [RSI].[SupplierInvoiceID], [RSI].[BranchID], [RSI].[CompanyID], [RSI].[InvoiceDate], [RSI].[SupplierID],
[RSI].[InvoiceNo], isnull(cast([RSI].[TotalAmount] as decimal(10, 2)), 0) [ReturnTotal], isnull([SP].[SPayment], 0) [ReturnPayment],
isnull(cast([RSI].[TotalAmount] as decimal(10, 2)), 0) - isnull([SP].[SPayment], 0) [ReturnRemainingPayment], [RSI].[UserID]
from [dbo].[tblSupplierReturnInvoice] [RSI]
full join 
(select [SupplierReturnInvoiceID],  
sum([PaymentAmount]) [SPayment]
from [dbo].[tblSupplierReturnPayment]
group by [SupplierReturnInvoiceID]) [SP]
on [RSI].[SupplierReturnInvoiceID] = [SP].[SupplierReturnInvoiceID] 
where [RSI].[SupplierInvoiceID] = ISNULL(@SupplierInvoiceID, 0)
and isnull(cast([RSI].[TotalAmount] as decimal(10, 2)), 0) - isnull([SP].[SPayment], 0) > 0
end;
GO
/****** Object:  StoredProcedure [dbo].[GetTotalByHeadAccount]    Script Date: 01.05.2024 22:58:01 ******/
CREATE proc [dbo].[GetTotalByHeadAccount](@HeadID as int, @FinancialYearID as int, @BranchID as int, @CompanyID as int)
as
begin
	select
	case when sum([TA].[Debit]) > sum([TA].[Credit]) then sum([TA].[Debit]) - sum([TA].[Credit])
		 when sum([TA].[Debit]) < sum([TA].[Credit]) then sum([TA].[Credit]) - sum([TA].[Debit]) else 0 end as TOTAL
	from (select [v_Transaction].[AccountTitle],
	case when sum([v_Transaction].[Debit]) > sum([v_Transaction].[Credit]) then isnull(sum([v_Transaction].[Debit]) - sum([v_Transaction].[Credit]), 0) else 0 end as [Debit],
	case when sum([v_Transaction].[Debit]) < sum([v_Transaction].[Credit]) then isnull(sum([v_Transaction].[Credit]) - sum([v_Transaction].[Debit]), 0) else 0 end as [Credit]
	from [v_Transaction] where [v_Transaction].[AccountHeadID] = isnull(@HeadID, 0) 
						 and [v_Transaction].[FinancialYearID] = isnull(@FinancialYearID, 0) 
						 and [v_Transaction].[BranchID] = @BranchID
						 and [v_Transaction].[CompanyID] = @CompanyID
	group by [v_Transaction].[AccountTitle]) [TA]
end
GO
/****** Object:  StoredProcedure [dbo].[GetTrialBalance]    Script Date: 01.05.2024 22:58:01 ******/
CREATE proc [dbo].[GetTrialBalance](@BranchID as int, @CompanyID as int, @FinancialYearID as int)
as
begin
select [JR].[FinancialYearID], [JR].[AccountSubControl], [JR].[AccountSubControlID],
case when [JR].[Debit] > [JR].[Credit] then [JR].[Debit] - [JR].[Credit] else 0 end as [Debit],
case when [JR].[Debit] < [JR].[Credit] then [JR].[Credit] - [JR].[Debit] else 0 end as [Credit],
[JR].[BranchID],
[JR].[CompanyID]
from
(select 
[TR].[FinancialYearID],
[ACTS].[AccountSubControl],
[ACTS].[AccountSubControlID],
isnull(sum([TR].[Debit]), 0) [Debit],
isnull(sum([TR].[Credit]), 0) [Credit],
[TR].[BranchID],
[TR].[CompanyID]
from [dbo].[tblTransaction] [TR]
inner join 
(select [AHC].[AccountHeadID],
[AHC].[AccountHeadName],
[AHC].[AccountControlID],
[AHC].[AccountControlName],
[AHC].[BranchID],
[AHC].[CompanyID],
[ACS].[AccountSubControlID],
[AHC].[AccountControl] + '-/- ' + [ACS].[AccountSubControlName] [AccountSubControl]
from [dbo].[tblAccountSubControl] [ACS]
inner join
(select [AH].[AccountHeadID],
[AH].[AccountHeadName],
[AC].[AccountControlID],
[AC].[AccountControlName],
[AH].[AccountHeadName] + '-/-' + [AC].[AccountControlName] [AccountControl],
[AC].[BranchID],
[AC].[CompanyID]
from [dbo].[tblAccountHead] [AH]
inner join [dbo].[tblAccountControl] [AC]
on [AH].[AccountHeadID] = [AC].[AccountHeadID]) [AHC]
on [ACS].[AccountControlID] = [AHC].[AccountControlID]) [ACTS]
on [TR].[AccountSubControlID] = [ACTS].[AccountSubControlID]
group by [TR].[FinancialYearID],
[ACTS].[AccountSubControl],
[ACTS].[AccountSubControlID],
[TR].[BranchID],
[TR].[CompanyID]) [JR]
where [JR].[BranchID] = @BranchID and [JR].[CompanyID] = @CompanyID
and [JR].[FinancialYearID] = @FinancialYearID
end
