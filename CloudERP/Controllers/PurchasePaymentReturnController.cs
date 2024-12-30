using CloudERP.Facades;
using CloudERP.Helpers;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchasePaymentReturnController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly PurchasePaymentReturnFacade _purchasePaymentReturnFacade;

        public PurchasePaymentReturnController(PurchasePaymentReturnFacade purchasePaymentReturnFacade, SessionHelper sessionHelper)
        {
            _purchasePaymentReturnFacade = purchasePaymentReturnFacade;
            _sessionHelper = sessionHelper;
        }

        // GET: PurchasePaymentReturn
        public async Task<ActionResult> ReturnPurchasePendingAmount(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(await _purchasePaymentReturnFacade.PurchaseRepository.PurchaseReturnPaymentPending(id));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> AllPurchasesPendingPayment()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(await _purchasePaymentReturnFacade.PurchaseRepository.GetReturnPurchasesPaymentPending(_sessionHelper.CompanyID, _sessionHelper.BranchID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> ReturnAmount(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (id == null)
                {
                    return RedirectToAction("AllPurchasesPendingPayment");
                }

                var list = await _purchasePaymentReturnFacade.SupplierReturnPaymentRepository.GetBySupplierReturnInvoiceId((int)id);
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
                    remainingAmount = await _purchasePaymentReturnFacade.SupplierReturnInvoiceRepository.GetTotalAmount((int)id);
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
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (paymentAmount > previousRemainingAmount)
                {
                    ViewBag.Message = Resources.Messages.PurchasePaymentRemainingAmountError;
                    var list = await _purchasePaymentReturnFacade.SupplierReturnPaymentRepository.GetBySupplierReturnInvoiceId((int)id);
                    double remainingAmount = 0;

                    foreach (var item in list)
                    {
                        remainingAmount = item.RemainingBalance;
                    }

                    if (remainingAmount == 0)
                    {
                        remainingAmount = await _purchasePaymentReturnFacade.SupplierReturnInvoiceRepository.GetTotalAmount((int)id);
                    }

                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;

                    return View(list);
                }

                string payinvoicenno = "RPP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var supplierID = await _purchasePaymentReturnFacade.SupplierReturnInvoiceRepository.GetSupplierIdByInvoice((int)id);
                var supplier = await _purchasePaymentReturnFacade.SupplierRepository.GetByIdAsync(supplierID);
                var purchaseInvoice = await _purchasePaymentReturnFacade.SupplierReturnInvoiceRepository.GetById((int)id);
                
                Session["Message"] = await _purchasePaymentReturnFacade.PurchaseEntry.ReturnPurchasePayment(_sessionHelper.CompanyID, _sessionHelper.BranchID, _sessionHelper.UserID, payinvoicenno, purchaseInvoice.SupplierInvoiceID.ToString(), purchaseInvoice.SupplierReturnInvoiceID, (float)purchaseInvoice.TotalAmount,
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