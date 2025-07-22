
namespace CMSuniVortex
{
    public interface ICuvModelListSetter<in T> where T : ICuvModel
    {
        void SetData(string cuvId, T[] models);
    }
}