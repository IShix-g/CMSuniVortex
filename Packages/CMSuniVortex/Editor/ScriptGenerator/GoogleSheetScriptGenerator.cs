
using System.Collections.Generic;
using System.IO;

namespace CMSuniVortex.Editor.GoogleSheet
{
    sealed class GoogleSheetScriptGenerator : ScriptGenerator
    {
        public override string GetName() => "Custom Google Sheet";
        public override string GetLogoName() => "GoogleSheetLogo";

        protected override IEnumerable<(string Path, string Text)> OnGenerate(string namespaceName, string className, string rootPath, bool isGenerateOutput)
        {
            if (string.IsNullOrEmpty(namespaceName))
            {
                namespaceName = "CMSuniVortex";
            }

            {
                var classPath = Path.Combine(rootPath, className + ".cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using System;
using UnityEngine;
using CMSuniVortex.GoogleSheet;

namespace {namespaceName}
{{
    [Serializable]
    public sealed class {className} : CustomGoogleSheetModel
    {{
        public ElementType Element;
        public string Text;
        public bool Boolean;
        public int Number;
        public Sprite Image;
        public string Date;
        
        public enum ElementType {{ Title, Narration, Character1, Character2, Character3, TextOnly }}

        protected override void OnDeserialize()
        {{
            Element = GetEnum<ElementType>(""Element"");
            Text = GetString(""Text"");
            Boolean = GetBool(""Boolean"");
            Number = GetInt(""Number"");
            Date = GetDate(""Date"");
            
            LoadSprite(""Image"", sprite => Image = sprite);
        }}
    }}
}}");
                }
            }

            {
                var classPath = Path.Combine(rootPath, className + "CustomGoogleSheetCuvModelList.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using CMSuniVortex.GoogleSheet;

namespace {namespaceName}
{{
    public sealed class {className}CustomGoogleSheetCuvModelList : CustomGoogleSheetCuvModelList<{className}> {{}}
}}");
                }
            }

            {
                var classPath = Path.Combine(rootPath, className + "CustomGoogleSheetCuvClient.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.GoogleSheet;

namespace {namespaceName}
{{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName(""YourCustomName"")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class {className}CustomGoogleSheetCuvClient : CustomGoogleSheetCuvClient<{className}, {className}CustomGoogleSheetCuvModelList> {{}}
}}");
                }
            }

            if(isGenerateOutput)
            {
                var classPath = Path.Combine(rootPath, className + "CustomGoogleSheetCuvReference.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using CMSuniVortex.GoogleSheet;

namespace {namespaceName}
{{
    public sealed class {className}CustomGoogleSheetCuvReference : CustomGoogleSheetCuvReference<{className}, {className}CustomGoogleSheetCuvModelList> {{}}
}}");
                }
            }

            if(isGenerateOutput)
            {
                var classPath = Path.Combine(rootPath, className + "CustomGoogleSheetCuvOutput.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.GoogleSheet;

namespace {namespaceName}
{{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName(""YourCustomName"")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class {className}CustomGoogleSheetCuvOutput : CustomGoogleSheetCuvOutput<{className}, {className}CustomGoogleSheetCuvModelList, {className}CustomGoogleSheetCuvReference> {{}}
}}");
                }
            }
            
            {
                var classPath = Path.Combine(rootPath, className + "CustomGoogleSheetCuvAddressableClient.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
#if ENABLE_ADDRESSABLES
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.GoogleSheet;

namespace {namespaceName}
{{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName(""YourCustomName"")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class {className}CustomGoogleSheetCuvAddressableClient : CustomGoogleSheetCuvAddressableClient<{className}, {className}CustomGoogleSheetCuvModelList> {{}}
}}
#endif");
                }
            }
            
            if(isGenerateOutput)
            {
                var classPath = Path.Combine(rootPath, className + "CustomGoogleSheetCuvAddressableReference.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
#if ENABLE_ADDRESSABLES
using CMSuniVortex.GoogleSheet;

namespace {namespaceName}
{{
    public sealed class {className}CustomGoogleSheetCuvAddressableReference : CustomGoogleSheetCuvAddressableReference<{className}, {className}CustomGoogleSheetCuvModelList> {{}}
}}
#endif");
                }
            }
            
            if(isGenerateOutput)
            {
                var classPath = Path.Combine(rootPath, className + "CustomGoogleSheetCuvAddressableOutput.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
#if ENABLE_ADDRESSABLES
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.GoogleSheet;

namespace {namespaceName}
{{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName(""YourCustomName"")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class {className}CustomGoogleSheetCuvAddressableOutput : CustomGoogleSheetCuvAddressableOutput<{className}, {className}CustomGoogleSheetCuvModelList, {className}CustomGoogleSheetCuvAddressableReference> {{}}
}}
#endif");
                }
            }
        }
    }
}