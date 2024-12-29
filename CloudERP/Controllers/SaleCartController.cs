using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CloudERP.Models;
using System.Threading.Tasks;
using CloudERP.Helpers;
using Domain.Models;
using CloudERP.Facades;

namespace CloudERP.Controllers
{
    public class SaleCartController : Controller
    {
        private readonly SaleCartFacade _saleCartFacade;
        private readonly SessionHelper _sessionHelper;

        public SaleCartController(SaleCartFacade saleCartFacade, SessionHelper sessionHelper)
        {
            _saleCartFacade = saleCartFacade;
            _sessionHelper = sessionHelper;
        }

        // GET: SaleCart/NewSale
        public async Task<ActionResult> NewSale()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var findDetail = await _saleCartFacade.SaleCartDetailRepository.GetByDefaultSettingAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID, _sessionHelper.UserID);

                double totalAmount = findDetail.Sum(item => item.SaleQuantity * item.SaleUnitPrice);
                ViewBag.TotalAmount = totalAmount;

                ViewBag.Products = await _saleCartFacade.StockRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);

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
                var product = await _saleCartFacade.StockRepository.GetByIdAsync(id);
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
        public async Task<ActionResult> AddItem(int PID, int Qty, float Price)
        {
            try
            {
                if (_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var checkQty = await _saleCartFacade.StockRepository.GetByIdAsync(PID);
                if (Qty > checkQty.Quantity)
                {
                    ViewBag.Message = Resources.Messages.SaleQuantityError;
                    return RedirectToAction("NewSale");
                }

                var findDetail = await _saleCartFacade.SaleCartDetailRepository.GetByProductIdAsync(PID, _sessionHelper.BranchID, _sessionHelper.CompanyID);

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
                        await _saleCartFacade.SaleCartDetailRepository.AddAsync(newItem);
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
        public async Task<ActionResult> DeleteConfirm(int id)
        {
            try
            {
                if (_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var product = await _saleCartFacade.SaleCartDetailRepository.GetByIdAsync(id);
                if (product != null)
                {
                    await _saleCartFacade.SaleCartDetailRepository.UpdateAsync(product);
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

        // POST: SaleCart/GetProduct
        [HttpPost]
        public async Task<ActionResult> GetProduct()
        {
            try
            {
                if (_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var productEntities = await _saleCartFacade.StockRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
                List<ProductMV> products = productEntities
                    .Select(item => new ProductMV()
                    {
                        Name = item.ProductName + " (AVL QTY: " + item.Quantity + ")",
                        ProductID = item.ProductID
                    })
                    .ToList();

                return Json(new { data = products }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return Json(new { error = Resources.Messages.UnexpectedErrorMessage + ex.Message });
            }
        }

        // POST: SaleCart/GetProductDetails/5
        [HttpPost]
        public async Task<ActionResult> GetProductDetails(int? id)
        {
            try
            {
                var product = await _saleCartFacade.StockRepository.GetByIdAsync((int)id);
                if (product != null)
                {
                    return Json(new { data = product.SaleUnitPrice }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { error = Resources.Messages.ProductNotFound });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return Json(new { error = Resources.Messages.UnexpectedErrorMessage + ex.Message });
            }
        }

        // POST: SaleCart/CancelSale
        [HttpPost]
        public async Task<ActionResult> CancelSale()
        {
            try
            {
                if (_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var saleDetails = await _saleCartFacade.SaleCartDetailRepository.GetByDefaultSettingAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID, _sessionHelper.UserID);
                await _saleCartFacade.SaleCartDetailRepository.DeleteAsync(saleDetails);

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
            try
            {
                Session["ErrorMessageSale"] = string.Empty;

                if (_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var saleDetails = await _saleCartFacade.SaleCartDetailRepository.GetByCompanyAndBranchAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID);
                if (saleDetails == null)
                {
                    Session["ErrorMessageSale"] = Resources.Messages.SaleCartEmpty;

                    return RedirectToAction("NewSale");
                }

                return View(await _saleCartFacade.CustomerRepository.GetByCompanyAndBranchAsync(_sessionHelper.CompanyID, _sessionHelper.CompanyID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: SaleCart/SaleConfirm
        [HttpPost]
        public async Task<ActionResult> SaleConfirm(FormCollection collection)
        {
            try
            {
                if (_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                int customerID = 0;
                bool IsPayment = false;
                string[] keys = collection.AllKeys;
                foreach (var name in keys)
                {
                    if (name.Contains("name"))
                    {
                        string idName = name;
                        string[] valueIDs = idName.Split(' ');
                        customerID = Convert.ToInt32(valueIDs[1]);
                    }
                }

                string Description = collection["item.Description"];

                if (collection["IsPayment"] != null)
                {
                    IsPayment = collection["IsPayment"].Contains("on");
                }

                var customer = await _saleCartFacade.CustomerRepository.GetByIdAsync(customerID);
                var saleDetails = await _saleCartFacade.SaleCartDetailRepository.GetAllAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID);

                double totalAmount = saleDetails.Sum(item => item.SaleQuantity * item.SaleUnitPrice);

                if (totalAmount == 0)
                {
                    ViewBag.Message = Resources.Messages.SaleCartEmpty;
                    return RedirectToAction("NewSale");
                }

                string invoiceNo = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var invoiceHeader = new CustomerInvoice()
                {
                    BranchID = _sessionHelper.BranchID,
                    Title = "Sale Invoice " + customer.Customername,
                    CompanyID = _sessionHelper.CompanyID,
                    Description = Description,
                    InvoiceDate = DateTime.Now,
                    InvoiceNo = invoiceNo,
                    CustomerID = customerID,
                    UserID = _sessionHelper.UserID,
                    TotalAmount = totalAmount
                };

                await _saleCartFacade.CustomerInvoiceRepository.AddAsync(invoiceHeader);

                await _saleCartFacade.CustomerInvoiceDetailRepository.AddSaleDetailsAsync(saleDetails, invoiceHeader.CustomerInvoiceID);

                string Message = await _saleCartFacade.SaleEntry.ConfirmSale(_sessionHelper.CompanyID, _sessionHelper.BranchID, _sessionHelper.UserID, invoiceNo, invoiceHeader.CustomerInvoiceID.ToString(), (float)totalAmount, customerID.ToString(), customer.Customername, IsPayment);
                if (Message.Contains("Success"))
                {
                    await _saleCartFacade.SaleEntry.CompleteSale(saleDetails);
                }

                if (Message.Contains("Success"))
                {
                    return RedirectToAction("PrintSaleInvoice", "SalePayment", new { id = invoiceHeader.CustomerInvoiceID });
                }

                Session["Message"] = Message;

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