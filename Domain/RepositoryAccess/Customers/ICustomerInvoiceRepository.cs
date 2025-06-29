﻿using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface ICustomerInvoiceRepository
    {
        Task AddAsync(CustomerInvoice customerInvoice);
        Task<double?> GetTotalAmountByIdAsync(int id);
        Task<CustomerInvoice> GetByIdAsync(int id);
        Task<CustomerInvoice> GetByInvoiceNoAsync(string invoiceNo);
    }
}
