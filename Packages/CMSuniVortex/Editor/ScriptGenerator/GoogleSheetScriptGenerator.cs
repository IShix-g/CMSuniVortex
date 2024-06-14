
using System.Collections.Generic;
using System.IO;

namespace CMSuniVortex.Editor.GoogleSheet
{
    public sealed class GoogleSheetScriptGenerator : ScriptGenerator
    {
        public override string GetName() => "Custom Google Sheet";
        public override string GetLogoName() => "GoogleSheetLogo";

        protected override IEnumerable<(string Path, string Text)> OnGenerate(string namespaceName, string className, string rootPath)
        {
            if (string.IsNullOrEmpty(namespaceName))
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
            
            yield return (Path.Combine(rootPath, className + "CustomGoogleSheetCuvModelList.cs"),
                $@"
using CMSuniVortex.GoogleSheet;

namespace {namespaceName}
{{
    public sealed class {className}CustomGoogleSheetCuvModelList : CustomGoogleSheetCuvModelList<{className}> {{}}
}}");
            
            yield return (Path.Combine(rootPath, className + "CustomGoogleSheetCuvClient.cs"),
                $@"
using CMSuniVortex.GoogleSheet;

namespace {namespaceName}
{{
    public sealed class {className}CustomGoogleSheetCuvClient : CustomGoogleSheetCuvClient<{className}, {className}CustomGoogleSheetCuvModelList> {{}}
}}");
        }
    }
}