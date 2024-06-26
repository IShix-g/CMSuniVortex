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
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;

namespace CMSuniVortex.Tests
{
    public class AddressableTest
    {
        [Test]
        public static void CreateGroupLocal()
        {
            var setting = new AddressableCuvSettings
            {
                AddressableType = AddressableType.Local,
                BuildCompressionMode = BundledAssetGroupSchema.BundleCompressionMode.LZMA,
                BundlePackingMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately,
                UpdateRestriction = AddressableCuvSettings.UpdateRestrictionType.CannotChangePostRelease
            };
            AddressableHelper.CreateGroupIfNotExists("Test", setting, true);
        }
        
        [Test]
        public static void CreateGroupRemote()
        {
            var setting = new AddressableCuvSettings
            {
                AddressableType = AddressableType.Remote,
                BuildCompressionMode = BundledAssetGroupSchema.BundleCompressionMode.LZMA,
                BundlePackingMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately,
                UpdateRestriction = AddressableCuvSettings.UpdateRestrictionType.CannotChangePostRelease
            };
            AddressableHelper.CreateGroupIfNotExists("Test", setting, true);
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