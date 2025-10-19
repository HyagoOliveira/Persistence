# Persistence

* Save Serialized data using encryption and compression
* Unity minimum version: **6000.1**
* Current version: **5.0.0**
* License: **MIT**
* Dependencies:
    - [com.actioncode.async-io : 2.0.0](https://github.com/HyagoOliveira/Async-IO/tree/2.0.0)
    - [com.actioncode.cryptography : 2.0.0](https://github.com/HyagoOliveira/Cryptography/tree/2.0.0)
    - [com.actioncode.scriptable-settings-provider : 1.0.0](https://github.com/HyagoOliveira/ScriptableSettingsProvider/tree/1.0.0)

## Summary

This package can save Serialized data, after encrypting and/or compressing it. You can select if you want to only encrypt, compress or use both.
Also, you can choose which serialization method to use (Json, XML or Binary).

Data serialization also works in WebGL builds by using the [Async-IO](https://github.com/HyagoOliveira/Async-IO) package.

To start to saving/loading data, you need first to create a **Persistence Settings** asset.

## How To Use

### Creating the Persistence Settings

Open the **Project Settings** Windows and select **Persistence** (below ActionCode group).

Click on the **Create** button and save a new Persistence Settings asset.

![The Persistence Settings Menu](/Docs~/PersistenceSettingsMenu.png "The Persistence Settings Menu")

Now you can select a Serializer, Compressor and a Cryptographer to use when saving/loading data. Each property has its own Tooltip description.

If you're using a Cryptographer method, set a new Cryptographer Key using the **Get New Cryptographer Key** button. 

>You should always use a **different Cryptographer Key** for every game you work in order to increase your data security.

Now, you should created a Serialized data class to persist its data using the Persistence Settings.

### Creating the Serialized Data Class

Your Serialized data class should always use the `System.Serializable` attribute.

```csharp
using System;
using UnityEngine;

[Serializable]
public sealed class PlayerData
{
    public int score;
    public int lastLevel;
    public string playerName;

    public DateTime Creation { get; private set; } = DateTime.Now;
    public DateTime LastUpdate { get; private set; } = DateTime.Now;

    public string ToJson() => JsonUtility.ToJson(this, prettyPrint: true);
}
```

>**Note 1**: if you're planning to use **XML Serialization** you should create a constructor to each serialized class.

>**Note 2**: *Json Utility* Serializer doesn't save *properties*, just *public fields* and *fields* with the `[SerializeField]` attribute. 
Also, some types are not supported as well. In most cases, this will not be a problem since serialized data classes should be as simple as possible. 
But if you need a custom serialization, create your own `Serializable` class like [SerializedTransform](https://github.com/HyagoOliveira/GameDataSystem/blob/main/Runtime/SerializedData/SerializedTransform.cs).

>**Note 3**: If you're using *Json Newtonsoft* serializer, you may use the [Newtonsoft.Json-for-Unity.Converters package](https://github.com/applejag/Newtonsoft.Json-for-Unity/wiki/Install-Converters-via-UPM) if you need to serialize types such as Vector2, Vector3, Matrix4x4, Quaternions, Color and any other Unity type.

Now that you have the serialized PlayerData class, let's persist it using the Persistence Settings.

### Persisting the Serialized Data Class

You can Save, Load, Delete and List your **PlayerData**:

```csharp
using System;
using UnityEngine;
using ActionCode.Persistence;

public class PlayerDataTester : MonoBehaviour
{
    public PersistenceSettings settings; // Don't forget to set this on the Inspector.

    private const string SAVE_DATA_NAME = "SaveSlot-00";

    [ContextMenu("Save")]
    public async void Save()
    {
        var data = new PlayerData
        {
            score = 100,
            lastLevel = 1,
            playerName = "MegaMan"
        };

        try
        {
            await settings.GetFileSystem().SaveAsync(data, SAVE_DATA_NAME);
            print(data.ToJson());
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    [ContextMenu("Load")]
    public async void Load()
    {
        var data = new PlayerData();

        try
        {
            var wasLoaded = await settings.GetFileSystem().TryLoadAsync(data, SAVE_DATA_NAME);
            print($"Was data loaded? {wasLoaded}");
            print(data.ToJson());
            // No exception will be thrown if 'SaveSlot-00' file does not exists.
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    [ContextMenu("Delete")]
    public void Delete()
    {
        settings.GetFileSystem().Delete(SAVE_DATA_NAME);
        print("Save data deleted.");
        // Delete() does not throws any exception, even though 'SaveSlot-00' file does not exists.
    }

    [ContextMenu("List")]
    public void List()
    {
        var fileNames = FileSystem.GetFileNames();
        print($"File Names: {string.Join(", ", fileNames)}");
        // GetFileNames() does not throws any exception.
    }
}
```

Always use a `try-catch` block to handling exceptions that may happen.

### Checking the Persisted Data

If you want to debug your persisted data in a human readable format, make sure to always use `savePrettyData = true` when saving it:

```csharp
await settings.GetFileSystem().SaveAsync(data, SAVE_DATA_NAME, savePrettyData: true);
```

This way a legible file with the Serializer extension (`.json` in this case) will be saved next to the encrypted/compressed one.

![The SaveSlotFiles](/Docs~/SaveSlotFiles.png "The Save Slot Files")

By default, `savePrettyData` is `true`.

In the development process, it is better to edit the pretty file and load the game from it. You can do it by using `useCompressedData = false` when loading:

```csharp
var wasLoaded = await settings.GetFileSystem().TryLoadAsync(data, SAVE_DATA_NAME, useCompressedData: false);
```

This approach is faster since it will not uncompress and decrypt the file.

By default, `useCompressedData` is `true`.

>**The pretty file is only saved by the Editor**. Your builds will never save a human readable file unless you set the **Compressor** and **Cryptographer** to **None**. `PersistenceSettings` will always save to and load from the encrypted/compressed data unless you explicitly change it.

### Where are the Persisted Data?

To check where the saved files are, click on the **Open Save Folder** button.

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
"com.actioncode.persistence": "https://github.com/HyagoOliveira/Persistence.git"
```

---

**Hyago Oliveira**

[GitHub](https://github.com/HyagoOliveira) -
[BitBucket](https://bitbucket.org/HyagoGow/) -
[LinkedIn](https://www.linkedin.com/in/hyago-oliveira/) -
<hyagogow@gmail.com>