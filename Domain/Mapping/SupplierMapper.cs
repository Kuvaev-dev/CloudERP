using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;

namespace Domain.Mapping
{
    public class SupplierMapper : BaseMapper<Supplier, tblSupplier>
    {
        public override Supplier MapToDomain(tblSupplier dbModel)
        {
            return new Supplier
            {
                SupplierID = dbModel.SupplierID,
                SupplierName = dbModel.SupplierName,
                SupplierAddress = dbModel.SupplierAddress,
                SupplierConatctNo = dbModel.SupplierConatctNo,
                SupplierEmail = dbModel.SupplierEmail,
                Discription = dbModel.Discription,
                CompanyID = dbModel.CompanyID,
                BranchID = dbModel.BranchID,
                UserID = dbModel.UserID
            };
        }

        public override tblSupplier MapToDatabase(Supplier domainModel)
        {
            return new tblSupplier
            {
                SupplierID = domainModel.SupplierID,
                SupplierName = domainModel.SupplierName,
                SupplierAddress = domainModel.SupplierAddress,
                SupplierConatctNo = domainModel.SupplierConatctNo,
                SupplierEmail = domainModel.SupplierEmail,
                Discription = domainModel.Discription,
                CompanyID = domainModel.CompanyID,
                BranchID = domainModel.BranchID,
                UserID = domainModel.UserID
            };
        }
    }
}
