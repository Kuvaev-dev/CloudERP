using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.Services.Sale;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SaleReturnController : Controller
    {
        private readonly ICustomerInvoiceRepository _customerInvoiceRepository;
        private readonly SessionHelper _sessionHelper;
        private readonly ISaleReturnService _saleReturnService;

        public SaleReturnController(
            ICustomerInvoiceRepository customerInvoiceRepository, 
            SessionHelper sessionHelper, 
            ISaleReturnService saleReturnService)
        {
            _customerInvoiceRepository = customerInvoiceRepository ?? throw new ArgumentNullException(nameof(ICustomerInvoiceRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _saleReturnService = saleReturnService ?? throw new ArgumentNullException(nameof(ISaleReturnService));
        }

        // GET: SaleReturn
        public async Task<ActionResult> FindSale()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                CustomerInvoice invoice;

                if (Session["SaleInvoiceNo"] != null)
                {
                    if (!string.IsNullOrEmpty(_sessionHelper.SaleInvoiceNo))
                    {
                        invoice = await _customerInvoiceRepository.GetByInvoiceNoAsync(_sessionHelper.SaleInvoiceNo);
                    }
                    else
                    {
                        invoice = await _customerInvoiceRepository.GetByIdAsync(0);
                    }
                }
                else
                {
                    invoice = await _customerInvoiceRepository.GetByIdAsync(0);
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FindSale(string invoiceID)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                Session["SaleInvoiceNo"] = string.Empty;
                Session["SaleReturnMessage"] = string.Empty;

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
        public async Task<ActionResult> ReturnConfirm(ReturnConfirmVM model)
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
                    return RedirectToAction("FindSale");
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