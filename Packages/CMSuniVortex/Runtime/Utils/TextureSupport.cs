
using System;
using System.IO;
using System.Text.RegularExpressions;
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
            if (importer == null)
            {
                return;
            }
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.SaveAndReimport();
        }
#endif
        
        public static string AppendImageExtension(string imagePath, UnityWebRequest request)
        {
            imagePath = imagePath.TrimEnd('/');
            var fileName = Path.GetFileName(imagePath);
            fileName = Regex.Replace(fileName, "[?<>:*|]", "");
            var directory = Path.GetDirectoryName(imagePath);
            imagePath = Path.Combine(directory, fileName);
            
            var extension = Path.GetExtension(imagePath);
            var isPngOrJpg = !string.IsNullOrEmpty(extension) && (
                extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase));

            if (isPngOrJpg)
            {
                return imagePath;
            }
            
            var contentType = request.GetResponseHeader("Content-Type");
            if (string.IsNullOrEmpty(contentType))
            {
                return imagePath + (string.IsNullOrEmpty(extension) ? ".png" : "");
            }

            string targetExt;
            if (contentType.Contains("image/png"))
            {
                targetExt = ".png";
            }
            else if (contentType.Contains("image/jpeg"))
            {
                targetExt = ".jpg";
            }
            else
            {
                return imagePath + (string.IsNullOrEmpty(extension) ? ".png" : "");
            }

            if (!string.IsNullOrEmpty(extension))
            {
                imagePath = Path.ChangeExtension(imagePath, targetExt);
            }
            else
            {
                imagePath += targetExt;
            }
            
            return imagePath;
        }
    }
}