﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Utilities
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class CurrencyApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _defaultCurrency;

        public CurrencyApiController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _defaultCurrency = _configuration["CurrencyApi:DefaultCurrency"] ?? throw new ArgumentNullException("DefaultCurrency is missing in configuration.");
        }

        [HttpGet]
        public ActionResult<string> GetCurrency(string currencyCode)
        {
            string defaultCurrency = _defaultCurrency ?? "UAH";
            return Ok(currencyCode ?? defaultCurrency);
        }
    }
}
