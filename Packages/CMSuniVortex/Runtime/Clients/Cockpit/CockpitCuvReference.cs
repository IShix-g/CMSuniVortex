
namespace CMSuniVortex.Cockpit
{
    public abstract class CockpitCuvReference<T, TS> : CuvReference<T, TS> where T : CockpitModel where TS : CockpitCuvModelList<T> {}
}