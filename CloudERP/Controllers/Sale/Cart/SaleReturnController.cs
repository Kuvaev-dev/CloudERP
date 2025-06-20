﻿using CloudERP.Models;
using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;
using Localization.CloudERP.Messages;

namespace CloudERP.Controllers.Sale.Cart
{
    public class SaleReturnController : Controller
    {
        private readonly ISessionHelper _sessionHelper;
        private readonly IHttpClientHelper _httpClient;

        public SaleReturnController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                HttpContext.Session.SetString("SaleInvoiceNo", string.Empty);
                HttpContext.Session.SetString("SaleReturnMessage", string.Empty);

                var response = await _httpClient.GetAsync<FindSaleResponse>(
                    $"salereturnapi/findsale?invoiceID={invoiceID}");

                ViewBag.InvoiceDetails = response?.InvoiceDetails;
                return View(response?.Invoice);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                HttpContext.Session.SetString("SaleInvoiceNo", string.Empty);
                HttpContext.Session.SetString("SaleReturnMessage", string.Empty);

                var returnConfirmDto = new SaleReturnConfirm
                {
                    CustomerInvoiceID = model.CustomerInvoiceID,
                    IsPayment = model.IsPayment,
                    ProductIDs = model.ProductReturns.Select(x => x.ProductID).ToList(),
                    ReturnQty = model.ProductReturns.Select(x => x.ReturnQty).ToList(),
                    BranchID = _sessionHelper.BranchID,
                    CompanyID = _sessionHelper.CompanyID,
                    UserID = _sessionHelper.UserID
                };

                var result = await _httpClient.PostAndReturnAsync<SaleReturnConfirmResult>(
                    "salereturnapi/processsalereturn", returnConfirmDto);

                HttpContext.Session.SetString("SaleInvoiceNo", result?.InvoiceNo ?? string.Empty);
                HttpContext.Session.SetString("SaleReturnMessage", result?.Message ?? string.Empty);

                if (result.IsSuccess)
                    return RedirectToAction("AllReturnSalesPendingAmount", "SalePaymentReturn");

                return RedirectToAction("FindSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}