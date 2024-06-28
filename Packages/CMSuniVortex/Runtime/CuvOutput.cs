
using UnityEngine;

namespace CMSuniVortex
{
    public abstract class CuvOutput<TModel, TModelList, TReference> : ICuvOutput
        where TModel : ICuvModel
        where TModelList : ScriptableObject, ICuvModelList<TModel>
        where TReference : CuvReference<TModel, TModelList>
    {
        public abstract TReference GetReference();
        public abstract void Select(string assetPath);
        public abstract void Deselect();
        public abstract void Generate(string buildPath, ICuvClient client, string[] listGuids);
        public abstract void Release();
    }
}