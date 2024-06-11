
using System.Collections.Generic;
using System.IO;

namespace CMSuniVortex.Editor.Cockpit
{
    public sealed class CockpitScriptGenerator : ScriptGenerator
    {
        public override string GetName() => "Cockpit";
        public override string GetLogoName() => "CockpitLogo";

        protected override IEnumerable<(string Path, string Text)> OnGenerate(string namespaceName, string className, string rootPath)
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

            var classPath = Path.Combine(rootPath, className + ".cs");
            if (!File.Exists(classPath))
            {
                yield return (classPath,
                    $@"
using System;
using UnityEngine;
{usings}using CMSuniVortex.Cockpit;

namespace {namespaceName}
{{
    [Serializable]
    public sealed class {className} : CockpitModel
    {{
        public string Text;
        public long Number;
        public Sprite Image;
        public bool Boolean;
        public Color Color;
        public string Date;
        public ItemType Select;
        public TagType[] Tags;

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
            LoadSprite(""image"", asset => Image = asset);
        }}
    }}
}}");
            }
            
            yield return (Path.Combine(rootPath, className + "CockpitCuvModelList.cs"),
                $@"
using CMSuniVortex.Cockpit;

namespace {namespaceName}
{{
    public sealed class {className}CockpitCuvModelList : CockpitCuvModelList<{className}> {{}}
}}");
            
            yield return (Path.Combine(rootPath, className + "CockpitCuvClient.cs"),
                $@"
{usings}using CMSuniVortex.Cockpit;
using Newtonsoft.Json;

namespace {namespaceName}
{{
    public sealed class {className}CockpitCuvClient : CockpitCuvClient<{className}, {className}CockpitCuvModelList>
    {{
        protected override JsonConverter<{className}> CreateConverter()
            => new CuvModelConverter<{className}>();
    }}
}}");
        }
    }
}