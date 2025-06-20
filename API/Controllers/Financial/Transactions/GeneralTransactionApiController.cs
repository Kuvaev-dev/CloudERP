﻿using API.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Financial.Transactions
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class GeneralTransactionApiController : ControllerBase
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

        [HttpPost]
        public async Task<ActionResult<string>> SaveTransaction([FromBody] GeneralTransactionMV model, int companyId, int branchId, int userId)
        {
            if (model == null) return BadRequest("Model cannot be null.");

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
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // GET: api/general-transaction/journal
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JournalModel>>> GetJournal(int companyId, int branchId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var journalEntries = await _generalTransactionRepository.GetJournal(companyId, branchId, fromDate, toDate);
                return Ok(journalEntries);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // GET: api/general-transaction/accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AllAccountModel>>> GetAccounts(int companyId, int branchId)
        {
            try
            {
                var accounts = await _generalTransactionRepository.GetAllAccounts(companyId, branchId);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}