
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CMSuniVortex.GoogleSheet
{
    public static class CustomGoogleSheetModelExtension
    {
        public static void FillContentsWithFilteredSheetData(this IList<IList<object>> sheet, Dictionary<string, string> contents, string keyId, int index)
        {
            contents.Clear();
            for (var s = 0; s < sheet[index].Count; s++)
            {
                var id = sheet[0][s].ToString();
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var value = sheet[index][s].ToString();
                if (id == keyId
                    && string.IsNullOrEmpty(value))
                {
                    continue;
                }
                contents.Add(id, value);
            }
        }
        
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

        public static string GetDate(this Dictionary<string, string> models, string key)
            => models.TryGetValue(key, out var obj)
                ? !string.IsNullOrEmpty(obj)
                  && DateTimeOffset.TryParse(obj, out var date)
                        ? date.ToString("o")
                        : default
                : default;
        
        public static void LoadSprite(this Dictionary<string, string> models, CustomGoogleSheetModel obj, string key, Action<Sprite> onSuccess = default)
            => obj.LoadSprite(models, key, onSuccess);
        
        public static void LoadTexture(this Dictionary<string, string> models, CustomGoogleSheetModel obj, string key, Action<Texture2D> onSuccess = default)
            => obj.LoadTexture(models, key, onSuccess);
    }
}