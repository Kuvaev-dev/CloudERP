namespace Domain.UtilsAccess
{
    public interface IPhoneNumberHelper
    {
        string ExtractNationalNumber(string internationalPhoneNumber);
    }
}
