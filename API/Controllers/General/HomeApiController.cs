using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using API.Models;
using Services.Facades;

namespace API.Controllers
{
    [RoutePrefix("api/home")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HomeApiController : ApiController
    {
        private readonly HomeFacade _homeFacade;

        public HomeApiController(HomeFacade homeFacade)
        {
            _homeFacade = homeFacade ?? throw new ArgumentNullException(nameof(HomeFacade));
        }

        [HttpPost, Route("login")]
        public async Task<IHttpActionResult> LoginUser([FromBody] LoginRequest loginRequest)
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
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("dashboard/{companyId:int}/{branchId:int}")]
        public async Task<IHttpActionResult> GetDashboardValues(int companyId, int branchId)
        {
            try
            {
                var dashboardValues = await _homeFacade.DashboardService.GetDashboardValues(branchId, companyId);
                return Ok(dashboardValues);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("currencies")]
        public async Task<IHttpActionResult> GetCurrencies()
        {
            try
            {
                var defaultCurrency = ConfigurationManager.AppSettings["DefaultCurrency"] ?? "USD";
                var rates = await _homeFacade.CurrencyService.GetExchangeRatesAsync(defaultCurrency);
                var currencies = rates.Keys.Select(k => new { Code = k, Name = k, Rate = rates[k].ToString("F2") });
                return Ok(currencies);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/home/forgot-password
        [HttpPost, Route("forgot-password")]
        public async Task<IHttpActionResult> ForgotPassword([FromBody] string email)
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
                return InternalServerError(ex);
            }
        }

        // GET: api/home/reset-password/{id}
        [HttpGet, Route("reset-password/{id:string}")]
        public async Task<IHttpActionResult> GetResetPassword(string id)
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
                return InternalServerError(ex);
            }
        }

        // POST: api/home/reset-password
        [HttpPost, Route("reset-password")]
        public async Task<IHttpActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
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
                return InternalServerError(ex);
            }
        }
    }
}