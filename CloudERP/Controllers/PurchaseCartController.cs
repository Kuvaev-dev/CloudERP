using CloudERP.Helpers;
using CloudERP.Models;
using DatabaseAccess;
using DatabaseAccess.Code;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchaseCartController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly IPurchaseEntry _purchaseEntry;
        private readonly IStockService _stockService;
        private readonly ISupplierService _supplierService;
        private readonly SessionHelper _sessionHelper;

        public PurchaseCartController(CloudDBEntities db, IPurchaseEntry purchaseEntry, IStockService stockService, ISupplierService supplierService, SessionHelper sessionHelper)
        {
            _db = db;
            _purchaseEntry = purchaseEntry;
            _stockService = stockService;
            _supplierService = supplierService;
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

                var findDetail = _db.tblPurchaseCartDetail.Where(i => i.BranchID == _sessionHelper.BranchID && i.CompanyID == _sessionHelper.CompanyID && i.UserID == _sessionHelper.UserID).ToList();
                double totalAmount = findDetail.Sum(item => item.PurchaseQuantity * item.PurchaseUnitPrice);
                ViewBag.TotalAmount = totalAmount;

                ViewBag.Products = await _stockService.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);

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
                var product = await _stockService.GetByIdAsync(id);
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
        public ActionResult AddItem(int PID, int Qty, float Price)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var findDetail = _db.tblPurchaseCartDetail.FirstOrDefault(i => i.ProductID == PID && i.BranchID == _sessionHelper.BranchID && i.CompanyID == _sessionHelper.CompanyID);
                if (findDetail == null)
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

                        _db.tblPurchaseCartDetail.Add(newItem);
                        _db.SaveChanges();
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
        public ActionResult GetProduct()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return Json(new { data = new List<ProductMV>() }, JsonRequestBehavior.AllowGet);
                }

                List<ProductMV> products = _db.tblStock.Where(p => p.BranchID == _sessionHelper.BranchID && p.CompanyID == _sessionHelper.CompanyID)
                                                        .Select(item => new ProductMV { Name = item.ProductName, ProductID = item.ProductID })
                                                        .ToList();

                return Json(new { data = products }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return Json(new { data = new List<ProductMV>() }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: PurchaseCart/DeleteConfirm
        public ActionResult DeleteConfirm(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var product = _db.tblPurchaseCartDetail.Find(id);
                if (product != null)
                {
                    _db.Entry(product).State = System.Data.Entity.EntityState.Deleted;
                    _db.SaveChanges();
                    ViewBag.Message = Resources.Messages.DeletedSuccessfully;
                    return RedirectToAction("NewPurchase");
                }

                ViewBag.Message = Resources.Messages.UnexpectedIssue;
                var find = _db.tblPurchaseCartDetail.Where(i => i.BranchID == _sessionHelper.BranchID && i.CompanyID == _sessionHelper.CompanyID && i.UserID == _sessionHelper.UserID).ToList();

                return View(find);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }

        // POST: PurchaseCart/CancelPurchase
        [HttpPost]
        public ActionResult CancelPurchase()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = _db.tblPurchaseCartDetail.Where(p => p.BranchID == _sessionHelper.BranchID && p.CompanyID == _sessionHelper.CompanyID && p.UserID == _sessionHelper.UserID).ToList();
                bool cancelstatus = false;

                foreach (var item in list)
                {
                    _db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    int noofrecords = _db.SaveChanges();
                    if (cancelstatus == false && noofrecords > 0)
                    {
                        cancelstatus = true;
                    }
                }

                if (cancelstatus)
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

                var checkPurchaseCart = _db.tblPurchaseCartDetail.FirstOrDefault(pd => pd.BranchID == _sessionHelper.BranchID && pd.CompanyID == _sessionHelper.CompanyID);
                if (checkPurchaseCart == null)
                {
                    Session["ErrorMessagePurchase"] = Resources.Messages.PurchaseCartIsEmpty;
                    return RedirectToAction("NewPurchase");
                }

                return View(await _supplierService.GetByCompanyAndBranchAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID));
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

                var supplier = await _supplierService.GetByIdAsync(supplierID);
                var purchaseDetails = _db.tblPurchaseCartDetail.Where(pd => pd.BranchID == _sessionHelper.BranchID && pd.CompanyID == _sessionHelper.CompanyID).ToList();
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

                _db.tblSupplierInvoice.Add(invoiceHeader);
                _db.SaveChanges();

                foreach (var item in purchaseDetails)
                {
                    var newPurchaseDetails = new tblSupplierInvoiceDetail()
                    {
                        ProductID = item.ProductID,
                        PurchaseQuantity = item.PurchaseQuantity,
                        PurchaseUnitPrice = item.PurchaseUnitPrice,
                        SupplierInvoiceID = invoiceHeader.SupplierInvoiceID
                    };
                    _db.tblSupplierInvoiceDetail.Add(newPurchaseDetails);
                }

                _db.SaveChanges();

                string Message = await _purchaseEntry.ConfirmPurchase(_sessionHelper.CompanyID, _sessionHelper.BranchID, _sessionHelper.UserID, invoiceNo, invoiceHeader.SupplierInvoiceID.ToString(), (float)totalAmount, supplierID.ToString(), supplier.SupplierName, IsPayment);

                if (Message.Contains("Success"))
                {
                    foreach (var item in purchaseDetails)
                    {
                        var stockItem = _db.tblStock.Find(item.ProductID);
                        if (stockItem != null)
                        {
                            stockItem.CurrentPurchaseUnitPrice = item.PurchaseUnitPrice;
                            stockItem.Quantity += item.PurchaseQuantity;
                            _db.Entry(stockItem).State = System.Data.Entity.EntityState.Modified;
                        }
                        _db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    }
                    _db.SaveChanges();
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