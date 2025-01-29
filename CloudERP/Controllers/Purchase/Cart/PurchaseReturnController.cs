using CloudERP.Helpers;
using DatabaseAccess.Repositories;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.Services.Purchase;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchaseReturnController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly ISupplierInvoiceRepository _supplierInvoiceRepository;
        private readonly ISupplierReturnInvoiceDetailRepository _supplierReturnInvoiceDetailRepository;
        private readonly IPurchaseReturnService _purchaseReturnService;

        public PurchaseReturnController(
            ISupplierInvoiceRepository supplierInvoiceRepository, 
            IPurchaseReturnService purchaseReturnService,
            ISupplierReturnInvoiceDetailRepository supplierReturnInvoiceDetailRepository,
            SessionHelper sessionHelper)
        {
            _supplierInvoiceRepository = supplierInvoiceRepository ?? throw new ArgumentNullException(nameof(ISupplierInvoiceRepository));
            _purchaseReturnService = purchaseReturnService ?? throw new ArgumentNullException(nameof(IPurchaseReturnService));
            _supplierReturnInvoiceDetailRepository = supplierReturnInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(ISupplierReturnInvoiceDetailRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: PurchaseReturn
        public ActionResult FindPurchase()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(new SupplierInvoice());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FindPurchase(string invoiceID)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                Session["InvoiceNo"] = string.Empty;
                Session["ReturnMessage"] = string.Empty;

                var invoiceDetails = _supplierReturnInvoiceDetailRepository.GetInvoiceDetails(invoiceID);
                ViewBag.InvoiceDetails = invoiceDetails;

                return View(await _supplierInvoiceRepository.GetByInvoiceNoAsync(invoiceID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReturnConfirm(PurchaseReturnConfirm returnDto)
        {
            var result = await _purchaseReturnService.ProcessReturnAsync(
                returnDto,
                _sessionHelper.BranchID,
                _sessionHelper.CompanyID,
                _sessionHelper.UserID);

            if (result.IsSuccess)
            {
                return RedirectToAction("FindPurchase", new { InvoiceNo = returnDto.SupplierInvoiceID, ReturnMessage = result.Value });
            }
            else
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(returnDto);
            }
        }
    }
}