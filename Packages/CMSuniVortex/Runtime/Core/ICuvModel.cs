
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace CMSuniVortex
{
    public interface ICuvModel
    {
        string GetID();
        void Deserialize(JObject obj);
        void Deserialized();
        HashSet<IEnumerator> Coroutines { get; }
    }
}