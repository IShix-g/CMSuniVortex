#if ENABLE_ADDRESSABLES
using System;
using System.Collections;
using System.Text.RegularExpressions;
using CMSuniVortex.Addressable;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;

namespace CMSuniVortex.Tests
{
    public class AddressableTest
    {
        [Test]
        public static void Test()
        {
            var url = "https://docs.google.com/spreddsheets/d/13XEuxW89jT4ICb2guBcgcgPrCmY_oGxDQgiWNOth7ww/edit?gid=0#gid=0";
            var regex = new Regex(@"spreadsheets/d/([a-zA-Z0-9-_]+)");
            var match = regex.Match(url);
            if (match.Success)
            {
                Debug.Log(match.Groups[1].Value);
            }
        }
        
        [Test]
        public static void CreateGroupLocal()
        {
            AddressableHelper.CreateGroupIfNotExists("Test", AddressableType.Local, true);
        }
        
        [Test]
        public static void CreateGroupRemote()
        {
            AddressableHelper.CreateGroupIfNotExists("Test", AddressableType.Remote, true);
        }
        
        [Test]
        public static void DeleteGroup()
        {
            AddressableHelper.DeleteGroup("Test");
        }
        
        [Test]
        public static void Set()
        {
            var guild = "c14ce121346a949128cb173ac749715f";
            AddressableHelper.AddTo("Test", guild);
        }
        
        [Test]
        public static void SetWithAddress()
        {
            var guild = "c14ce121346a949128cb173ac749715f";
            AddressableHelper.AddTo("Test", guild, "testModelList");
        }
        
        [Test]
        public static void SetWithLabel()
        {
            var guild = "c14ce121346a949128cb173ac749715f";
            AddressableHelper.AddTo("Test", guild, default, new []{ "preLoad", "test2" });
        }
        
        [Test]
        public static void SetWithAddressAndLabel()
        {
            var guild = "c14ce121346a949128cb173ac749715f";
            AddressableHelper.AddTo("Test", guild, default, new []{ "preLoad", "test2" });
        }
        
        [Test]
        public static void Remove()
        {
            var guild = "c14ce121346a949128cb173ac749715f";
            AddressableHelper.RemoveFrom("Test", guild);
        }
        
        [Test]
        public static void SetLabel()
        {
            AddressableHelper.SetLabels(new []{ "preLoad", "test2" });
        }
        
        [Test]
        public static void RemoveLabel()
        {
            AddressableHelper.RemoveLabels(new []{ "preLoad", "test2" });
        }
    }
}
#endif