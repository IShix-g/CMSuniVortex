
using System;
using System.Collections.Generic;
using CMSuniVortex.GoogleSheet;
using UnityEngine;

namespace GoogleSheetTest.Scripts
{
    [Serializable]
    public sealed class TestCustomGoogleSheetModel : CustomGoogleSheetModel
    {
        public ElementType Element;
        public string Text;
        public FacialType Facial;
        public bool Boolean;
        public int Number;
        public Sprite Image;
        
        public enum ElementType { Title, Narration, Character1, Character2, Character3, TextOnly }
        public enum FacialType { Natural, Smile, Frown, Surprise, Anger, Sadness }

        public override void Deserialize(Dictionary<string, string> models)
        {
            Element = models.GetEnum<ElementType>("Element");
            Text = models.GetString("Text");
            Facial = models.GetEnum<FacialType>("Facial");
            Boolean = models.GetBool("Boolean");
            Number = models.GetInt("Number");
            
            LoadSprite(models, "Image", sprite => Image = sprite);
        }
    }
}