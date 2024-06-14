
[日本語](IntegrationWithCockpit_jp.md)

## Required Environment for Cockpit

- A server that can use PHP
- SQLite or MongoDB

Please check [Requirements](https://getcockpit.com/documentation/core/quickstart/requirements) for more details.

If you want to use it locally, you can install it on [Xampp](https://www.apachefriends.org/).

## Cockpit Installation

Download the free version from [Cockpit](https://getcockpit.com/start-journey) and install it. Detailed installation will not be explained here.

## Configuring Contents

Set up the Collection. The Name is input into `CuvImporter's` `Client > Model Name`.

![](assets/cockpit/collection.png)

### Adding a Field

Click "ADD FIELD".

![](assets/cockpit/addField.png)

### Text

![](assets/cockpit/create_text.png)

After addition, you can retrieve the Name as ID in Unity as follows:

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

You can add constants from Options to Select or Tag.

![](assets/cockpit/select.png)

It's also possible to convert to `enum`.

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

### Listing

In addition to the following, there are multiple retrieval methods like GetStrings(key).

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

### Entering Items

After saving the Field, we will test the retrieval, so enter random multiple Items.

![](assets/cockpit/edit_item.png)

### Roles

After entering the Item, configure the Roles so they can be retrieved from the outside. Click the setting mark on the bottom left.

![](assets/cockpit/items.png)

Click on ROLES & PERMISSIONS

![](assets/cockpit/roles.png)

From "ADD ROLE" at the bottom right, select only Read of the CONTENT's Items created earlier and create with "CREATE ROLE".

![](assets/cockpit/add_role.png)

Click on Api in the left menu and specify the Role just created. The selection place is hard to see, please click on the position of the dropdown in the image.

![](assets/cockpit/api.png)

After setting, it will look like below. Click on "REST" to confirm if it works properly. Enter the Api key in CuvImporter's Client > Api Key.

![](assets/cockpit/api_setted.png)

Perform GET /content/items/{model} test. Here, check if it can be retrieved properly. If something strange happens, please remember to first run this test. It will certainly be useful.

![](assets/cockpit/api_test.png)

### Import

Go up to Unity, enter the necessary information of CuvImporter and click on the Import button. If it can be retrieved, it is done.

|            | Explanation                                   | e.g.                               |
|------------|-----------------------------------------------|------------------------------------|
| Build Path | The path to generate assets                   | Assets/Generated/                  |
| Languages  | Specify languages, at least one is required even if not used | English                            |
| Base Url   | URL of the Cockpit                            | https://devx.myonick.biz/cockpit/  |
| Api Key    | Api Key set in [Roles](#Roles)                | API-a92fac21986ac045e143f07c27c60e09f19ae |
| Model Name | Name set in [Cockpit installation](#cockpit-installation) | Model  |

<img src="assets/cockpit/cuv_importer.png" width="600"/>

## Language Setting

Select "LOCALES" from the setting mark on the bottom left.

![](assets/cockpit/select_locales.png)

As language, configure the value of [SystemLanguage](https://docs.unity3d.com/ja/2021.3/ScriptReference/SystemLanguage.html).

![](assets/cockpit/create_locale.png)

Please turn on the Localize field of the Item you want to localize.

![](assets/cockpit/select_localize_field.png)

Then, "TRANSLATION" will be displayed on the Item editing screen.

![](assets/cockpit/edit_item2.png)