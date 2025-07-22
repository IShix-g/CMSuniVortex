
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CMSuniVortex
{
    public interface ICuvLocalizedAsyncReference : ICuvKeyReference, ICuvLanguageSelectable
    {
        event Action OnInitializeLocalize;
        event Action<SystemLanguage> OnStartLoadLanguage;
        event Action<SystemLanguage> OnLoadedLanguage;
        int ContentsLength { get; }
        bool IsLoading { get; }
        bool IsInitializedLocalize { get; }
        IEnumerator WaitForLoadLocalizationCo(Action onReady);
        Task WaitForLoadLocalizationAsync(CancellationToken token = default);
        bool HasContents();
        void Cancel();
        void Release();
    }
}