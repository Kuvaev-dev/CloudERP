using Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface IEmployeeStatisticsService
    {
        Task<IEnumerable<EmployeeStatistics>> GetStatisticsAsync(DateTime startDate, DateTime endDate, int branchID, int companyID);
    }
}
