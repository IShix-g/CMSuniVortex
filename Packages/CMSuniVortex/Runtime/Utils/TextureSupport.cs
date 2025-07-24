
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMSuniVortex
{
    public static class TextureSupport
    {
#if UNITY_EDITOR
        public static void SetTextureTypeToSprite(string assetPath)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.SaveAndReimport();
            }
        }
#endif
        
        public static string AppendImageExtension(string imagePath, UnityWebRequest request)
        {
            imagePath = imagePath.TrimEnd('/');
            var fileName = Path.GetFileName(imagePath);
            fileName = Regex.Replace(fileName, "[?<>:*|]", "");
            var directory = Path.GetDirectoryName(imagePath);
            imagePath = Path.Combine(directory, fileName);
            
            if (Path.HasExtension(imagePath)
                && !imagePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                && !imagePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                && !imagePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                imagePath = imagePath.Replace(Path.GetExtension(imagePath), "");
            }

            if (Path.HasExtension(imagePath))
            {
                return imagePath;
            }
            
            var contentType = request.GetResponseHeader("Content-Type");
            if (contentType.Contains("image/png"))
            {
                imagePath += ".png";
            }
            else if (contentType.Contains("image/jpeg"))
            {
                imagePath += ".jpg";
            }
            else
            {
                Debug.LogWarning("Unknown image format from Content-Type header, encoding as PNG instead. path: " + imagePath);
                imagePath += ".png";
            }
            return imagePath;
        }
    }
}