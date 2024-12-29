using CloudERP.Facades;
using CloudERP.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SalePaymentReturnController : Controller
    {
        private readonly SalePaymentReturnFacade _salePaymentReturnFacade;
        private readonly SessionHelper _sessionHelper;

        public SalePaymentReturnController(SalePaymentReturnFacade salePaymentReturnFacade, SessionHelper sessionHelper)
        {
            _salePaymentReturnFacade = salePaymentReturnFacade;
            _sessionHelper = sessionHelper;
        }

        // GET: SalePaymentReturn
        public async Task<ActionResult> ReturnSalePendingAmount()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = await _salePaymentReturnFacade.SaleRepository.GetReturnSaleAmountPending(_sessionHelper.CompanyID, _sessionHelper.BranchID);

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> AllReturnSalesPendingAmount()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = await _salePaymentReturnFacade.SaleRepository.GetReturnSaleAmountPending(_sessionHelper.CompanyID, _sessionHelper.BranchID);

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> ReturnAmount(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = await _salePaymentReturnFacade.CustomerReturnPaymentRepository.GetListByReturnInvoiceIdAsync((int)id);

                double remainingAmount = list.Sum(item => item.RemainingBalance);
                if (remainingAmount == 0)
                {
                    return RedirectToAction("AllReturnSalesPendingAmount");
                }

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ReturnAmount(int? id, float previousRemainingAmount, float paymentAmount)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                if (paymentAmount > previousRemainingAmount)
                {
                    ViewBag.Message = Resources.Messages.PurchasePaymentRemainingAmountError;
                    var list = await _salePaymentReturnFacade.CustomerReturnPaymentRepository.GetListByReturnInvoiceIdAsync((int)id);
                    double remainingAmount = list.Sum(item => item.RemainingBalance);

                    if (remainingAmount == 0)
                    {
                        remainingAmount = await _salePaymentReturnFacade.CustomerReturnInvoiceRepository.GetTotalAmountByIdAsync((int)id);
                    }

                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;

                    return View(list);
                }

                string payInvoiceNo = "RIP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var customerFromReturnInvoice = await _salePaymentReturnFacade.CustomerReturnInvoiceRepository.GetByIdAsync((int)id);
                var customer = await _salePaymentReturnFacade.CustomerRepository.GetByIdAsync(customerFromReturnInvoice.CustomerID);

                string message = await _salePaymentReturnFacade.SaleEntry.ReturnSalePayment(
                    _sessionHelper.CompanyID, 
                    _sessionHelper.BranchID, 
                    _sessionHelper.UserID, 
                    payInvoiceNo, 
                    customerFromReturnInvoice.CustomerInvoiceID.ToString(), 
                    customerFromReturnInvoice.CustomerReturnInvoiceID, 
                    (float)customerFromReturnInvoice.TotalAmount,
                    paymentAmount, 
                    customer.CustomerID.ToString(), 
                    customer.Customername, 
                    previousRemainingAmount - paymentAmount);

                Session["SaleMessage"] = message;

                return RedirectToAction("PurchasePaymentReturn", new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}