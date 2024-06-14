
using System.Collections.Generic;

namespace CMSuniVortex
{
    public interface IObjectDeserializer
    {
        void Deserialize(Dictionary<string, string> obj);
        void Deserialized();
    }
}