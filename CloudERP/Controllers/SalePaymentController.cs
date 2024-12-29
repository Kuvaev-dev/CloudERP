using System;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using CloudERP.Helpers;
using CloudERP.Facades;

namespace CloudERP.Controllers
{
    public class SalePaymentController : Controller
    {
        private readonly SalePaymentFacade _salePaymentFacade;
        private readonly SessionHelper _sessionHelper;

        public SalePaymentController(SalePaymentFacade salePaymentFacade, SessionHelper sessionHelper)
        {
            _salePaymentFacade = salePaymentFacade;
            _sessionHelper = sessionHelper;
        }

        // GET: PurchasePayment
        public async Task<ActionResult> RemainingPaymentList()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = await _salePaymentFacade.SaleRepository.RemainingPaymentList(_sessionHelper.CompanyID, _sessionHelper.BranchID);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return View("Error");
            }
        }

        public async Task<ActionResult> PaidHistory(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = await _salePaymentFacade.SaleRepository.SalePaymentHistory(id.Value);
                var returnDetails = await _salePaymentFacade.CustomerReturnInvoiceRepository.GetListByIdAsync((int)id);

                if (returnDetails != null && returnDetails.Count() > 0)
                {
                    ViewData["ReturnSaleDetails"] = returnDetails;
                }

                double remainingAmount = 0;
                double totalInvoiceAmount = await _salePaymentFacade.CustomerInvoiceRepository.GetTotalAmountByIdAsync((int)id);
                double totalPaidAmount = await _salePaymentFacade.CustomerPaymentRepository.GetTotalPaidAmountById((int)id);

                remainingAmount = totalInvoiceAmount - totalPaidAmount;

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PaidAmount(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = await _salePaymentFacade.SaleRepository.SalePaymentHistory(id.Value);
                double remainingAmount = 0;

                foreach (var item in list)
                {
                    remainingAmount = item.RemainingBalance;
                }

                if (remainingAmount == 0)
                {
                    remainingAmount = await _salePaymentFacade.CustomerInvoiceRepository.GetTotalAmountByIdAsync((int)id);
                }

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> PaidAmount(int? id, float previousRemainingAmount, float paidAmount)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                if (paidAmount > previousRemainingAmount)
                {
                    ViewBag.Message = Resources.Messages.PurchasePaymentRemainingAmountError;
                    var list = await _salePaymentFacade.SaleRepository.SalePaymentHistory(id.Value);
                    double remainingAmount = 0;

                    foreach (var item in list)
                    {
                        remainingAmount = item.RemainingBalance;
                    }

                    if (remainingAmount == 0)
                    {
                        remainingAmount = await _salePaymentFacade.CustomerInvoiceRepository.GetTotalAmountByIdAsync((int)id);
                    }

                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;

                    return View(list);
                }

                string payInvoiceNo = "INP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var customerInvoice = await _salePaymentFacade.CustomerInvoiceRepository.GetByIdAsync((int)id);
                var customer = await _salePaymentFacade.CustomerRepository.GetByIdAsync(customerInvoice.CustomerID);
                string message = await _salePaymentFacade.SaleEntry.SalePayment(
                    _sessionHelper.CompanyID, 
                    _sessionHelper.BranchID, 
                    _sessionHelper.UserID, 
                    payInvoiceNo, 
                    Convert.ToString(id), 
                    (float)customerInvoice.TotalAmount,
                    paidAmount, 
                    Convert.ToString(customer.CustomerID), customer.Customername, previousRemainingAmount - paidAmount);

                TempData["Message"] = message;

                return RedirectToAction("RemainingPaymentList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                var list = await _salePaymentFacade.SaleRepository.SalePaymentHistory(id.Value);
                double remainingAmount = 0;

                foreach (var item in list)
                {
                    remainingAmount = item.RemainingBalance;
                }

                if (remainingAmount == 0)
                {
                    remainingAmount = await _salePaymentFacade.CustomerInvoiceRepository.GetTotalAmountByIdAsync((int)id);
                }

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;

                return View(list);
            }
        }

        public async Task<ActionResult> CustomSalesHistory(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = await _salePaymentFacade.SaleRepository.CustomSalesList(_sessionHelper.CompanyID, _sessionHelper.BranchID, FromDate, ToDate);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> SubCustomSalesHistory(DateTime FromDate, DateTime ToDate, int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = await _salePaymentFacade.SaleRepository.CustomSalesList(_sessionHelper.CompanyID, id ?? _sessionHelper.BranchID, FromDate, ToDate);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> SaleItemDetail(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = await _salePaymentFacade.CustomerInvoiceDetailRepository.GetListByIdAsync((int)id);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PrintSaleInvoice(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = await _salePaymentFacade.CustomerInvoiceDetailRepository.GetListByIdAsync((int)id);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}