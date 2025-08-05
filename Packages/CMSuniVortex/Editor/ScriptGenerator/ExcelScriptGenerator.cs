
using System.Collections.Generic;
using System.IO;

namespace CMSuniVortex.Editor.Excel
{
    internal sealed class ExcelScriptGenerator : ScriptGenerator
    {
        public override string GetName() => "Excel";
        public override string GetLogoName() => "ExcelLogo";

        protected override IEnumerable<(string Path, string Text)> OnGenerate(string namespaceName, string className, string rootPath, bool isGenerateOutput, bool useAddressables, bool useLocalization)
        {
            if (string.IsNullOrEmpty(namespaceName))
            {
                namespaceName = "CMSuniVortex";
            }

            var localize = useLocalization ? "Localized" : string.Empty;
            
            {
                var classPath = Path.Combine(rootPath, className + ".cs");
                var addressableNameSpace = useAddressables
                    ? "using UnityEngine.AddressableAssets;\n"
                    : string.Empty;
                var spriteField = useAddressables
                    ? "public AssetReferenceSprite Image;"
                    : "public Sprite Image;";
                var loadSprite = useAddressables
                    ? "LoadSpriteReference(\"Image\", sprite => Image = sprite);"
                    : "LoadSprite(\"Image\", sprite => Image = sprite);";
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using System;
using UnityEngine;
using CMSuniVortex.Excel;
{addressableNameSpace}
namespace {namespaceName}
{{
    [Serializable]
    public sealed class {className} : ExcelModel
    {{
        public ElementType Element;
        public string Text;
        public bool Boolean;
        public int Number;
        public string Date;
        {spriteField}
        
        public enum ElementType {{ Title, Narration, Character1, Character2, Character3, TextOnly }}

        protected override void OnDeserialize()
        {{
            Element = GetEnum<ElementType>(""Element"");
            Text = GetString(""Text"");
            Boolean = GetBool(""Boolean"");
            Number = GetInt(""Number"");
            Date = GetDate(""Date"");
            
            {loadSprite}
        }}
    }}
}}");
                }
            }

            {
                var classPath = Path.Combine(rootPath, className + "ExcelCuvModelList.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using CMSuniVortex.Excel;

namespace {namespaceName}
{{
    public sealed class {className}ExcelCuvModelList : ExcelCuvModelList<{className}> {{}}
}}");
                }
            }

            if(!useAddressables)
            {
                var classPath = Path.Combine(rootPath, $"{className}ExcelCuv{localize}Client.cs");
                if (!File.Exists(classPath))
                {
                    
                    
                    yield return (classPath,
                        $@"
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.Excel;

namespace {namespaceName}
{{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName(""YourCustomName"")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class {className}ExcelCuv{localize}Client : ExcelCuv{localize}Client<{className}, {className}ExcelCuvModelList> {{}}
}}");
                }
            }

            if(isGenerateOutput && !useAddressables)
            {
                var classPath = Path.Combine(rootPath, className + "ExcelCuvReference.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using CMSuniVortex.Excel;

namespace {namespaceName}
{{
    public sealed class {className}ExcelCuvReference : ExcelCuvReference<{className}, {className}ExcelCuvModelList> {{}}
}}");
                }
            }

            if(isGenerateOutput && !useAddressables)
            {
                var classPath = Path.Combine(rootPath, className + "ExcelCuvOutput.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.Excel;

namespace {namespaceName}
{{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName(""YourCustomName"")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class {className}ExcelCuvOutput : ExcelCuvOutput<{className}, {className}ExcelCuvModelList, {className}ExcelCuvReference> {{}}
}}");
                }
            }
            
            if(useAddressables)
            {
                var classPath = Path.Combine(rootPath, $"{className}ExcelCuvAddressable{localize}Client.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.Excel;

namespace {namespaceName}
{{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName(""YourCustomName"")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class {className}ExcelCuvAddressable{localize}Client : ExcelCuvAddressable{localize}Client<{className}, {className}ExcelCuvModelList> {{}}
}}");
                }
            }
            
            if(isGenerateOutput && useAddressables)
            {
                var classPath = Path.Combine(rootPath, className + "ExcelCuvAddressableReference.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using CMSuniVortex.Excel;

namespace {namespaceName}
{{
    public sealed class {className}ExcelCuvAddressableReference : ExcelCuvAddressableReference<{className}, {className}ExcelCuvModelList> {{}}
}}");
                }
            }
            
            if(isGenerateOutput && useAddressables)
            {
                var classPath = Path.Combine(rootPath, className + "ExcelCuvAddressableOutput.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.Excel;

namespace {namespaceName}
{{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName(""YourCustomName"")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class {className}ExcelCuvAddressableOutput : ExcelCuvAddressableOutput<{className}, {className}ExcelCuvModelList, {className}ExcelCuvAddressableReference> {{}}
}}");
                }
            }
        }
    }
}