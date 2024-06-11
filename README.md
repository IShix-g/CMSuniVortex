![Logo](docs/assets/logo.png)

A plugin for easily importing CMS data to `ScriptableObject`

![Import](docs/assets/import.png)

[日本語のRead me](docs/README_jp.md)

## Why use this plugin?

This plugin is designed with the concept of **"Easy to input, and performs at its best"**.

### Ease of Input

When it comes to input, CMS comes up. CMS is packed with know-how to **easily input without stress**. There is also the ease of being able to update from anywhere.

### High-Performance Data

`ScriptableObject` is a data format optimized for handling with Unity. No better performance can be achieved.

However, these two may seem unrelated at first glance. But CMSuniVortex seamlessly connects CMS and `ScriptableObject`. It is not just a plugin, but a solution born out of pursuing efficiency and performance.

## Supported CMS

- [Cockpit](https://getcockpit.com/)

※ We want to support various CMS. We are looking for collaborators. For more details, please check [here](https://github.com/IShix-g/CMSuniVortex/issues/1).

## Unity Version

Unity 2021.3.x higher

## Getting started

### Install via git URL

Add the URL to the Package Manager

URL : `https://github.com/IShix-g/CMSuniVortex.git?path=Packages/CMSuniVortex`

![Package Manager](docs/assets/package_manager.png)

## Quick Start

### Create CuvImporter

Right-click on the project and select `CMSuniVortex > create CuvImporter` to generate `CuvImporter`.

![create](docs/assets/create.png)

### Generation of Code

Click the `Script Generator` button of the created `CuvImporter`.

![open generator](docs/assets/open_generator.png)

Enter the required information and generate the code.

<img alt="create classes" src="docs/assets/create_classes.png" width="600"/>

|                 | explanation                         | e.g.                       |
|-----------------|-------------------------------------|----------------------------|
| Full Class Name | Specify the class name. You can also specify the namespace. | namespace.ClassName |
| Build Path      | Specify the path of the directory where the code will be generated.  | Assets/Models/             |


### Enter required information in CuvImporter

After generation, return to CuvImporter and enter the required information. Specify the script you just generated for the Client.

|            | explanation                              | e.g.           |
|------------|------------------------------------------|----------------|
| Build Path | Specify the output directory for the imported data.             | Assets/Models/ |
| Languages  | Specify the language, even if it's not in use, you must select at least one. | English |
| Client     | Specify the Client generated by the Script Generator. | Test.ClassNameCockpitCuvClient |

The naming rule for generating the client is: "Full class name specified at the time of generation" + "CMS name" + "CuvClient".

<img alt="select client" src="docs/assets/select_client.png" width="600"/>

### Starting the Import

After entering, click import and you're done.

<img alt="start import" src="docs/assets/start_import.png" width="600"/>

### Cockpit CMS Test
You can test using Cockpit CMS. Be sure to specify the Client as *CockpitCuvClient.

|            | value                                        |
|------------|----------------------------------------------|
| Base Url   | [https://devx.myonick.biz/cockpit/](https://devx.myonick.biz/cockpit/) |
| Api Key    | API-a92fac21986ac045e143f07c27c60e09f19ae856 |
| Model Name | Model                                        |

#### Login Information

|     | value                                                                  |
|-----|------------------------------------------------------------------------|
| URL | [https://devx.myonick.biz/cockpit/](https://devx.myonick.biz/cockpit/) |
| ID  | guest                                                                  |
| PW  | guest                                                                  |

#### Points to Note for the Test Server

- Please use with moderation.
- Do not access too often.
- Do not perform continuous imports.
- Advertisements are displayed because it uses a free rental server, for which I have no involvement.
- Please note that inappropriate access will be stopped without notice.

## Performance Test

The performance test was the trigger for me to develop this plugin. There are roughly three ways to download and display data.

### 1. Addressable

#### Merits

With `ScriptableObject` and `Sprite`, you can get good performance without needing to deserialize or convert data.

#### Concerns

A programmer is needed because it needs to be outputted in Unity. Alternatively, a substantial conversion system needs to be built.

<details><summary>Test Code</summary>


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

Webview from [Cross Platform Essential Kit](https://assetstore.unity.com/packages/tools/integration/cross-platform-native-plugins-essential-kit-mobile-ios-android-140111)

#### Merits

- Can be used for both WEB pages and applications.
- The layout can be free even after release.

#### Concerns

- Concerned about the amount of memory used.

<details><summary>Test Code</summary>

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


### 3. Json

Display by converting JSON obtained from the server using [UnityWebRequest](https://docs.unity3d.com/ja/2021.3/ScriptReference/Networking.UnityWebRequest.html).

#### Merits
- Can be used for WEB and apps.
- Apart from initialization, it seems lighter than WebView.

#### Concerns
- There is a concern about the initialization cost if there are many images (only one image in the test).
- If you don't cache data, you need to provide your own caching mechanism.

<details><summary>Test Code</summary>

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

### What We Learned From the Test Results

From these tests, we learned that:

- Addressable performs the best.
- WebView uses significant memory on Android. It might not be possible to fully release all memory.
- Json has a significant initialization cost when there are many images.

From these results, I wanted to use Addressable, which has the best performance, but also allows for easy updates from the CMS, so I developed this plugin.

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

## Plans for the Future

Currently, I only support generating `ScriptableObject`, but I want to build Addressable and send it to the server. I also want to increase support for CMS. If you're interested, I'd appreciate your help.