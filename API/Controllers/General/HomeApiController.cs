using API.Models;
using Domain.Models.FinancialModels;
using Microsoft.AspNetCore.Mvc;
using Services.Facades;

namespace API.Controllers.General
{
    [ApiController]
    public class HomeApiController : ControllerBase
    {
        private readonly HomeFacade _homeFacade;
        private readonly IConfiguration _configuration;

        public HomeApiController(HomeFacade homeFacade, IConfiguration configuration)
        {
            _homeFacade = homeFacade ?? throw new ArgumentNullException(nameof(homeFacade));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost]
        public async Task<ActionResult<object>> LoginUser([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var user = await _homeFacade.AuthService.AuthenticateUserAsync(loginRequest.Email, loginRequest.Password);
                if (user != null)
                {
                    var employee = await _homeFacade.EmployeeRepository.GetByUserIdAsync(user.UserID);
                    if (employee == null) return Unauthorized();

                    var company = await _homeFacade.CompanyRepository.GetByIdAsync(employee.CompanyID);
                    if (company == null) return Unauthorized();

                    return Ok(new
                    {
                        user,
                        employee,
                        company
                    });
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<DashboardModel>> GetDashboardValues(int companyId, int branchId)
        {
            try
            {
                var dashboardValues = await _homeFacade.DashboardService.GetDashboardValues(branchId, companyId);
                if (dashboardValues == null) return NotFound();
                return Ok(dashboardValues);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetCurrencies()
        {
            try
            {
                var defaultCurrency = _configuration["DefaultCurrency"] ?? "USD";
                var rates = await _homeFacade.CurrencyService.GetExchangeRatesAsync(defaultCurrency);
                var currencies = rates.Keys.Select(k => new { Code = k, Name = k, Rate = rates[k].ToString("F2") });
                return Ok(currencies);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // POST: api/home/forgot-password
        [HttpPost]
        public async Task<ActionResult<string>> ForgotPassword([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(Localization.CloudERP.Messages.Messages.EmailIsRequired);
            }

            try
            {
                if (await _homeFacade.AuthService.IsPasswordResetRequestedRecentlyAsync(email))
                {
                    return BadRequest(Localization.CloudERP.Messages.Messages.PasswordResetAlreadyRequested);
                }

                var user = await _homeFacade.UserRepository.GetByEmailAsync(email);
                if (user != null)
                {
                    user.ResetPasswordCode = Guid.NewGuid().ToString();
                    user.ResetPasswordExpiration = DateTime.Now.AddHours(1);
                    user.LastPasswordResetRequest = DateTime.Now;

                    await _homeFacade.UserRepository.UpdateAsync(user);

                    var resetLink = Url.Link("ResetPassword", new { id = user.ResetPasswordCode });
                    _homeFacade.AuthService.SendPasswordResetEmailAsync(resetLink, user.Email, user.ResetPasswordCode);

                    return Ok("Password reset email sent.");
                }

                return Ok("Password reset email sent.");
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // GET: api/home/reset-password/{id}
        [HttpGet]
        public async Task<ActionResult<object>> GetResetPassword(string id)
        {
            try
            {
                var user = await _homeFacade.UserRepository.GetByPasswordCodesAsync(id, DateTime.Now);
                if (user != null)
                {
                    return Ok(new { ResetCode = id });
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // POST: api/home/reset-password
        [HttpPost]
        public async Task<ActionResult<string>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (!await _homeFacade.AuthService.ResetPasswordAsync(request.ResetCode, request.NewPassword, request.ConfirmPassword))
                {
                    return BadRequest("Passwords do not match or link expired.");
                }

                return Ok("Password reset successfully.");
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}