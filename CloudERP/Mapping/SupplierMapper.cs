using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public class SupplierMapper : BaseMapper<Supplier, SupplierMV>
    {
        public override Supplier MapToDomain(SupplierMV viewModel)
        {
            return new Supplier
            {
                SupplierID = viewModel.SupplierID,
                SupplierName = viewModel.SupplierName,
                SupplierAddress = viewModel.SupplierAddress,
                SupplierConatctNo = viewModel.SupplierConatctNo,
                SupplierEmail = viewModel.SupplierEmail,
                Discription = viewModel.Discription,
                CompanyID = viewModel.CompanyID,
                BranchID = viewModel.BranchID,
                UserID = viewModel.UserID,
                IsActive = viewModel.IsActive
            };
        }

        public override SupplierMV MapToViewModel(Supplier domainModel)
        {
            return new SupplierMV
            {
                SupplierID = domainModel.SupplierID,
                SupplierName = domainModel.SupplierName,
                SupplierAddress = domainModel.SupplierAddress,
                SupplierConatctNo = domainModel.SupplierConatctNo,
                SupplierEmail = domainModel.SupplierEmail,
                Discription = domainModel.Discription,
                CompanyID = domainModel.CompanyID,
                BranchID = domainModel.BranchID,
                UserID = domainModel.UserID,
                IsActive = domainModel.IsActive
            };
        }
    }
}