using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface ISupplierReturnInvoiceRepository
    {
        Task<List<tblSupplierReturnInvoice>> GetListById(int id); 
    }

    internal class SupplierReturnInvoiceRepository : ISupplierReturnInvoiceRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SupplierReturnInvoiceRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<tblSupplierReturnInvoice>> GetListById(int id)
        {
            return await _dbContext.tblSupplierReturnInvoice.Where(r => r.SupplierInvoiceID == id).ToListAsync();
        }
    }
}
