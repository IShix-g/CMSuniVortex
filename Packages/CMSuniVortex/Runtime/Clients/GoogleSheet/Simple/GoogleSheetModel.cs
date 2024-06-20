
using System;
using System.Collections.Generic;

#if ENABLE_ADDRESSABLES
using CMSuniVortex.Addressable;
using UnityEngine.AddressableAssets;
#endif

namespace CMSuniVortex.GoogleSheet
{
    [Serializable]
    public class GoogleSheetModel : ICuvModel
#if ENABLE_ADDRESSABLES
        ,IAddressableModel
#endif
    {
        public string Key;
        public string Text;
        public string Comment;

        public string GetKey() => Key;
#if ENABLE_ADDRESSABLES
        HashSet<AddressableAction> IAddressableModel.AddressableActions { get; }

        void IAddressableModel.LoadSpriteReference(string key, Action<AssetReferenceSprite> completed)
            => throw new NotImplementedException();

        void IAddressableModel.LoadTextureReference(string key, Action<AssetReferenceTexture2D> completed)
            => throw new NotImplementedException();
#endif
    }
}