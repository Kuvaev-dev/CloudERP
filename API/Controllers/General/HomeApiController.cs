using Domain.Models;
using Domain.Models.FinancialModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.Facades;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers.General
{
    [ApiController]
    [Route("api/[controller]/[action]")]
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
        public async Task<ActionResult<LoginResponse>> LoginUser([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var user = await _homeFacade.AuthService.AuthenticateUserAsync(loginRequest.Email, loginRequest.Password);
                if (user == null) return Unauthorized("User authentication failed");

                var employee = await _homeFacade.EmployeeRepository.GetByUserIdAsync(user.UserID);
                if (employee == null) return Unauthorized("Employee authentication failed");

                var company = await _homeFacade.CompanyRepository.GetByIdAsync(employee.CompanyID);
                if (company == null) return Unauthorized("Company authentication failed");

                var token = GenerateJwtToken(user);

                var response = new LoginResponse
                {
                    User = new Domain.Models.User
                    {
                        UserID = user.UserID,
                        UserTypeID = user.UserTypeID,
                        FullName = user.FullName,
                        Email = user.Email,
                        ContactNo = user.ContactNo,
                        UserName = user.UserName,
                        Password = user.Password,
                        Salt = user.Salt,
                        IsActive = user.IsActive,
                    },
                    Employee = new Domain.Models.Employee
                    {
                        EmployeeID = employee.EmployeeID,
                        FullName = employee.FullName,
                        Photo = employee.Photo,
                        RegistrationDate = employee.RegistrationDate,
                        Designation = employee.Designation,
                        BranchID = employee.BranchID,
                        BranchTypeID = employee.BranchTypeID,
                        BrchID = employee.BrchID,
                        CompanyID = employee.CompanyID
                    },
                    Company = new Domain.Models.Company
                    {
                        CompanyID = company.CompanyID,
                        Name = company.Name,
                        Logo = company.Logo
                    },
                    Token = token
                };

                return Ok(response);
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
                return Ok(dashboardValues);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<Dictionary<string, decimal>>> GetCurrencies()
        {
            try
            {
                var defaultCurrency = _configuration["CurrencyApi:DefaultCurrency"] ?? "USD";
                var rates = await _homeFacade.CurrencyService.GetExchangeRatesAsync(defaultCurrency);
                var currencies = rates.ToDictionary(k => k.Key, v => decimal.TryParse(v.Value.ToString(), out var parsed) ? parsed : 0m);
                return Ok(currencies);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> ForgotPassword([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(Localization.CloudERP.Messages.EmailIsRequired);

            try
            {
                if (await _homeFacade.AuthService.IsPasswordResetRequestedRecentlyAsync(email))
                    return BadRequest(Localization.CloudERP.Messages.PasswordResetAlreadyRequested);

                var user = await _homeFacade.UserRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                user.ResetPasswordCode = Guid.NewGuid().ToString();
                user.ResetPasswordExpiration = DateTime.Now.AddHours(1);
                user.LastPasswordResetRequest = DateTime.Now;

                await _homeFacade.UserRepository.UpdateAsync(user);

                var resetLink = Url.Link("ResetPassword", new { id = user.ResetPasswordCode });
                _homeFacade.AuthService.SendPasswordResetEmailAsync(resetLink, user.Email, user.ResetPasswordCode);

                return Ok("Password reset email sent.");
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetResetPassword(string id)
        {
            try
            {
                var user = await _homeFacade.UserRepository.GetByPasswordCodesAsync(id, DateTime.Now);
                if (user == null) return NotFound("Model not found.");

                return Ok(new { ResetCode = id });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var success = await _homeFacade.AuthService.ResetPasswordAsync(request.ResetCode, request.NewPassword, request.ConfirmPassword);
                if (!success) return BadRequest("Passwords do not match or link expired.");

                return Ok("Password reset successfully.");
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        private string GenerateJwtToken(Domain.Models.User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}