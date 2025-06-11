using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;
using Localization.CloudERP.Messages;

namespace CloudERP.Controllers.Sale.Cart
{
    public class SaleCartController : Controller
    {
        private readonly ISessionHelper _sessionHelper;
        private readonly IHttpClientHelper _httpClient;

        public SaleCartController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        // GET: SaleCart/NewSale
        public async Task<ActionResult> NewSale()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var findDetail = await _httpClient.GetAsync<IEnumerable<SaleCartDetail>>(
                    $"salecartapi/getsalecartdetails" +
                    $"?branchId={_sessionHelper.BranchID}&companyId={_sessionHelper.CompanyID}&userId={_sessionHelper.UserID}");

                ViewBag.TotalAmount = findDetail?.Sum(item => item.SaleQuantity * item.SaleUnitPrice);
                ViewBag.Products = await _httpClient.GetAsync<List<Domain.Models.Stock>>(
                    $"stockapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");

                return View(findDetail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<IActionResult> GetProductDetails(int id)
        {
            try
            {
                var product = await _httpClient.GetAsync<Domain.Models.Stock>(
                    $"salecartapi/getproductdetails?id={id}");

                return Json(new { data = product?.SaleUnitPrice });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                var checkQty = await _httpClient.GetAsync<Domain.Models.Stock>($"stockapi/getbyid?id={PID}");

                if (Qty > checkQty?.Quantity)
                {
                    ViewBag.Message = Messages.SaleQuantityError;
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

                await _httpClient.PostAsync("salecartapi/additem", newItem);

                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                var success = await _httpClient.DeleteAsync($"salecartapi/deleteitem?id={id}");
                if (success) ViewBag.ErrorMessage = Messages.DeletedSuccessfully;
                else ViewBag.ErrorMessage = Messages.UnexpectedErrorMessage;

                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> SelectCustomer()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(await _httpClient.GetAsync<IEnumerable<Customer>>(
                    $"customerapi/getbysetting?branchId={_sessionHelper.BranchID}&companyId={_sessionHelper.CompanyID}"));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("NewPurchase");
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
                var responseMessage = await _httpClient.PostAsync(
                    $"salecartapi/cancelsale" +
                    $"?branchId={_sessionHelper.BranchID}&companyId={_sessionHelper.CompanyID}&userId={_sessionHelper.UserID}",
                    new { });

                ViewBag.Message = responseMessage;

                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                saleConfirmDto.CompanyID = _sessionHelper.CompanyID;
                saleConfirmDto.BranchID = _sessionHelper.BranchID;
                saleConfirmDto.UserID = _sessionHelper.UserID;

                var result = await _httpClient.PostAndReturnAsync<Dictionary<string, int>>(
                    "salecartapi/confirmsale", saleConfirmDto);

                if (result != null && result.TryGetValue("invoiceId", out int invoiceId) && invoiceId > 0)
                {
                    return RedirectToAction("PrintSaleInvoice", "SalePayment", new { id = invoiceId });
                }

                ViewBag.ErrorMessage = Messages.UnexpectedErrorMessage;
                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}