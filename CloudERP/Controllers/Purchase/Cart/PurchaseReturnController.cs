using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.Services;
using System;
using System.Linq;
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
        public async Task<ActionResult> ReturnConfirm(PurchaseReturnConfirmVM model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                Session["SaleInvoiceNo"] = string.Empty;
                Session["SaleReturnMessage"] = string.Empty;

                var returnConfirmDto = new PurchaseReturnConfirm
                {
                    SupplierInvoiceID = model.SupplierInvoiceID,
                    IsPayment = model.IsPayment,
                    ProductIDs = model.ProductReturns.Select(x => x.ProductID).ToList(),
                    ReturnQty = model.ProductReturns.Select(x => x.ReturnQty).ToList()
                };

                var result = await _purchaseReturnService.ProcessReturnAsync(
                    returnConfirmDto,
                    _sessionHelper.BranchID,
                    _sessionHelper.CompanyID,
                    _sessionHelper.UserID);

                Session["SaleInvoiceNo"] = result.InvoiceNo;
                Session["SaleReturnMessage"] = result.Message;

                if (result.IsSuccess)
                {
                    return RedirectToAction("AllPurchasesPendingPayment", "PurchasePaymentReturn");
                }

                return RedirectToAction("FindPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}