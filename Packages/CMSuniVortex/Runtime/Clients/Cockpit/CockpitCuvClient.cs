
using System.Collections.Generic;

namespace CMSuniVortex.Cockpit
{
    public abstract class CockpitCuvClient<T, TS> 
        : CockpitCuvClientBase<T, TS>
        where T : CockpitModel 
        where TS : CockpitCuvModelList<T>
    {
        static readonly string[] s_cuvIds = { "default" };
        
        public override IReadOnlyList<string> GetCuvIds() => s_cuvIds;
    }
}