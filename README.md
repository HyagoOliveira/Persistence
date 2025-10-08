# Persistence

* Save Serialized data using encryption and compression
* Unity minimum version: **6000.1**
* Current version: **3.1.2**
* License: **MIT**
* Dependencies:
    - [com.actioncode.async-io : 1.0.0](https://github.com/HyagoOliveira/Async-IO/tree/1.0.0)
    - [com.actioncode.cryptography : 1.0.0](https://github.com/HyagoOliveira/Cryptography/tree/1.0.0)
    - [com.actioncode.scriptable-settings-provider : 1.0.0](https://github.com/HyagoOliveira/ScriptableSettingsProvider/tree/1.0.0)

## Summary

This package can save Serialized data, first encrypting and/or compressing it. You can select if you want to only encrypt, compress or use both.
Also, you can choose witch serialization method to use (Json, XML or Binary).

To start to save/load data, you need first to create a **Persistence Settings** asset and use it on your Serialized data class.

## How To Use

### Creating a Persistence Settings

Open the **Project Settings** Windows and select Persistence (below ActionCode group).

Click on the **Create** button and save a new Persistence Settings asset.

![The Persistence Settings Menu](/Docs~/PersistenceSettingsMenu.png "The Persistence Settings Menu")

Now you can select a Serializer, Compression, Cryptographer and other settings to use when saving your data. Each property has a Tooltip description.

If you're using a Cryptographer method, set a new Cryptographer Key using the **Get New Cryptographer Key** button. 

>**Important**: you should always use a different Cryptographer Key for every game you work in order to increase your data security.   

### Creating the Serialized Data Class

Your Serialized data class should always use the `System.Serializable` attribute.

```csharp
using System;

[Serializable]
public sealed class PlayerData
{
    public int score;
    public int lastLevel;
    public string playerName;

    public DateTime Creation { get; private set; } = DateTime.Now;
    public DateTime LastUpdate { get; private set; } = DateTime.Now;
}
```

>**Note 1**: if you're planning to use **XML Serialization** you should create a constructor to each serialized class.

>**Note 2**: *Json Utility* serializer doesn't save *properties*, just *public fields* and *fields* with the `[SerializeField]` attribute. 
Also, some types are not supported as well.
To solve this, please use *Json Newtonsoft* serializer and install the [Newtonsoft Json package](https://github.com/applejag/Newtonsoft.Json-for-Unity/wiki/Install-official-via-UPM).

>**Note 3**: If you're using *Json Newtonsoft* serializer, you may use the [Newtonsoft.Json-for-Unity.Converters package](https://github.com/applejag/Newtonsoft.Json-for-Unity/wiki/Install-Converters-via-UPM) if you need to serialize types such as Vector2, Vector3, Matrix4x4, Quaternions, Color and any other Unity type.

### Save/Load the Serialized Data Class

You can Save/Load your **PlayerData** like so:

```csharp
using UnityEngine;
using ActionCode.Persistence;

public sealed class PlayerDataTester : MonoBehaviour
{
    public PersistenceSettings settings;

    [ContextMenu("Save")]
    public async void Save()
    {
        var data = new PlayerData
        {
            score = 100,
            lastLevel = 1,
            playerName = "MegaMan"
        };
        var wasSaved = await settings.Save(data, slot: 0);

        print("Was data saved? " + wasSaved);
        print(data);
    }

    [ContextMenu("Load")]
    public async void Load()
    {
        var data = new PlayerData();
        var wasLoaded = await settings.TryLoad(data, slot: 0);

        print("Was data loaded? " + hasData);
        print(data);
    }
}
```

>Don't forget to link the settings asset reference created by the Persistence Settings menu.

### Checking the Persisted Data

Go to the Persistence Menu into the Project Settings or select the Persistence Settings asset.

If you want to debug your persisted data, make sure to enable the **Save Raw File**. 
This way a legible file with the serializer extension will be saved next the encrypted/compressed one.

![The SaveSlotFiles](/Docs~/SaveSlotFiles.png "The Save Slot Files")

>**This raw file is only saved on Editor**. Your build will always save to and load from the encrypted/compressed data.

To check the files, click on the **Open Save Folder** button.

![The SaveRawFile](/Docs~/SaveRawFile-OpenSaveFolder.png "The Save Raw File option")

When in the develop process, you can enable the optional boolean parameter *useRawFile* from any `PersistenceSettings.TryLoad<T>()` function to quickly load your data from the raw file.

```csharp
var wasLoaded = await settings.TryLoad(data, slot: 0, useRawFile: true);
```

This function is faster since it will not uncompress and decrypt the raw file and you can edit this text file manually.

### Delete and List Data

You can also List all files or delete them using PersistenceSettings class.

## Installation

### Using the Package Registry Server

Follow the instructions inside [here](https://cutt.ly/ukvj1c8) and the package **ActionCode-Persistence** 
will be available for you to install using the **Package Manager** windows.

### Using the Git URL

You will need a **Git client** installed on your computer with the Path variable already set. 

- Use the **Package Manager** "Add package from git URL..." feature and paste this URL: `https://github.com/HyagoOliveira/Persistence.git`

- You can also manually modify you `Packages/manifest.json` file and add this line inside `dependencies` attribute: 

```json
"com.actioncode.persistence": "https://github.com/HyagoOliveira/Persistence.git"
```

---

**Hyago Oliveira**

[GitHub](https://github.com/HyagoOliveira) -
[BitBucket](https://bitbucket.org/HyagoGow/) -
[LinkedIn](https://www.linkedin.com/in/hyago-oliveira/) -
<hyagogow@gmail.com>