
![Logo](ReadMeImages/logo.png)

CMS data can be easily imported into `ScriptableObject`.

![Import](ReadMeImages/import.png)

## 何故、このプラグインを使う必要があるのか？

このプラグインは **「簡単に入力でき、最高なパフォーマンスを発揮する」** というコンセプトを持っています。

### 入力の簡単さ

入力といえばCMS。CMSには **簡単にストレス無く入力できる** ノウハウが詰まっています。また、どこからでも更新できる気軽さもあります。

### パフォーマンスの良いデータ

`ScriptableObject`はUnityで扱うように最適化されたデータ形式です。より良いパフォーマンスを期待できるものはありません。

しかしながら、これら2つは一見無関係なものに見えるかもしれません。しかしCMSと`ScriptableObject`をシームレスに繋ぎ合わせてくれるのがCMSuniVortexです。 これは単なるプラグインではなく、効率性とパフォーマンスを追求した結果が生み出した解決策です。CMSuniVortexがあなたのプロジェクトに強力な推進力を与えることを願ってます。

## 対応するCMS

- [Cockpit](https://getcockpit.com/)

※ 色々なCMSに対応したいです。協力者募集中です。詳しくは[コチラ](https://github.com/IShix-g/CMSuniVortex/issues/1)をご覧ください。

## Unity Version
Unity 2021.3.x higher

## Getting started

### Install via git URL
Add the Url to Package Manager

URL : `https://github.com/IShix-g/CMSuniVortex.git?path=Packages/CMSuniVortex`

![Package Manager](ReadMeImages/package_manager.png)

## Quick Start

### CuvImporterの作成

Project上を右クリックし「CMSuniVortex > create CuvImporter」から`CuvImporter`を生成します。

![create](ReadMeImages/create.png)

### コードの生成

生成した`CuvImporter`の「Script Generator」ボタンをクリック

![open generator](ReadMeImages/open_generator.png)

必要情報を入力しコードを生成します

![create classes](ReadMeImages/create_classes.png)

|                 | explanation                   | e.g.                |
|-----------------|-------------------------------|---------------------|
| Full Class Name | クラス名を指定。namespaceを指定する事も可能です。 | namespace.ClassName |
| Build Path      | コードを生成するディレクトリのパスを指定          | Assets/Models/      |


### CuvImporterに必要情報の入力

生成後、CuvImporterに戻り必要情報を入力します。Clientに先ほど生成したスクリプトを指定します。

|            | explanation                        | e.g.           |
|------------|------------------------------------|----------------|
| Build Path | インポートしたデータの出力先ディレクトリを指定            | Assets/Models/ |
| Languages  | 言語を指定、利用していなくても必ず1つ選択する必要があります。    | English|
| Clint      | Script Generatorで生成したClientを指定します。 | Test.ClassNameCockpitCuvClient|

Clientが生成される命名ルール: 「生成時に指定したFull class name」 + 「CMS name」 + 「CuvClient」

![select client](ReadMeImages/select_client.png)

### インポートの開始

入力後インポートをクリックし完了です。出力されたデータを見てみてください。

![start import](ReadMeImages/start_import.png)

### Cockpit CMSテスト
実際にCockpit CMSを使ったテストができます。Clientを必ず*CockpitCuvClientを指定してください。

|            | value                                        |
|------------|----------------------------------------------|
| Base Url   | [https://devx.myonick.biz/cockpit/](https://devx.myonick.biz/cockpit/)|
| Api Key    | API-a92fac21986ac045e143f07c27c60e09f19ae856 |
| Model Name | Model                                        |

#### ログイン情報

|     | value                                                                  |
|-----|------------------------------------------------------------------------|
| URL | [https://devx.myonick.biz/cockpit/](https://devx.myonick.biz/cockpit/) |
| ID  | guest                                                                  |
| PW  | guest                                                                  |

#### テストサーバーの注意点

- 利用の際は節度をもった利用をお願いします。
- 頻繁にアクセスしすぎない。
- 連続したImportをしない。
- 無料のレンタルサーバーを利用している為、広告が表示されますが、私は一切関与していません。
- 不適切なアクセスを発見次第、予告なく停止しますのでご了承ください。

## パフォーマンステスト

私が、このプラグインを開発するきっかけになったのがこのパフォーマンステストです。 データをダウンロードし、表示するには大まかに以下の3つの方法があります。

### 1. Addressable

#### メリット

`ScriptableObject`や`Sprite`を使う事で、デシリアライズやデータ変換が必要無くパフォーマンスが良い

#### 懸念点

Unityで書き出す必要があるのでプログラマーが必要。またはそれなりの変換システムを構築する必要がある

<details><summary>テストコード</summary>

```csharp

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Profiling;

public sealed class AddressableTest : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] Text _text;
    [SerializeField] Button _loadButton;
    [SerializeField] Button _unloadButton;

    AsyncOperationHandle<AddressableData> _handle;
    
    void Start()
    {
        _loadButton.onClick.AddListener(OnLoadButtonClicked);
        _unloadButton.onClick.AddListener(OnUnloadButtonClicked);
        _loadButton.interactable = true;
        _unloadButton.interactable = false;
    }

    void OnDestroy() => Unload();

    async void OnLoadButtonClicked()
    {
        _loadButton.interactable = false;
        _unloadButton.interactable = true;
        
        Profiler.BeginSample("AddressableTestProfile1");
        _handle = Addressables.LoadAssetAsync<AddressableData>("AddressableData");
        Profiler.EndSample();
        await _handle.Task;
        
        Profiler.BeginSample("AddressableTestProfile2");
        var obj = _handle.Result;
        _image.sprite = obj.Image;
        _text.text = obj.GetText();
        Profiler.EndSample();
    }

    void OnUnloadButtonClicked()
    {
        Unload();
        _loadButton.interactable = true;
        _unloadButton.interactable = false;
    }

    void Unload()
    {
        if (_image != default)
        {
            _image.sprite = default;
        }
        if (_text != default)
        {
            _text.text = default;
        }
        
        if (_handle.IsValid())
        {
            Addressables.Release(_handle);
        }
    }
}

[CreateAssetMenu(fileName = "AddressableData", menuName = "ScriptableObject/AddressableData", order = 0)]
public sealed class AddressableData : ScriptableObject
{
    public int ID;
    public string Title;
    public string Contents;
    public Sprite Image;

    public string GetText() => "ID:" + ID + "\nTitle:" + Title + "\nContents:" + Contents;
}
```
</details>

### 2. WebView

[Cross Platform Essential Kit](https://assetstore.unity.com/packages/tools/integration/cross-platform-native-plugins-essential-kit-mobile-ios-android-140111)のWebViewを使用

#### メリット

- WEBページとアプリで兼用できる
- リリース後もレイアウトが自由

#### 懸念点

- メモリ使用量が心配

<details><summary>テストコード</summary>

```csharp

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

public sealed class WebViewTest : MonoBehaviour
{
    const string url = "https://xxx.xxxx.com/webview/";
    
    [SerializeField] Button _openButton;
    [SerializeField] Button _closeButton;

    WebView _webView;

    void Start()
    {
        _openButton.onClick.AddListener(ClickOpenButton);
        _closeButton.onClick.AddListener(ClickCloseButton);
        _openButton.interactable = true;
        _closeButton.interactable = false;
    }

    void OnEnable()
    {
        WebView.OnShow += OnWebViewShow;
        WebView.OnHide += OnWebViewHide;
    }
    
    void OnDisable()
    {
        WebView.OnShow -= OnWebViewShow;
        WebView.OnHide -= OnWebViewHide;
    }
    
    void ClickOpenButton()
    {
        _openButton.interactable = false;
        
        Profiler.BeginSample("WebViewTestProfile");
        _webView = WebView.CreateInstance();
        _webView.SetNormalizedFrame(new Rect(0.1f, 0.2f, 0.8f, 0.6f));
        _webView.LoadURL(URLString.URLWithPath(url));
        _webView.Show();
        Profiler.EndSample();
    }
    
    void ClickCloseButton() => _webView.Hide();
    
    void OnWebViewShow(WebView view) => _closeButton.interactable = true;

    void OnWebViewHide(WebView view)
    {
        _openButton.interactable = true;
        _closeButton.interactable = false;
    }
}
```

Webページ
```html
<!DOCTYPE html>
<html lang="ja">
<head>
<meta charset="utf-8">
<title>Test</title>
<meta name="viewport" content="width=device-width,initial-scale=1.0">
<meta name="format-detection" content="telephone=no,email=no,address=no">
<style type="text/css">
	img{
		max-width: 100%;
	}
</style>
</head>
<body>

<div id="myData">
  <h2 id="title"></h2>
  <p id="contents"></p>
  <img id="image" src="" alt="Image">
</div>

<script src="https://code.jquery.com/jquery-1.12.4.min.js"></script>

<script>
$.ajax({
  url: 'getModel.php',
  dataType: 'json',
  success: function(data) {
    $('#title').text(data.Title);
    $('#contents').text(data.Contents);
    $('#image').attr('src', data.Image);
  },
  error: function (request, status, error) {
    console.log("Error: Could not fetch data");
  }
});
</script>
</body>
</html>
```

データを取得するAPI
```php
<?php

class Model {
    public $Id;
    public $Title;
    public $Contents;
    public $Image;
}

mb_language("uni");
mb_internal_encoding("UTF-8");
header('Content-type: application/json');

$model = new Model();
$model->Id = 2222;
$model->Title = '猫　ねこ';
$model->Contents = '猫は、古代のミアキスと言う豹のような大きな動物が起源と言われています。 今から４０００～５０００年前にエジプトから発生し、住み良い環境を求め分化して中東に行きました。';
$model->Image = 'https://xxx.xxxx.com/webview/cat.jpg';
echo json_encode( $model );
```
</details>

### [テスト3] Json

[UnityWebRequest](https://docs.unity3d.com/ja/2021.3/ScriptReference/Networking.UnityWebRequest.html)でサーバーから取得したJsonを変換して表示。

#### メリット
- WEBとアプリで兼用できる
- 初期化以外はWebViewより軽そう

#### 懸念点
- jsonのデシリアライズやSpriteの生成が必要なので初期化コストが心配
- データをキャッシュしないのでキャッシュする機構を自身で用意する必要がある

<details><summary>テストコード</summary>

※データを取得するAPIはWebViewで使ったものを利用しているので省略

```csharp

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Profiling;

public sealed class JsonTest : MonoBehaviour
{
    const string apiUrl = "https://xxx.xxxx.com/webview/getModel.php";
    
    [SerializeField] Image _image;
    [SerializeField] Text _text;
    [SerializeField] Button _loadButton;
    [SerializeField] Button _unloadButton;
    
    [Serializable]
    sealed class Model
    {
        public int ID;
        public string Title;
        public string Contents;
        public string Image;
        
        public string GetText() => "ID:" + ID + "\nTitle:" + Title + "\nContents:" + Contents;
    }

    void Start()
    {
        _loadButton.onClick.AddListener(OnLoadButtonClicked);
        _unloadButton.onClick.AddListener(OnUnloadButtonClicked);
        _loadButton.interactable = true;
        _unloadButton.interactable = false;
    }

    void OnDestroy() => Unload();
        
    void OnLoadButtonClicked()
    {
        _loadButton.interactable = false;
        _unloadButton.interactable = false;
        
        StartCoroutine(LoadCo((model, sprite) =>
        {
            _text.text = model.GetText();
            _image.sprite = sprite;
            Profiler.EndSample();
            _unloadButton.interactable = true;
        }));
    }
    
    IEnumerator LoadCo(Action<Model, Sprite> onSuccess)
    {
        Profiler.BeginSample("JsonTestProfile1");
        using var request = UnityWebRequest.Get(apiUrl);
        Profiler.EndSample();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Profiler.BeginSample("JsonTestProfile2");
            var model = JsonUtility.FromJson<Model>(request.downloadHandler.text);
            using var imgRequest = UnityWebRequestTexture.GetTexture(model.Image);
            Profiler.EndSample();
            yield return imgRequest.SendWebRequest();
            
            if (imgRequest.result == UnityWebRequest.Result.Success)
            {
                Profiler.BeginSample("JsonTestProfile3");
                var texture = ((DownloadHandlerTexture)imgRequest.downloadHandler).texture;
                var sprite = Sprite.Create(
                    texture, 
                    new Rect(0, 0, texture.width, texture.height), 
                    new Vector2(0.5f, 0.5f));
                
                onSuccess?.Invoke(model, sprite);
            }
            else
            {
                Debug.LogError(imgRequest.error);
            }
        }
        else
        {
            Debug.LogError(request.error);
        }
    }
    
    void OnUnloadButtonClicked()
    {
        Unload();
        _loadButton.interactable = true;
        _unloadButton.interactable = false;
    }

    void Unload()
    {
        if (_image != default
            && _image.sprite != default)
        {
            var tex = _image.sprite.texture;
            _image.sprite = null;
            DestroyImmediate(tex);
            Resources.UnloadUnusedAssets();
        }
        if (_text != default)
        {
            _text.text = default;
        }
    }
}
```
</details>

### 計測結果からわかった事

このテストから下記の事がわかりました。

- Addressableが一番パフォーマンスが良い。
- WebViewはAndroidで無視できないメモリを使う。※すべてのメモリを解放できない可能性あり
- Jsonは画像が多いと無視できない初期化コストが発生する。(テストでは画像1枚のみ)

この結果からパフォーマンスの良いAddressableを使いつつ、デメリットを解消する為に気軽に更新できるCMSから入力できるようにしたいと思いこのプラグインを開発しました。

#### iOS : iPhone SE2 17.5.1

|  | GC Alloc | Time | Size |
|---|:--|:--|---|
| Addressables | 3.2KB | 0.24ms | 1.1MB |
| WebView | 22.9KB | 0.52ms | 2MB |
| Json | 15KB | 3.75ms | 2.3MB |

#### Android : Galaxy S10 Android11

|  | GC Alloc | Time | Size |
|---|:--|:--|---|
| Addressables | 3.1KB | 0.24ms | 9MB |
| WebView | 31.8KB | 0.56ms | 70MB |
| Json | 4.3KB | 1.18ms | 9.7MB |


## 今後について

今は、まだ`ScriptableObject`の生成までしか対応していませんが、Addressableをビルドしてサーバーに送信できるようにしたいと思っています。
また、CMSの対応も増やしていきたいです。興味ある方は是非よろしくお願いいたします。

