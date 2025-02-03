using System;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using CloudERP.Helpers;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.Services.Sale;

namespace CloudERP.Controllers
{
    public class SaleCartController : Controller
    {
        private readonly ISaleCartDetailRepository _saleCartDetailRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ISaleCartService _saleCartService;
        private readonly SessionHelper _sessionHelper;

        public SaleCartController(
            ISaleCartDetailRepository saleCartDetailRepository,
            ICustomerRepository customerRepository,
            IStockRepository stockRepository,
            SessionHelper sessionHelper, 
            ISaleCartService saleCartService)
        {
            _saleCartDetailRepository = saleCartDetailRepository ?? throw new ArgumentNullException(nameof(ISaleCartDetailRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(ICustomerRepository));
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(IStockRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _saleCartService = saleCartService ?? throw new ArgumentNullException(nameof(ISaleCartService));
        }

        // GET: SaleCart/NewSale
        public async Task<ActionResult> NewSale()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var findDetail = await _saleCartDetailRepository.GetByDefaultSettingAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID, _sessionHelper.UserID);

                ViewBag.TotalAmount = findDetail.Sum(item => item.SaleQuantity * item.SaleUnitPrice);

                ViewBag.Products = await _stockRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);

                return View(findDetail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<JsonResult> GetProductDetails(int id)
        {
            try
            {
                var product = await _stockRepository.GetByIdAsync(id);
                if (product != null)
                {
                    return Json(new { data = product.SaleUnitPrice }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { data = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return Json(new { error = Resources.Messages.ProductDetailsFetchingError });
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
                var checkQty = await _stockRepository.GetByIdAsync(PID);
                if (Qty > checkQty.Quantity)
                {
                    ViewBag.Message = Resources.Messages.SaleQuantityError;
                    return RedirectToAction("NewSale");
                }

                var findDetail = await _saleCartDetailRepository.GetByProductIdAsync(PID, _sessionHelper.BranchID, _sessionHelper.CompanyID);

                if (findDetail == null)
                {
                    if (PID > 0 && Qty > 0 && Price > 0)
                    {
                        var newItem = new SaleCartDetail()
                        {
                            ProductID = PID,
                            SaleQuantity = Qty,
                            SaleUnitPrice = Price,
                            BranchID = _sessionHelper.BranchID,
                            CompanyID = _sessionHelper.CompanyID,
                            UserID = _sessionHelper.UserID
                        };
                        await _saleCartDetailRepository.AddAsync(newItem);
                        ViewBag.Message = Resources.Messages.ItemAddedSuccessfully;
                    }
                }
                else
                {
                    ViewBag.Message = Resources.Messages.AlreadyExists;
                }

                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: SaleCart/DeleteConfirm/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirm(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var product = await _saleCartDetailRepository.GetByIdAsync(id);
                if (product != null)
                {
                    await _saleCartDetailRepository.DeleteAsync(id);
                    ViewBag.Message = Resources.Messages.DeletedSuccessfully;
                    return RedirectToAction("NewSale");
                }

                ViewBag.Message = Resources.Messages.ProductNotFound;
                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                var saleDetails = await _saleCartDetailRepository.GetByDefaultSettingAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID, _sessionHelper.UserID);
                await _saleCartDetailRepository.DeleteListAsync(saleDetails);

                ViewBag.Message = Resources.Messages.SaleCanceledSuccessfully;

                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: SaleCart/SelectCustomer
        public async Task<ActionResult> SelectCustomer()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                Session["ErrorMessageSale"] = string.Empty;

                var saleDetails = await _saleCartDetailRepository.GetByCompanyAndBranchAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID);
                if (saleDetails == null)
                {
                    Session["ErrorMessageSale"] = Resources.Messages.SaleCartEmpty;

                    return RedirectToAction("NewSale");
                }

                return View(await _customerRepository.GetByCompanyAndBranchAsync(_sessionHelper.CompanyID, _sessionHelper.CompanyID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                var result = await _saleCartService.ConfirmSaleAsync(
                    saleConfirmDto,
                    _sessionHelper.BranchID,
                    _sessionHelper.CompanyID,
                    _sessionHelper.UserID);

                if (result.IsSuccess)
                {
                    return RedirectToAction("PrintSaleInvoice", "SalePayment", new { id = result.Value });
                }

                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}