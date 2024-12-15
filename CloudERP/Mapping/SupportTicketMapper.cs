using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public class SupportTicketMapper : BaseMapper<SupportTicket, SupportTicketMV>
    {
        public override SupportTicket MapToDomain(SupportTicketMV viewModel)
        {
            return new SupportTicket
            {
                TicketID = viewModel.TicketID,
                Subject = viewModel.Subject,
                Email = viewModel.Email,
                Message = viewModel.Message,
                DateCreated = viewModel.DateCreated,
                IsResolved = viewModel.IsResolved
            };
        }

        public override SupportTicketMV MapToViewModel(SupportTicket domainModel)
        {
            return new SupportTicketMV
            {
                TicketID = domainModel.TicketID,
                Name = domainModel.Name,
                Email = domainModel.Email,
                Message = domainModel.Message,
                DateCreated = domainModel.DateCreated,
                IsResolved = domainModel.IsResolved
            };
        }
    }
}