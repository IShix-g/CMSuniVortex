
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CMSuniVortex
{
    /// <summary>
    /// A generic JSON converter for CuvModel objects.
    /// </summary>
    public sealed class CuvModelConverter<T> : JsonConverter<T> where T : ICuvModel, new()
    {
        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
#if UNITY_EDITOR
            var obj = JObject.Load(reader);
            var model = new T();
            model.Deserialize(obj);
            return model;
#else
        throw new NotImplementedException();
#endif
        }
    }
}