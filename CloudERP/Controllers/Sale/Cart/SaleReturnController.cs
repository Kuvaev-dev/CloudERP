using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Services.ServiceAccess;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SaleReturnController : Controller
    {
        private readonly ICustomerInvoiceRepository _customerInvoiceRepository;
        private readonly ICustomerReturnInvoiceDetailRepository _customerReturnInvoiceDetailRepository;
        private readonly SessionHelper _sessionHelper;
        private readonly ISaleReturnService _saleReturnService;

        public SaleReturnController(
            ICustomerInvoiceRepository customerInvoiceRepository,
            ICustomerReturnInvoiceDetailRepository customerReturnInvoiceDetailRepository,
            SessionHelper sessionHelper, 
            ISaleReturnService saleReturnService)
        {
            _customerInvoiceRepository = customerInvoiceRepository ?? throw new ArgumentNullException(nameof(ICustomerInvoiceRepository));
            _customerReturnInvoiceDetailRepository = customerReturnInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(ICustomerReturnInvoiceDetailRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _saleReturnService = saleReturnService ?? throw new ArgumentNullException(nameof(ISaleReturnService));
        }

        // GET: SaleReturn
        public ActionResult FindSale()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(new CustomerInvoice());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FindSale(string invoiceID)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                Session["SaleInvoiceNo"] = string.Empty;
                Session["SaleReturnMessage"] = string.Empty;

                var invoiceDetails = _customerReturnInvoiceDetailRepository.GetInvoiceDetails(invoiceID);
                ViewBag.InvoiceDetails = invoiceDetails;

                return View(await _customerInvoiceRepository.GetByInvoiceNoAsync(invoiceID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReturnConfirm(SaleReturnConfirmMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                Session["SaleInvoiceNo"] = string.Empty;
                Session["SaleReturnMessage"] = string.Empty;

                var returnConfirmDto = new SaleReturnConfirm
                {
                    CustomerInvoiceID = model.CustomerInvoiceID,
                    IsPayment = model.IsPayment,
                    ProductIDs = model.ProductReturns.Select(x => x.ProductID).ToList(),
                    ReturnQty = model.ProductReturns.Select(x => x.ReturnQty).ToList()
                };

                var result = await _saleReturnService.ProcessReturnConfirmAsync(
                    returnConfirmDto,
                    _sessionHelper.BranchID,
                    _sessionHelper.CompanyID,
                    _sessionHelper.UserID);

                Session["SaleInvoiceNo"] = result.InvoiceNo;
                Session["SaleReturnMessage"] = result.Message;

                if (result.IsSuccess)
                {
                    return RedirectToAction("AllReturnSalesPendingAmount", "SalePaymentReturn");
                }

                return RedirectToAction("FindSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}