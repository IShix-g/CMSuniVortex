
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace CMSuniVortex
{
    public interface ICuvAsyncReference
    {
        event Action<SystemLanguage> OnChangeLanguage;
        bool IsLoading { get; }
        bool IsInitialized { get; }
        SystemLanguage Language { get; }
        IEnumerator Initialize(Action onLoaded);
        IEnumerator Initialize(SystemLanguage language, Action onLoaded);
        IEnumerator ChangeLanguage(SystemLanguage language, Action onLoaded);
        Task ChangeLanguageAsync(SystemLanguage language);
        Task InitializeAsync();
        Task InitializeAsync(SystemLanguage language);
        bool HasContents();
        string[] GetKeys();
        void Release();
    }
}