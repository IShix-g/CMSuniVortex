
using System;
using UnityEngine;
using CMSuniVortex.Excel;

namespace Test
{
    [Serializable]
    public sealed class EModelLocalized : ExcelModel
    {
        public ElementType Element;
        public string Text;
        public bool Boolean;
        public int Number;
        public Sprite Image;
        public string Date;
        
        public enum ElementType { Title, Narration, Character1, Character2, Character3, TextOnly }

        protected override void OnDeserialize()
        {
            Element = GetEnum<ElementType>("Element");
            Text = GetString("Text");
            Boolean = GetBool("Boolean");
            Number = GetInt("Number");
            Date = GetDate("Date");
            
            LoadSprite("Image", sprite => Image = sprite);
        }
    }
}