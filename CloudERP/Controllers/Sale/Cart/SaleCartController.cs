using CloudERP.Helpers;
using Domain.Models;
using Domain.Models.FinancialModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utils.Helpers;

namespace CloudERP.Controllers
{
    public class SaleCartController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClientHelper _httpClientHelper;

        public SaleCartController(SessionHelper sessionHelper, HttpClientHelper httpClientHelper)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _httpClientHelper = httpClientHelper ?? throw new ArgumentNullException(nameof(httpClientHelper));
        }

        // GET: SaleCart/NewSale
        public async Task<ActionResult> NewSale()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var findDetail = await _httpClientHelper.GetAsync<List<SaleCartDetail>>(
                    $"salecart/newSaleDetails?branchId={_sessionHelper.BranchID}&companyId={_sessionHelper.CompanyID}&userId={_sessionHelper.UserID}");

                ViewBag.TotalAmount = findDetail.Sum(item => item.SaleQuantity * item.SaleUnitPrice);
                ViewBag.Products = await _httpClientHelper.GetAsync<List<Stock>>(
                    $"stock/products?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");

                return View(findDetail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<JsonResult> GetProductDetails(int id)
        {
            try
            {
                var product = await _httpClientHelper.GetAsync<Stock>(
                    $"salecart/productDetails/{id}");

                return Json(new { data = product.SaleUnitPrice }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return Json(new { error = "Product details fetching error" });
            }
        }

        // POST: SaleCart/AddItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddItem(int PID, int Qty, float Price)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var checkQty = await _httpClientHelper.GetAsync<Stock>(
                    $"stock/products/{PID}");

                if (Qty > checkQty.Quantity)
                {
                    ViewBag.Message = "Insufficient quantity available.";
                    return RedirectToAction("NewSale");
                }

                var newItem = new SaleCartDetail()
                {
                    ProductID = PID,
                    SaleQuantity = Qty,
                    SaleUnitPrice = Price,
                    BranchID = _sessionHelper.BranchID,
                    CompanyID = _sessionHelper.CompanyID,
                    UserID = _sessionHelper.UserID
                };

                var responseMessage = await _httpClientHelper.PostAsync<string>(
                    "salecart/addItem", newItem);

                ViewBag.Message = responseMessage;

                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: SaleCart/DeleteConfirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirm(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var responseMessage = await _httpClientHelper.PostAsync<string>(
                    $"salecart/deleteItem/{id}");

                ViewBag.Message = responseMessage;
                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: SaleCart/CancelSale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelSale()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var responseMessage = await _httpClientHelper.PostAsync<string>(
                    $"salecart/cancelSale?branchId={_sessionHelper.BranchID}&companyId={_sessionHelper.CompanyID}&userId={_sessionHelper.UserID}");

                ViewBag.Message = responseMessage;

                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: SaleCart/SaleConfirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaleConfirm(SaleConfirm saleConfirmDto)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var response = await _httpClientHelper.PostAsync<Result<int>>(
                    "salecart/confirmSale", saleConfirmDto);

                if (response.Success)
                {
                    return RedirectToAction("PrintSaleInvoice", "SalePayment", new { id = response.Value });
                }

                TempData["ErrorMessage"] = response.ErrorMessage;
                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}