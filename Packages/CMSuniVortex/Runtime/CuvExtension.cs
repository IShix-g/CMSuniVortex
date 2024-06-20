
using System.Globalization;
using UnityEngine;

namespace CMSuniVortex
{
    public static class CuvExtension
    {
        public static string SetParam(this string text, string param, double value)
            => text.SetParam(param, value.ToString(CultureInfo.InvariantCulture));
        
        public static string SetParam(this string text, string param, float value)
            => text.SetParam(param, value.ToString(CultureInfo.InvariantCulture));
        
        public static string SetParam(this string text, string param, long value)
            => text.SetParam(param, value.ToString());
        
        public static string SetParam(this string text, string param, int value)
            => text.SetParam(param, value.ToString());

        public static string SetParam(this string text, string param, string value)
        {
            var paramText = "{" + param + "}";
#if DEBUG
            if (!text.Contains(paramText))
            {
                Debug.LogWarning("param did not exist. : " + param );
            }
#endif
            return text.Replace(paramText, value);
        }
    }
}