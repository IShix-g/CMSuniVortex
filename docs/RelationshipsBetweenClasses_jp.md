
## 各クラスの役割

### [CuvImporter](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Runtime/CuvImporter.cs)

CMSからのインポートの管理。必要な状態を保存する

#### 保存している内容

- ビルド先のパス
- 対象言語 (配列)
- 現在使用中の`CuvClient`

<img src="assets/select_client.png" width="600"/>

### [CuvClient](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Runtime/CuvClient.cs)

どのCMSを使い、どのモデルを利用するかを決定する。実装すると`CuvImporter`のプルダウンに表示される。

#### CMSの種類

- Cockpit CMS
- Google Sheets

#### ファイルの参照方法

- 直接参照
- [Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/index.html)からの参照

### CuvClientで使えるAttribute

#### `[IgnoreImporter]`アトリビュート
指定すると`CuvImporter`のプルダウンから除外され表示されません。

```csharp
[IgnoreImporter] // <--
public sealed class TestCockpitCuvClient : CockpitCuvClient<TestCockpitModel, TestCockpitCuvModelList>
{
    protected override JsonConverter<TestCockpitModel> CreateConverter()
        => new CuvModelConverter<TestCockpitModel>();
}

```

#### `[CuvDisplayName("Name")]`アトリビュート
指定すると`CuvImporter`のプルダウンに表示される名称を変更できます。

```csharp
[CuvDisplayName("YourCustomName")] // <--
public sealed class TestCockpitCuvClient : CockpitCuvClient<TestCockpitModel, TestCockpitCuvModelList>
{
    protected override JsonConverter<TestCockpitModel> CreateConverter()
        => new CuvModelConverter<TestCockpitModel>();
}

```

### [CuvModelList](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Runtime/CuvModelList.cs)

`CuvClient`によって生成される。データを対象言語別に`ScriptableObject`に保存する。モデルが配列で格納され`Key`で取得できる。

### [ICuvModel](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Runtime/ICuvModel.cs)

モデル。CMSの1記事に相当するデータ。

### [ICuvOutput](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Runtime/ICuvOutput.cs)

`CuvModelList`をどのようにして参照するか決定し、`CuvReference`を生成する。実装すると`CuvImporter`のプルダウンに表示される。

#### 種類

- 直接参照
- [Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/index.html)からの参照

### [CuvReference](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Runtime/CuvReference.cs)

言語別に格納された`CuvModelList<T>`の参照を管理する`ScriptableObject`。利用側は、ここから取得する。

## Compornent

### `CuvModelKey("ref")` アトリビュート

生成した`CuvReference`を直接参照して使用しても良いですが、このコンポーネントを使うと設定した`Key`一覧をプルダウンで表示してくれるので便利です。

<img src="assets/googleSheet/cuv_model_key.png" width="600"/>


引数に参照したい`CuvReference`のフィールド名を渡します。

```csharp
public abstract class Test : MonoBehaviour
{
    [SerializeField] GoogleSheetCuvReference _reference;
    [SerializeField, CuvModelKey("_reference")] string _key;

```

### CuvComponent

`CuvModelKey`をラップして使いやすくしたクラスです。

```csharp
using CMSuniVortex.Compornents;
using CMSuniVortex.GoogleSheet;
using UnityEngine;
using UnityEngine.UI;

public sealed class TestText : CuvComponent<GoogleSheetCuvReference>
{
    [SerializeField] Text _text;
        
    protected override void OnChangeLanguage(GoogleSheetCuvReference reference, string key)
    {
        if (reference.GetList().TryGetByKey(key, out var model))
        {
            _text.text = model.Text;
        }
    }
}
```

### CuvAsyncComponent

`CuvComponent`の非同期版です。

### SetParam

`{}`で囲う事でパラメーターを埋め込む事ができます。

- 文言 `You have earned {number} coins.`
- 表示 `You have earned 5 coins.`

```csharp
var text = model.Text.SetParam("number", 5);
```