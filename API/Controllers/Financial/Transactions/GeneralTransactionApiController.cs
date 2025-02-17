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

        [HttpPost, Route("save-transaction/{companyId:int}/{branchId:int}/{userId:int}")]
        public async Task<IHttpActionResult> SaveTransaction([FromBody] GeneralTransactionMV model, int companyId, int branchId, int userId)
        {
            if (model == null) return BadRequest("Invalid data.");

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
        [HttpGet, Route("journal/{companyId:int}/{branchId:int}/{fromDate:DateTime}/{toDate:DateTime}")]
        public async Task<IHttpActionResult> GetJournal(int companyId, int branchId, DateTime fromDate, DateTime toDate)
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
        [HttpGet, Route("sub-journal/{id:int}/{companyId:int}/{branchId:int}/{fromDate:DateTime}/{toDate:DateTime}")]
        public async Task<IHttpActionResult> GetSubJournal(int companyId, int id, DateTime fromDate, DateTime toDate)
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
        [HttpGet, Route("accounts/{companyId:int}/{branchId:int}")]
        public async Task<IHttpActionResult> GetAccounts(int companyId, int branchId)
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