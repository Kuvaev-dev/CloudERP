using CloudERP.Helpers;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.Services;
using System;
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
                var findDetail = await _purchaseCartDetailRepository.GetByDefaultSettingsAsync(
                    _sessionHelper.BranchID, _sessionHelper.CompanyID, _sessionHelper.UserID);

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
        public async Task<JsonResult> GetProductDetails(int? id)
        {
            try
            {
                var product = await _stockRepository.GetByIdAsync(id.Value);
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
        public async Task<ActionResult> AddItem(int? PID, int? Qty, float? Price)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var checkQty = await _stockRepository.GetByIdAsync(PID.Value);
                if (Qty > checkQty.Quantity)
                {
                    ViewBag.Message = Resources.Messages.SaleQuantityError;
                    return RedirectToAction("NewSale");
                }

                var findDetail = await _purchaseCartDetailRepository.GetByProductIdAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID, PID.Value);

                if (findDetail == null)
                {
                    if (PID > 0 && Qty > 0 && Price > 0)
                    {
                        var newItem = new PurchaseCartDetail()
                        {
                            ProductID = PID.Value,
                            PurchaseQuantity = Qty.Value,
                            PurchaseUnitPrice = Price.Value,
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

        // GET: PurchaseCart/DeleteConfirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirm(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var product = await _purchaseCartDetailRepository.GetByIdAsync(id.Value);
                if (product != null)
                {
                    await _purchaseCartDetailRepository.DeleteAsync(product);
                    ViewBag.Message = Resources.Messages.DeletedSuccessfully;
                    return RedirectToAction("NewPurchase");
                }

                ViewBag.Message = Resources.Messages.UnexpectedIssue;

                return RedirectToAction("NewPurchase");
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
                var purchaseDetails = await _purchaseCartDetailRepository.GetByDefaultSettingsAsync(
                    _sessionHelper.BranchID, _sessionHelper.CompanyID, _sessionHelper.UserID);
                await _purchaseCartDetailRepository.DeleteListAsync(purchaseDetails);

                ViewBag.Message = Resources.Messages.PurchaseIsCanceled;

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
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                Session["ErrorMessagePurchase"] = string.Empty;

                var purchaseDetails = await _purchaseCartDetailRepository.GetByBranchAndCompanyAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID);
                if (purchaseDetails == null)
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
        public async Task<ActionResult> PurchaseConfirm(PurchaseConfirm purchaseConfirmDto)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var result = await _purchaseCartService.ConfirmPurchaseAsync(
                    purchaseConfirmDto, 
                    _sessionHelper.CompanyID, 
                    _sessionHelper.BranchID, 
                    _sessionHelper.UserID);

                if (result.IsSuccess)
                {
                    return RedirectToAction("PrintPurchaseInvoice", "PurchasePayment", new { id = result.Value });
                }

                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("NewPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }
    }
}