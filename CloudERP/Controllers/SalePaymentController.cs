using System;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using Domain.RepositoryAccess;
using Domain.EntryAccess;
using CloudERP.Helpers;

namespace CloudERP.Controllers
{
    public class SalePaymentController : Controller
    {
        private readonly ISaleRepository _sale;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerInvoiceRepository _customerInvoiceRepository;
        private readonly ICustomerInvoiceDetailRepository _customerInvoiceDetailRepository;
        private readonly ICustomerReturnInvoiceRepository _customerReturnInvoiceRepository;
        private readonly ICustomerPaymentRepository _customerPaymentRepository;
        private readonly ISaleEntry _saleEntry;
        private readonly SessionHelper _sessionHelper;

        public SalePaymentController(ISaleRepository sale, ISaleEntry saleEntry, ICustomerInvoiceRepository customerInvoiceRepository, ICustomerReturnInvoiceRepository customerReturnInvoiceRepository, ICustomerPaymentRepository customerPaymentRepository, SessionHelper sessionHelper, ICustomerRepository customerRepository, ICustomerInvoiceDetailRepository customerInvoiceDetailRepository)
        {
            _sale = sale;
            _saleEntry = saleEntry;
            _customerInvoiceRepository = customerInvoiceRepository;
            _customerReturnInvoiceRepository = customerReturnInvoiceRepository;
            _sessionHelper = sessionHelper;
            _customerPaymentRepository = customerPaymentRepository;
            _customerRepository = customerRepository;
            _customerInvoiceDetailRepository = customerInvoiceDetailRepository;
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

                var list = await _sale.RemainingPaymentList(_sessionHelper.CompanyID, _sessionHelper.BranchID);

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

                var list = await _sale.SalePaymentHistory(id.Value);
                var returnDetails = await _customerReturnInvoiceRepository.GetListByIdAsync((int)id);

                if (returnDetails != null && returnDetails.Count() > 0)
                {
                    ViewData["ReturnSaleDetails"] = returnDetails;
                }

                double remainingAmount = 0;
                double totalInvoiceAmount = await _customerInvoiceRepository.GetTotalAmountByIdAsync((int)id);
                double totalPaidAmount = await _customerPaymentRepository.GetTotalPaidAmountById((int)id);

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

                var list = await _sale.SalePaymentHistory(id.Value);
                double remainingAmount = 0;

                foreach (var item in list)
                {
                    remainingAmount = item.RemainingBalance;
                }

                if (remainingAmount == 0)
                {
                    remainingAmount = await _customerInvoiceRepository.GetTotalAmountByIdAsync((int)id);
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
                    var list = await _sale.SalePaymentHistory(id.Value);
                    double remainingAmount = 0;

                    foreach (var item in list)
                    {
                        remainingAmount = item.RemainingBalance;
                    }

                    if (remainingAmount == 0)
                    {
                        remainingAmount = await _customerInvoiceRepository.GetTotalAmountByIdAsync((int)id);
                    }

                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;

                    return View(list);
                }

                string payInvoiceNo = "INP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var customerInvoice = await _customerInvoiceRepository.GetByIdAsync((int)id);
                var customer = await _customerRepository.GetByIdAsync(customerInvoice.CustomerID);
                string message = await _saleEntry.SalePayment(
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
                var list = await _sale.SalePaymentHistory(id.Value);
                double remainingAmount = 0;

                foreach (var item in list)
                {
                    remainingAmount = item.RemainingBalance;
                }

                if (remainingAmount == 0)
                {
                    remainingAmount = await _customerInvoiceRepository.GetTotalAmountByIdAsync((int)id);
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

                var list = await _sale.CustomSalesList(_sessionHelper.CompanyID, _sessionHelper.BranchID, FromDate, ToDate);

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

                var list = await _sale.CustomSalesList(_sessionHelper.CompanyID, id ?? _sessionHelper.BranchID, FromDate, ToDate);

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

                var list = await _customerInvoiceDetailRepository.GetListByIdAsync((int)id);

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

                var list = await _customerInvoiceDetailRepository.GetListByIdAsync((int)id);

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