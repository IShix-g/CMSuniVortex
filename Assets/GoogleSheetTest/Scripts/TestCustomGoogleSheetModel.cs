
using System;
using System.Collections.Generic;
using CMSuniVortex.GoogleSheet;
using UnityEngine;

namespace GoogleSheetTest
{
    [Serializable]
    public sealed class TestCustomGoogleSheetModel : CustomGoogleSheetModel
    {
        public ElementType Element;
        public string Text;
        public bool Boolean;
        public int Number;
        public Sprite Image;
        public string Date;
        
        public enum ElementType { Title, Narration, Character1, Character2, Character3, TextOnly }

        public override void Deserialize(Dictionary<string, string> models)
        {
            Element = models.GetEnum<ElementType>("Element");
            Text = models.GetString("Text");
            Boolean = models.GetBool("Boolean");
            Number = models.GetInt("Number");
            Date = models.GetDate("Date");
            
            models.LoadSprite(this, "Image", sprite => Image = sprite);
        }
    }
}