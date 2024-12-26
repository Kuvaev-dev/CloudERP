using CloudERP.Helpers;
using Domain.EntryAccess;
using Domain.RepositoryAccess;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchasePaymentReturnController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly IPurchaseRepository _purchase;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ISupplierReturnPaymentRepository _supplierReturnPaymentRepository;
        private readonly ISupplierReturnInvoiceRepository _supplierReturnInvoiceRepository;
        private readonly IPurchaseEntry _purchaseEntry;

        public PurchasePaymentReturnController(IPurchaseRepository purchase, ISupplierReturnPaymentRepository supplierReturnPaymentRepository, IPurchaseEntry purchaseEntry, SessionHelper sessionHelper, ISupplierReturnInvoiceRepository supplierReturnInvoiceRepository, ISupplierRepository supplierRepository)
        {
            _purchase = purchase;
            _supplierReturnPaymentRepository = supplierReturnPaymentRepository;
            _purchaseEntry = purchaseEntry;
            _sessionHelper = sessionHelper;
            _supplierReturnInvoiceRepository = supplierReturnInvoiceRepository;
            _supplierRepository = supplierRepository;
        }

        // GET: PurchasePaymentReturn
        public async Task<ActionResult> ReturnPurchasePendingAmount(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                return View(await _purchase.PurchaseReturnPaymentPending(id));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> AllPurchasesPendingPayment()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                return View(await _purchase.GetReturnPurchasesPaymentPending(_sessionHelper.CompanyID, _sessionHelper.BranchID));
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
                if (id == null)
                {
                    return RedirectToAction("AllPurchasesPendingPayment");
                }

                var list = await _supplierReturnPaymentRepository.GetBySupplierReturnInvoiceId((int)id);
                double remainingAmount = 0;

                foreach (var item in list)
                {
                    remainingAmount = item.RemainingBalance;
                    if (remainingAmount == 0)
                    {
                        return RedirectToAction("AllPurchasesPendingPayment");
                    }
                }

                if (remainingAmount == 0)
                {
                    remainingAmount = await _supplierReturnInvoiceRepository.GetTotalAmount((int)id);
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
                    var list = await _supplierReturnPaymentRepository.GetBySupplierReturnInvoiceId((int)id);
                    double remainingAmount = 0;

                    foreach (var item in list)
                    {
                        remainingAmount = item.RemainingBalance;
                    }

                    if (remainingAmount == 0)
                    {
                        remainingAmount = await _supplierReturnInvoiceRepository.GetTotalAmount((int)id);
                    }

                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;

                    return View(list);
                }

                string payinvoicenno = "RPP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var supplierID = await _supplierReturnInvoiceRepository.GetSupplierIdByInvoice((int)id);
                var supplier = await _supplierRepository.GetByIdAsync(supplierID);
                var purchaseInvoice = await _supplierReturnInvoiceRepository.GetById((int)id);
                
                Session["Message"] = await _purchaseEntry.ReturnPurchasePayment(_sessionHelper.CompanyID, _sessionHelper.BranchID, _sessionHelper.UserID, payinvoicenno, purchaseInvoice.SupplierInvoiceID.ToString(), purchaseInvoice.SupplierReturnInvoiceID, (float)purchaseInvoice.TotalAmount,
                    paymentAmount, Convert.ToString(supplier?.SupplierID), supplier?.SupplierName, previousRemainingAmount - paymentAmount); ;

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