using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceCLip2
{
    public static class TextUtilities
    {
        private static TextInfo ti = new CultureInfo("en-US", true).TextInfo;

        public static string ToTitleCase(string input)
        {
            return String.IsNullOrEmpty(input) ? String.Empty : ti.ToTitleCase(input);
        }

        public static string ToLower(string input)
        {
            return String.IsNullOrEmpty(input) ? String.Empty : input.ToLowerInvariant();
        }

        public static string ToUpper(string input)
        {
            return String.IsNullOrEmpty(input) ? String.Empty : input.ToUpperInvariant();
        }

        public static string TrimSpacesStart(string input)
        {
            return String.IsNullOrEmpty(input) ? String.Empty : input.TrimStart();
        }

        public static string TrimSpacesEnd(string input)
        {
            return String.IsNullOrEmpty(input) ? String.Empty : input.TrimEnd();
        }

        public static string TrimSpaces(string input)
        {
            return String.IsNullOrEmpty(input) ? String.Empty : input.Trim();
        }

        public static string RemoveWhiteWSpaces(string input)
        {
            if (input != null)
            {
                return new string(input.ToCharArray()
                    .Where(c => !Char.IsWhiteSpace(c))
                    .ToArray());
            }

            return String.Empty;
        }
    }
}
