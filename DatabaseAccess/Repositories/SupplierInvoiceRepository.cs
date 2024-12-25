using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    internal class SupplierInvoiceRepository : ISupplierInvoiceRepository
    {
        private readonly CloudDBEntities _db;

        public SupplierInvoiceRepository(CloudDBEntities db)
        {
            _db = db;
        }

        public async Task AddAsync(tblSupplierInvoice tblSupplierInvoice)
        {
            _db.tblSupplierInvoice.Add(tblSupplierInvoice);
            await _db.SaveChangesAsync();
        }
    }
}
