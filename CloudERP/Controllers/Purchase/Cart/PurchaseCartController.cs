using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.Services.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchaseCartController : Controller
    {
        private readonly IPurchaseCartDetailRepository _purchaseCartDetailRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly SessionHelper _sessionHelper;
        private readonly IPurchaseCartService _purchaseCartService;

        public PurchaseCartController(
            IPurchaseCartDetailRepository purchaseCartDetailRepository,
            IStockRepository stockRepository, 
            ISupplierRepository supplierRepository, 
            SessionHelper sessionHelper, 
            IPurchaseCartService purchaseCartService)
        {
            _purchaseCartDetailRepository = purchaseCartDetailRepository ?? throw new ArgumentNullException(nameof(IPurchaseCartDetailRepository));
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(IStockRepository));
            _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(ISupplierRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _purchaseCartService = purchaseCartService ?? throw new ArgumentNullException(nameof(IPurchaseCartService));
        }

        // GET: PurchaseCart/NewPurchase
        public async Task<ActionResult> NewPurchase()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var findDetail = await _purchaseCartDetailRepository.GetByDefaultSettingsAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID, _sessionHelper.UserID);

                ViewBag.TotalAmount = findDetail.Sum(item => item.PurchaseQuantity * item.PurchaseUnitPrice);

                ViewBag.Products = await _stockRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);

                return View(findDetail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetProductDetails(int id)
        {
            try
            {
                var product = await _stockRepository.GetByIdAsync(id);
                if (product != null)
                {
                    return Json(new { product.CurrentPurchaseUnitPrice }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { CurrentPurchaseUnitPrice = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return Json(new { CurrentPurchaseUnitPrice = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: PurchaseCart/AddItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddItem(int PID, int Qty, float Price)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (await _purchaseCartDetailRepository.GetByProductIdAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID, PID) == null)
                {
                    if (PID > 0 && Qty > 0 && Price > 0)
                    {
                        var newItem = new PurchaseCartDetail()
                        {
                            ProductID = PID,
                            PurchaseQuantity = Qty,
                            PurchaseUnitPrice = Price,
                            BranchID = _sessionHelper.BranchID,
                            CompanyID = _sessionHelper.CompanyID,
                            UserID = _sessionHelper.UserID
                        };

                        await _purchaseCartDetailRepository.AddAsync(newItem);
                        ViewBag.Message = Resources.Messages.ItemAddedSuccessfully;
                    }
                }
                else
                {
                    ViewBag.Message = Resources.Messages.AlreadyExists;
                }

                return RedirectToAction("NewPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }

        // POST: PurchaseCart/GetProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetProduct()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                    return Json(new { data = new List<ProductMV>() }, JsonRequestBehavior.AllowGet);

                var products = await _stockRepository.GetAllAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID);

                return Json(new { data = products.Select(item => new ProductMV { Name = item.ProductName, ProductID = item.ProductID }).ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return Json(new { data = new List<ProductMV>() }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: PurchaseCart/DeleteConfirm
        public async Task<ActionResult> DeleteConfirm(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var product = await _purchaseCartDetailRepository.GetByIdAsync(id);
                if (product != null)
                {
                    await _purchaseCartDetailRepository.UpdateAsync(product);
                    ViewBag.Message = Resources.Messages.DeletedSuccessfully;
                    return RedirectToAction("NewPurchase");
                }

                ViewBag.Message = Resources.Messages.UnexpectedIssue;

                return View(await _purchaseCartDetailRepository.GetByDefaultSettingsAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID, _sessionHelper.UserID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }

        // POST: PurchaseCart/CancelPurchase
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelPurchase()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (await _purchaseCartDetailRepository.IsCanceled(_sessionHelper.BranchID, _sessionHelper.CompanyID, _sessionHelper.UserID))
                {
                    ViewBag.Message = Resources.Messages.PurchaseIsCanceled;
                }
                else
                {
                    ViewBag.Message = Resources.Messages.UnexpectedIssue;
                }

                return RedirectToAction("NewPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }

        // GET: PurchaseCart/SelectSupplier
        public async Task<ActionResult> SelectSupplier()
        {
            Session["ErrorMessagePurchase"] = string.Empty;

            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (await _purchaseCartDetailRepository.GetByBranchAndCompanyAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID) == null)
                {
                    Session["ErrorMessagePurchase"] = Resources.Messages.PurchaseCartIsEmpty;
                    return RedirectToAction("NewPurchase");
                }

                return View(await _supplierRepository.GetByCompanyAndBranchAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }

        // POST: PurchaseCart/PurchaseConfirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PurchaseConfirm(FormCollection collection)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var purchaseDto = new Domain.Models.FinancialModels.PurchaseConfirm
                {
                    SupplierId = GetSupplierId(collection),
                    Description = GetDescription(collection),
                    IsPayment = GetIsPayment(collection),
                    PurchaseDetails = await _purchaseCartDetailRepository.GetByBranchAndCompanyAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID)
                };

                string message = await _purchaseCartService.ConfirmPurchase(
                    purchaseDto, 
                    _sessionHelper.CompanyID, 
                    _sessionHelper.BranchID, 
                    _sessionHelper.UserID);

                if (message == Resources.Messages.PurchaseCartIsEmpty)
                {
                    ViewBag.Message = message;
                    return RedirectToAction("NewPurchase");
                }

                return RedirectToAction("PrintPurchaseInvoice", "PurchasePayment", new { id = message });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }

        private int GetSupplierId(FormCollection collection)
        {
            foreach (var name in collection.AllKeys)
            {
                if (name.Contains("name"))
                {
                    string[] valueIDs = name.Split(' ');
                    return Convert.ToInt32(valueIDs[1]);
                }
            }
            return 0;
        }

        private string GetDescription(FormCollection collection)
        {
            string[] descriptionList = collection["item.Description"].Split(',');
            return descriptionList.Length > 0 ? descriptionList[0] : string.Empty;
        }

        private bool GetIsPayment(FormCollection collection)
        {
            return collection["IsPayment"] != null && collection["IsPayment"].Split(',')[0] == "on";
        }
    }
}