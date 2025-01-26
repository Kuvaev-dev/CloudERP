using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Domain.Helpers
{
    public class AllowedDomainsAttribute : ValidationAttribute
    {
        private readonly string[] _allowedDomains;

        public AllowedDomainsAttribute(params string[] allowedDomains)
        {
            _allowedDomains = allowedDomains ?? throw new ArgumentNullException(nameof(allowedDomains));
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string email && email.Contains("@"))
            {
                var emailDomain = email.Split('@').Last().ToLower();
                if (_allowedDomains.Any(d => d.Equals(emailDomain, StringComparison.OrdinalIgnoreCase)))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    string errorMessage = string.Format(
                        Localization.Localization.DomainNotAvailable ?? "Domain {0} is not allowed. Allowed domains: {1}",
                        emailDomain,
                        string.Join(", ", _allowedDomains)
                    );
                    return new ValidationResult(errorMessage);
                }
            }
            return new ValidationResult(Localization.Localization.EmailAddressField ?? "Invalid email format.");
        }
    }
}
