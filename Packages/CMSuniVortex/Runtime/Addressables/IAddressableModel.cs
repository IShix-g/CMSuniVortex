#if ENABLE_ADDRESSABLES
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace CMSuniVortex.Addressable
{
    public interface IAddressableModel
    {
        HashSet<AddressableAction> AddressableActions { get; }
        void LoadSpriteReference(string key, Action<AssetReferenceSprite> completeAction);
        void LoadTextureReference(string key, Action<AssetReferenceTexture2D> completeAction);
    }
}
#endif