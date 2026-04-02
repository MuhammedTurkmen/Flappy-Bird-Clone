using UnityEngine;

namespace MET.Core.Utilities.Extentions
{
    public static class StringExtentions 
    {
        public static Color ToColor(this string hexcode) 
        {
            if (ColorUtility.TryParseHtmlString(hexcode, out Color result)) return result;
            return Color.white;
        }

        public static string FirstCharToUpperInvariant(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (s.Length == 1)
                return char.ToUpperInvariant(s[0]).ToString();

            return char.ToUpperInvariant(s[0]) + s.Substring(1);
        }

    }
}