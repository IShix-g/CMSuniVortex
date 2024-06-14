## Cockpitの必須環境

- PHPが使えるサーバー
- SQLite or MongoDB

詳しくは[Requirements](https://getcockpit.com/documentation/core/quickstart/requirements)を確認ください。

ローカルで使いたいなら[Xampp](https://www.apachefriends.org/)にインストールできます。

## Cockpitのインストール

[Cockpit](https://getcockpit.com/start-journey)からfreeの方をダウンロードしインストールします。インストールの詳細はここでは説明しません。

## コンテンツの設定

Collectionを設定します。Nameは`CuvImporter`の`Client > Model Name`に入力します。

![](assets/cockpit/collection.png)

### Fieldの追加

「ADD FIELD」をクリックします。

![](assets/cockpit/addField.png)

### Text

![](assets/cockpit/create_text.png)

追加したものは下記のようにNameをIDとしてUnity上で取得できます。

```csharp
[Serializable]
public sealed class TestCockpitModel : CockpitModel
{
    public string Text;

    protected override void OnDeserialize()
    {
        Text = GetString("text");
    }
}
```

### Select

SelectやTagはOptionsから定数を追加できます。

![](assets/cockpit/select.png)

`enum`に変換する事も可能です。

```csharp
[Serializable]
public sealed class TestCockpitModel : CockpitModel
{
    public ItemType Select;

    public enum ItemType { Item1, Item2, Item3 }

    protected override void OnDeserialize()
    {
        Select = GetSelect<ItemType>("select");
    }
}
```

### 一覧

下記以外にも、`GetStrings(key)`などの複数取得系のメソッドもあります。

```csharp
using System;
using CMSuniVortex.Cockpit;
using UnityEngine;

namespace CMSuniVortex.Tests
{
    [Serializable]
    public sealed class TestCockpitModel : CockpitModel
    {
        public string Text;
        public long Number;
        public Sprite Image;
        public bool Boolean;
        public Color Color;
        public string Date;
        public ItemType Select;
        public TagType[] Tags;

        public enum TagType { Tag1, Tag2, Tag3 }

        public enum ItemType { Item1, Item2, Item3 }

        protected override void OnDeserialize()
        {
            Text = GetString("text");
            Number = GetLong("number");
            Boolean = GetBool("boolean");
            Color = GetColor("color");
            Date = GetDate("date");
            Select = GetSelect<ItemType>("select");
            Tags = GetTag<TagType>("tags");
            LoadSprite("image", asset => Image = asset);
        }
    }
}
```

### Itemの入力

Fieldを保存後、取得テストをするので適当に複数のItemを入力します。

![](assets/cockpit/edit_item.png)

### Roles

Itemを入力したあとは、外部から取得できるようにRolesの設定をします。
左下の設定マークをクリック

![](assets/cockpit/items.png)

ROLES & PERMISSIONSをクリック

![](assets/cockpit/roles.png)

右下の「ADD ROLE」から先ほど作成したCONTENTのItemsのReadのみを選択して「CREATE ROLE」で作成します。

![](assets/cockpit/add_role.png)

左メニューのApiをクリックして先ほど作成したRoleを指定します。選択する場所が分かりづらいです。画像のドロップダウンの位置をクリックしてください。

![](assets/cockpit/api.png)

設定後、下記のようになります。「REST」をクリックして正常に動作するか確認します。
Api keyは`CuvImporter`の`Client > Api Key`に入力します。

![](assets/cockpit/api_setted.png)

GET /content/items/{model}テストをします。ここで問題なく取得できるかを確認します。何か動作がおかしい場合は、まずこちらのテストを実行するという事を覚えておいてください。きっと役に立ちます。

![](assets/cockpit/api_test.png)

### インポート

Unity上に移動し`CuvImporter`の必要情報を入力しImportボタンをクリックしてください。
取得できれば完了です。

|            | explanation                                 | e.g.                          |
|------------|---------------------------------------------|-------------------------------|
| Build Path | アセットを生成するパス                                 | Assets/Generated/ |
| Languages  | 言語を指定、利用していなくても必ず1つ選択する必要があります。             | English |
| Base Url   | CockpitのURL                                 | https://devx.myonick.biz/cockpit/ |
| Api Key   | [Roles](#Roles)に設定したApi Key                 | API-a92fac21986ac045e143f07c27c60e09f19ae‹ |
| Model Name   | [cockpitのインストール](#cockpitのインストール) で設定したName | Model  |

<img src="assets/cockpit/cuv_importer.png" width="600"/>

## 言語の設定

左下の設定マークから「LOCALES」を選択します。

![](assets/cockpit/select_locales.png)

言語は[SystemLanguage](https://docs.unity3d.com/ja/2021.3/ScriptReference/SystemLanguage.html)の値を設定してください。

![](assets/cockpit/create_locale.png)

ローカライズしたいItemのLocalize fieldをオンにしてください。

![](assets/cockpit/select_localize_field.png)

そうするとItemの編集画面に「TRANSLATION」が表示されるようになります。

![](assets/cockpit/edit_item2.png)

## Unityの設定

[Readme](README_jp.md)をごらんください。