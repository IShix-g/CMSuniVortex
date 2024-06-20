
using System;
using UnityEngine;
using CMSuniVortex.Cockpit;
using UnityEngine.AddressableAssets;

namespace Tests
{
    [Serializable]
    public sealed class CatAddressableDetails : CockpitModel
    {
        public string Text;
        public long Number;
        public AssetReferenceSprite Image;
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
            LoadSpriteReference("image", asset => Image = asset);
        }
    }
}