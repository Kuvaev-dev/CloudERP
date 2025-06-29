﻿using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface ISaleCartService
    {
        Task<Result<int>> ConfirmSaleAsync(SaleConfirm saleConfirmDto);
    }
}
