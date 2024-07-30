#if UNITY_EDITOR && ENABLE_ADDRESSABLES
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets;
using UnityEngine.AddressableAssets;

using Object = UnityEngine.Object;

namespace CMSuniVortex.Addressable
{
    public static class AddressableHelper
    {
        public static bool IsInstalled() => AddressableAssetSettingsDefaultObject.Settings != default;
        
        public static AddressableAssetSettings GetSettings()
        {
            if (!IsInstalled())
            {
                throw new InvalidOperationException("Addressables is not installed. Execute \"Create Addressables Settings\" in Window > Asset Management > Addressables > Group. For more Infomation : https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsGettingStarted.html#installation");
            }
            return AddressableAssetSettingsDefaultObject.Settings;
        }
        
        public static bool HasGroup(string groupName) => IsInstalled() && GetSettings().FindGroup(groupName) != default;
        
        public static void CreateGroupIfNotExists(string groupName, AddressableCuvSettings cuvSettings, bool isOverride)
        {
            var settings = GetSettings();
            var group = settings.FindGroup(groupName);
            if (!isOverride
                && group != default)
            {
                return;
            }

            if (group == default)
            {
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
            }
            
            {
                var schema = group.GetSchema<BundledAssetGroupSchema>();
                var buildPathName = cuvSettings.AddressableType == AddressableType.Local
                    ? AddressableAssetSettings.kLocalBuildPath
                    : AddressableAssetSettings.kRemoteBuildPath;
                var loadPathName = cuvSettings.AddressableType == AddressableType.Local
                    ? AddressableAssetSettings.kLocalLoadPath
                    : AddressableAssetSettings.kRemoteLoadPath;
                schema.BuildPath.SetVariableByName ( settings, buildPathName );
                schema.LoadPath.SetVariableByName( settings, loadPathName );
                schema.BundleMode = cuvSettings.BundlePackingMode;
                schema.Compression = cuvSettings.BuildCompressionMode;
                schema.RetryCount = cuvSettings.RetryCount;
            }
            {
                var schema = group.GetSchema<ContentUpdateGroupSchema>();
                schema.StaticContent = cuvSettings.UpdateRestriction == AddressableCuvSettings.UpdateRestrictionType.CannotChangePostRelease;
            }
        }
        
        public static void DeleteGroup(string groupName)
        {
            if (!IsInstalled())
            {
                return;
            }
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var group = settings.FindGroup(groupName);
            if (group != default)
            {
                settings.RemoveGroup(group);
            }
        }
        
        public static string GetAddress(Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(path);
            return GetAddress(guid);
        }

        public static string GetAddress(string guid)
        {
            var settings = GetSettings();
            var entry = settings.FindAssetEntry(guid);
            return entry?.address;
        }
        
        public static void AddTo(string groupName, Object obj, string newAddress = default, string[] labels = default)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(path);
            AddTo(groupName, guid, newAddress, labels);
        }

        public static void AddTo(string groupName, string guid, string newAddress = default, string[] labels = default)
        {
            var settings = GetSettings();
            var group = settings.FindGroup(groupName);
            if (group == default)
            {
                throw new ArgumentException("A non-existent Addressable Group was specified. group name:" + groupName);
            }
            var entry = settings.CreateOrMoveEntry(guid, group, false, false);
            entry.address = newAddress;

            if (entry.labels is {Count: > 0})
            {
                var labelList = new List<string>(entry.labels);
                foreach (var label in labelList)
                {
                    if (!string.IsNullOrEmpty(label))
                    {
                        entry.SetLabel(label, false);
                    }
                }
            }
            
            if (labels is {Length: > 0})
            {
                SetLabels(labels);
                
                foreach (var label in labels)
                {
                    if (!string.IsNullOrEmpty(label))
                    {
                        entry.SetLabel(label, true);
                    }
                }
            }
            
            var entries = new List<AddressableAssetEntry> { entry };
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entries, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entries, true, false);
        }

        public static void RemoveFromAll(string groupName)
        {
            var settings = GetSettings();
            var group = settings.FindGroup(groupName);
            if (group == default)
            {
                Debug.Log($"Group {groupName} does not exist.");
                return;
            }

            if (group.entries.Count == 0)
            {
                Debug.Log("No assets were found in the Group. name: " + groupName);
                return;
            }
            
            var entries = new List<AddressableAssetEntry>(group.entries);
            foreach (var entry in entries)
            {
                group.RemoveAssetEntry(entry);
            }
            Debug.Log("Removed all assets from group. name: " + groupName);
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, entries, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, entries, true, false);
        }
        
        public static void RemoveFrom(string groupName, Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(path);
            RemoveFrom(groupName, guid);
        }
        
        public static void RemoveFrom(string groupName, string guid)
        {
            var settings = GetSettings();
            var group = settings.FindGroup(groupName);
            if (group == default)
            {
                Debug.Log($"Group {groupName} does not exist.");
                return;
            }
            var entry = settings.FindAssetEntry(guid);
            if (entry == default)
            {
                return;
            }
            settings.RemoveAssetEntry(guid);
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, null, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, null, true, false);
        }
        
        public static bool HasAsset(Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(path);
            return HasAsset(guid);
        }
        
        public static bool HasAsset(string guid)
        {
            var settings = GetSettings();
            var entry = settings.FindAssetEntry(guid);
            return entry != default;
        }
        
        public static void AttachAsset<T>(this AssetReferenceT<T> assetRef, Object obj) where T : Object
        {
            if (!HasAsset(obj))
            {
                var path = AssetDatabase.GetAssetPath(obj);
                Debug.LogError("The target asset is not addressable. target: " + path);
                return;
            }
            assetRef.SetEditorAsset(obj);
        }
        
        public static void SetLabels(string[] labels)
        {
            var settings = GetSettings();
            var labelNames = settings.GetLabels();
            foreach (var label in labels)
            {
                if (!string.IsNullOrEmpty(label)
                    && !labelNames.Contains(label))
                {
                    settings.AddLabel(label);
                }
            }
        }
        
        public static void RemoveLabels(string[] labels)
        {
            var settings = GetSettings();
            var labelNames = settings.GetLabels();
            foreach (var label in labels)
            {
                if (labelNames.Contains(label))
                {
                    settings.RemoveLabel(label);
                }
            }
        }
    }
}
#endif