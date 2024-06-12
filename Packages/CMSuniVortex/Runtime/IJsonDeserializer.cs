
using Newtonsoft.Json.Linq;

namespace CMSuniVortex
{
    public interface IJsonDeserializer
    {
        void Deserialize(JObject obj);
        void Deserialized();
    }
}