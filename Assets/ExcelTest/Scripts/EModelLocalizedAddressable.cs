
using System;
using UnityEngine;
using CMSuniVortex.Excel;
using UnityEngine.AddressableAssets;

namespace Test
{
    [Serializable]
    public sealed class EModelLocalizedAddressable : ExcelModel
    {
        public ElementType Element;
        public string Text;
        public bool Boolean;
        public int Number;
        public AssetReferenceSprite Image;
        public string Date;
        
        public enum ElementType { Title, Narration, Character1, Character2, Character3, TextOnly }

        protected override void OnDeserialize()
        {
            Element = GetEnum<ElementType>("Element");
            Text = GetString("Text");
            Boolean = GetBool("Boolean");
            Number = GetInt("Number");
            Date = GetDate("Date");
            
            LoadSpriteReference("Image", sprite => Image = sprite);
        }
    }
}