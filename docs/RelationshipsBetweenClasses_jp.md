
# 各クラスの役割

## [CuvImporter](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Runtime/CuvImporter.cs)

CMSからのインポートの管理。必要な状態を保存する

### 保存している内容

- ビルド先のパス
- 対象言語 (配列)
- 現在使用中の`CuvClient`

## [CuvClient](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Runtime/CuvClient.cs)

どのCMSを使い、どのモデルを利用するかを決定する。実装すれば自動的に`CuvImporter`のプルダウンに表示される。

<img src="assets/select_client.png" width="600"/>

`[IgnoreImporter]`アトリビュートを指定すると`CuvImporter`のプルダウンから除外され表示されません。

```csharp
[IgnoreImporter]
public sealed class TestCockpitCuvClient : CockpitCuvClient<TestCockpitModel, TestCockpitCuvModelList>
{
    protected override JsonConverter<TestCockpitModel> CreateConverter()
        => new CuvModelConverter<TestCockpitModel>();
}

```

## [CuvModelList<T>](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Runtime/CuvModelList.cs)

データを対象言語別に保存するデータ。モデルが配列で保存される。

## ICuvModel

モデル。CMSの1記事に相当するデータを表す。

# 実装の説明

実装済みのCockpitがどのように実装されているかを知る事で更に理解が深まると思います。

[Cockpitの実装ファイル一覧はコチラ](https://github.com/IShix-g/CMSuniVortex/tree/main/Packages/CMSuniVortex/Runtime/Clients/Cockpit)

## ICuvModel

[CockpitModel](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Runtime/Clients/Cockpit/CockpitModel.cs)は、`ICuvModel` を実装した `abstract class`です。`Newtonsoft.Json.Linq.JObject`を受けて整形したデータを子クラスに返します。例えば下記のようなメソッドです。

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

[CockpitCuvModelList<T>](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Runtime/Clients/Cockpit/CockpitCuvModelList.cs)は、ジェネリック型で`CockpitModel`を保証させる為のクラスです。また、`ScriptableObject`を継承している為、ジェネリック型が使えず`abstract class`になっています。下記のようにシンプルにモデルの型を渡すシンプルな実装です。

```csharp
public sealed class TestCockpitCuvModelList : CockpitCuvModelList<TestCockpitModel> {}
```

## CuvClient

[CockpitCuvClient](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Runtime/Clients/Cockpit/CockpitCuvClient.cs)では、CMSからデータをロードしモデルを生成し`CuvImporter`に渡す処理を記述します。こちらも`sealed class`になっています。下記のように実装します。

```csharp
public sealed class TestCockpitCuvClient : CockpitCuvClient<TestCockpitModel, TestCockpitCuvModelList>
{
    protected override JsonConverter<TestCockpitModel> CreateConverter()
        => new CuvModelConverter<TestCockpitModel>();
}
```

## 実装済み子クラスの一覧

`TestCockpitModel`を利用する為に下記3つのスクリプトを作成する必要があります。
面倒なのでScript Generatorで生成できるようにしています。

- [TestCockpitCuvClient - CockpitCuvClient<TestCockpitModel, TestCockpitCuvModelList>](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Samples~/Import/Scripts/TestCockpitCuvClient.cs)
- [TestCockpitCuvModelList - CockpitCuvModelList<TestCockpitModel>](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Samples~/Import/Scripts/TestCockpitCuvModelList.cs)
- [TestCockpitModel - CockpitModel](https://github.com/IShix-g/CMSuniVortex/blob/main/Packages/CMSuniVortex/Samples~/Import/Scripts/TestCockpitModel.cs)