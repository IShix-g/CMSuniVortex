
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CMSuniVortex
{
    public interface ICuvLocalizedReference : ICuvLanguageSelectable
    {
        event Action OnInitializeLocalize;
        event Action<SystemLanguage> OnChangeLanguage;
        int ContentsLength { get; }
        bool IsInitializedLocalize { get; }
        IEnumerator WaitForLoadLocalizationCo(Action onReady);
        Task WaitForLoadLocalizationAsync(CancellationToken token = default);
        bool HasContents();
        string[] GetKeys();
    }
}