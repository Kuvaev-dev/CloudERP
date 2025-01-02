using CloudERP.Helpers;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.Services;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchaseReturnController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly ISupplierInvoiceRepository _supplierInvoiceRepository;
        private readonly IPurchaseReturnService _purchaseReturnService;

        public PurchaseReturnController(
            ISupplierInvoiceRepository supplierInvoiceRepository, 
            IPurchaseReturnService purchaseReturnService, 
            SessionHelper sessionHelper)
        {
            _supplierInvoiceRepository = supplierInvoiceRepository;
            _purchaseReturnService = purchaseReturnService;
            _sessionHelper = sessionHelper;
        }

        // GET: PurchaseReturn
        public async Task<ActionResult> FindPurchase()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                SupplierInvoice invoice;

                if (_sessionHelper.InvoiceNo != null)
                {
                    if (!string.IsNullOrEmpty(_sessionHelper.InvoiceNo))
                    {
                        invoice = await _supplierInvoiceRepository.GetByInvoiceNoAsync(_sessionHelper.InvoiceNo);
                    }
                    else
                    {
                        invoice = await _supplierInvoiceRepository.GetByIdAsync(0);
                    }
                }
                else
                {
                    invoice = await _supplierInvoiceRepository.GetByIdAsync(0);
                }

                return View(invoice);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> FindPurchase(string invoiceID)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                Session["InvoiceNo"] = string.Empty;
                Session["ReturnMessage"] = string.Empty;

                return View(await _supplierInvoiceRepository.GetByInvoiceNoAsync(invoiceID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ReturnConfirm(PurchaseReturnConfirm returnDto)
        {
            var result = await _purchaseReturnService.ProcessReturnAsync(
                returnDto,
                _sessionHelper.BranchID,
                _sessionHelper.CompanyID,
                _sessionHelper.UserID);

            if (result.Success)
            {
                return RedirectToAction("FindPurchase", new { InvoiceNo = returnDto.SupplierInvoiceID, ReturnMessage = result.Message });
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(returnDto);
            }
        }
    }
}