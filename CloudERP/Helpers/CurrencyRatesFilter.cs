using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Domain.UtilsAccess;

namespace CloudERP.Helpers
{
    public class CurrencyRatesFilter : IAsyncActionFilter
    {
        private readonly IHttpClientHelper _httpClientHelper;

        public CurrencyRatesFilter(IHttpClientHelper httpClientHelper)
        {
            _httpClientHelper = httpClientHelper;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.Controller as Controller;
            if (controller != null)
            {
                var currencies = await _httpClientHelper.GetAsync<Dictionary<string, decimal>>("homeapi/getcurrencies");
                if (currencies != null)
                {
                    controller.ViewBag.Currencies = currencies;
                }

                var httpContext = context.HttpContext;
                controller.ViewBag.SelectedCurrency = httpContext.Session.GetString("SelectedCurrency") ?? "UAH";
                controller.ViewBag.CultureCode = httpContext.Session.GetString("Culture") ?? "en-US";
            }

            await next();
        }
    }
}
