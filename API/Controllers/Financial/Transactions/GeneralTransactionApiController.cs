using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using API.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace API.Controllers
{
    [RoutePrefix("api/general-transaction")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class GeneralTransactionApiController : ApiController
    {
        private readonly IGeneralTransactionService _generalTransactionService;
        private readonly IGeneralTransactionRepository _generalTransactionRepository;

        public GeneralTransactionApiController(
            IGeneralTransactionService generalTransactionService,
            IGeneralTransactionRepository generalTransactionRepository)
        {
            _generalTransactionService = generalTransactionService ?? throw new ArgumentNullException(nameof(generalTransactionService));
            _generalTransactionRepository = generalTransactionRepository ?? throw new ArgumentNullException(nameof(generalTransactionRepository));
        }

        [HttpPost, Route("save-transaction?companyId={companyId:int}&branchId={branchId:int}&userId={userId:int}")]
        public async Task<IHttpActionResult> SaveTransaction([FromBody] GeneralTransactionMV model, [FromUri] int companyId, [FromUri] int branchId, [FromUri] int userId)
        {
            if (model == null)
                return BadRequest("Invalid data.");

            try
            {
                var message = await _generalTransactionService.ConfirmTransactionAsync(
                    model.TransferAmount,
                    userId,
                    branchId,
                    companyId,
                    model.DebitAccountControlID,
                    model.CreditAccountControlID,
                    model.Reason);

                return Ok(new { Message = message });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/general-transaction/journal
        [HttpGet, Route("journal?companyId={companyId:int}&branchId={branchId:int}&fromDate={fromDate:DateTime}&toDate={toDate:DateTime}")]
        public async Task<IHttpActionResult> GetJournal([FromUri] int companyId, [FromUri] int branchId, [FromUri] DateTime fromDate, [FromUri] DateTime toDate)
        {
            try
            {
                var journalEntries = await _generalTransactionRepository.GetJournal(companyId, branchId, fromDate, toDate);
                return Ok(journalEntries);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/general-transaction/sub-journal/{id}
        [HttpGet, Route("sub-journal/{id:int}")]
        public async Task<IHttpActionResult> GetSubJournal([FromUri] int companyId, [FromUri] int id, [FromUri] DateTime fromDate, [FromUri] DateTime toDate)
        {
            try
            {
                var subJournalEntries = await _generalTransactionRepository.GetJournal(companyId, id, fromDate, toDate);
                return Ok(subJournalEntries);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/general-transaction/accounts
        [HttpGet, Route("accounts?companyId={companyId:int}&branchId={branchId:int}")]
        public async Task<IHttpActionResult> GetAccounts([FromUri] int companyId, [FromUri] int branchId)
        {
            try
            {
                var accounts = await _generalTransactionRepository.GetAllAccounts(companyId, branchId);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}