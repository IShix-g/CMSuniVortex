
using System.Collections.Generic;
using System.IO;

namespace CMSuniVortex.Editor.GoogleSheet
{
    internal sealed class GoogleSheetScriptGenerator : ScriptGenerator
    {
        public override string GetName() => "Google Sheet";
        public override string GetLogoName() => "GoogleSheetLogo";

        protected override IEnumerable<(string Path, string Text)> OnGenerate(string namespaceName, string className, string rootPath, bool isGenerateOutput, bool useAddressables, bool useLocalization)
        {
            if (string.IsNullOrEmpty(namespaceName))
            {
                namespaceName = "CMSuniVortex";
            }

            var localize = useLocalization ? "Localized" : string.Empty;
            
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

            if(!useAddressables)
            {
                var classPath = Path.Combine(rootPath, $"{className}CustomGoogleSheetCuv{localize}Client.cs");
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
    public sealed class {className}CustomGoogleSheetCuv{localize}Client : CustomGoogleSheetCuv{localize}Client<{className}, {className}CustomGoogleSheetCuvModelList> {{}}
}}");
                }
            }

            if(isGenerateOutput && !useAddressables)
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

            if(isGenerateOutput && !useAddressables)
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
            
            if(useAddressables)
            {
                var classPath = Path.Combine(rootPath, $"{className}CustomGoogleSheetCuvAddressable{localize}Client.cs");
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
    public sealed class {className}CustomGoogleSheetCuvAddressable{localize}Client : CustomGoogleSheetCuvAddressable{localize}Client<{className}, {className}CustomGoogleSheetCuvModelList> {{}}
}}");
                }
            }
            
            if(isGenerateOutput && useAddressables)
            {
                var classPath = Path.Combine(rootPath, className + "CustomGoogleSheetCuvAddressableReference.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using CMSuniVortex.GoogleSheet;

namespace {namespaceName}
{{
    public sealed class {className}CustomGoogleSheetCuvAddressableReference : CustomGoogleSheetCuvAddressableReference<{className}, {className}CustomGoogleSheetCuvModelList> {{}}
}}");
                }
            }
            
            if(isGenerateOutput && useAddressables)
            {
                var classPath = Path.Combine(rootPath, className + "CustomGoogleSheetCuvAddressableOutput.cs");
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
    public sealed class {className}CustomGoogleSheetCuvAddressableOutput : CustomGoogleSheetCuvAddressableOutput<{className}, {className}CustomGoogleSheetCuvModelList, {className}CustomGoogleSheetCuvAddressableReference> {{}}
}}");
                }
            }
        }
    }
}