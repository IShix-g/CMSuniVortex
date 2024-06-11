
# クラスの役割

## CuvImporter

CMSからのデータインポートの管理。必要な状態を保存する

### 保存している内容

- ビルド先のパス
- 対象言語 (配列)
- 現在使用中の`CuvClient`

## CuvClient

どのCMSを使い、どのモデルを利用するかを決定する。実装すれば自動的に`CuvImporter`のプルダウンに表示される。

<img src="/Users/umac/Harapeco/Apps/IShix/CMSuniVortex/CMSuniVortex/docs/assets/select_client.png" width="600"/>

## CuvModelList<T>

CMSのデータの保存先、対象言語と指定したモデルが配列で保存される。

## ICuvModel

モデル。CMSの1記事に相当するデータを表す。

# 実装の説明

既に実装済みのCockpitがどのように実装されているかを知る事で更に理解が深まると思います。

[Cockpitの実装ファイル一覧はコチラ](https://github.com/IShix-g/CMSuniVortex/tree/main/Packages/CMSuniVortex/Runtime/Cockpit)

## ICuvModel

[CockpitModel](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Runtime/Cockpit/CockpitModel.cs)は、`ICuvModel` を実装した `abstract class`です。`Newtonsoft.Json.Linq.JObject`を受けて整形したデータを子クラスに返します。例えば下記のようなメソッドです。

```csharp
public string GetString(string key) => Get<string>(key);

T Get<T>(string key)
    => JObject.TryGetValue(key, out var value)
       && value.Type != JTokenType.Null
        ? value.Value<T>()
        : default;
```

子クラスからは下記のように利用します。

```csharp
public sealed class TestCockpitModel : CockpitModel
{
    public string Text;

    protected override void OnDeserialize()
    {
        Text = GetString("text");
    }
}
```

CMSによってどういうメソッドが実装できるか分からないのでCMS単位でクラスを作るようにしました。

## CuvModelList<T>

[CockpitCuvModelList<T>](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Runtime/Cockpit/CockpitCuvModelList.cs)は、ジェネリック型でモデルが`CockpitModel`を保証させる為のクラスです。また、`ScriptableObject`を継承している為、ジェネリック型が使えず`abstract class`になっています。下記のようにシンプルにモデルの型を渡すシンプルな実装です。

```csharp
public sealed class TestCockpitCuvModelList : CockpitCuvModelList<TestCockpitModel> {}
```

## CuvClient

[CockpitCuvClient](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Runtime/Cockpit/CockpitCuvClient.cs)では、CMSからデータをロードしモデルを生成し`CuvImporter`に渡す処理を記述します。こちらも`sealed class`になっています。下記のように実装します。

```csharp
public sealed class TestCockpitCuvClient : CockpitCuvClient<TestCockpitModel, TestCockpitCuvModelList>
{
    protected override JsonConverter<TestCockpitModel> CreateConverter()
        => new CuvModelConverter<TestCockpitModel>();
}
```