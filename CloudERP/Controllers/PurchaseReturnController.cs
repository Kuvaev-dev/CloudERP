using CloudERP.Helpers;
using Domain.EntryAccess;
using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchaseReturnController : Controller
    {
        private readonly IPurchaseEntry _purchaseEntry;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ISupplierInvoiceRepository _supplierInvoiceRepository;
        private readonly ISupplierReturnInvoiceRepository _supplierReturnInvoiceRepository;
        private readonly ISupplierInvoiceDetailRepository _supplierInvoiceDetailRepository;
        private readonly ISupplierReturnInvoiceDetailRepository _supplierReturnInvoiceDetailRepository;
        private readonly IStockRepository _stockRepository;
        private readonly SessionHelper _sessionHelper;

        public PurchaseReturnController(IPurchaseEntry purchaseEntry, ISupplierInvoiceRepository supplierInvoiceRepository, ISupplierReturnInvoiceRepository supplierReturnInvoiceRepository, ISupplierInvoiceDetailRepository supplierInvoiceDetailRepository, SessionHelper sessionHelper, ISupplierRepository supplierRepository, ISupplierReturnInvoiceDetailRepository supplierReturnInvoiceDetailRepository, IStockRepository stockRepository)
        {
            _purchaseEntry = purchaseEntry;
            _supplierInvoiceRepository = supplierInvoiceRepository;
            _supplierReturnInvoiceRepository = supplierReturnInvoiceRepository;
            _supplierInvoiceDetailRepository = supplierInvoiceDetailRepository;
            _sessionHelper = sessionHelper;
            _supplierRepository = supplierRepository;
            _supplierReturnInvoiceDetailRepository = supplierReturnInvoiceDetailRepository;
            _stockRepository = stockRepository;
        }

        // GET: PurchaseReturn
        public async Task<ActionResult> FindPurchase()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                SupplierInvoice invoice;

                if (_sessionHelper.InvoiceNo != null)
                {
                    if (!string.IsNullOrEmpty(_sessionHelper.InvoiceNo))
                    {
                        invoice = await _supplierInvoiceRepository.GetByInvoiceNoAsync(_sessionHelper.InvoiceNo);
                    }
                    else
                    {
                        invoice = await _supplierInvoiceRepository.GetByIdAsync(0);
                    }
                }
                else
                {
                    invoice = await _supplierInvoiceRepository.GetByIdAsync(0);
                }

                return View(invoice);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> FindPurchase(string invoiceID)
        {
            try
            {
                Session["InvoiceNo"] = string.Empty;
                Session["ReturnMessage"] = string.Empty;

                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                return View(await _supplierInvoiceRepository.GetByInvoiceNoAsync(invoiceID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ReturnConfirm(FormCollection collection)
        {
            try
            {
                Session["InvoiceNo"] = string.Empty;
                Session["ReturnMessage"] = string.Empty;

                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                int supplierID = 0;
                int SupplierInvoiceID = 0;
                bool IsPayment = false;
                List<int> ProductIDs = new List<int>();
                List<int> ReturnQty = new List<int>();
                string[] keys = collection.AllKeys;

                foreach (var name in keys)
                {
                    if (name.Contains("ProductID "))
                    {
                        string idName = name;
                        string[] valueIDs = idName.Split(' ');
                        ProductIDs.Add(Convert.ToInt32(valueIDs[1]));
                        ReturnQty.Add(Convert.ToInt32(collection[idName].Split(',')[0]));
                    }
                }

                string Description = Resources.Messages.PurchaseReturn;
                string[] SupplierInvoiceIDs = collection["supplierInvoiceID"].Split(',');

                if (SupplierInvoiceIDs != null)
                {
                    if (SupplierInvoiceIDs[0] != null)
                    {
                        SupplierInvoiceID = Convert.ToInt32(SupplierInvoiceIDs[0]);
                    }
                }

                if (collection["IsPayment"] != null)
                {
                    string[] isPaymentDirCet = collection["IsPayment"].Split(',');
                    IsPayment = isPaymentDirCet[0] == "on";
                }

                double TotalAmount = 0;
                var purchaseDetails = await _supplierInvoiceDetailRepository.GetListByIdAsync(SupplierInvoiceID);
                for (int i = 0; i < purchaseDetails.Count(); i++)
                {
                    foreach (var productID in ProductIDs)
                    {
                        if (productID == purchaseDetails[i].ProductID)
                        {
                            TotalAmount += (ReturnQty[i] * purchaseDetails[i].PurchaseUnitPrice);
                        }
                    }
                }

                var supplierInvoice = await _supplierInvoiceRepository.GetByIdAsync(SupplierInvoiceID);
                supplierID = supplierInvoice.SupplierID;
                if (TotalAmount == 0)
                {
                    Session["InvoiceNo"] = supplierInvoice.InvoiceNo;
                    Session["ReturnMessage"] = Resources.Messages.OneProductReturnQtyError;
                    return RedirectToAction("FindPurchase");
                }

                string invoiceNo = "RPU" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var returnInvoiceHeader = new SupplierReturnInvoice()
                {
                    BranchID = _sessionHelper.BranchID,
                    CompanyID = _sessionHelper.CompanyID,
                    Description = Description,
                    InvoiceDate = DateTime.Now,
                    InvoiceNo = invoiceNo,
                    SupplierID = supplierID,
                    UserID = _sessionHelper.UserID,
                    TotalAmount = TotalAmount,
                    SupplierInvoiceID = SupplierInvoiceID
                };
                await _supplierReturnInvoiceRepository.AddAsync(returnInvoiceHeader);

                var supplier = await _supplierRepository.GetByIdAsync(supplierID);
                string Message = await _purchaseEntry.ReturnPurchase(_sessionHelper.CompanyID, _sessionHelper.BranchID, _sessionHelper.UserID, invoiceNo, returnInvoiceHeader.SupplierInvoiceID.ToString(), returnInvoiceHeader.SupplierReturnInvoiceID, (float)TotalAmount, supplierID.ToString(), supplier.SupplierName, IsPayment);

                if (Message.Contains("Success"))
                {
                    for (int i = 0; i < purchaseDetails.Count; i++)
                    {
                        foreach (var productID in ProductIDs)
                        {
                            if (productID == purchaseDetails[i].ProductID)
                            {
                                if (ReturnQty[i] > 0)
                                {
                                    var returnProductDetails = new SupplierReturnInvoiceDetail()
                                    {
                                        SupplierInvoiceID = SupplierInvoiceID,
                                        PurchaseReturnQuantity = ReturnQty[i],
                                        ProductID = productID,
                                        PurchaseReturnUnitPrice = purchaseDetails[i].PurchaseUnitPrice,
                                        SupplierReturnInvoiceID = returnInvoiceHeader.SupplierReturnInvoiceID,
                                        SupplierInvoiceDetailID = purchaseDetails[i].SupplierInvoiceDetailID
                                    };
                                    await _supplierReturnInvoiceDetailRepository.AddAsync(returnProductDetails);

                                    var stock = await _stockRepository.GetByIdAsync(productID);
                                    if (stock != null)
                                    {
                                        stock.Quantity -= ReturnQty[i];

                                        await _stockRepository.UpdateAsync(stock);
                                    }
                                }
                            }
                        }
                    }

                    Session["InvoiceNo"] = supplierInvoice.InvoiceNo;
                    Session["ReturnMessage"] = Resources.Messages.ReturnSuccessfully;

                    return RedirectToAction("FindPurchase");
                }

                Session["InvoiceNo"] = supplierInvoice.InvoiceNo;
                Session["ReturnMessage"] = Resources.Messages.UnexpectedIssue;

                return RedirectToAction("FindPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}