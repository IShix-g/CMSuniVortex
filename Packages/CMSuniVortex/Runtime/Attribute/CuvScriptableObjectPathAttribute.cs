
using System;
using UnityEngine.Assertions;

namespace CMSuniVortex
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class CuvScriptableObjectPathAttribute : Attribute
    {
        public string FilePath { get; private set; }

        public CuvScriptableObjectPathAttribute(string filePath)
        {
            FilePath = !string.IsNullOrEmpty(filePath)
                ? filePath.Trim().TrimEnd('/')
                : throw new ArgumentException("Invalid relative path (it is empty)");

            Assert.IsTrue(FilePath.StartsWith("Assets/"), "Please specify the path starting from 'Assets/'. filePath: " + filePath);
            Assert.IsTrue(FilePath.Contains("/Resources/"), "Please specify the Resources directory. filePath: " + filePath);
            Assert.IsTrue(FilePath.EndsWith(".asset"), "Please specify the path ending with '.asset'. filePath: " + filePath);
        }
    }
}