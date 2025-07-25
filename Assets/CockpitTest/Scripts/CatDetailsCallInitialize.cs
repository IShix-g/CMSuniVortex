
using System;
using UnityEngine;
using CMSuniVortex.Cockpit;

namespace Tests
{
    [Serializable]
    public sealed class CatDetailsCallInitialize : CockpitModel
    {
        public string Text;
        public long Number;
        public Sprite Image;
        public bool Boolean;
        public Color Color;
        public string Date;
        public ItemType Select;
        public TagType[] Tags;
        public string Param;

        public enum TagType { Tag1, Tag2, Tag3 }

        public enum ItemType { Item1, Item2, Item3 }

        protected override void OnDeserialize()
        {
            Text = GetString("text");
            Number = GetLong("number");
            Boolean = GetBool("boolean");
            Color = GetColor("color");
            Date = GetDate("date");
            Select = GetSelect<ItemType>("select");
            Tags = GetTag<TagType>("tags");
            Param = GetString("param");
            LoadSprite("image", asset => Image = asset);
        }
    }
}