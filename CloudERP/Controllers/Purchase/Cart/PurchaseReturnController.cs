﻿using CloudERP.Models;
using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;
using Localization.CloudERP.Messages;

namespace CloudERP.Controllers.Purchase.Cart
{
    public class PurchaseReturnController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;

        public PurchaseReturnController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

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
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                HttpContext.Session.SetString("InvoiceNo", string.Empty);
                HttpContext.Session.SetString("ReturnMessage", string.Empty);

                var response = await _httpClient.GetAsync<FindPuchaseResponse>(
                    $"purchasereturnapi/findpurchase?invoiceID={invoiceID}");

                ViewBag.InvoiceDetails = response.InvoiceDetails;
                return View(response.Invoice);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                HttpContext.Session.SetString("PurchaseInvoiceNo", string.Empty);
                HttpContext.Session.SetString("PurchaseReturnMessage", string.Empty);

                var returnConfirmDto = new PurchaseReturnConfirm
                {
                    SupplierInvoiceID = model.SupplierInvoiceID,
                    IsPayment = model.IsPayment,
                    ProductIDs = model.ProductReturns.Select(x => x.ProductID).ToList(),
                    ReturnQty = model.ProductReturns.Select(x => x.ReturnQty).ToList(),
                    BranchID = _sessionHelper.BranchID,
                    CompanyID = _sessionHelper.CompanyID,
                    UserID = _sessionHelper.UserID
                };

                var result = await _httpClient.PostAndReturnAsync<PurchaseReturnConfirmResult>
                    ("purchasereturnapi/processpurchasereturn", returnConfirmDto);
                
                HttpContext.Session.SetString("PurchaseInvoiceNo", result?.InvoiceNo ?? string.Empty);
                HttpContext.Session.SetString("PurchaseReturnMessage", result?.Message ?? string.Empty);

                if (result.IsSuccess) 
                    return RedirectToAction("AllPurchasesPendingPayment", "PurchasePaymentReturn");

                return RedirectToAction("FindPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}