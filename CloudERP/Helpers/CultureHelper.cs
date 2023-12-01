using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudERP.Helpers
{
    public class CultureHelper
    {
        private static readonly List<string> _validCultures = new List<string> { "en", "ru", "uk" };
        private static readonly string _defaultCulture = "en";

        public static string GetImplementedCulture(string name)
        {
            if (string.IsNullOrEmpty(name))
                return _defaultCulture;
 
            if (_validCultures.Where(c => c.Equals(name, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return name;

            return _defaultCulture;
        }
    }
}