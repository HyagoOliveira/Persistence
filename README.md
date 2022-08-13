# Persistence

* Save Serialized data using encryption and compression
* Unity minimum version: **2019.3**
* Current version: **1.0.0**
* License: **MIT**
* Dependencies:
    - [com.actioncode.cryptography : 1.0.0](https://github.com/HyagoOliveira/Cryptography/tree/1.0.0)
    - [com.actioncode.scriptable-settings-provider : 1.0.0](https://github.com/HyagoOliveira/ScriptableSettingsProvider/tree/1.0.0)

## Summary

This package can save Serialized data, first encrypting and/or compressing it. You can select if you want to only encrypt, compress or use both.
Also, you can choose witch serialization method (Json, XML, Binary) to use.

To do it, you need first to create a **Persistence Settings** asset and use it on your Serialized class.

## How To Use

### Creating a Persistence Settings

Open the **Project Settings** Windows and select Persistence (below ActionCode group).

Click on the **Create** button and save a new Persistence Settings asset.

![The Persistence Settings Menu](/Docs~/PersistenceSettingsMenu.png "The Persistence Settings Menu")

Now you can select a Serializer, Compression, Cryptographer and other settings to use when save your data. Each property has a Tooltip description.

If you're using a Cryptographer method, set a new Cryptographer Key using the **Get New Cryptographer Key** button.  

### Creating the Serialized Class

Your Serialized class should always use the `System.Serializable` attribute.

```csharp
using System;

[Serializable]
public sealed class PlayerData
{
    public int score;
    public int lastLevel;
    public string playerName;
    public DateTime LastUpdateTime = DateTime.Now;
}
```

>Note: if you're planning to use **XML Serialization** you should create a constructor to each serialized class.

Now you can Save/Load your **PlayerData** like so:

```csharp
using UnityEngine;
using ActionCode.Persistence;

public sealed class PlayerDataManager : MonoBehaviour
{
    public PersistenceSettings settings;

    public void Save()
    {
        var data = new PlayerData
        {
            score = 100,
            lastLevel = 1,
            playerName = "MegaMan"
        };

        var wasSaved = settings.Save(data, slot: 0);
        //var wasSaved = settings.Save(data, name: "PlayerSave");

        if (wasSaved) print("Player data was Saved!");
    }

    public void Load()
    {
        var wasLoaded = settings.TryLoad(out PlayerData data, slot: 0);
        //var wasLoaded = settings.TryLoad(out PlayerData data, name: "PlayerSave");

        if (wasLoaded) print("Player data was loaded!");
    }
}
```

Don't forget to link the settings asset reference created by the Persistence Settings menu.

### Checking the Persisted Data

Go to the Persistence Menu into the Project Settings or select the Persistence Settings asset.

If you want to debug your persisted data, make sure to enable the **Save Raw File**. 
This way a legible file will be saved next the encrypted/compressed one.

>**This file is only saved on Editor**. Your build will always save only the encrypted/compressed data. 

To check the files, click on the **Open Save Folder** button.

![The SaveRawFile](/Docs~/SaveRawFile-OpenSaveFolder.png "The Save Raw File option")

## Installation

### Using the Package Registry Server

Follow the instructions inside [here](https://cutt.ly/ukvj1c8) and the package **ActionCode-Persistence** 
will be available for you to install using the **Package Manager** windows.

### Using the Git URL

You will need a **Git client** installed on your computer with the Path variable already set. 

- Use the **Package Manager** "Add package from git URL..." feature and paste this URL: `https://github.com/HyagoOliveira/Persistence.git`

- You can also manually modify you `Packages/manifest.json` file and add this line inside `dependencies` attribute: 

```json
"com.actioncode.persistence":"https://github.com/HyagoOliveira/Persistence.git"
```

---

**Hyago Oliveira**

[GitHub](https://github.com/HyagoOliveira) -
[BitBucket](https://bitbucket.org/HyagoGow/) -
[LinkedIn](https://www.linkedin.com/in/hyago-oliveira/) -
<hyagogow@gmail.com>