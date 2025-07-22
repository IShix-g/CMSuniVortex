
> [!IMPORTANT]
> **2.Xではローカライズ機能が強化されましたが、アップグレードには破壊的な変更が含まれます。アップグレードの前に必ずバックアップを取ってください。**

## ローカライズを利用する - スクリプトの生成

ローカライズを利用するには、Script Generatorで、`Use localization`にチェックを入れてスクリプトを生成します。

`Window > CMSuniVortex > open Script Generator`

<img alt="start import" src="assets/2x_generator.jpg" width="500"/>

## ローカライズを利用する - CuvImporterの設定

`Window > CMSuniVortex > create CuvImporter`でCuvImporterを生成し設定します。

- ① Script Generatorで生成したClientを選択します。
- ② Outputを選択する。Outputを選択する事でローカライズが機能します。

設定後、Importボタンでインポートし、Outputボタンでアウトプットしてください。

<img alt="start import" src="assets/cuvImporter_lang_client.jpg" width="500"/>

詳細な設定方法を知りたい方は、生成したCuvImporterの`doc`ボタンから確認できます。

<img alt="start import" src="assets/cuvImporter_doc.jpg" width="500"/>

### おすすめはAddressables?
はい。Addressablesは必要なデータ以外ロードしないのでメモリの節約になりおすすめです。 `Sprite`や`Texture`が多い場合は特におすすめです。  
しかし、利用にはある程度の知識が必要です。利用の前にある程度学んでいただくのをお勧めします。

## データを表示する

表示するにはコンポーネントを利用すると楽です。具体的には、Addressablesなら`CuvAddressableLocalized<T>`、それ以外は`CuvLocalized<T>`を利用します。そして下記のように`T`に受け渡すReferenceクラスを指定します。そして、OnChangeLanguageで渡すのはModelクラスです。クラスの名称は、生成したスクリプトを参照してください。

```csharp
using CMSuniVortex;

public abstract class CuvAddressableLocalizedTest : CuvAddressableLocalized<MetaLocalizeAddressableCustomGoogleSheetCuvAddressableReference>
{
    protected abstract void OnChangeLanguage(MetaLocalizeAddressable catDetails);
    
    protected override void OnChangeLanguage(MetaLocalizeAddressableCustomGoogleSheetCuvAddressableReference reference, string key)
    {
        if (reference.TryGetByKey(key, out var model))
        {
            OnChangeLanguage(model);
        }
    }
}
```

そして、上記で生成したクラスを継承して使います。  
下記は、`Sprite`を`Image`に表示する例です。

KeyはGoogleSheetならSheetで設定したKey名です。

<img alt="start import" src="assets/localized_compornent_image.jpg" width="500"/>

```csharp
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public sealed class CuvAddressableLocalizedImageTest : CuvAddressableLocalizedTest
{
    [SerializeField] Image _image;

    AsyncOperationHandle<Sprite> _handle;
    
    protected override async void OnChangeLanguage(MetaLocalizeAddressable model)
    {
        _handle = Addressables.LoadAssetAsync<Sprite>(model.Image);
        await _handle.Task;
        if (_handle.Status == AsyncOperationStatus.Succeeded)
        {
            _image.sprite = _handle.Result;
        }
        else if (_handle.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError(_handle.OperationException);
        }
    }

    void OnDestroy()
    {
        if (_handle.IsValid())
        {
            Addressables.Release(_handle);
        }
    }

    protected override void Reset()
    {
        base.Reset();
        _image = GetComponent<Image>();
    }
}
```

## 言語を切り替える

一番簡単なのは、`CuvLanguageDropDown`をDropDownに設定する事です。  
DropDown以外の場合、`CuvLanguages`を継承したオリジナルクラスを作ってください。
下記は、DropDownの実装です。

```csharp

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CMSuniVortex
{
    public sealed class CuvLanguageDropDown : CuvLanguages
    {
        [SerializeField] Dropdown _dropdown;

        protected override void OnInitialized()
        {
            var options = new List<Dropdown.OptionData>();
            foreach (var language in Languages)
            {
                var languageString = language.ToString();
                var data = new Dropdown.OptionData(languageString);
                options.Add(data);
            }
            _dropdown.options = options;
            _dropdown.value = GetLanguageIndex(ActiveLanguage);
            _dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        void OnValueChanged(int index)
        {
            var language = GetLanguageAt(index);
            ChangeLanguage(language);
        }

        void Reset() => _dropdown = GetComponent<Dropdown>();
    }
}
```

## 言語の環境設定

言語の設定で使われる言語は、存在するデータ(CuvImporter)から取得するので、言語を選択していないと表示されません。設定しなくてもローカライズは動作するので言語データを作った後、必要あれば設定してください。

言語の設定をしない場合、下記のような設定となります。

- **Languages:** すべてのCuvImporterから取得し、すべての言語を対象とする
- **Default Language:** CuvImporterで設定した1番目の言語を初期値とします
- **Start Language:** 端末の言語を使用
- **Save Language:** 言語の選択が実行された場合、保存され次回から保存した言語を使用する

`Window > CMSuniVortex > open Language Setting`

<img alt="start import" src="assets/language_setting.jpg" width="500"/>