
using System.Collections.Generic;
using System.IO;

namespace CMSuniVortex.Editor.Cockpit
{
    internal sealed class CockpitScriptGenerator : ScriptGenerator
    {
        public override string GetName() => "Cockpit";
        public override string GetLogoName() => "CockpitLogo";

        protected override IEnumerable<(string Path, string Text)> OnGenerate(string namespaceName, string className, string rootPath, bool isGenerateOutput, bool useAddressables, bool useLocalization)
        {
            var usings = string.Empty;
            if (!string.IsNullOrEmpty(namespaceName))
            {
                usings = "using CMSuniVortex;\n";
            }
            else
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
using CMSuniVortex.Cockpit;
{addressableNameSpace}
namespace {namespaceName}
{{
    [Serializable]
    public sealed class {className} : CockpitModel
    {{
        public string Text;
        public long Number;
        public bool Boolean;
        public Color Color;
        public string Date;
        public ItemType Select;
        public TagType[] Tags;
        public string Param;
        {spriteField}

        public enum TagType {{ Tag1, Tag2, Tag3 }}

        public enum ItemType {{ Item1, Item2, Item3 }}

        protected override void OnDeserialize()
        {{
            Text = GetString(""text"");
            Number = GetLong(""number"");
            Boolean = GetBool(""boolean"");
            Color = GetColor(""color"");
            Date = GetDate(""date"");
            Select = GetSelect<ItemType>(""select"");
            Tags = GetTag<TagType>(""tags"");
            Param = GetString(""param"");

            {loadSprite}
        }}
    }}
}}");
                }
            }

            {
                var classPath = Path.Combine(rootPath, className + "CockpitCuvModelList.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using CMSuniVortex.Cockpit;

namespace {namespaceName}
{{
    public sealed class {className}CockpitCuvModelList : CockpitCuvModelList<{className}> {{}}
}}");
                }
            }
            
            if(!useAddressables)
            {
                
                var classPath = Path.Combine(rootPath, $"{className}CockpitCuv{localize}Client.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                    $@"
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.Cockpit;
using Newtonsoft.Json;

namespace {namespaceName}
{{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName(""YourCustomName"")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class {className}CockpitCuv{localize}Client : CockpitCuv{localize}Client<{className}, {className}CockpitCuvModelList>
    {{
        protected override JsonConverter<{className}> CreateConverter()
            => new CuvModelConverter<{className}>();
    }}
}}"); 
                }
            }

            if(isGenerateOutput && !useAddressables)
            {
                var classPath = Path.Combine(rootPath, className + "CockpitCuvReference.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using CMSuniVortex.Cockpit;

namespace {namespaceName}
{{
    public sealed class {className}CockpitCuvReference : CockpitCuvReference<{className}, {className}CockpitCuvModelList> {{}}
}}");
                }
            }

            if(isGenerateOutput && !useAddressables)
            {
                var classPath = Path.Combine(rootPath, className + "CockpitCuvOutput.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.Cockpit;

namespace {namespaceName}
{{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName(""YourCustomName"")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class {className}CockpitCuvOutput : CockpitCuvOutput<{className}, {className}CockpitCuvModelList, {className}CockpitCuvReference> {{}}
}}");
                }
            }

            if(useAddressables)
            {
                var classPath = Path.Combine(rootPath, $"{className}CockpitCuvAddressable{localize}Client.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.Cockpit;
using Newtonsoft.Json;

namespace {namespaceName}
{{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName(""YourCustomName"")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class {className}CockpitCuvAddressable{localize}Client : CockpitCuvAddressable{localize}Client<{className}, {className}CockpitCuvModelList>
    {{
        protected override JsonConverter<{className}> CreateConverter()
            => new CuvModelConverter<{className}>();
    }}
}}");
                }
            }

            if(isGenerateOutput && useAddressables)
            {
                var classPath = Path.Combine(rootPath, className + "CockpitCuvAddressableReference.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using CMSuniVortex.Cockpit;

namespace {namespaceName}
{{
    public sealed class {className}CockpitCuvAddressableReference : CockpitCuvAddressableReference<{className}, {className}CockpitCuvModelList> {{}}
}}");
                }
            }

            if(isGenerateOutput && useAddressables)
            {
                var classPath = Path.Combine(rootPath, className + "CockpitCuvAddressableOutput.cs");
                if (!File.Exists(classPath))
                {
                    yield return (classPath,
                        $@"
using System.ComponentModel;
using CMSuniVortex;
using CMSuniVortex.Cockpit;

namespace {namespaceName}
{{
    // [CuvIgnore] // Enabling this attribute will exclude it from the Client drop-down.
    // [DisplayName(""YourCustomName"")] // Enabling this attribute changes the name on the client drop-down.
    public sealed class {className}CockpitCuvAddressableOutput : CockpitCuvAddressableOutput<{className}, {className}CockpitCuvModelList, {className}CockpitCuvAddressableReference> {{}}
}}");
                }
            }
        }
    }
}