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
    public class SaleReturnController : Controller
    {
        private readonly ISaleEntry _saleEntry;
        private readonly IStockRepository _stockRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerInvoiceRepository _customerInvoiceRepository;
        private readonly ICustomerInvoiceDetailRepository _customerInvoiceDetailRepository;
        private readonly ICustomerReturnInvoiceRepository _customerReturnInvoiceRepository;
        private readonly ICustomerReturnInvoiceDetailRepository _customerReturnInvoiceDetailRepository;
        private readonly SessionHelper _sessionHelper;

        public SaleReturnController(ISaleEntry saleEntry, ICustomerInvoiceRepository customerInvoiceRepository, SessionHelper sessionHelper, ICustomerInvoiceDetailRepository customerInvoiceDetailRepository, ICustomerReturnInvoiceRepository customerReturnInvoiceRepository, ICustomerRepository customerRepository, ICustomerReturnInvoiceDetailRepository customerReturnInvoiceDetailRepository, IStockRepository stockRepository)
        {
            _saleEntry = saleEntry;
            _customerInvoiceRepository = customerInvoiceRepository;
            _sessionHelper = sessionHelper;
            _customerInvoiceDetailRepository = customerInvoiceDetailRepository;
            _customerReturnInvoiceRepository = customerReturnInvoiceRepository;
            _customerRepository = customerRepository;
            _customerReturnInvoiceDetailRepository = customerReturnInvoiceDetailRepository;
            _stockRepository = stockRepository;
        }

        // GET: SaleReturn
        public async Task<ActionResult> FindSale()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                CustomerInvoice invoice;

                if (Session["SaleInvoiceNo"] != null)
                {
                    if (!string.IsNullOrEmpty(_sessionHelper.SaleInvoiceNo))
                    {
                        invoice = await _customerInvoiceRepository.GetByInvoiceNoAsync(_sessionHelper.SaleInvoiceNo);
                    }
                    else
                    {
                        invoice = await _customerInvoiceRepository.GetByIdAsync(0);
                    }
                }
                else
                {
                    invoice = await _customerInvoiceRepository.GetByIdAsync(0);
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
        public async Task<ActionResult> FindSale(string invoiceID)
        {
            try
            {
                Session["SaleInvoiceNo"] = string.Empty;
                Session["SaleReturnMessage"] = string.Empty;

                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                return View(await _customerInvoiceRepository.GetByInvoiceNoAsync(invoiceID));
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
                Session["SaleInvoiceNo"] = string.Empty;
                Session["SaleReturnMessage"] = string.Empty;

                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                int customerID = 0;
                int CustomerInvoiceID = 0;
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

                string Description = "Sale Return";
                string[] CustomerInvoiceIDs = collection["customerInvoiceID"].Split(',');
                if (CustomerInvoiceIDs != null && CustomerInvoiceIDs.Length > 0)
                {
                    CustomerInvoiceID = Convert.ToInt32(CustomerInvoiceIDs[0]);
                }

                if (collection["IsPayment"] != null && collection["IsPayment"].Contains("on"))
                {
                    IsPayment = true;
                }

                double TotalAmount = 0;
                var saleDetails = await _customerInvoiceDetailRepository.GetListByIdAsync(CustomerInvoiceID);
                var list = saleDetails.ToList();
                for (int i = 0; i < saleDetails.Count(); i++)
                {
                    foreach (var productID in ProductIDs)
                    {
                        if (productID == list[i].ProductID)
                        {
                            TotalAmount += (ReturnQty[i] * list[i].SaleUnitPrice);
                        }
                    }
                }

                var customerInvoice = await _customerInvoiceRepository.GetByIdAsync(CustomerInvoiceID);
                customerID = customerInvoice.CustomerID;

                if (TotalAmount == 0)
                {
                    Session["SaleInvoiceNo"] = customerInvoice.InvoiceNo;
                    Session["SaleReturnMessage"] = Resources.Messages.OneProductReturnQtyError;
                    return RedirectToAction("FindSale");
                }

                string invoiceNo = "RIN" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var returnInvoiceHeader = new CustomerReturnInvoice()
                {
                    BranchID = _sessionHelper.BranchID,
                    CompanyID = _sessionHelper.CompanyID,
                    Description = Description,
                    InvoiceDate = DateTime.Now,
                    InvoiceNo = invoiceNo,
                    CustomerID = customerID,
                    UserID = _sessionHelper.UserID,
                    TotalAmount = TotalAmount,
                    CustomerInvoiceID = CustomerInvoiceID
                };

                await _customerReturnInvoiceRepository.AddAsync(returnInvoiceHeader);

                var customer = await _customerRepository.GetByIdAsync(customerID);
                string Message = await _saleEntry.ReturnSale(
                    _sessionHelper.CompanyID, 
                    _sessionHelper.BranchID, 
                    _sessionHelper.UserID, 
                    invoiceNo, 
                    returnInvoiceHeader.CustomerInvoiceID.ToString(), 
                    returnInvoiceHeader.CustomerReturnInvoiceID, 
                    (float)TotalAmount, 
                    customerID.ToString(), 
                    customer.Customername, 
                    IsPayment);
                var saleDetailsList = saleDetails.ToList();
                if (Message.Contains("Success"))
                {
                    for (int i = 0; i < saleDetails.Count(); i++)
                    {
                        foreach (var productID in ProductIDs)
                        {
                            if (productID == saleDetailsList[i].ProductID)
                            {
                                if (ReturnQty[i] > 0)
                                {
                                    var returnProductDetails = new CustomerReturnInvoiceDetail()
                                    {
                                        CustomerInvoiceID = CustomerInvoiceID,
                                        SaleReturnQuantity = ReturnQty[i],
                                        ProductID = productID,
                                        SaleReturnUnitPrice = saleDetailsList[i].SaleUnitPrice,
                                        CustomerReturnInvoiceID = returnInvoiceHeader.CustomerReturnInvoiceID,
                                        CustomerInvoiceDetailID = saleDetailsList[i].CustomerInvoiceDetailID
                                    };

                                    await _customerReturnInvoiceDetailRepository.AddAsync(returnProductDetails);

                                    var stock = await _stockRepository.GetByIdAsync(productID);
                                    stock.Quantity += ReturnQty[i];
                                    await _stockRepository.UpdateAsync(stock);
                                }
                            }
                        }
                    }

                    Session["SaleInvoiceNo"] = customerInvoice.InvoiceNo;
                    Session["SaleReturnMessage"] = Resources.Messages.ReturnSuccessfully;

                    return RedirectToAction("FindSale");
                }

                Session["SaleInvoiceNo"] = customerInvoice.InvoiceNo;
                Session["SaleReturnMessage"] = Resources.Messages.UnexpectedIssue;

                return RedirectToAction("FindSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}