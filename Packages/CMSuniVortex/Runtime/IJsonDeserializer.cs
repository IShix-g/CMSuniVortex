
using Newtonsoft.Json.Linq;

namespace CMSuniVortex
{
    public interface IJsonDeserializer : IDeserializationNotifier
    {
        void Deserialize(JObject obj);
    }
}