using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;

namespace Domain.Mapping
{
    public class SupportTicketMapper : IMapper<SupportTicket, tblSupportTicket>
    {
        public SupportTicket MapToDomain(tblSupportTicket dbModel)
        {
            return new SupportTicket
            {
                TicketID = dbModel.TicketID,
                Subject = dbModel.Subject,
                Name = dbModel.tblUser.FullName,
                Email = dbModel.Email,
                Message = dbModel.Message,
                DateCreated = dbModel.DateCreated,
                IsResolved = dbModel.IsResolved,
                CompanyID = dbModel.CompanyID,
                BranchID = dbModel.BranchID,
                UserID = dbModel.UserID
            };
        }

        public tblSupportTicket MapToDatabase(SupportTicket domainModel)
        {
            return new tblSupportTicket
            {
                TicketID = domainModel.TicketID,
                Name = domainModel.Name,
                Subject = domainModel.Subject,
                Email = domainModel.Email,
                Message = domainModel.Message,
                DateCreated = domainModel.DateCreated,
                IsResolved = domainModel.IsResolved,
                CompanyID = domainModel.CompanyID,
                BranchID = domainModel.BranchID,
                UserID = domainModel.UserID
            };
        }
    }
}
