
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/30/2024 22:07:16
-- Generated from EDMX file: D:\FINAL_YEAR_PROJ\Application\CloudERP\DatabaseAccess\CloudDB.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [CloudErpV1];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_tblAccountControl_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblAccountControl] DROP CONSTRAINT [FK_tblAccountControl_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblAccountControl_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblAccountControl] DROP CONSTRAINT [FK_tblAccountControl_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblAccountControl_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblAccountControl] DROP CONSTRAINT [FK_tblAccountControl_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblAccountHead_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblAccountHead] DROP CONSTRAINT [FK_tblAccountHead_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblAccountSetting_tblAccountActivity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblAccountSetting] DROP CONSTRAINT [FK_tblAccountSetting_tblAccountActivity];
GO
IF OBJECT_ID(N'[dbo].[FK_tblAccountSetting_tblAccountControl]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblAccountSetting] DROP CONSTRAINT [FK_tblAccountSetting_tblAccountControl];
GO
IF OBJECT_ID(N'[dbo].[FK_tblAccountSetting_tblAccountHead]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblAccountSetting] DROP CONSTRAINT [FK_tblAccountSetting_tblAccountHead];
GO
IF OBJECT_ID(N'[dbo].[FK_tblAccountSetting_tblAccountSubControl]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblAccountSetting] DROP CONSTRAINT [FK_tblAccountSetting_tblAccountSubControl];
GO
IF OBJECT_ID(N'[dbo].[FK_tblAccountSetting_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblAccountSetting] DROP CONSTRAINT [FK_tblAccountSetting_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblAccountSetting_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblAccountSetting] DROP CONSTRAINT [FK_tblAccountSetting_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblAccountSubControl_tblAccountControl]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblAccountSubControl] DROP CONSTRAINT [FK_tblAccountSubControl_tblAccountControl];
GO
IF OBJECT_ID(N'[dbo].[FK_tblAccountSubControl_tblAccountHead]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblAccountSubControl] DROP CONSTRAINT [FK_tblAccountSubControl_tblAccountHead];
GO
IF OBJECT_ID(N'[dbo].[FK_tblAccountSubControl_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblAccountSubControl] DROP CONSTRAINT [FK_tblAccountSubControl_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblAccountSubControl_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblAccountSubControl] DROP CONSTRAINT [FK_tblAccountSubControl_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblBranch_tblBranchType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblBranch] DROP CONSTRAINT [FK_tblBranch_tblBranchType];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCategory_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCategory] DROP CONSTRAINT [FK_tblCategory_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCategory_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCategory] DROP CONSTRAINT [FK_tblCategory_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCategory_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCategory] DROP CONSTRAINT [FK_tblCategory_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomer_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomer] DROP CONSTRAINT [FK_tblCustomer_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomer_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomer] DROP CONSTRAINT [FK_tblCustomer_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomer_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomer] DROP CONSTRAINT [FK_tblCustomer_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerInvoice_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerInvoice] DROP CONSTRAINT [FK_tblCustomerInvoice_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerInvoice_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerInvoice] DROP CONSTRAINT [FK_tblCustomerInvoice_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerInvoice_tblCustomer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerInvoice] DROP CONSTRAINT [FK_tblCustomerInvoice_tblCustomer];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerInvoice_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerInvoice] DROP CONSTRAINT [FK_tblCustomerInvoice_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerInvoiceDetail_tblCustomerInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerInvoiceDetail] DROP CONSTRAINT [FK_tblCustomerInvoiceDetail_tblCustomerInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerInvoiceDetail_tblStock]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerInvoiceDetail] DROP CONSTRAINT [FK_tblCustomerInvoiceDetail_tblStock];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerPayment_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerPayment] DROP CONSTRAINT [FK_tblCustomerPayment_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerPayment_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerPayment] DROP CONSTRAINT [FK_tblCustomerPayment_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerPayment_tblCustomerInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerPayment] DROP CONSTRAINT [FK_tblCustomerPayment_tblCustomerInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerPayment_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerPayment] DROP CONSTRAINT [FK_tblCustomerPayment_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerReturnInvoice_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerReturnInvoice] DROP CONSTRAINT [FK_tblCustomerReturnInvoice_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerReturnInvoice_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerReturnInvoice] DROP CONSTRAINT [FK_tblCustomerReturnInvoice_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerReturnInvoice_tblCustomer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerReturnInvoice] DROP CONSTRAINT [FK_tblCustomerReturnInvoice_tblCustomer];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerReturnInvoice_tblCustomerInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerReturnInvoice] DROP CONSTRAINT [FK_tblCustomerReturnInvoice_tblCustomerInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerReturnInvoice_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerReturnInvoice] DROP CONSTRAINT [FK_tblCustomerReturnInvoice_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerReturnInvoiceDetail_tblCustomerInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerReturnInvoiceDetail] DROP CONSTRAINT [FK_tblCustomerReturnInvoiceDetail_tblCustomerInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerReturnInvoiceDetail_tblCustomerInvoiceDetail]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerReturnInvoiceDetail] DROP CONSTRAINT [FK_tblCustomerReturnInvoiceDetail_tblCustomerInvoiceDetail];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerReturnInvoiceDetail_tblCustomerReturnInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerReturnInvoiceDetail] DROP CONSTRAINT [FK_tblCustomerReturnInvoiceDetail_tblCustomerReturnInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerReturnInvoiceDetail_tblStock]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerReturnInvoiceDetail] DROP CONSTRAINT [FK_tblCustomerReturnInvoiceDetail_tblStock];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerReturnPayment_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerReturnPayment] DROP CONSTRAINT [FK_tblCustomerReturnPayment_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerReturnPayment_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerReturnPayment] DROP CONSTRAINT [FK_tblCustomerReturnPayment_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerReturnPayment_tblCustomer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerReturnPayment] DROP CONSTRAINT [FK_tblCustomerReturnPayment_tblCustomer];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerReturnPayment_tblCustomerInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerReturnPayment] DROP CONSTRAINT [FK_tblCustomerReturnPayment_tblCustomerInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerReturnPayment_tblCustomerReturnInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerReturnPayment] DROP CONSTRAINT [FK_tblCustomerReturnPayment_tblCustomerReturnInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_tblCustomerReturnPayment_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblCustomerReturnPayment] DROP CONSTRAINT [FK_tblCustomerReturnPayment_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblEmployee_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblEmployee] DROP CONSTRAINT [FK_tblEmployee_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblEmployee_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblEmployee] DROP CONSTRAINT [FK_tblEmployee_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblFinancialYear_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblFinancialYear] DROP CONSTRAINT [FK_tblFinancialYear_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblPayroll_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblPayroll] DROP CONSTRAINT [FK_tblPayroll_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblPayroll_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblPayroll] DROP CONSTRAINT [FK_tblPayroll_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblPayroll_tblEmployee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblPayroll] DROP CONSTRAINT [FK_tblPayroll_tblEmployee];
GO
IF OBJECT_ID(N'[dbo].[FK_tblPayroll_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblPayroll] DROP CONSTRAINT [FK_tblPayroll_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblPurchaseCart_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblPurchaseCart] DROP CONSTRAINT [FK_tblPurchaseCart_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblPurchaseCartDetail_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblPurchaseCartDetail] DROP CONSTRAINT [FK_tblPurchaseCartDetail_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblPurchaseCartDetail_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblPurchaseCartDetail] DROP CONSTRAINT [FK_tblPurchaseCartDetail_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblPurchaseCartDetail_tblStock]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblPurchaseCartDetail] DROP CONSTRAINT [FK_tblPurchaseCartDetail_tblStock];
GO
IF OBJECT_ID(N'[dbo].[FK_tblPurchaseCartDetail_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblPurchaseCartDetail] DROP CONSTRAINT [FK_tblPurchaseCartDetail_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblPurchaseCartTable_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblPurchaseCart] DROP CONSTRAINT [FK_tblPurchaseCartTable_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblPurchaseCartTable_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblPurchaseCart] DROP CONSTRAINT [FK_tblPurchaseCartTable_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblPurchaseCartTable_tblSupplier]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblPurchaseCart] DROP CONSTRAINT [FK_tblPurchaseCartTable_tblSupplier];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSaleCartDetail_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSaleCartDetail] DROP CONSTRAINT [FK_tblSaleCartDetail_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSaleCartDetail_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSaleCartDetail] DROP CONSTRAINT [FK_tblSaleCartDetail_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSaleCartDetail_tblStock]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSaleCartDetail] DROP CONSTRAINT [FK_tblSaleCartDetail_tblStock];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSaleCartDetail_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSaleCartDetail] DROP CONSTRAINT [FK_tblSaleCartDetail_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblStock_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblStock] DROP CONSTRAINT [FK_tblStock_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblStock_tblCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblStock] DROP CONSTRAINT [FK_tblStock_tblCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_tblStock_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblStock] DROP CONSTRAINT [FK_tblStock_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblStock_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblStock] DROP CONSTRAINT [FK_tblStock_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplier_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplier] DROP CONSTRAINT [FK_tblSupplier_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplier_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplier] DROP CONSTRAINT [FK_tblSupplier_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplier_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplier] DROP CONSTRAINT [FK_tblSupplier_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierInvoice_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierInvoice] DROP CONSTRAINT [FK_tblSupplierInvoice_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierInvoiceDetail_tblStock]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierInvoiceDetail] DROP CONSTRAINT [FK_tblSupplierInvoiceDetail_tblStock];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierInvoiceDetail_tblSupplierInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierInvoiceDetail] DROP CONSTRAINT [FK_tblSupplierInvoiceDetail_tblSupplierInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierInvoiceTable_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierInvoice] DROP CONSTRAINT [FK_tblSupplierInvoiceTable_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierInvoiceTable_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierInvoice] DROP CONSTRAINT [FK_tblSupplierInvoiceTable_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierInvoiceTable_tblSupplier]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierInvoice] DROP CONSTRAINT [FK_tblSupplierInvoiceTable_tblSupplier];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierPayment_tblSupplier]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierPayment] DROP CONSTRAINT [FK_tblSupplierPayment_tblSupplier];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierPayment_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierPayment] DROP CONSTRAINT [FK_tblSupplierPayment_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierReturnInvoice_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierReturnInvoice] DROP CONSTRAINT [FK_tblSupplierReturnInvoice_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierReturnInvoice_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierReturnInvoice] DROP CONSTRAINT [FK_tblSupplierReturnInvoice_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierReturnInvoice_tblSupplier]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierReturnInvoice] DROP CONSTRAINT [FK_tblSupplierReturnInvoice_tblSupplier];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierReturnInvoice_tblSupplierInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierReturnInvoice] DROP CONSTRAINT [FK_tblSupplierReturnInvoice_tblSupplierInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierReturnInvoice_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierReturnInvoice] DROP CONSTRAINT [FK_tblSupplierReturnInvoice_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierReturnInvoiceDetail_tblStock]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierReturnInvoiceDetail] DROP CONSTRAINT [FK_tblSupplierReturnInvoiceDetail_tblStock];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierReturnInvoiceDetail_tblSupplierInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierReturnInvoiceDetail] DROP CONSTRAINT [FK_tblSupplierReturnInvoiceDetail_tblSupplierInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierReturnInvoiceDetail_tblSupplierInvoiceDetail]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierReturnInvoiceDetail] DROP CONSTRAINT [FK_tblSupplierReturnInvoiceDetail_tblSupplierInvoiceDetail];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierReturnInvoiceDetail_tblSupplierReturnInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierReturnInvoiceDetail] DROP CONSTRAINT [FK_tblSupplierReturnInvoiceDetail_tblSupplierReturnInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierReturnPayment_tblBranch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierReturnPayment] DROP CONSTRAINT [FK_tblSupplierReturnPayment_tblBranch];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierReturnPayment_tblCompany]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierReturnPayment] DROP CONSTRAINT [FK_tblSupplierReturnPayment_tblCompany];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierReturnPayment_tblSupplier]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierReturnPayment] DROP CONSTRAINT [FK_tblSupplierReturnPayment_tblSupplier];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierReturnPayment_tblSupplierInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierReturnPayment] DROP CONSTRAINT [FK_tblSupplierReturnPayment_tblSupplierInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierReturnPayment_tblSupplierReturnInvoice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierReturnPayment] DROP CONSTRAINT [FK_tblSupplierReturnPayment_tblSupplierReturnInvoice];
GO
IF OBJECT_ID(N'[dbo].[FK_tblSupplierReturnPayment_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblSupplierReturnPayment] DROP CONSTRAINT [FK_tblSupplierReturnPayment_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblTransaction_tblAccountControl]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblTransaction] DROP CONSTRAINT [FK_tblTransaction_tblAccountControl];
GO
IF OBJECT_ID(N'[dbo].[FK_tblTransaction_tblAccountHead]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblTransaction] DROP CONSTRAINT [FK_tblTransaction_tblAccountHead];
GO
IF OBJECT_ID(N'[dbo].[FK_tblTransaction_tblFinancialYear]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblTransaction] DROP CONSTRAINT [FK_tblTransaction_tblFinancialYear];
GO
IF OBJECT_ID(N'[dbo].[FK_tblTransaction_tblUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblTransaction] DROP CONSTRAINT [FK_tblTransaction_tblUser];
GO
IF OBJECT_ID(N'[dbo].[FK_tblTransection_tblAccountHead]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblTransaction] DROP CONSTRAINT [FK_tblTransection_tblAccountHead];
GO
IF OBJECT_ID(N'[dbo].[FK_tblTransection_tblAccountSubControl]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblTransaction] DROP CONSTRAINT [FK_tblTransection_tblAccountSubControl];
GO
IF OBJECT_ID(N'[dbo].[FK_tblTransection_tblFinancialYear]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblTransaction] DROP CONSTRAINT [FK_tblTransection_tblFinancialYear];
GO
IF OBJECT_ID(N'[dbo].[FK_tblUser_tblUserType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tblUser] DROP CONSTRAINT [FK_tblUser_tblUserType];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[tblAccountActivity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblAccountActivity];
GO
IF OBJECT_ID(N'[dbo].[tblAccountControl]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblAccountControl];
GO
IF OBJECT_ID(N'[dbo].[tblAccountHead]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblAccountHead];
GO
IF OBJECT_ID(N'[dbo].[tblAccountSetting]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblAccountSetting];
GO
IF OBJECT_ID(N'[dbo].[tblAccountSubControl]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblAccountSubControl];
GO
IF OBJECT_ID(N'[dbo].[tblBranch]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblBranch];
GO
IF OBJECT_ID(N'[dbo].[tblBranchType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblBranchType];
GO
IF OBJECT_ID(N'[dbo].[tblCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblCategory];
GO
IF OBJECT_ID(N'[dbo].[tblCompany]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblCompany];
GO
IF OBJECT_ID(N'[dbo].[tblCustomer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblCustomer];
GO
IF OBJECT_ID(N'[dbo].[tblCustomerInvoice]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblCustomerInvoice];
GO
IF OBJECT_ID(N'[dbo].[tblCustomerInvoiceDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblCustomerInvoiceDetail];
GO
IF OBJECT_ID(N'[dbo].[tblCustomerPayment]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblCustomerPayment];
GO
IF OBJECT_ID(N'[dbo].[tblCustomerReturnInvoice]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblCustomerReturnInvoice];
GO
IF OBJECT_ID(N'[dbo].[tblCustomerReturnInvoiceDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblCustomerReturnInvoiceDetail];
GO
IF OBJECT_ID(N'[dbo].[tblCustomerReturnPayment]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblCustomerReturnPayment];
GO
IF OBJECT_ID(N'[dbo].[tblEmployee]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblEmployee];
GO
IF OBJECT_ID(N'[dbo].[tblFinancialYear]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblFinancialYear];
GO
IF OBJECT_ID(N'[dbo].[tblPayroll]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblPayroll];
GO
IF OBJECT_ID(N'[dbo].[tblPurchaseCart]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblPurchaseCart];
GO
IF OBJECT_ID(N'[dbo].[tblPurchaseCartDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblPurchaseCartDetail];
GO
IF OBJECT_ID(N'[dbo].[tblSaleCartDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblSaleCartDetail];
GO
IF OBJECT_ID(N'[dbo].[tblStock]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblStock];
GO
IF OBJECT_ID(N'[dbo].[tblSupplier]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblSupplier];
GO
IF OBJECT_ID(N'[dbo].[tblSupplierInvoice]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblSupplierInvoice];
GO
IF OBJECT_ID(N'[dbo].[tblSupplierInvoiceDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblSupplierInvoiceDetail];
GO
IF OBJECT_ID(N'[dbo].[tblSupplierPayment]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblSupplierPayment];
GO
IF OBJECT_ID(N'[dbo].[tblSupplierReturnInvoice]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblSupplierReturnInvoice];
GO
IF OBJECT_ID(N'[dbo].[tblSupplierReturnInvoiceDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblSupplierReturnInvoiceDetail];
GO
IF OBJECT_ID(N'[dbo].[tblSupplierReturnPayment]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblSupplierReturnPayment];
GO
IF OBJECT_ID(N'[dbo].[tblTransaction]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblTransaction];
GO
IF OBJECT_ID(N'[dbo].[tblUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblUser];
GO
IF OBJECT_ID(N'[dbo].[tblUserType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tblUserType];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'tblAccountActivity'
CREATE TABLE [dbo].[tblAccountActivity] (
    [AccountActivityID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100)  NOT NULL
);
GO

-- Creating table 'tblAccountControl'
CREATE TABLE [dbo].[tblAccountControl] (
    [AccountControlID] int IDENTITY(1,1) NOT NULL,
    [CompanyID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [AccountHeadID] int  NOT NULL,
    [AccountControlName] varchar(50)  NOT NULL,
    [UserID] int  NOT NULL
);
GO

-- Creating table 'tblAccountHead'
CREATE TABLE [dbo].[tblAccountHead] (
    [AccountHeadID] int IDENTITY(1,1) NOT NULL,
    [AccountHeadName] varchar(50)  NOT NULL,
    [Code] int  NOT NULL,
    [UserID] int  NOT NULL
);
GO

-- Creating table 'tblAccountSetting'
CREATE TABLE [dbo].[tblAccountSetting] (
    [AccountSettingID] int IDENTITY(1,1) NOT NULL,
    [AccountHeadID] int  NOT NULL,
    [AccountControlID] int  NOT NULL,
    [AccountSubControlID] int  NOT NULL,
    [AccountActivityID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [BranchID] int  NOT NULL
);
GO

-- Creating table 'tblAccountSubControl'
CREATE TABLE [dbo].[tblAccountSubControl] (
    [AccountSubControlID] int IDENTITY(1,1) NOT NULL,
    [AccountHeadID] int  NOT NULL,
    [AccountControlID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [AccountSubControlName] varchar(50)  NOT NULL,
    [UserID] int  NOT NULL
);
GO

-- Creating table 'tblBranch'
CREATE TABLE [dbo].[tblBranch] (
    [BranchID] int IDENTITY(1,1) NOT NULL,
    [BranchTypeID] int  NOT NULL,
    [BranchName] varchar(50)  NOT NULL,
    [BranchContact] nvarchar(50)  NOT NULL,
    [BranchAddress] varchar(300)  NOT NULL,
    [CompanyID] int  NOT NULL,
    [BrchID] int  NULL
);
GO

-- Creating table 'tblBranchType'
CREATE TABLE [dbo].[tblBranchType] (
    [BranchTypeID] int IDENTITY(1,1) NOT NULL,
    [BranchType] varchar(50)  NOT NULL
);
GO

-- Creating table 'tblCategory'
CREATE TABLE [dbo].[tblCategory] (
    [CategoryID] int IDENTITY(1,1) NOT NULL,
    [CategoryName] varchar(50)  NOT NULL,
    [BranchID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [UserID] int  NOT NULL
);
GO

-- Creating table 'tblCompany'
CREATE TABLE [dbo].[tblCompany] (
    [CompanyID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(200)  NOT NULL,
    [Logo] nvarchar(200)  NOT NULL,
    [Description] nvarchar(500)  NULL
);
GO

-- Creating table 'tblCustomer'
CREATE TABLE [dbo].[tblCustomer] (
    [CustomerID] int IDENTITY(1,1) NOT NULL,
    [Customername] varchar(150)  NOT NULL,
    [CustomerContact] nvarchar(150)  NOT NULL,
    [CustomerArea] varchar(50)  NOT NULL,
    [CustomerAddress] varchar(300)  NOT NULL,
    [Description] varchar(300)  NOT NULL,
    [BranchID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [UserID] int  NOT NULL
);
GO

-- Creating table 'tblCustomerInvoice'
CREATE TABLE [dbo].[tblCustomerInvoice] (
    [CustomerInvoiceID] int IDENTITY(1,1) NOT NULL,
    [CustomerID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [InvoiceNo] nvarchar(150)  NOT NULL,
    [Title] nvarchar(150)  NOT NULL,
    [TotalAmount] float  NOT NULL,
    [InvoiceDate] datetime  NOT NULL,
    [Description] varchar(500)  NULL,
    [UserID] int  NOT NULL
);
GO

-- Creating table 'tblCustomerInvoiceDetail'
CREATE TABLE [dbo].[tblCustomerInvoiceDetail] (
    [CustomerInvoiceDetailID] int IDENTITY(1,1) NOT NULL,
    [CustomerInvoiceID] int  NOT NULL,
    [ProductID] int  NOT NULL,
    [SaleQuantity] int  NOT NULL,
    [SaleUnitPrice] float  NOT NULL
);
GO

-- Creating table 'tblCustomerPayment'
CREATE TABLE [dbo].[tblCustomerPayment] (
    [CustomerPaymentID] int IDENTITY(1,1) NOT NULL,
    [CustomerID] int  NOT NULL,
    [CustomerInvoiceID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [InvoiceNo] nvarchar(150)  NOT NULL,
    [TotalAmount] float  NOT NULL,
    [PaidAmount] float  NOT NULL,
    [RemainingBalance] float  NOT NULL,
    [UserID] int  NOT NULL,
    [InvoiceDate] datetime  NULL
);
GO

-- Creating table 'tblCustomerReturnInvoice'
CREATE TABLE [dbo].[tblCustomerReturnInvoice] (
    [CustomerReturnInvoiceID] int IDENTITY(1,1) NOT NULL,
    [CustomerInvoiceID] int  NOT NULL,
    [CustomerID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [InvoiceNo] nvarchar(150)  NOT NULL,
    [TotalAmount] float  NOT NULL,
    [InvoiceDate] datetime  NOT NULL,
    [Description] nvarchar(500)  NOT NULL,
    [UserID] int  NOT NULL
);
GO

-- Creating table 'tblCustomerReturnInvoiceDetail'
CREATE TABLE [dbo].[tblCustomerReturnInvoiceDetail] (
    [CustomerReturnInvoiceDetailID] int IDENTITY(1,1) NOT NULL,
    [CustomerInvoiceDetailID] int  NOT NULL,
    [CustomerInvoiceID] int  NOT NULL,
    [CustomerReturnInvoiceID] int  NOT NULL,
    [ProductID] int  NOT NULL,
    [SaleReturnQuantity] int  NOT NULL,
    [SaleReturnUnitPrice] float  NOT NULL
);
GO

-- Creating table 'tblCustomerReturnPayment'
CREATE TABLE [dbo].[tblCustomerReturnPayment] (
    [CustomerReturnPaymentID] int IDENTITY(1,1) NOT NULL,
    [CustomerReturnInvoiceID] int  NOT NULL,
    [CustomerID] int  NOT NULL,
    [CustomerInvoiceID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [InvoiceNo] nvarchar(150)  NOT NULL,
    [TotalAmount] float  NOT NULL,
    [PaidAmount] float  NOT NULL,
    [RemainingBalance] float  NOT NULL,
    [UserID] int  NOT NULL,
    [InvoiceDate] datetime  NULL
);
GO

-- Creating table 'tblEmployee'
CREATE TABLE [dbo].[tblEmployee] (
    [EmployeeID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(150)  NOT NULL,
    [ContactNo] nvarchar(50)  NOT NULL,
    [Photo] nvarchar(150)  NULL,
    [Email] nvarchar(150)  NOT NULL,
    [Address] varchar(300)  NOT NULL,
    [TIN] nvarchar(50)  NOT NULL,
    [Designation] nvarchar(150)  NOT NULL,
    [Description] nvarchar(300)  NOT NULL,
    [MonthlySalary] float  NOT NULL,
    [BranchID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [UserID] int  NULL
);
GO

-- Creating table 'tblFinancialYear'
CREATE TABLE [dbo].[tblFinancialYear] (
    [FinancialYearID] int IDENTITY(1,1) NOT NULL,
    [UserID] int  NOT NULL,
    [FinancialYear] nvarchar(150)  NOT NULL,
    [StartDate] datetime  NOT NULL,
    [EndDate] datetime  NOT NULL,
    [IsActive] bit  NOT NULL
);
GO

-- Creating table 'tblPayroll'
CREATE TABLE [dbo].[tblPayroll] (
    [PayrollID] int IDENTITY(1,1) NOT NULL,
    [EmployeeID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [TransferAmount] float  NOT NULL,
    [PayrollInvoiceNo] nvarchar(150)  NOT NULL,
    [PaymentDate] datetime  NOT NULL,
    [SalaryMonth] nvarchar(50)  NOT NULL,
    [SalaryYear] nvarchar(50)  NOT NULL,
    [UserID] int  NOT NULL
);
GO

-- Creating table 'tblPurchaseCart'
CREATE TABLE [dbo].[tblPurchaseCart] (
    [PurchaseCartID] int IDENTITY(1,1) NOT NULL,
    [SupplierID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [InvoiceNo] int  NOT NULL,
    [TotalAmount] float  NOT NULL,
    [InvoiceDate] datetime  NOT NULL,
    [Description] nvarchar(150)  NOT NULL,
    [UserID] int  NOT NULL
);
GO

-- Creating table 'tblPurchaseCartDetail'
CREATE TABLE [dbo].[tblPurchaseCartDetail] (
    [PurchaseCartDetailID] int IDENTITY(1,1) NOT NULL,
    [ProductID] int  NOT NULL,
    [PurchaseQuantity] int  NOT NULL,
    [PurchaseUnitPrice] float  NOT NULL,
    [CompanyID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [UserID] int  NOT NULL
);
GO

-- Creating table 'tblSaleCartDetail'
CREATE TABLE [dbo].[tblSaleCartDetail] (
    [SaleCartDetailID] int IDENTITY(1,1) NOT NULL,
    [ProductID] int  NOT NULL,
    [SaleQuantity] int  NOT NULL,
    [SaleUnitPrice] float  NOT NULL,
    [CompanyID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [UserID] int  NOT NULL
);
GO

-- Creating table 'tblStock'
CREATE TABLE [dbo].[tblStock] (
    [ProductID] int IDENTITY(1,1) NOT NULL,
    [CategoryID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [ProductName] nvarchar(80)  NOT NULL,
    [Quantity] int  NOT NULL,
    [SaleUnitPrice] float  NOT NULL,
    [CurrentPurchaseUnitPrice] float  NOT NULL,
    [ExpiryDate] datetime  NOT NULL,
    [Manufacture] datetime  NOT NULL,
    [StockTreshHoldQuantity] int  NOT NULL,
    [Description] nvarchar(300)  NULL,
    [UserID] int  NOT NULL,
    [IsActive] bit  NULL
);
GO

-- Creating table 'tblSupplier'
CREATE TABLE [dbo].[tblSupplier] (
    [SupplierID] int IDENTITY(1,1) NOT NULL,
    [SupplierName] nvarchar(150)  NOT NULL,
    [SupplierConatctNo] nvarchar(20)  NOT NULL,
    [SupplierAddress] nvarchar(150)  NULL,
    [SupplierEmail] nvarchar(150)  NULL,
    [Discription] nvarchar(300)  NULL,
    [BranchID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [UserID] int  NOT NULL
);
GO

-- Creating table 'tblSupplierInvoice'
CREATE TABLE [dbo].[tblSupplierInvoice] (
    [SupplierInvoiceID] int IDENTITY(1,1) NOT NULL,
    [SupplierID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [InvoiceNo] nvarchar(150)  NOT NULL,
    [TotalAmount] float  NOT NULL,
    [InvoiceDate] datetime  NOT NULL,
    [Description] nvarchar(150)  NOT NULL,
    [UserID] int  NOT NULL
);
GO

-- Creating table 'tblSupplierInvoiceDetail'
CREATE TABLE [dbo].[tblSupplierInvoiceDetail] (
    [SupplierInvoiceDetailID] int IDENTITY(1,1) NOT NULL,
    [SupplierInvoiceID] int  NOT NULL,
    [ProductID] int  NOT NULL,
    [PurchaseQuantity] int  NOT NULL,
    [PurchaseUnitPrice] float  NOT NULL
);
GO

-- Creating table 'tblSupplierPayment'
CREATE TABLE [dbo].[tblSupplierPayment] (
    [SupplierPaymentID] int IDENTITY(1,1) NOT NULL,
    [SupplierID] int  NOT NULL,
    [SupplierInvoiceID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [InvoiceNo] nvarchar(150)  NOT NULL,
    [TotalAmount] float  NOT NULL,
    [PaymentAmount] float  NOT NULL,
    [RemainingBalance] float  NOT NULL,
    [UserID] int  NOT NULL,
    [InvoiceDate] datetime  NULL
);
GO

-- Creating table 'tblSupplierReturnInvoice'
CREATE TABLE [dbo].[tblSupplierReturnInvoice] (
    [SupplierReturnInvoiceID] int IDENTITY(1,1) NOT NULL,
    [SupplierInvoiceID] int  NOT NULL,
    [SupplierID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [InvoiceNo] nvarchar(100)  NOT NULL,
    [TotalAmount] float  NOT NULL,
    [InvoiceDate] datetime  NOT NULL,
    [Description] nvarchar(500)  NULL,
    [UserID] int  NOT NULL
);
GO

-- Creating table 'tblSupplierReturnInvoiceDetail'
CREATE TABLE [dbo].[tblSupplierReturnInvoiceDetail] (
    [SupplierReturnInvoiceDetailID] int IDENTITY(1,1) NOT NULL,
    [SupplierInvoiceID] int  NOT NULL,
    [SupplierInvoiceDetailID] int  NOT NULL,
    [SupplierReturnInvoiceID] int  NOT NULL,
    [ProductID] int  NOT NULL,
    [PurchaseReturnQuantity] int  NOT NULL,
    [PurchaseReturnUnitPrice] float  NOT NULL
);
GO

-- Creating table 'tblSupplierReturnPayment'
CREATE TABLE [dbo].[tblSupplierReturnPayment] (
    [SupplierReturnPaymentID] int IDENTITY(1,1) NOT NULL,
    [SupplierReturnInvoiceID] int  NOT NULL,
    [SupplierInvoiceID] int  NOT NULL,
    [SupplierID] int  NOT NULL,
    [CompanyID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [InvoiceNo] nvarchar(150)  NOT NULL,
    [TotalAmount] float  NOT NULL,
    [PaymentAmount] float  NOT NULL,
    [RemainingBalance] float  NOT NULL,
    [UserID] int  NOT NULL,
    [InvoiceDate] datetime  NULL
);
GO

-- Creating table 'tblTransaction'
CREATE TABLE [dbo].[tblTransaction] (
    [TransactionID] int IDENTITY(1,1) NOT NULL,
    [FinancialYearID] int  NOT NULL,
    [AccountHeadID] int  NOT NULL,
    [AccountControlID] int  NOT NULL,
    [AccountSubControlID] int  NOT NULL,
    [InvoiceNo] nvarchar(150)  NOT NULL,
    [CompanyID] int  NOT NULL,
    [BranchID] int  NOT NULL,
    [Credit] float  NOT NULL,
    [Debit] float  NOT NULL,
    [TransectionDate] datetime  NOT NULL,
    [TransectionTitle] nvarchar(150)  NOT NULL,
    [UserID] int  NOT NULL
);
GO

-- Creating table 'tblUser'
CREATE TABLE [dbo].[tblUser] (
    [UserID] int IDENTITY(1,1) NOT NULL,
    [UserTypeID] int  NOT NULL,
    [FullName] nvarchar(150)  NOT NULL,
    [Email] nvarchar(150)  NOT NULL,
    [ContactNo] nvarchar(20)  NOT NULL,
    [UserName] nvarchar(150)  NOT NULL,
    [Password] nvarchar(150)  NOT NULL,
    [ResetPasswordCode] nvarchar(150)  NULL,
    [LastPasswordResetRequest] datetime  NULL,
    [ResetPasswordExpiration] datetime  NULL,
    [IsActive] bit  NOT NULL,
    [Salt] nvarchar(200)  NULL
);
GO

-- Creating table 'tblUserType'
CREATE TABLE [dbo].[tblUserType] (
    [UserTypeID] int IDENTITY(1,1) NOT NULL,
    [UserType] nvarchar(150)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [AccountActivityID] in table 'tblAccountActivity'
ALTER TABLE [dbo].[tblAccountActivity]
ADD CONSTRAINT [PK_tblAccountActivity]
    PRIMARY KEY CLUSTERED ([AccountActivityID] ASC);
GO

-- Creating primary key on [AccountControlID] in table 'tblAccountControl'
ALTER TABLE [dbo].[tblAccountControl]
ADD CONSTRAINT [PK_tblAccountControl]
    PRIMARY KEY CLUSTERED ([AccountControlID] ASC);
GO

-- Creating primary key on [AccountHeadID] in table 'tblAccountHead'
ALTER TABLE [dbo].[tblAccountHead]
ADD CONSTRAINT [PK_tblAccountHead]
    PRIMARY KEY CLUSTERED ([AccountHeadID] ASC);
GO

-- Creating primary key on [AccountSettingID] in table 'tblAccountSetting'
ALTER TABLE [dbo].[tblAccountSetting]
ADD CONSTRAINT [PK_tblAccountSetting]
    PRIMARY KEY CLUSTERED ([AccountSettingID] ASC);
GO

-- Creating primary key on [AccountSubControlID] in table 'tblAccountSubControl'
ALTER TABLE [dbo].[tblAccountSubControl]
ADD CONSTRAINT [PK_tblAccountSubControl]
    PRIMARY KEY CLUSTERED ([AccountSubControlID] ASC);
GO

-- Creating primary key on [BranchID] in table 'tblBranch'
ALTER TABLE [dbo].[tblBranch]
ADD CONSTRAINT [PK_tblBranch]
    PRIMARY KEY CLUSTERED ([BranchID] ASC);
GO

-- Creating primary key on [BranchTypeID] in table 'tblBranchType'
ALTER TABLE [dbo].[tblBranchType]
ADD CONSTRAINT [PK_tblBranchType]
    PRIMARY KEY CLUSTERED ([BranchTypeID] ASC);
GO

-- Creating primary key on [CategoryID] in table 'tblCategory'
ALTER TABLE [dbo].[tblCategory]
ADD CONSTRAINT [PK_tblCategory]
    PRIMARY KEY CLUSTERED ([CategoryID] ASC);
GO

-- Creating primary key on [CompanyID] in table 'tblCompany'
ALTER TABLE [dbo].[tblCompany]
ADD CONSTRAINT [PK_tblCompany]
    PRIMARY KEY CLUSTERED ([CompanyID] ASC);
GO

-- Creating primary key on [CustomerID] in table 'tblCustomer'
ALTER TABLE [dbo].[tblCustomer]
ADD CONSTRAINT [PK_tblCustomer]
    PRIMARY KEY CLUSTERED ([CustomerID] ASC);
GO

-- Creating primary key on [CustomerInvoiceID] in table 'tblCustomerInvoice'
ALTER TABLE [dbo].[tblCustomerInvoice]
ADD CONSTRAINT [PK_tblCustomerInvoice]
    PRIMARY KEY CLUSTERED ([CustomerInvoiceID] ASC);
GO

-- Creating primary key on [CustomerInvoiceDetailID] in table 'tblCustomerInvoiceDetail'
ALTER TABLE [dbo].[tblCustomerInvoiceDetail]
ADD CONSTRAINT [PK_tblCustomerInvoiceDetail]
    PRIMARY KEY CLUSTERED ([CustomerInvoiceDetailID] ASC);
GO

-- Creating primary key on [CustomerPaymentID] in table 'tblCustomerPayment'
ALTER TABLE [dbo].[tblCustomerPayment]
ADD CONSTRAINT [PK_tblCustomerPayment]
    PRIMARY KEY CLUSTERED ([CustomerPaymentID] ASC);
GO

-- Creating primary key on [CustomerReturnInvoiceID] in table 'tblCustomerReturnInvoice'
ALTER TABLE [dbo].[tblCustomerReturnInvoice]
ADD CONSTRAINT [PK_tblCustomerReturnInvoice]
    PRIMARY KEY CLUSTERED ([CustomerReturnInvoiceID] ASC);
GO

-- Creating primary key on [CustomerReturnInvoiceDetailID] in table 'tblCustomerReturnInvoiceDetail'
ALTER TABLE [dbo].[tblCustomerReturnInvoiceDetail]
ADD CONSTRAINT [PK_tblCustomerReturnInvoiceDetail]
    PRIMARY KEY CLUSTERED ([CustomerReturnInvoiceDetailID] ASC);
GO

-- Creating primary key on [CustomerReturnPaymentID] in table 'tblCustomerReturnPayment'
ALTER TABLE [dbo].[tblCustomerReturnPayment]
ADD CONSTRAINT [PK_tblCustomerReturnPayment]
    PRIMARY KEY CLUSTERED ([CustomerReturnPaymentID] ASC);
GO

-- Creating primary key on [EmployeeID] in table 'tblEmployee'
ALTER TABLE [dbo].[tblEmployee]
ADD CONSTRAINT [PK_tblEmployee]
    PRIMARY KEY CLUSTERED ([EmployeeID] ASC);
GO

-- Creating primary key on [FinancialYearID] in table 'tblFinancialYear'
ALTER TABLE [dbo].[tblFinancialYear]
ADD CONSTRAINT [PK_tblFinancialYear]
    PRIMARY KEY CLUSTERED ([FinancialYearID] ASC);
GO

-- Creating primary key on [PayrollID] in table 'tblPayroll'
ALTER TABLE [dbo].[tblPayroll]
ADD CONSTRAINT [PK_tblPayroll]
    PRIMARY KEY CLUSTERED ([PayrollID] ASC);
GO

-- Creating primary key on [PurchaseCartID] in table 'tblPurchaseCart'
ALTER TABLE [dbo].[tblPurchaseCart]
ADD CONSTRAINT [PK_tblPurchaseCart]
    PRIMARY KEY CLUSTERED ([PurchaseCartID] ASC);
GO

-- Creating primary key on [PurchaseCartDetailID] in table 'tblPurchaseCartDetail'
ALTER TABLE [dbo].[tblPurchaseCartDetail]
ADD CONSTRAINT [PK_tblPurchaseCartDetail]
    PRIMARY KEY CLUSTERED ([PurchaseCartDetailID] ASC);
GO

-- Creating primary key on [SaleCartDetailID] in table 'tblSaleCartDetail'
ALTER TABLE [dbo].[tblSaleCartDetail]
ADD CONSTRAINT [PK_tblSaleCartDetail]
    PRIMARY KEY CLUSTERED ([SaleCartDetailID] ASC);
GO

-- Creating primary key on [ProductID] in table 'tblStock'
ALTER TABLE [dbo].[tblStock]
ADD CONSTRAINT [PK_tblStock]
    PRIMARY KEY CLUSTERED ([ProductID] ASC);
GO

-- Creating primary key on [SupplierID] in table 'tblSupplier'
ALTER TABLE [dbo].[tblSupplier]
ADD CONSTRAINT [PK_tblSupplier]
    PRIMARY KEY CLUSTERED ([SupplierID] ASC);
GO

-- Creating primary key on [SupplierInvoiceID] in table 'tblSupplierInvoice'
ALTER TABLE [dbo].[tblSupplierInvoice]
ADD CONSTRAINT [PK_tblSupplierInvoice]
    PRIMARY KEY CLUSTERED ([SupplierInvoiceID] ASC);
GO

-- Creating primary key on [SupplierInvoiceDetailID] in table 'tblSupplierInvoiceDetail'
ALTER TABLE [dbo].[tblSupplierInvoiceDetail]
ADD CONSTRAINT [PK_tblSupplierInvoiceDetail]
    PRIMARY KEY CLUSTERED ([SupplierInvoiceDetailID] ASC);
GO

-- Creating primary key on [SupplierPaymentID] in table 'tblSupplierPayment'
ALTER TABLE [dbo].[tblSupplierPayment]
ADD CONSTRAINT [PK_tblSupplierPayment]
    PRIMARY KEY CLUSTERED ([SupplierPaymentID] ASC);
GO

-- Creating primary key on [SupplierReturnInvoiceID] in table 'tblSupplierReturnInvoice'
ALTER TABLE [dbo].[tblSupplierReturnInvoice]
ADD CONSTRAINT [PK_tblSupplierReturnInvoice]
    PRIMARY KEY CLUSTERED ([SupplierReturnInvoiceID] ASC);
GO

-- Creating primary key on [SupplierReturnInvoiceDetailID] in table 'tblSupplierReturnInvoiceDetail'
ALTER TABLE [dbo].[tblSupplierReturnInvoiceDetail]
ADD CONSTRAINT [PK_tblSupplierReturnInvoiceDetail]
    PRIMARY KEY CLUSTERED ([SupplierReturnInvoiceDetailID] ASC);
GO

-- Creating primary key on [SupplierReturnPaymentID] in table 'tblSupplierReturnPayment'
ALTER TABLE [dbo].[tblSupplierReturnPayment]
ADD CONSTRAINT [PK_tblSupplierReturnPayment]
    PRIMARY KEY CLUSTERED ([SupplierReturnPaymentID] ASC);
GO

-- Creating primary key on [TransactionID] in table 'tblTransaction'
ALTER TABLE [dbo].[tblTransaction]
ADD CONSTRAINT [PK_tblTransaction]
    PRIMARY KEY CLUSTERED ([TransactionID] ASC);
GO

-- Creating primary key on [UserID] in table 'tblUser'
ALTER TABLE [dbo].[tblUser]
ADD CONSTRAINT [PK_tblUser]
    PRIMARY KEY CLUSTERED ([UserID] ASC);
GO

-- Creating primary key on [UserTypeID] in table 'tblUserType'
ALTER TABLE [dbo].[tblUserType]
ADD CONSTRAINT [PK_tblUserType]
    PRIMARY KEY CLUSTERED ([UserTypeID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [AccountActivityID] in table 'tblAccountSetting'
ALTER TABLE [dbo].[tblAccountSetting]
ADD CONSTRAINT [FK_tblAccountSetting_tblAccountActivity]
    FOREIGN KEY ([AccountActivityID])
    REFERENCES [dbo].[tblAccountActivity]
        ([AccountActivityID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblAccountSetting_tblAccountActivity'
CREATE INDEX [IX_FK_tblAccountSetting_tblAccountActivity]
ON [dbo].[tblAccountSetting]
    ([AccountActivityID]);
GO

-- Creating foreign key on [BranchID] in table 'tblAccountControl'
ALTER TABLE [dbo].[tblAccountControl]
ADD CONSTRAINT [FK_tblAccountControl_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblAccountControl_tblBranch'
CREATE INDEX [IX_FK_tblAccountControl_tblBranch]
ON [dbo].[tblAccountControl]
    ([BranchID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblAccountControl'
ALTER TABLE [dbo].[tblAccountControl]
ADD CONSTRAINT [FK_tblAccountControl_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblAccountControl_tblCompany'
CREATE INDEX [IX_FK_tblAccountControl_tblCompany]
ON [dbo].[tblAccountControl]
    ([CompanyID]);
GO

-- Creating foreign key on [UserID] in table 'tblAccountControl'
ALTER TABLE [dbo].[tblAccountControl]
ADD CONSTRAINT [FK_tblAccountControl_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblAccountControl_tblUser'
CREATE INDEX [IX_FK_tblAccountControl_tblUser]
ON [dbo].[tblAccountControl]
    ([UserID]);
GO

-- Creating foreign key on [AccountControlID] in table 'tblAccountSetting'
ALTER TABLE [dbo].[tblAccountSetting]
ADD CONSTRAINT [FK_tblAccountSetting_tblAccountControl]
    FOREIGN KEY ([AccountControlID])
    REFERENCES [dbo].[tblAccountControl]
        ([AccountControlID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblAccountSetting_tblAccountControl'
CREATE INDEX [IX_FK_tblAccountSetting_tblAccountControl]
ON [dbo].[tblAccountSetting]
    ([AccountControlID]);
GO

-- Creating foreign key on [AccountControlID] in table 'tblAccountSubControl'
ALTER TABLE [dbo].[tblAccountSubControl]
ADD CONSTRAINT [FK_tblAccountSubControl_tblAccountControl]
    FOREIGN KEY ([AccountControlID])
    REFERENCES [dbo].[tblAccountControl]
        ([AccountControlID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblAccountSubControl_tblAccountControl'
CREATE INDEX [IX_FK_tblAccountSubControl_tblAccountControl]
ON [dbo].[tblAccountSubControl]
    ([AccountControlID]);
GO

-- Creating foreign key on [AccountControlID] in table 'tblTransaction'
ALTER TABLE [dbo].[tblTransaction]
ADD CONSTRAINT [FK_tblTransaction_tblAccountControl]
    FOREIGN KEY ([AccountControlID])
    REFERENCES [dbo].[tblAccountControl]
        ([AccountControlID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblTransaction_tblAccountControl'
CREATE INDEX [IX_FK_tblTransaction_tblAccountControl]
ON [dbo].[tblTransaction]
    ([AccountControlID]);
GO

-- Creating foreign key on [UserID] in table 'tblAccountHead'
ALTER TABLE [dbo].[tblAccountHead]
ADD CONSTRAINT [FK_tblAccountHead_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblAccountHead_tblUser'
CREATE INDEX [IX_FK_tblAccountHead_tblUser]
ON [dbo].[tblAccountHead]
    ([UserID]);
GO

-- Creating foreign key on [AccountHeadID] in table 'tblAccountSetting'
ALTER TABLE [dbo].[tblAccountSetting]
ADD CONSTRAINT [FK_tblAccountSetting_tblAccountHead]
    FOREIGN KEY ([AccountHeadID])
    REFERENCES [dbo].[tblAccountHead]
        ([AccountHeadID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblAccountSetting_tblAccountHead'
CREATE INDEX [IX_FK_tblAccountSetting_tblAccountHead]
ON [dbo].[tblAccountSetting]
    ([AccountHeadID]);
GO

-- Creating foreign key on [AccountHeadID] in table 'tblAccountSubControl'
ALTER TABLE [dbo].[tblAccountSubControl]
ADD CONSTRAINT [FK_tblAccountSubControl_tblAccountHead]
    FOREIGN KEY ([AccountHeadID])
    REFERENCES [dbo].[tblAccountHead]
        ([AccountHeadID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblAccountSubControl_tblAccountHead'
CREATE INDEX [IX_FK_tblAccountSubControl_tblAccountHead]
ON [dbo].[tblAccountSubControl]
    ([AccountHeadID]);
GO

-- Creating foreign key on [AccountHeadID] in table 'tblTransaction'
ALTER TABLE [dbo].[tblTransaction]
ADD CONSTRAINT [FK_tblTransaction_tblAccountHead]
    FOREIGN KEY ([AccountHeadID])
    REFERENCES [dbo].[tblAccountHead]
        ([AccountHeadID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblTransaction_tblAccountHead'
CREATE INDEX [IX_FK_tblTransaction_tblAccountHead]
ON [dbo].[tblTransaction]
    ([AccountHeadID]);
GO

-- Creating foreign key on [AccountHeadID] in table 'tblTransaction'
ALTER TABLE [dbo].[tblTransaction]
ADD CONSTRAINT [FK_tblTransection_tblAccountHead]
    FOREIGN KEY ([AccountHeadID])
    REFERENCES [dbo].[tblAccountHead]
        ([AccountHeadID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblTransection_tblAccountHead'
CREATE INDEX [IX_FK_tblTransection_tblAccountHead]
ON [dbo].[tblTransaction]
    ([AccountHeadID]);
GO

-- Creating foreign key on [AccountSubControlID] in table 'tblAccountSetting'
ALTER TABLE [dbo].[tblAccountSetting]
ADD CONSTRAINT [FK_tblAccountSetting_tblAccountSubControl]
    FOREIGN KEY ([AccountSubControlID])
    REFERENCES [dbo].[tblAccountSubControl]
        ([AccountSubControlID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblAccountSetting_tblAccountSubControl'
CREATE INDEX [IX_FK_tblAccountSetting_tblAccountSubControl]
ON [dbo].[tblAccountSetting]
    ([AccountSubControlID]);
GO

-- Creating foreign key on [BranchID] in table 'tblAccountSetting'
ALTER TABLE [dbo].[tblAccountSetting]
ADD CONSTRAINT [FK_tblAccountSetting_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblAccountSetting_tblBranch'
CREATE INDEX [IX_FK_tblAccountSetting_tblBranch]
ON [dbo].[tblAccountSetting]
    ([BranchID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblAccountSetting'
ALTER TABLE [dbo].[tblAccountSetting]
ADD CONSTRAINT [FK_tblAccountSetting_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblAccountSetting_tblCompany'
CREATE INDEX [IX_FK_tblAccountSetting_tblCompany]
ON [dbo].[tblAccountSetting]
    ([CompanyID]);
GO

-- Creating foreign key on [BranchID] in table 'tblAccountSubControl'
ALTER TABLE [dbo].[tblAccountSubControl]
ADD CONSTRAINT [FK_tblAccountSubControl_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblAccountSubControl_tblBranch'
CREATE INDEX [IX_FK_tblAccountSubControl_tblBranch]
ON [dbo].[tblAccountSubControl]
    ([BranchID]);
GO

-- Creating foreign key on [UserID] in table 'tblAccountSubControl'
ALTER TABLE [dbo].[tblAccountSubControl]
ADD CONSTRAINT [FK_tblAccountSubControl_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblAccountSubControl_tblUser'
CREATE INDEX [IX_FK_tblAccountSubControl_tblUser]
ON [dbo].[tblAccountSubControl]
    ([UserID]);
GO

-- Creating foreign key on [AccountSubControlID] in table 'tblTransaction'
ALTER TABLE [dbo].[tblTransaction]
ADD CONSTRAINT [FK_tblTransection_tblAccountSubControl]
    FOREIGN KEY ([AccountSubControlID])
    REFERENCES [dbo].[tblAccountSubControl]
        ([AccountSubControlID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblTransection_tblAccountSubControl'
CREATE INDEX [IX_FK_tblTransection_tblAccountSubControl]
ON [dbo].[tblTransaction]
    ([AccountSubControlID]);
GO

-- Creating foreign key on [BranchTypeID] in table 'tblBranch'
ALTER TABLE [dbo].[tblBranch]
ADD CONSTRAINT [FK_tblBranch_tblBranchType]
    FOREIGN KEY ([BranchTypeID])
    REFERENCES [dbo].[tblBranchType]
        ([BranchTypeID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblBranch_tblBranchType'
CREATE INDEX [IX_FK_tblBranch_tblBranchType]
ON [dbo].[tblBranch]
    ([BranchTypeID]);
GO

-- Creating foreign key on [BranchID] in table 'tblCategory'
ALTER TABLE [dbo].[tblCategory]
ADD CONSTRAINT [FK_tblCategory_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCategory_tblBranch'
CREATE INDEX [IX_FK_tblCategory_tblBranch]
ON [dbo].[tblCategory]
    ([BranchID]);
GO

-- Creating foreign key on [BranchID] in table 'tblCustomer'
ALTER TABLE [dbo].[tblCustomer]
ADD CONSTRAINT [FK_tblCustomer_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomer_tblBranch'
CREATE INDEX [IX_FK_tblCustomer_tblBranch]
ON [dbo].[tblCustomer]
    ([BranchID]);
GO

-- Creating foreign key on [BranchID] in table 'tblCustomerInvoice'
ALTER TABLE [dbo].[tblCustomerInvoice]
ADD CONSTRAINT [FK_tblCustomerInvoice_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerInvoice_tblBranch'
CREATE INDEX [IX_FK_tblCustomerInvoice_tblBranch]
ON [dbo].[tblCustomerInvoice]
    ([BranchID]);
GO

-- Creating foreign key on [BranchID] in table 'tblCustomerPayment'
ALTER TABLE [dbo].[tblCustomerPayment]
ADD CONSTRAINT [FK_tblCustomerPayment_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerPayment_tblBranch'
CREATE INDEX [IX_FK_tblCustomerPayment_tblBranch]
ON [dbo].[tblCustomerPayment]
    ([BranchID]);
GO

-- Creating foreign key on [BranchID] in table 'tblCustomerReturnInvoice'
ALTER TABLE [dbo].[tblCustomerReturnInvoice]
ADD CONSTRAINT [FK_tblCustomerReturnInvoice_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerReturnInvoice_tblBranch'
CREATE INDEX [IX_FK_tblCustomerReturnInvoice_tblBranch]
ON [dbo].[tblCustomerReturnInvoice]
    ([BranchID]);
GO

-- Creating foreign key on [BranchID] in table 'tblCustomerReturnPayment'
ALTER TABLE [dbo].[tblCustomerReturnPayment]
ADD CONSTRAINT [FK_tblCustomerReturnPayment_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerReturnPayment_tblBranch'
CREATE INDEX [IX_FK_tblCustomerReturnPayment_tblBranch]
ON [dbo].[tblCustomerReturnPayment]
    ([BranchID]);
GO

-- Creating foreign key on [BranchID] in table 'tblEmployee'
ALTER TABLE [dbo].[tblEmployee]
ADD CONSTRAINT [FK_tblEmployee_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblEmployee_tblBranch'
CREATE INDEX [IX_FK_tblEmployee_tblBranch]
ON [dbo].[tblEmployee]
    ([BranchID]);
GO

-- Creating foreign key on [BranchID] in table 'tblPayroll'
ALTER TABLE [dbo].[tblPayroll]
ADD CONSTRAINT [FK_tblPayroll_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblPayroll_tblBranch'
CREATE INDEX [IX_FK_tblPayroll_tblBranch]
ON [dbo].[tblPayroll]
    ([BranchID]);
GO

-- Creating foreign key on [BranchID] in table 'tblPurchaseCartDetail'
ALTER TABLE [dbo].[tblPurchaseCartDetail]
ADD CONSTRAINT [FK_tblPurchaseCartDetail_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblPurchaseCartDetail_tblBranch'
CREATE INDEX [IX_FK_tblPurchaseCartDetail_tblBranch]
ON [dbo].[tblPurchaseCartDetail]
    ([BranchID]);
GO

-- Creating foreign key on [BranchID] in table 'tblPurchaseCart'
ALTER TABLE [dbo].[tblPurchaseCart]
ADD CONSTRAINT [FK_tblPurchaseCartTable_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblPurchaseCartTable_tblBranch'
CREATE INDEX [IX_FK_tblPurchaseCartTable_tblBranch]
ON [dbo].[tblPurchaseCart]
    ([BranchID]);
GO

-- Creating foreign key on [BranchID] in table 'tblSaleCartDetail'
ALTER TABLE [dbo].[tblSaleCartDetail]
ADD CONSTRAINT [FK_tblSaleCartDetail_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSaleCartDetail_tblBranch'
CREATE INDEX [IX_FK_tblSaleCartDetail_tblBranch]
ON [dbo].[tblSaleCartDetail]
    ([BranchID]);
GO

-- Creating foreign key on [BranchID] in table 'tblStock'
ALTER TABLE [dbo].[tblStock]
ADD CONSTRAINT [FK_tblStock_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblStock_tblBranch'
CREATE INDEX [IX_FK_tblStock_tblBranch]
ON [dbo].[tblStock]
    ([BranchID]);
GO

-- Creating foreign key on [BranchID] in table 'tblSupplier'
ALTER TABLE [dbo].[tblSupplier]
ADD CONSTRAINT [FK_tblSupplier_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplier_tblBranch'
CREATE INDEX [IX_FK_tblSupplier_tblBranch]
ON [dbo].[tblSupplier]
    ([BranchID]);
GO

-- Creating foreign key on [BranchID] in table 'tblSupplierInvoice'
ALTER TABLE [dbo].[tblSupplierInvoice]
ADD CONSTRAINT [FK_tblSupplierInvoiceTable_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierInvoiceTable_tblBranch'
CREATE INDEX [IX_FK_tblSupplierInvoiceTable_tblBranch]
ON [dbo].[tblSupplierInvoice]
    ([BranchID]);
GO

-- Creating foreign key on [BranchID] in table 'tblSupplierReturnInvoice'
ALTER TABLE [dbo].[tblSupplierReturnInvoice]
ADD CONSTRAINT [FK_tblSupplierReturnInvoice_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierReturnInvoice_tblBranch'
CREATE INDEX [IX_FK_tblSupplierReturnInvoice_tblBranch]
ON [dbo].[tblSupplierReturnInvoice]
    ([BranchID]);
GO

-- Creating foreign key on [BranchID] in table 'tblSupplierReturnPayment'
ALTER TABLE [dbo].[tblSupplierReturnPayment]
ADD CONSTRAINT [FK_tblSupplierReturnPayment_tblBranch]
    FOREIGN KEY ([BranchID])
    REFERENCES [dbo].[tblBranch]
        ([BranchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierReturnPayment_tblBranch'
CREATE INDEX [IX_FK_tblSupplierReturnPayment_tblBranch]
ON [dbo].[tblSupplierReturnPayment]
    ([BranchID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblCategory'
ALTER TABLE [dbo].[tblCategory]
ADD CONSTRAINT [FK_tblCategory_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCategory_tblCompany'
CREATE INDEX [IX_FK_tblCategory_tblCompany]
ON [dbo].[tblCategory]
    ([CompanyID]);
GO

-- Creating foreign key on [UserID] in table 'tblCategory'
ALTER TABLE [dbo].[tblCategory]
ADD CONSTRAINT [FK_tblCategory_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCategory_tblUser'
CREATE INDEX [IX_FK_tblCategory_tblUser]
ON [dbo].[tblCategory]
    ([UserID]);
GO

-- Creating foreign key on [CategoryID] in table 'tblStock'
ALTER TABLE [dbo].[tblStock]
ADD CONSTRAINT [FK_tblStock_tblCategory]
    FOREIGN KEY ([CategoryID])
    REFERENCES [dbo].[tblCategory]
        ([CategoryID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblStock_tblCategory'
CREATE INDEX [IX_FK_tblStock_tblCategory]
ON [dbo].[tblStock]
    ([CategoryID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblCustomer'
ALTER TABLE [dbo].[tblCustomer]
ADD CONSTRAINT [FK_tblCustomer_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomer_tblCompany'
CREATE INDEX [IX_FK_tblCustomer_tblCompany]
ON [dbo].[tblCustomer]
    ([CompanyID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblCustomerInvoice'
ALTER TABLE [dbo].[tblCustomerInvoice]
ADD CONSTRAINT [FK_tblCustomerInvoice_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerInvoice_tblCompany'
CREATE INDEX [IX_FK_tblCustomerInvoice_tblCompany]
ON [dbo].[tblCustomerInvoice]
    ([CompanyID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblCustomerPayment'
ALTER TABLE [dbo].[tblCustomerPayment]
ADD CONSTRAINT [FK_tblCustomerPayment_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerPayment_tblCompany'
CREATE INDEX [IX_FK_tblCustomerPayment_tblCompany]
ON [dbo].[tblCustomerPayment]
    ([CompanyID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblCustomerReturnInvoice'
ALTER TABLE [dbo].[tblCustomerReturnInvoice]
ADD CONSTRAINT [FK_tblCustomerReturnInvoice_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerReturnInvoice_tblCompany'
CREATE INDEX [IX_FK_tblCustomerReturnInvoice_tblCompany]
ON [dbo].[tblCustomerReturnInvoice]
    ([CompanyID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblCustomerReturnPayment'
ALTER TABLE [dbo].[tblCustomerReturnPayment]
ADD CONSTRAINT [FK_tblCustomerReturnPayment_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerReturnPayment_tblCompany'
CREATE INDEX [IX_FK_tblCustomerReturnPayment_tblCompany]
ON [dbo].[tblCustomerReturnPayment]
    ([CompanyID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblEmployee'
ALTER TABLE [dbo].[tblEmployee]
ADD CONSTRAINT [FK_tblEmployee_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblEmployee_tblCompany'
CREATE INDEX [IX_FK_tblEmployee_tblCompany]
ON [dbo].[tblEmployee]
    ([CompanyID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblPayroll'
ALTER TABLE [dbo].[tblPayroll]
ADD CONSTRAINT [FK_tblPayroll_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblPayroll_tblCompany'
CREATE INDEX [IX_FK_tblPayroll_tblCompany]
ON [dbo].[tblPayroll]
    ([CompanyID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblPurchaseCartDetail'
ALTER TABLE [dbo].[tblPurchaseCartDetail]
ADD CONSTRAINT [FK_tblPurchaseCartDetail_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblPurchaseCartDetail_tblCompany'
CREATE INDEX [IX_FK_tblPurchaseCartDetail_tblCompany]
ON [dbo].[tblPurchaseCartDetail]
    ([CompanyID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblPurchaseCart'
ALTER TABLE [dbo].[tblPurchaseCart]
ADD CONSTRAINT [FK_tblPurchaseCartTable_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblPurchaseCartTable_tblCompany'
CREATE INDEX [IX_FK_tblPurchaseCartTable_tblCompany]
ON [dbo].[tblPurchaseCart]
    ([CompanyID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblSaleCartDetail'
ALTER TABLE [dbo].[tblSaleCartDetail]
ADD CONSTRAINT [FK_tblSaleCartDetail_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSaleCartDetail_tblCompany'
CREATE INDEX [IX_FK_tblSaleCartDetail_tblCompany]
ON [dbo].[tblSaleCartDetail]
    ([CompanyID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblStock'
ALTER TABLE [dbo].[tblStock]
ADD CONSTRAINT [FK_tblStock_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblStock_tblCompany'
CREATE INDEX [IX_FK_tblStock_tblCompany]
ON [dbo].[tblStock]
    ([CompanyID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblSupplier'
ALTER TABLE [dbo].[tblSupplier]
ADD CONSTRAINT [FK_tblSupplier_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplier_tblCompany'
CREATE INDEX [IX_FK_tblSupplier_tblCompany]
ON [dbo].[tblSupplier]
    ([CompanyID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblSupplierInvoice'
ALTER TABLE [dbo].[tblSupplierInvoice]
ADD CONSTRAINT [FK_tblSupplierInvoiceTable_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierInvoiceTable_tblCompany'
CREATE INDEX [IX_FK_tblSupplierInvoiceTable_tblCompany]
ON [dbo].[tblSupplierInvoice]
    ([CompanyID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblSupplierReturnInvoice'
ALTER TABLE [dbo].[tblSupplierReturnInvoice]
ADD CONSTRAINT [FK_tblSupplierReturnInvoice_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierReturnInvoice_tblCompany'
CREATE INDEX [IX_FK_tblSupplierReturnInvoice_tblCompany]
ON [dbo].[tblSupplierReturnInvoice]
    ([CompanyID]);
GO

-- Creating foreign key on [CompanyID] in table 'tblSupplierReturnPayment'
ALTER TABLE [dbo].[tblSupplierReturnPayment]
ADD CONSTRAINT [FK_tblSupplierReturnPayment_tblCompany]
    FOREIGN KEY ([CompanyID])
    REFERENCES [dbo].[tblCompany]
        ([CompanyID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierReturnPayment_tblCompany'
CREATE INDEX [IX_FK_tblSupplierReturnPayment_tblCompany]
ON [dbo].[tblSupplierReturnPayment]
    ([CompanyID]);
GO

-- Creating foreign key on [UserID] in table 'tblCustomer'
ALTER TABLE [dbo].[tblCustomer]
ADD CONSTRAINT [FK_tblCustomer_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomer_tblUser'
CREATE INDEX [IX_FK_tblCustomer_tblUser]
ON [dbo].[tblCustomer]
    ([UserID]);
GO

-- Creating foreign key on [CustomerID] in table 'tblCustomerInvoice'
ALTER TABLE [dbo].[tblCustomerInvoice]
ADD CONSTRAINT [FK_tblCustomerInvoice_tblCustomer]
    FOREIGN KEY ([CustomerID])
    REFERENCES [dbo].[tblCustomer]
        ([CustomerID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerInvoice_tblCustomer'
CREATE INDEX [IX_FK_tblCustomerInvoice_tblCustomer]
ON [dbo].[tblCustomerInvoice]
    ([CustomerID]);
GO

-- Creating foreign key on [CustomerID] in table 'tblCustomerReturnInvoice'
ALTER TABLE [dbo].[tblCustomerReturnInvoice]
ADD CONSTRAINT [FK_tblCustomerReturnInvoice_tblCustomer]
    FOREIGN KEY ([CustomerID])
    REFERENCES [dbo].[tblCustomer]
        ([CustomerID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerReturnInvoice_tblCustomer'
CREATE INDEX [IX_FK_tblCustomerReturnInvoice_tblCustomer]
ON [dbo].[tblCustomerReturnInvoice]
    ([CustomerID]);
GO

-- Creating foreign key on [CustomerID] in table 'tblCustomerReturnPayment'
ALTER TABLE [dbo].[tblCustomerReturnPayment]
ADD CONSTRAINT [FK_tblCustomerReturnPayment_tblCustomer]
    FOREIGN KEY ([CustomerID])
    REFERENCES [dbo].[tblCustomer]
        ([CustomerID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerReturnPayment_tblCustomer'
CREATE INDEX [IX_FK_tblCustomerReturnPayment_tblCustomer]
ON [dbo].[tblCustomerReturnPayment]
    ([CustomerID]);
GO

-- Creating foreign key on [UserID] in table 'tblCustomerInvoice'
ALTER TABLE [dbo].[tblCustomerInvoice]
ADD CONSTRAINT [FK_tblCustomerInvoice_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerInvoice_tblUser'
CREATE INDEX [IX_FK_tblCustomerInvoice_tblUser]
ON [dbo].[tblCustomerInvoice]
    ([UserID]);
GO

-- Creating foreign key on [CustomerInvoiceID] in table 'tblCustomerInvoiceDetail'
ALTER TABLE [dbo].[tblCustomerInvoiceDetail]
ADD CONSTRAINT [FK_tblCustomerInvoiceDetail_tblCustomerInvoice]
    FOREIGN KEY ([CustomerInvoiceID])
    REFERENCES [dbo].[tblCustomerInvoice]
        ([CustomerInvoiceID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerInvoiceDetail_tblCustomerInvoice'
CREATE INDEX [IX_FK_tblCustomerInvoiceDetail_tblCustomerInvoice]
ON [dbo].[tblCustomerInvoiceDetail]
    ([CustomerInvoiceID]);
GO

-- Creating foreign key on [CustomerInvoiceID] in table 'tblCustomerPayment'
ALTER TABLE [dbo].[tblCustomerPayment]
ADD CONSTRAINT [FK_tblCustomerPayment_tblCustomerInvoice]
    FOREIGN KEY ([CustomerInvoiceID])
    REFERENCES [dbo].[tblCustomerInvoice]
        ([CustomerInvoiceID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerPayment_tblCustomerInvoice'
CREATE INDEX [IX_FK_tblCustomerPayment_tblCustomerInvoice]
ON [dbo].[tblCustomerPayment]
    ([CustomerInvoiceID]);
GO

-- Creating foreign key on [CustomerInvoiceID] in table 'tblCustomerReturnInvoice'
ALTER TABLE [dbo].[tblCustomerReturnInvoice]
ADD CONSTRAINT [FK_tblCustomerReturnInvoice_tblCustomerInvoice]
    FOREIGN KEY ([CustomerInvoiceID])
    REFERENCES [dbo].[tblCustomerInvoice]
        ([CustomerInvoiceID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerReturnInvoice_tblCustomerInvoice'
CREATE INDEX [IX_FK_tblCustomerReturnInvoice_tblCustomerInvoice]
ON [dbo].[tblCustomerReturnInvoice]
    ([CustomerInvoiceID]);
GO

-- Creating foreign key on [CustomerInvoiceID] in table 'tblCustomerReturnInvoiceDetail'
ALTER TABLE [dbo].[tblCustomerReturnInvoiceDetail]
ADD CONSTRAINT [FK_tblCustomerReturnInvoiceDetail_tblCustomerInvoice]
    FOREIGN KEY ([CustomerInvoiceID])
    REFERENCES [dbo].[tblCustomerInvoice]
        ([CustomerInvoiceID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerReturnInvoiceDetail_tblCustomerInvoice'
CREATE INDEX [IX_FK_tblCustomerReturnInvoiceDetail_tblCustomerInvoice]
ON [dbo].[tblCustomerReturnInvoiceDetail]
    ([CustomerInvoiceID]);
GO

-- Creating foreign key on [CustomerInvoiceID] in table 'tblCustomerReturnPayment'
ALTER TABLE [dbo].[tblCustomerReturnPayment]
ADD CONSTRAINT [FK_tblCustomerReturnPayment_tblCustomerInvoice]
    FOREIGN KEY ([CustomerInvoiceID])
    REFERENCES [dbo].[tblCustomerInvoice]
        ([CustomerInvoiceID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerReturnPayment_tblCustomerInvoice'
CREATE INDEX [IX_FK_tblCustomerReturnPayment_tblCustomerInvoice]
ON [dbo].[tblCustomerReturnPayment]
    ([CustomerInvoiceID]);
GO

-- Creating foreign key on [ProductID] in table 'tblCustomerInvoiceDetail'
ALTER TABLE [dbo].[tblCustomerInvoiceDetail]
ADD CONSTRAINT [FK_tblCustomerInvoiceDetail_tblStock]
    FOREIGN KEY ([ProductID])
    REFERENCES [dbo].[tblStock]
        ([ProductID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerInvoiceDetail_tblStock'
CREATE INDEX [IX_FK_tblCustomerInvoiceDetail_tblStock]
ON [dbo].[tblCustomerInvoiceDetail]
    ([ProductID]);
GO

-- Creating foreign key on [CustomerInvoiceDetailID] in table 'tblCustomerReturnInvoiceDetail'
ALTER TABLE [dbo].[tblCustomerReturnInvoiceDetail]
ADD CONSTRAINT [FK_tblCustomerReturnInvoiceDetail_tblCustomerInvoiceDetail]
    FOREIGN KEY ([CustomerInvoiceDetailID])
    REFERENCES [dbo].[tblCustomerInvoiceDetail]
        ([CustomerInvoiceDetailID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerReturnInvoiceDetail_tblCustomerInvoiceDetail'
CREATE INDEX [IX_FK_tblCustomerReturnInvoiceDetail_tblCustomerInvoiceDetail]
ON [dbo].[tblCustomerReturnInvoiceDetail]
    ([CustomerInvoiceDetailID]);
GO

-- Creating foreign key on [UserID] in table 'tblCustomerPayment'
ALTER TABLE [dbo].[tblCustomerPayment]
ADD CONSTRAINT [FK_tblCustomerPayment_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerPayment_tblUser'
CREATE INDEX [IX_FK_tblCustomerPayment_tblUser]
ON [dbo].[tblCustomerPayment]
    ([UserID]);
GO

-- Creating foreign key on [UserID] in table 'tblCustomerReturnInvoice'
ALTER TABLE [dbo].[tblCustomerReturnInvoice]
ADD CONSTRAINT [FK_tblCustomerReturnInvoice_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerReturnInvoice_tblUser'
CREATE INDEX [IX_FK_tblCustomerReturnInvoice_tblUser]
ON [dbo].[tblCustomerReturnInvoice]
    ([UserID]);
GO

-- Creating foreign key on [CustomerReturnInvoiceID] in table 'tblCustomerReturnInvoiceDetail'
ALTER TABLE [dbo].[tblCustomerReturnInvoiceDetail]
ADD CONSTRAINT [FK_tblCustomerReturnInvoiceDetail_tblCustomerReturnInvoice]
    FOREIGN KEY ([CustomerReturnInvoiceID])
    REFERENCES [dbo].[tblCustomerReturnInvoice]
        ([CustomerReturnInvoiceID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerReturnInvoiceDetail_tblCustomerReturnInvoice'
CREATE INDEX [IX_FK_tblCustomerReturnInvoiceDetail_tblCustomerReturnInvoice]
ON [dbo].[tblCustomerReturnInvoiceDetail]
    ([CustomerReturnInvoiceID]);
GO

-- Creating foreign key on [CustomerReturnInvoiceID] in table 'tblCustomerReturnPayment'
ALTER TABLE [dbo].[tblCustomerReturnPayment]
ADD CONSTRAINT [FK_tblCustomerReturnPayment_tblCustomerReturnInvoice]
    FOREIGN KEY ([CustomerReturnInvoiceID])
    REFERENCES [dbo].[tblCustomerReturnInvoice]
        ([CustomerReturnInvoiceID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerReturnPayment_tblCustomerReturnInvoice'
CREATE INDEX [IX_FK_tblCustomerReturnPayment_tblCustomerReturnInvoice]
ON [dbo].[tblCustomerReturnPayment]
    ([CustomerReturnInvoiceID]);
GO

-- Creating foreign key on [ProductID] in table 'tblCustomerReturnInvoiceDetail'
ALTER TABLE [dbo].[tblCustomerReturnInvoiceDetail]
ADD CONSTRAINT [FK_tblCustomerReturnInvoiceDetail_tblStock]
    FOREIGN KEY ([ProductID])
    REFERENCES [dbo].[tblStock]
        ([ProductID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerReturnInvoiceDetail_tblStock'
CREATE INDEX [IX_FK_tblCustomerReturnInvoiceDetail_tblStock]
ON [dbo].[tblCustomerReturnInvoiceDetail]
    ([ProductID]);
GO

-- Creating foreign key on [UserID] in table 'tblCustomerReturnPayment'
ALTER TABLE [dbo].[tblCustomerReturnPayment]
ADD CONSTRAINT [FK_tblCustomerReturnPayment_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblCustomerReturnPayment_tblUser'
CREATE INDEX [IX_FK_tblCustomerReturnPayment_tblUser]
ON [dbo].[tblCustomerReturnPayment]
    ([UserID]);
GO

-- Creating foreign key on [EmployeeID] in table 'tblPayroll'
ALTER TABLE [dbo].[tblPayroll]
ADD CONSTRAINT [FK_tblPayroll_tblEmployee]
    FOREIGN KEY ([EmployeeID])
    REFERENCES [dbo].[tblEmployee]
        ([EmployeeID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblPayroll_tblEmployee'
CREATE INDEX [IX_FK_tblPayroll_tblEmployee]
ON [dbo].[tblPayroll]
    ([EmployeeID]);
GO

-- Creating foreign key on [UserID] in table 'tblFinancialYear'
ALTER TABLE [dbo].[tblFinancialYear]
ADD CONSTRAINT [FK_tblFinancialYear_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblFinancialYear_tblUser'
CREATE INDEX [IX_FK_tblFinancialYear_tblUser]
ON [dbo].[tblFinancialYear]
    ([UserID]);
GO

-- Creating foreign key on [FinancialYearID] in table 'tblTransaction'
ALTER TABLE [dbo].[tblTransaction]
ADD CONSTRAINT [FK_tblTransaction_tblFinancialYear]
    FOREIGN KEY ([FinancialYearID])
    REFERENCES [dbo].[tblFinancialYear]
        ([FinancialYearID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblTransaction_tblFinancialYear'
CREATE INDEX [IX_FK_tblTransaction_tblFinancialYear]
ON [dbo].[tblTransaction]
    ([FinancialYearID]);
GO

-- Creating foreign key on [FinancialYearID] in table 'tblTransaction'
ALTER TABLE [dbo].[tblTransaction]
ADD CONSTRAINT [FK_tblTransection_tblFinancialYear]
    FOREIGN KEY ([FinancialYearID])
    REFERENCES [dbo].[tblFinancialYear]
        ([FinancialYearID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblTransection_tblFinancialYear'
CREATE INDEX [IX_FK_tblTransection_tblFinancialYear]
ON [dbo].[tblTransaction]
    ([FinancialYearID]);
GO

-- Creating foreign key on [UserID] in table 'tblPayroll'
ALTER TABLE [dbo].[tblPayroll]
ADD CONSTRAINT [FK_tblPayroll_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblPayroll_tblUser'
CREATE INDEX [IX_FK_tblPayroll_tblUser]
ON [dbo].[tblPayroll]
    ([UserID]);
GO

-- Creating foreign key on [UserID] in table 'tblPurchaseCart'
ALTER TABLE [dbo].[tblPurchaseCart]
ADD CONSTRAINT [FK_tblPurchaseCart_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblPurchaseCart_tblUser'
CREATE INDEX [IX_FK_tblPurchaseCart_tblUser]
ON [dbo].[tblPurchaseCart]
    ([UserID]);
GO

-- Creating foreign key on [SupplierID] in table 'tblPurchaseCart'
ALTER TABLE [dbo].[tblPurchaseCart]
ADD CONSTRAINT [FK_tblPurchaseCartTable_tblSupplier]
    FOREIGN KEY ([SupplierID])
    REFERENCES [dbo].[tblSupplier]
        ([SupplierID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblPurchaseCartTable_tblSupplier'
CREATE INDEX [IX_FK_tblPurchaseCartTable_tblSupplier]
ON [dbo].[tblPurchaseCart]
    ([SupplierID]);
GO

-- Creating foreign key on [ProductID] in table 'tblPurchaseCartDetail'
ALTER TABLE [dbo].[tblPurchaseCartDetail]
ADD CONSTRAINT [FK_tblPurchaseCartDetail_tblStock]
    FOREIGN KEY ([ProductID])
    REFERENCES [dbo].[tblStock]
        ([ProductID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblPurchaseCartDetail_tblStock'
CREATE INDEX [IX_FK_tblPurchaseCartDetail_tblStock]
ON [dbo].[tblPurchaseCartDetail]
    ([ProductID]);
GO

-- Creating foreign key on [UserID] in table 'tblPurchaseCartDetail'
ALTER TABLE [dbo].[tblPurchaseCartDetail]
ADD CONSTRAINT [FK_tblPurchaseCartDetail_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblPurchaseCartDetail_tblUser'
CREATE INDEX [IX_FK_tblPurchaseCartDetail_tblUser]
ON [dbo].[tblPurchaseCartDetail]
    ([UserID]);
GO

-- Creating foreign key on [ProductID] in table 'tblSaleCartDetail'
ALTER TABLE [dbo].[tblSaleCartDetail]
ADD CONSTRAINT [FK_tblSaleCartDetail_tblStock]
    FOREIGN KEY ([ProductID])
    REFERENCES [dbo].[tblStock]
        ([ProductID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSaleCartDetail_tblStock'
CREATE INDEX [IX_FK_tblSaleCartDetail_tblStock]
ON [dbo].[tblSaleCartDetail]
    ([ProductID]);
GO

-- Creating foreign key on [UserID] in table 'tblSaleCartDetail'
ALTER TABLE [dbo].[tblSaleCartDetail]
ADD CONSTRAINT [FK_tblSaleCartDetail_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSaleCartDetail_tblUser'
CREATE INDEX [IX_FK_tblSaleCartDetail_tblUser]
ON [dbo].[tblSaleCartDetail]
    ([UserID]);
GO

-- Creating foreign key on [UserID] in table 'tblStock'
ALTER TABLE [dbo].[tblStock]
ADD CONSTRAINT [FK_tblStock_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblStock_tblUser'
CREATE INDEX [IX_FK_tblStock_tblUser]
ON [dbo].[tblStock]
    ([UserID]);
GO

-- Creating foreign key on [ProductID] in table 'tblSupplierInvoiceDetail'
ALTER TABLE [dbo].[tblSupplierInvoiceDetail]
ADD CONSTRAINT [FK_tblSupplierInvoiceDetail_tblStock]
    FOREIGN KEY ([ProductID])
    REFERENCES [dbo].[tblStock]
        ([ProductID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierInvoiceDetail_tblStock'
CREATE INDEX [IX_FK_tblSupplierInvoiceDetail_tblStock]
ON [dbo].[tblSupplierInvoiceDetail]
    ([ProductID]);
GO

-- Creating foreign key on [ProductID] in table 'tblSupplierReturnInvoiceDetail'
ALTER TABLE [dbo].[tblSupplierReturnInvoiceDetail]
ADD CONSTRAINT [FK_tblSupplierReturnInvoiceDetail_tblStock]
    FOREIGN KEY ([ProductID])
    REFERENCES [dbo].[tblStock]
        ([ProductID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierReturnInvoiceDetail_tblStock'
CREATE INDEX [IX_FK_tblSupplierReturnInvoiceDetail_tblStock]
ON [dbo].[tblSupplierReturnInvoiceDetail]
    ([ProductID]);
GO

-- Creating foreign key on [UserID] in table 'tblSupplier'
ALTER TABLE [dbo].[tblSupplier]
ADD CONSTRAINT [FK_tblSupplier_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplier_tblUser'
CREATE INDEX [IX_FK_tblSupplier_tblUser]
ON [dbo].[tblSupplier]
    ([UserID]);
GO

-- Creating foreign key on [SupplierID] in table 'tblSupplierInvoice'
ALTER TABLE [dbo].[tblSupplierInvoice]
ADD CONSTRAINT [FK_tblSupplierInvoiceTable_tblSupplier]
    FOREIGN KEY ([SupplierID])
    REFERENCES [dbo].[tblSupplier]
        ([SupplierID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierInvoiceTable_tblSupplier'
CREATE INDEX [IX_FK_tblSupplierInvoiceTable_tblSupplier]
ON [dbo].[tblSupplierInvoice]
    ([SupplierID]);
GO

-- Creating foreign key on [SupplierID] in table 'tblSupplierPayment'
ALTER TABLE [dbo].[tblSupplierPayment]
ADD CONSTRAINT [FK_tblSupplierPayment_tblSupplier]
    FOREIGN KEY ([SupplierID])
    REFERENCES [dbo].[tblSupplier]
        ([SupplierID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierPayment_tblSupplier'
CREATE INDEX [IX_FK_tblSupplierPayment_tblSupplier]
ON [dbo].[tblSupplierPayment]
    ([SupplierID]);
GO

-- Creating foreign key on [SupplierID] in table 'tblSupplierReturnInvoice'
ALTER TABLE [dbo].[tblSupplierReturnInvoice]
ADD CONSTRAINT [FK_tblSupplierReturnInvoice_tblSupplier]
    FOREIGN KEY ([SupplierID])
    REFERENCES [dbo].[tblSupplier]
        ([SupplierID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierReturnInvoice_tblSupplier'
CREATE INDEX [IX_FK_tblSupplierReturnInvoice_tblSupplier]
ON [dbo].[tblSupplierReturnInvoice]
    ([SupplierID]);
GO

-- Creating foreign key on [SupplierID] in table 'tblSupplierReturnPayment'
ALTER TABLE [dbo].[tblSupplierReturnPayment]
ADD CONSTRAINT [FK_tblSupplierReturnPayment_tblSupplier]
    FOREIGN KEY ([SupplierID])
    REFERENCES [dbo].[tblSupplier]
        ([SupplierID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierReturnPayment_tblSupplier'
CREATE INDEX [IX_FK_tblSupplierReturnPayment_tblSupplier]
ON [dbo].[tblSupplierReturnPayment]
    ([SupplierID]);
GO

-- Creating foreign key on [UserID] in table 'tblSupplierInvoice'
ALTER TABLE [dbo].[tblSupplierInvoice]
ADD CONSTRAINT [FK_tblSupplierInvoice_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierInvoice_tblUser'
CREATE INDEX [IX_FK_tblSupplierInvoice_tblUser]
ON [dbo].[tblSupplierInvoice]
    ([UserID]);
GO

-- Creating foreign key on [SupplierInvoiceID] in table 'tblSupplierInvoiceDetail'
ALTER TABLE [dbo].[tblSupplierInvoiceDetail]
ADD CONSTRAINT [FK_tblSupplierInvoiceDetail_tblSupplierInvoice]
    FOREIGN KEY ([SupplierInvoiceID])
    REFERENCES [dbo].[tblSupplierInvoice]
        ([SupplierInvoiceID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierInvoiceDetail_tblSupplierInvoice'
CREATE INDEX [IX_FK_tblSupplierInvoiceDetail_tblSupplierInvoice]
ON [dbo].[tblSupplierInvoiceDetail]
    ([SupplierInvoiceID]);
GO

-- Creating foreign key on [SupplierInvoiceID] in table 'tblSupplierReturnInvoice'
ALTER TABLE [dbo].[tblSupplierReturnInvoice]
ADD CONSTRAINT [FK_tblSupplierReturnInvoice_tblSupplierInvoice]
    FOREIGN KEY ([SupplierInvoiceID])
    REFERENCES [dbo].[tblSupplierInvoice]
        ([SupplierInvoiceID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierReturnInvoice_tblSupplierInvoice'
CREATE INDEX [IX_FK_tblSupplierReturnInvoice_tblSupplierInvoice]
ON [dbo].[tblSupplierReturnInvoice]
    ([SupplierInvoiceID]);
GO

-- Creating foreign key on [SupplierInvoiceID] in table 'tblSupplierReturnInvoiceDetail'
ALTER TABLE [dbo].[tblSupplierReturnInvoiceDetail]
ADD CONSTRAINT [FK_tblSupplierReturnInvoiceDetail_tblSupplierInvoice]
    FOREIGN KEY ([SupplierInvoiceID])
    REFERENCES [dbo].[tblSupplierInvoice]
        ([SupplierInvoiceID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierReturnInvoiceDetail_tblSupplierInvoice'
CREATE INDEX [IX_FK_tblSupplierReturnInvoiceDetail_tblSupplierInvoice]
ON [dbo].[tblSupplierReturnInvoiceDetail]
    ([SupplierInvoiceID]);
GO

-- Creating foreign key on [SupplierInvoiceID] in table 'tblSupplierReturnPayment'
ALTER TABLE [dbo].[tblSupplierReturnPayment]
ADD CONSTRAINT [FK_tblSupplierReturnPayment_tblSupplierInvoice]
    FOREIGN KEY ([SupplierInvoiceID])
    REFERENCES [dbo].[tblSupplierInvoice]
        ([SupplierInvoiceID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierReturnPayment_tblSupplierInvoice'
CREATE INDEX [IX_FK_tblSupplierReturnPayment_tblSupplierInvoice]
ON [dbo].[tblSupplierReturnPayment]
    ([SupplierInvoiceID]);
GO

-- Creating foreign key on [SupplierInvoiceDetailID] in table 'tblSupplierReturnInvoiceDetail'
ALTER TABLE [dbo].[tblSupplierReturnInvoiceDetail]
ADD CONSTRAINT [FK_tblSupplierReturnInvoiceDetail_tblSupplierInvoiceDetail]
    FOREIGN KEY ([SupplierInvoiceDetailID])
    REFERENCES [dbo].[tblSupplierInvoiceDetail]
        ([SupplierInvoiceDetailID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierReturnInvoiceDetail_tblSupplierInvoiceDetail'
CREATE INDEX [IX_FK_tblSupplierReturnInvoiceDetail_tblSupplierInvoiceDetail]
ON [dbo].[tblSupplierReturnInvoiceDetail]
    ([SupplierInvoiceDetailID]);
GO

-- Creating foreign key on [UserID] in table 'tblSupplierPayment'
ALTER TABLE [dbo].[tblSupplierPayment]
ADD CONSTRAINT [FK_tblSupplierPayment_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierPayment_tblUser'
CREATE INDEX [IX_FK_tblSupplierPayment_tblUser]
ON [dbo].[tblSupplierPayment]
    ([UserID]);
GO

-- Creating foreign key on [UserID] in table 'tblSupplierReturnInvoice'
ALTER TABLE [dbo].[tblSupplierReturnInvoice]
ADD CONSTRAINT [FK_tblSupplierReturnInvoice_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierReturnInvoice_tblUser'
CREATE INDEX [IX_FK_tblSupplierReturnInvoice_tblUser]
ON [dbo].[tblSupplierReturnInvoice]
    ([UserID]);
GO

-- Creating foreign key on [SupplierReturnInvoiceID] in table 'tblSupplierReturnInvoiceDetail'
ALTER TABLE [dbo].[tblSupplierReturnInvoiceDetail]
ADD CONSTRAINT [FK_tblSupplierReturnInvoiceDetail_tblSupplierReturnInvoice]
    FOREIGN KEY ([SupplierReturnInvoiceID])
    REFERENCES [dbo].[tblSupplierReturnInvoice]
        ([SupplierReturnInvoiceID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierReturnInvoiceDetail_tblSupplierReturnInvoice'
CREATE INDEX [IX_FK_tblSupplierReturnInvoiceDetail_tblSupplierReturnInvoice]
ON [dbo].[tblSupplierReturnInvoiceDetail]
    ([SupplierReturnInvoiceID]);
GO

-- Creating foreign key on [SupplierReturnInvoiceID] in table 'tblSupplierReturnPayment'
ALTER TABLE [dbo].[tblSupplierReturnPayment]
ADD CONSTRAINT [FK_tblSupplierReturnPayment_tblSupplierReturnInvoice]
    FOREIGN KEY ([SupplierReturnInvoiceID])
    REFERENCES [dbo].[tblSupplierReturnInvoice]
        ([SupplierReturnInvoiceID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierReturnPayment_tblSupplierReturnInvoice'
CREATE INDEX [IX_FK_tblSupplierReturnPayment_tblSupplierReturnInvoice]
ON [dbo].[tblSupplierReturnPayment]
    ([SupplierReturnInvoiceID]);
GO

-- Creating foreign key on [UserID] in table 'tblSupplierReturnPayment'
ALTER TABLE [dbo].[tblSupplierReturnPayment]
ADD CONSTRAINT [FK_tblSupplierReturnPayment_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblSupplierReturnPayment_tblUser'
CREATE INDEX [IX_FK_tblSupplierReturnPayment_tblUser]
ON [dbo].[tblSupplierReturnPayment]
    ([UserID]);
GO

-- Creating foreign key on [UserID] in table 'tblTransaction'
ALTER TABLE [dbo].[tblTransaction]
ADD CONSTRAINT [FK_tblTransaction_tblUser]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[tblUser]
        ([UserID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblTransaction_tblUser'
CREATE INDEX [IX_FK_tblTransaction_tblUser]
ON [dbo].[tblTransaction]
    ([UserID]);
GO

-- Creating foreign key on [UserTypeID] in table 'tblUser'
ALTER TABLE [dbo].[tblUser]
ADD CONSTRAINT [FK_tblUser_tblUserType]
    FOREIGN KEY ([UserTypeID])
    REFERENCES [dbo].[tblUserType]
        ([UserTypeID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tblUser_tblUserType'
CREATE INDEX [IX_FK_tblUser_tblUserType]
ON [dbo].[tblUser]
    ([UserTypeID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------