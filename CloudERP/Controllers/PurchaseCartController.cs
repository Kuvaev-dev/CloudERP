using CloudERP.Helpers;
using CloudERP.Models;
using DatabaseAccess;
using DatabaseAccess.Code;
using DatabaseAccess.Repositories;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchaseCartController : Controller
    {
        private readonly IPurchaseEntry _purchaseEntry;
        private readonly IStockRepository _stockRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IPurchaseCartDetailRepository _purchaseCartDetailRepository;
        private readonly ISupplierInvoiceRepository _supplierInvoiceRepository;
        private readonly ISupplierInvoiceDetailRepository _supplierInvoiceDetailRepository;
        private readonly SessionHelper _sessionHelper;

        public PurchaseCartController(IPurchaseEntry purchaseEntry, IStockRepository stockRepository, ISupplierRepository supplierRepository, IPurchaseCartDetailRepository purchaseCartDetailRepository, ISupplierInvoiceRepository supplierInvoiceRepository, ISupplierInvoiceDetailRepository supplierInvoiceDetailRepository, SessionHelper sessionHelper)
        {
            _purchaseEntry = purchaseEntry;
            _stockRepository = stockRepository;
            _supplierRepository = supplierRepository;
            _purchaseCartDetailRepository = purchaseCartDetailRepository;
            _supplierInvoiceRepository = supplierInvoiceRepository;
            _supplierInvoiceDetailRepository = supplierInvoiceDetailRepository;
            _sessionHelper = sessionHelper;
        }

        // GET: PurchaseCart/NewPurchase
        public async Task<ActionResult> NewPurchase()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

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
        public async Task<ActionResult> AddItem(int PID, int Qty, float Price)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                if (await _purchaseCartDetailRepository.GetByProductIdAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID, PID) == null)
                {
                    if (PID > 0 && Qty > 0 && Price > 0)
                    {
                        var newItem = new tblPurchaseCartDetail()
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
        public async Task<ActionResult> GetProduct()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return Json(new { data = new List<ProductMV>() }, JsonRequestBehavior.AllowGet);
                }

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
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

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
        public async Task<ActionResult> CancelPurchase()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

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
            try
            {
                Session["ErrorMessagePurchase"] = string.Empty;

                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

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
        public async Task<ActionResult> PurchaseConfirm(FormCollection collection)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                int supplierID = 0;
                bool IsPayment = false;
                string[] keys = collection.AllKeys;
                foreach (var name in keys)
                {
                    if (name.Contains("name"))
                    {
                        string idName = name;
                        string[] valueIDs = idName.Split(' ');
                        supplierID = Convert.ToInt32(valueIDs[1]);
                    }
                }

                string Description = string.Empty;
                string[] DescriptionList = collection["item.Description"].Split(',');
                if (DescriptionList != null && DescriptionList.Length > 0)
                {
                    Description = DescriptionList[0];
                }

                if (collection["IsPayment"] != null)
                {
                    string[] isPaymentDirCet = collection["IsPayment"].Split(',');
                    if (isPaymentDirCet[0] == "on")
                    {
                        IsPayment = true;
                    }
                }

                var supplier = await _supplierRepository.GetByIdAsync(supplierID);
                var purchaseDetails = await _purchaseCartDetailRepository.GetByBranchAndCompanyAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID);

                double totalAmount = purchaseDetails.Sum(item => item.PurchaseQuantity * item.PurchaseUnitPrice);
                if (totalAmount == 0)
                {
                    ViewBag.Message = Resources.Messages.PurchaseCartIsEmpty;
                    return RedirectToAction("NewPurchase");
                }

                string invoiceNo = "PUR" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var invoiceHeader = new tblSupplierInvoice()
                {
                    BranchID = _sessionHelper.BranchID,
                    CompanyID = _sessionHelper.CompanyID,
                    Description = Description,
                    InvoiceDate = DateTime.Now,
                    InvoiceNo = invoiceNo,
                    SupplierID = supplierID,
                    UserID = _sessionHelper.UserID,
                    TotalAmount = totalAmount
                };
                await _supplierInvoiceRepository.AddAsync(invoiceHeader);

                foreach (var item in purchaseDetails)
                {
                    var newPurchaseDetails = new tblSupplierInvoiceDetail()
                    {
                        ProductID = item.ProductID,
                        PurchaseQuantity = item.PurchaseQuantity,
                        PurchaseUnitPrice = item.PurchaseUnitPrice,
                        SupplierInvoiceID = invoiceHeader.SupplierInvoiceID
                    };
                    await _supplierInvoiceDetailRepository.AddAsync(newPurchaseDetails);
                }

                string Message = await _purchaseEntry.ConfirmPurchase(_sessionHelper.CompanyID, _sessionHelper.BranchID, _sessionHelper.UserID, invoiceNo, invoiceHeader.SupplierInvoiceID.ToString(), (float)totalAmount, supplierID.ToString(), supplier.SupplierName, IsPayment);

                if (Message.Contains("Success"))
                {
                    foreach (var item in purchaseDetails)
                    {
                        var stockItem = await _stockRepository.GetByIdAsync(item.ProductID);
                        if (stockItem != null)
                        {
                            stockItem.CurrentPurchaseUnitPrice = item.PurchaseUnitPrice;
                            stockItem.Quantity += item.PurchaseQuantity;
                            await _stockRepository.UpdateAsync(stockItem);
                        }
                        await _purchaseCartDetailRepository.DeleteAsync(item);
                    }
                    return RedirectToAction("PrintPurchaseInvoice", "PurchasePayment", new { id = invoiceHeader.SupplierInvoiceID });
                }

                Session["Message"] = Message;

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