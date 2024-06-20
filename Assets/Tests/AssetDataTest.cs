
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using NUnit.Framework;

namespace CMSuniVortex.Tests
{
    public sealed class AssetDataTest
    {
        const string _guid1 = "c14ce121346a949128cb173ac749715f";
        const string _guid2 = "107add0dfec32489aa1d3904ff990be6";
        
        [TestCase(_guid1)]
        public void LoadTest(string listGuid)
        {
            var listPath = AssetDatabase.GUIDToAssetPath(listGuid);
            var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(listPath);
            var sObj = new SerializedObject(obj);
            var iterator = sObj.GetIterator();
            var guids = new HashSet<string>();
            while (iterator.NextVisible(true))
            {
                if (iterator.propertyType == SerializedPropertyType.ObjectReference
                    && iterator.objectReferenceValue != default)
                {
                    var path = AssetDatabase.GetAssetPath(iterator.objectReferenceValue);
                    var guid = AssetDatabase.AssetPathToGUID(path);
                    guids.Add(guid);
                }
            }

            Debug.Log(guids.Aggregate((a,b) => a + "," + b));
        }
        
        [TestCase(_guid1)]
        public void LoadTest2(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            Debug.Log(obj.GetType());
            
        }
    }
}