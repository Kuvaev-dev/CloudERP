using System.ComponentModel.DataAnnotations;
using System.Linq;

public class AllowedDomainsAttribute : ValidationAttribute
{
    private readonly string[] _allowedDomains;

    public AllowedDomainsAttribute(string[] allowedDomains)
    {
        _allowedDomains = allowedDomains;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value != null)
        {
            var email = value.ToString();
            var emailDomain = email.Split('@').Last();
            if (_allowedDomains.Contains(emailDomain))
            {
                return ValidationResult.Success;
            }
            else
            {
                string errorMessage = string.Format(
                    Localization.DomainNotAvailable,
                    emailDomain,
                    string.Join(", ", _allowedDomains)
                );
                return new ValidationResult(errorMessage);
            }
        }
        return new ValidationResult("Email " + Localization.RequiredField);
    }
}
