
using System;
using System.Collections.Generic;
using CMSuniVortex.Cockpit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;

namespace CMSuniVortex.Tests
{
    public sealed class CockpitTest
    {
        const string _testData = "[{\"title\":\"Cat\",\"image\":{\"path\":\"/2024/06/02/cat_uid_665c9b0cd8113.jpeg\",\"title\":\"Cat\",\"mime\":\"image/jpeg\",\"type\":\"image\",\"description\":\"\",\"tags\":[],\"size\":301431,\"colors\":[\"#a68663\",\"#2c2220\",\"#f3e4be\",\"#61351f\",\"#e5d0a4\"],\"width\":1024,\"height\":1024,\"_hash\":\"b7954f7cc5f9ddbbcd6f6aea1c3e6aa2\",\"_created\":1717345036,\"_modified\":1717345036,\"_cby\":\"665c919e84008b446702dda5\",\"altText\":\"cat.jpeg\",\"thumbhash\":\"96-57-10-7-0-151-200-247-127-117-137-140-114-118-104-135-104-121-119-143-36-167-96-3\",\"folder\":\"\",\"_id\":\"665c9b0cabaf2041bc015462\"},\"_modified\":1717518962,\"_mby\":\"665c919e84008b446702dda5\",\"_created\":1717345118,\"_state\":1,\"_cby\":\"665c919e84008b446702dda5\",\"_id\":\"665c9b5e1c923559e8052b40\",\"text\":\"Cat\",\"is-active\":true,\"color\":\"#ff1f1f\",\"link\":null,\"date\":\"2024-06-01\",\"Text\":\"Cat\",\"Image\":{\"path\":\"/2024/06/02/cat_uid_665c9b0cd8113.jpeg\",\"title\":\"Cat\",\"mime\":\"image/jpeg\",\"type\":\"image\",\"description\":\"\",\"tags\":[],\"size\":301431,\"colors\":[\"#a68663\",\"#2c2220\",\"#f3e4be\",\"#61351f\",\"#e5d0a4\"],\"width\":1024,\"height\":1024,\"_hash\":\"b7954f7cc5f9ddbbcd6f6aea1c3e6aa2\",\"_created\":1717345036,\"_modified\":1717345036,\"_cby\":\"665c919e84008b446702dda5\",\"altText\":\"cat.jpeg\",\"thumbhash\":\"96-57-10-7-0-151-200-247-127-117-137-140-114-118-104-135-104-121-119-143-36-167-96-3\",\"folder\":\"\",\"_id\":\"665c9b0cabaf2041bc015462\"},\"IsActive\":true,\"Color\":\"#ff0000\",\"Date\":\"2024-06-01\",\"_text\":\"Cat1\",\"_image\":{\"path\":\"/2024/06/02/cat_uid_665c9b0cd8113.jpeg\",\"title\":\"Cat\",\"mime\":\"image/jpeg\",\"type\":\"image\",\"description\":\"\",\"tags\":[],\"size\":301431,\"colors\":[\"#a68663\",\"#2c2220\",\"#f3e4be\",\"#61351f\",\"#e5d0a4\"],\"width\":1024,\"height\":1024,\"_hash\":\"b7954f7cc5f9ddbbcd6f6aea1c3e6aa2\",\"_created\":1717345036,\"_modified\":1717345036,\"_cby\":\"665c919e84008b446702dda5\",\"altText\":\"cat.jpeg\",\"thumbhash\":\"96-57-10-7-0-151-200-247-127-117-137-140-114-118-104-135-104-121-119-143-36-167-96-3\",\"folder\":\"\",\"_id\":\"665c9b0cabaf2041bc015462\"},\"_isActive\":null,\"_color\":\"#ff0000\",\"_date\":\"2024-06-01\",\"isActive\":null,\"boolean\":true,\"number\":1111,\"select\":\"Item1\",\"set\":null,\"tag\":[\"Tag1\",\"Tag2\"],\"tags\":[\"Tag1\",\"Tag2\"]},{\"title\":\"Cat2\",\"image\":{\"path\":\"/2024/06/02/_e0b6fe31-5979-4065-84b4-d372126b52a0_uid_665c9c0fc46a8.jpeg\",\"title\":\"e0b6fe315979406584b4d372126b52a0\",\"mime\":\"image/jpeg\",\"type\":\"image\",\"description\":\"\",\"tags\":[],\"size\":256747,\"colors\":[\"#2a3b46\",\"#ebe1cd\",\"#9e6543\",\"#d0925e\",\"#959893\"],\"width\":1024,\"height\":1024,\"_hash\":\"b4dd01f212bedab340bc2437dd80ecfb\",\"_created\":1717345295,\"_modified\":1717345295,\"_cby\":\"665c919e84008b446702dda5\",\"altText\":\"_e0b6fe31-5979-4065-84b4-d372126b52a0.jpeg\",\"thumbhash\":\"233-24-10-15-2-224-199-172-171-136-136-126-116-167-120-119-120-134-183-7-196-94-64-10\",\"folder\":\"\",\"_id\":\"665c9c0f1c923559e8052b41\"},\"_state\":1,\"_modified\":1717517482,\"_mby\":\"665c919e84008b446702dda5\",\"_created\":1717345310,\"_cby\":\"665c919e84008b446702dda5\",\"_id\":\"665c9c1e1c923559e8052b42\",\"text\":\"Cat2\",\"is-active\":true,\"color\":\"#b833ff\",\"date\":\"2024-03-01\",\"Text\":\"Cat2\",\"Image\":{\"path\":\"/2024/06/02/_e0b6fe31-5979-4065-84b4-d372126b52a0_uid_665c9c0fc46a8.jpeg\",\"title\":\"e0b6fe315979406584b4d372126b52a0\",\"mime\":\"image/jpeg\",\"type\":\"image\",\"description\":\"\",\"tags\":[],\"size\":256747,\"colors\":[\"#2a3b46\",\"#ebe1cd\",\"#9e6543\",\"#d0925e\",\"#959893\"],\"width\":1024,\"height\":1024,\"_hash\":\"b4dd01f212bedab340bc2437dd80ecfb\",\"_created\":1717345295,\"_modified\":1717345295,\"_cby\":\"665c919e84008b446702dda5\",\"altText\":\"_e0b6fe31-5979-4065-84b4-d372126b52a0.jpeg\",\"thumbhash\":\"233-24-10-15-2-224-199-172-171-136-136-126-116-167-120-119-120-134-183-7-196-94-64-10\",\"folder\":\"\",\"_id\":\"665c9c0f1c923559e8052b41\"},\"IsActive\":false,\"Color\":\"#00d5ff\",\"Date\":\"2024-06-02\",\"_text\":\"Cat2\",\"_image\":{\"path\":\"/2024/06/02/_e0b6fe31-5979-4065-84b4-d372126b52a0_uid_665c9c0fc46a8.jpeg\",\"title\":\"e0b6fe315979406584b4d372126b52a0\",\"mime\":\"image/jpeg\",\"type\":\"image\",\"description\":\"\",\"tags\":[],\"size\":256747,\"colors\":[\"#2a3b46\",\"#ebe1cd\",\"#9e6543\",\"#d0925e\",\"#959893\"],\"width\":1024,\"height\":1024,\"_hash\":\"b4dd01f212bedab340bc2437dd80ecfb\",\"_created\":1717345295,\"_modified\":1717345295,\"_cby\":\"665c919e84008b446702dda5\",\"altText\":\"_e0b6fe31-5979-4065-84b4-d372126b52a0.jpeg\",\"thumbhash\":\"233-24-10-15-2-224-199-172-171-136-136-126-116-167-120-119-120-134-183-7-196-94-64-10\",\"folder\":\"\",\"_id\":\"665c9c0f1c923559e8052b41\"},\"_isActive\":true,\"_color\":\"#00ff59\",\"_date\":\"2024-06-02\",\"isActive\":null,\"boolean\":false,\"number\":2222,\"select\":\"Item2\",\"set\":null,\"tag\":[\"Tag2\",\"Tag3\"],\"tags\":[\"Tag2\",\"Tag3\"]}]";

        [Serializable, JsonConverter(typeof(CuvModelConverter<TestModel>))]
        public sealed class TestModel : CockpitModel
        {
            public string Text;
            public long Number;
            public bool Boolean;
            public Color Color;
            public string Date;
            public ItemType Select;
            public TagType[] Tags;

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
            }
        }

        [Test]
        public void DeserializeTest()
        {
            var models = JsonConvert.DeserializeObject<TestModel[]>(_testData);
            Assert.AreEqual(2, models.Length);

            {
                var model = models[0];
                Assert.AreEqual("Cat", model.Text);
                Assert.AreEqual(1111, model.Number);
                Assert.AreEqual(true, model.Boolean);
                Assert.AreEqual("ff1f1f", ColorUtility.ToHtmlStringRGB(model.Color).ToLower());
                StringAssert.Contains("2024-06-01", model.Date);
                Assert.AreEqual(TestModel.ItemType.Item1, model.Select);
                Assert.AreEqual(new[] {TestModel.TagType.Tag1, TestModel.TagType.Tag2}, model.Tags);
            }

            {
                var model = models[1];
                Assert.AreEqual("Cat2", model.Text);
                Assert.AreEqual(2222, model.Number);
                Assert.AreEqual(false, model.Boolean);
                Assert.AreEqual("b833ff", ColorUtility.ToHtmlStringRGB(model.Color).ToLower());
                StringAssert.Contains("2024-03-01", model.Date);
                Assert.AreEqual(TestModel.ItemType.Item2, model.Select);
                Assert.AreEqual(new[] {TestModel.TagType.Tag2, TestModel.TagType.Tag3}, model.Tags);
            }
        }
        
        class TestModel2 : CockpitModel
        {
            IReadOnlyDictionary<string, TestEnum> _maps;
            
            public enum TestEnum { Enum1, Enum2 }
            
            public void SetMap(IReadOnlyDictionary<string, TestEnum> maps) => _maps = maps;
            
            protected override void OnDeserialize()
            {
                {
                    var result = TryGetSelect<TestEnum>("A", _maps, out var value);
                    Assert.That(result, Is.EqualTo(CuvEnumResult.Success));
                    Assert.That(value, Is.EqualTo(TestEnum.Enum1));
                }
                {
                    var result = TryGetSelect<TestEnum>("B", _maps, out var value);
                    Assert.That(result, Is.EqualTo(CuvEnumResult.EmptyValue));
                    Assert.That(value, Is.EqualTo(TestEnum.Enum1));
                }
                {
                    var result = TryGetSelect<TestEnum>("C", _maps, out var value);
                    Assert.That(result, Is.EqualTo(CuvEnumResult.NotFoundInMap));
                    Assert.That(value, Is.EqualTo(TestEnum.Enum1));
                }
                {
                    var result = TryGetSelect<TestEnum>("Z", _maps, out var value);
                    Assert.That(result, Is.EqualTo(CuvEnumResult.NotHasKey));
                    Assert.That(value, Is.EqualTo(TestEnum.Enum1));
                }
            }
        }
        
        [Test]
        public void EnumDeserializeTest()
        {
            var model = new TestModel2();
            model.SetMap(new Dictionary<string, TestModel2.TestEnum>()
            {
                {"AA", TestModel2.TestEnum.Enum1},
                {"BB", TestModel2.TestEnum.Enum2},
            });
            var dictionary = new Dictionary<string, string>()
            {
                {"A", "AA"},
                {"B", ""},
                {"C", "CC"},
            };
            var obj = JObject.FromObject(dictionary);
            ((IJsonDeserializer) model).Deserialize(obj);
        }
    }
}