using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface ISupplierInvoiceDetailRepository
    {
        Task AddAsync(tblSupplierInvoiceDetail tblSupplierInvoiceDetail);
    }

    internal class SupplierInvoiceDetailRepository : ISupplierInvoiceDetailRepository
    {
        private readonly CloudDBEntities _db;

        public SupplierInvoiceDetailRepository(CloudDBEntities db)
        {
            _db = db;
        }

        public async Task AddAsync(tblSupplierInvoiceDetail tblSupplierInvoiceDetail)
        {
            _db.tblSupplierInvoiceDetail.Add(tblSupplierInvoiceDetail);
            await _db.SaveChangesAsync();
        }
    }
}
