using Domain.UtilsAccess;
using PhoneNumbers;

namespace CloudERP.Helpers
{
    public class PhoneNumberHelper : IPhoneNumberHelper
    {
        private static readonly PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();

        public string ExtractNationalNumber(string internationalPhoneNumber)
        {
            if (string.IsNullOrWhiteSpace(internationalPhoneNumber))
                return string.Empty;

            try
            {
                var phoneNumber = phoneUtil.Parse(internationalPhoneNumber, null);
                return phoneNumber.NationalNumber.ToString();
            }
            catch (NumberParseException)
            {
                return string.Empty;
            }
        }
    }
}
