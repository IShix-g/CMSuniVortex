
using System;
using System.Collections.Generic;

namespace CMSuniVortex.GoogleSheet
{
    public static class CustomGoogleSheetModelExtension
    {
        public static string GetString(this Dictionary<string, string> models, string key)
            => models.TryGetValue(key, out var obj) ? obj : string.Empty;
        
        public static T GetEnum<T>(this Dictionary<string, string> models, string key) where T : struct, Enum
            => models.TryGetValue(key, out var obj)
                ? Enum.TryParse<T>(obj, out var val) ? val : default
                : default;
        
        public static bool GetBool(this Dictionary<string, string> models, string key)
            => models.TryGetValue(key, out var obj)
               && string.Equals(obj, "TRUE", StringComparison.OrdinalIgnoreCase);

        public static int GetInt(this Dictionary<string, string> models, string key)
            => models.TryGetValue(key, out var obj)
                ? !string.IsNullOrEmpty(obj)
                    ? int.Parse(obj)
                    : default
                : default;
        
        public static long GetLong(this Dictionary<string, string> models, string key)
            => models.TryGetValue(key, out var obj)
                ? !string.IsNullOrEmpty(obj)
                    ? long.Parse(obj)
                    : default
                : default;
        
        public static float GetFloat(this Dictionary<string, string> models, string key)
            => models.TryGetValue(key, out var obj)
                ? !string.IsNullOrEmpty(obj)
                    ? float.Parse(obj)
                    : default
                : default;
        
        public static double GetDouble(this Dictionary<string, string> models, string key)
            => models.TryGetValue(key, out var obj)
                ? !string.IsNullOrEmpty(obj)
                    ? double.Parse(obj)
                    : default
                : default;
    }
}