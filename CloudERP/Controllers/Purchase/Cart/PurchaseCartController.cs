using CloudERP.Helpers;
using Domain.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchaseCartController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public PurchaseCartController(SessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
            _httpClient = new HttpClientHelper();
        }

        public async Task<ActionResult> NewPurchase()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var details = await _httpClient.GetAsync<PurchaseCartDetail[]>(
                    $"details/{_sessionHelper.BranchID}/{_sessionHelper.CompanyID}/{_sessionHelper.UserID}");

                ViewBag.TotalAmount = details.Sum(item => item.PurchaseQuantity * item.PurchaseUnitPrice);
                return View(details);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddItem(int? PID, int? Qty, float? Price)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var newItem = new PurchaseCartDetail
                {
                    ProductID = PID.Value,
                    PurchaseQuantity = Qty.Value,
                    PurchaseUnitPrice = Price.Value,
                    BranchID = _sessionHelper.BranchID,
                    CompanyID = _sessionHelper.CompanyID,
                    UserID = _sessionHelper.UserID
                };

                var success = await _httpClient.PostAsync("additem", newItem);
                if (success) ViewBag.Message = "Item added successfully";

                return RedirectToAction("NewPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirm(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var success = await _httpClient.PostAsync($"delete/{id}", new { });
                if (success) ViewBag.Message = "Deleted successfully";

                return RedirectToAction("NewPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelPurchase()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var success = await _httpClient.PostAsync($"cancel", new
                {
                    branchId = _sessionHelper.BranchID,
                    companyId = _sessionHelper.CompanyID,
                    userId = _sessionHelper.UserID
                });

                if (success) ViewBag.Message = "Purchase canceled";

                return RedirectToAction("NewPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }
    }
}