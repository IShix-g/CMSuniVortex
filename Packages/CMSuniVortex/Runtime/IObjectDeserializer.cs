
using System.Collections.Generic;

namespace CMSuniVortex
{
    public interface IObjectDeserializer : IDeserializationNotifier
    {
        void Deserialize(Dictionary<string, string> obj);
    }
}