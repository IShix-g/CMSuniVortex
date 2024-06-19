#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if ENABLE_ADDRESSABLES
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
#endif

using Object = UnityEngine.Object;

namespace CMSuniVortex.Addressable
{
    public static class AddressableHelper
    {
        public static bool IsInstalled()
        {
#if ENABLE_ADDRESSABLES
            return AddressableAssetSettingsDefaultObject.Settings != default;
#else
            return false;
#endif
        }
        
#if ENABLE_ADDRESSABLES
        public static AddressableAssetSettings GetSettings()
        {
            if (!IsInstalled())
            {
                throw new InvalidOperationException("Addressables is not installed.");
            }
            return AddressableAssetSettingsDefaultObject.Settings;
        }
#endif
        
        public static bool HasGroup(string groupName)
        {
#if ENABLE_ADDRESSABLES
            return IsInstalled() && AddressableAssetSettingsDefaultObject.Settings.FindGroup(groupName) != default;
#else
            return false;
#endif
        }
        
        public static void CreateGroupIfNotExists(string groupName, AddressablePathType type)
        {
#if ENABLE_ADDRESSABLES
            var settings = GetSettings();
            var group = settings.FindGroup(groupName);
            if (group != default)
            {
                return;
            }
            group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
            var schema = group.GetSchema<BundledAssetGroupSchema>();
            var buildPathName = type == AddressablePathType.Local ? "Local.BuildPath" : "Remote.BuildPath";
            var loadPathName = type == AddressablePathType.Local ? "Local.LoadPath" : "Remote.LoadPath";
            schema.BuildPath.SetVariableByName ( settings, buildPathName );
            schema.LoadPath.SetVariableByName( settings, loadPathName );
#endif
        }
        
        public static void DeleteGroup(string groupName)
        {
#if ENABLE_ADDRESSABLES
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
#endif
        }
        
        public static string GetAddress(Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(path);
            return GetAddress(guid);
        }

        public static string GetAddress(string guid)
        {
#if ENABLE_ADDRESSABLES
            var settings = GetSettings();
            var entry = settings.FindAssetEntry(guid);
            return entry?.address;
#endif
        }
        
        public static void AddTo(string groupName, Object obj, string newAddress = default, string[] labels = default)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(path);
            AddTo(groupName, guid, newAddress, labels);
        }

        public static void AddTo(string groupName, string guid, string newAddress = default, string[] labels = default)
        {
#if ENABLE_ADDRESSABLES
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
                    entry.SetLabel(label, false);
                }
            }
            
            if (labels is {Length: > 0})
            {
                SetLabels(labels);
                
                foreach (var label in labels)
                {
                    entry.SetLabel(label, true);
                }
            }
            
            var entriesAdded = new List<AddressableAssetEntry> { entry };
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
#endif
        }

        public static void RemoveFrom(string groupName, Object obj)
        {
#if ENABLE_ADDRESSABLES
            var path = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(path);
            RemoveFrom(groupName, guid);
#endif
        }
        
        public static void RemoveFrom(string groupName, string guid)
        {
#if ENABLE_ADDRESSABLES
            var settings = GetSettings();
            var group = settings.FindGroup(groupName);
            if (group == default)
            {
                throw new ArgumentException("A non-existent Addressable Group was specified. group name:" + groupName);
            }
            var entry = settings.FindAssetEntry(guid);
            if (entry == null)
            {
                return;
            }
            settings.RemoveAssetEntry(guid);
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, null, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, null, true, false);
#endif
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
        
#if ENABLE_ADDRESSABLES
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
#endif
        
        public static void SetLabels(string[] labels)
        {
            var settings = GetSettings();
            var labelNames = settings.GetLabels();
            foreach (var label in labels)
            {
                if (!labelNames.Contains(label))
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