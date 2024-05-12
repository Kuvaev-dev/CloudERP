using System.Globalization;
using System.Threading;

public class ResourceManagerHelper
{
    public static void SetCulture(string cultureCode)
    {
        try
        {
            CultureInfo culture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
        catch (CultureNotFoundException)
        {
            SetCulture("en-US");
        }
    }
}