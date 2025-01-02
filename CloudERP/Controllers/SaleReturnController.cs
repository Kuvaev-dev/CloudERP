using CloudERP.Helpers;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SaleReturnController : Controller
    {
        private readonly ICustomerInvoiceRepository _customerInvoiceRepository;
        private readonly SessionHelper _sessionHelper;
        private readonly ISaleReturnService _saleReturnService;

        public SaleReturnController(ICustomerInvoiceRepository customerInvoiceRepository, SessionHelper sessionHelper, ISaleReturnService saleReturnService)
        {
            _customerInvoiceRepository = customerInvoiceRepository;
            _sessionHelper = sessionHelper;
            _saleReturnService = saleReturnService;
        }

        // GET: SaleReturn
        public async Task<ActionResult> FindSale()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
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
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                Session["SaleInvoiceNo"] = string.Empty;
                Session["SaleReturnMessage"] = string.Empty;

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
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                Session["SaleInvoiceNo"] = string.Empty;
                Session["SaleReturnMessage"] = string.Empty;

                var returnConfirmDto = new SaleReturnConfirm
                {
                    ProductIDs = new List<int>(),
                    ReturnQty = new List<int>(),
                    CustomerInvoiceID = Convert.ToInt32(collection["customerInvoiceID"].Split(',')[0]),
                    IsPayment = collection["IsPayment"] != null && collection["IsPayment"].Contains("on")
                };

                string[] keys = collection.AllKeys;
                foreach (var name in keys)
                {
                    if (name.Contains("ProductID "))
                    {
                        string idName = name;
                        string[] valueIDs = idName.Split(' ');
                        returnConfirmDto.ProductIDs.Add(Convert.ToInt32(valueIDs[1]));
                        returnConfirmDto.ReturnQty.Add(Convert.ToInt32(collection[idName].Split(',')[0]));
                    }
                }

                var result = await _saleReturnService.ProcessReturnConfirmAsync(
                    returnConfirmDto,
                    _sessionHelper.BranchID,
                    _sessionHelper.CompanyID,
                    _sessionHelper.UserID);

                Session["SaleInvoiceNo"] = result.InvoiceNo;
                Session["SaleReturnMessage"] = result.Message;

                if (result.IsSuccess)
                {
                    return RedirectToAction("FindSale");
                }

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