using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudERP.Helpers
{
    public class CultureHelper
    {
        // Valid cultures
        private static readonly List<string> _validCultures = new List<string> { "en", "ru", "uk" };

        // Default culture
        private static readonly string _defaultCulture = "en";

        public static string GetImplementedCulture(string name)
        {
            // If no culture is specified, return the default culture
            if (string.IsNullOrEmpty(name))
                return _defaultCulture;

            // Make sure the culture is valid
            if (_validCultures.Where(c => c.Equals(name, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return name;

            // If the culture is not valid, return the default culture
            return _defaultCulture;
        }
    }
}