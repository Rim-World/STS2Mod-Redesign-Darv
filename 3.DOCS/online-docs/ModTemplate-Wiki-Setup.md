> [!IMPORTANT]  
> Familiarity with C# and object-oriented programming are highly recommended if you want to mod Slay the Spire 2. We suggest finding other resources to learn C# first if you're new to programming or to C# specifically, such as a [Codeacademy course](https://www.codecademy.com/learn/learn-c-sharp) or a text guide like [Essential C#](https://essentialcsharp.com/home).

These steps will get you started with a project built on top of [BaseLib](https://github.com/Alchyr/BaseLib-StS2), a community-developed library that helps smooth the modding process and reduce boilerplate.

## 1. Install a C# IDE
We recommend installing [Rider](https://www.jetbrains.com/rider/) because this tutorial uses Rider for its examples and screenshots. If you have experience writing C# and prefer another IDE, feel free to use that instead (If you do use another IDE, note that Godot requires a `.sln` file, not `.slinx`).

## 2. Download Dependencies
- Download the latest version of [MegaDot](https://megadot.megacrit.com/) (Mega Crit's custom version of the Godot game engine)
  - As a fallback, you can also use the version of [Godot .NET](https://godotengine.org/download/archive/) that exactly matches the latest MegaDot version
- Install [.NET SDK](https://dotnet.microsoft.com/en-us/download) (9.0 or higher)
- Subscribe to [BaseLib on the Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3737335127). It will install in a folder in your Steam library, at `Steam/steamapps/workshop/content/2868840/3737335127/BaseLib`.
  - If for some reason that doesn't work, you can [download the latest release of BaseLib from GitHub](https://github.com/Alchyr/BaseLib-StS2/releases/) and put it in your Slay the Spire 2 mods folder. (If you're not sure where that is, consult [this page](https://github.com/Alchyr/ModTemplate-StS2/wiki/Modding-Basics#mod-files-and-where-they-go).) You can download the `.zip` and extract the files or download the `.dll`, `.pck`, and `.json` individually. (This is more complex to set up and means BaseLib won't automatically update.)

## 3. Install the Modding Templates
Open a terminal and run the following command:

    dotnet new install Alchyr.Sts2.Templates

In Rider: first create a new solution as a placeholder, then click the Terminal button in the bottom left <img hspace="5" height="30" alt="image" src="https://github.com/user-attachments/assets/0b0ae10b-0a97-45a6-95a2-667d9e0de05d" /> or use a keybind to open the terminal. If the option to open the terminal isn't there, create a new solution to get it to show up.
- Alternatively, you can download this project from Github and add it as it as a template in Rider by starting from step 7 [here](https://www.jetbrains.com/help/rider/Install_custom_project_templates.html#create-custom-project-template). 

## 4. Create a Solution Using a Template
In Rider, go to `File` > `New Solution` to create a solution using one of the 3 included templates:
- **Slay the Spire 2 Character** - a template for creating a new character
- **Slay the Spire 2 Content** - a template for adding new content (cards, relics, potions, etc.)
- **Empty Slay the Spire 2 Mod** - a minimal template, for anything else

<details>
<summary><b>If not using Rider</b></summary>
<hr/>
You can run the following command to create a project from the template

```
dotnet new alchyrsts2mod --ModAuthor !YourName -o !ModName
```

Replace `!YourName` with your name and `!ModName` with the name of your mod (don't include the `!`). Don't use spaces or underscores in your mod name. Replace `alchyrsts2mod` with `alchyrsts2charmod` for a character mod, or `alchyrsts2contentmod` for a content mod. To see all available options, run `dotnet new alchyrsts2mod --help`.
<hr/>
</details>

<img hspace="20" width="902" height="693" alt="image" src="https://github.com/user-attachments/assets/13fc965c-18bc-4ffd-9d7f-ed34dbc3b35d" />

When creating a solution:   

- **Check the box for `Put solution and project in same directory`**
- The project name should not contain any spaces
- The format must be `.sln`
- Expand `Advanced Settings` (in Rider) to adjust author and some other options. 

## 5. Edit `Directory.Build.props`
Open the `Directory.Build.props` file. Find the `<GodotPath>` property. Change it to where you installed MegaDot/Godot. Do not put quotes around it. 

Try pressing the `Build` button (Hammer icon on the top bar).

<img hspace="40" width="231" height="104" alt="image" src="https://github.com/user-attachments/assets/55f4916e-7324-4152-b0fe-217d646b8404"/>

If Slay the Spire 2 is not installed at the default location, you will get an error similar to this 

    Error  :  Slay the Spire 2 data not found at path '?/steamapps/common/Slay the Spire 2/data_sts2_windows_x86_64'

To fix this, uncomment the `<Sts2Path>` line and change it to the directory you have StS 2 installed at.

If using the character template and you get an error related to localization, the project is set up correctly.

## 6. Edit `ModName.json`
You can change the mod's display name in the mod's manifest `.json`. The file will have the same name as the project. Do not modify the id; this determines the names of the files the game will attempt to load. The other values can be modified for the mod you are making; see [the mod manifest documentation](https://github.com/Alchyr/ModTemplate-StS2/wiki/Modding-Basics#mod-manifest) for details.

## 7. Generate Character Localization
If you are using the character template, you will need to generate the localization for the character (otherwise, skip this step). Open the character class in `YourModCode/Character/YourMod.cs`. It should have two errors like this; one for "character" localization and one for "ancient" localization.

<img hspace="10" width="1099" height="132" alt="image" src="https://github.com/user-attachments/assets/1766bd33-d989-478b-a13d-ee55d76423b8" />

For each one, push alt+enter and choose "Generate localization", then move the generated text to the appropriate file (`YourMod/localization/eng/characters.json` and `YourMod/localization/eng/ancients.json`).

<img hspace="10" width="488" height="83" alt="image" src="https://github.com/user-attachments/assets/af612cea-8e18-4006-9b45-46e126a382a3" />

Localization can be generated similarly for cards, relics, ancients, and other content. You can find the full list of supported types [here](https://github.com/Alchyr/StS2ModAnalyzers/blob/master/ModAnalyzers/ModAnalyzers/LocalizationAnalyzer.cs#L28).

## 8. Get Your New Text to Show Up
> [!WARNING]
> You must publish your mod to a local folder every time you make any non-code changes (text, images, scenes, etc.) for them to show up.

For your mod's text, images, and any other non-code changes to work, you must publish your mod. Just pressing `Build` is not enough. To set up publishing, right click the project in the left sidebar and choose `Publish`.

<img hspace="10" width="536" height="329" alt="image" src="https://github.com/user-attachments/assets/1acda21b-8361-4d8b-809f-b95158951b72" />

In Rider, choose `Local folder`. Publish options can be left as default. 

Publishing will compile your mod's `.dll`, generate a `.pck` with your mod's assets, and copy the mod's files (`.dll`, `.pck`, and `.json`) to Slay the Spire 2's mods folder so that the game loads it. If Godot's path is not set correctly in `Directory.Build.props` attempting to publish will give you an error (see step 5).

Publishing is slightly slower than building due to the additional process of generating the `.pck` through Godot. If you modify only `.cs` code files, you can `Build` instead by clicking the hammer button on the top bar. This will only compile the `.dll` containing your code and copy it to the mods folder. If you make any changes to other files, you must publish for them to show up.

## Running the game

After publishing, you should be able to run the game and see your mod in the mod list (under Settings -> Mod Settings).

## Next steps
You will need more information to develop a mod. Here's what you should start with.
* Note all the generally useful information in [Modding Basics](https://github.com/Alchyr/ModTemplate-StS2/wiki/Modding-Basics)
* Learn how to [decompile the game](https://github.com/Alchyr/ModTemplate-StS2/wiki/Decompiling) and [extract game assets and text](https://github.com/Alchyr/ModTemplate-StS2/wiki/Extracting-Assets-and-Text); seeing how the base game does things is one of the best ways to learn
* Understand how [testing and debugging](https://github.com/Alchyr/ModTemplate-StS2/wiki/Testing-and-Debugging) works so you can check if your code works, understand errors, and fix things
* Read through the [other reference material linked from the main page](https://github.com/Alchyr/ModTemplate-StS2/wiki#adding-content), covering specific types of content, external resources, and more

## Troubleshooting Common Issues

- Error: **`[MSB4236] The SDK 'Godot.NET.Sdk/4.5.1' specified could not be found.`**\
Run the following in a terminal
```
dotnet nuget add source https://api.nuget.org/v3/index.json
```

- Error: **`[MSB4236] The SDK 'Microsoft.NET.SDK.WorkloadAutoImportPropsLocator' specified could not be found.`**\
Go into `File` > `Settings` > search for `toolset` and change `MSBuild version` to one mentioning rider

<img hspace="40" width="1000" alt="change-msbuild" src="https://github.com/user-attachments/assets/c25178d7-ac6a-4f0d-b988-f588f0355700" />

- Publish doesn't work
  - Add `DOTNET_ROOT=~/.dotnet` (valid path to wherever dotnet is) at start of GodotPublish command.

- Very long error that starts with `Godot.Bridge.CSharpInstanceBridge.Call` and has a lot of **`System.ArgumentException: Value does not fall within the expected range.
   at MonoMod.Core.Interop.CoreCLR.V60.InvokeCompileMethod(IntPtr functionPtr, IntPtr thisPtr, IntPtr corJitInfo, CORINFO_METHOD_INFO* methodInfo, UInt32 flags, Byte** nativeEntry, UInt32* nativeSizeOfCode)`**
  - Either enable publicizing the StS 2 assembly (by going to your csproj file, finding the `<Publicize Include="sts2"` line, and changing `False` to `True` on the line above it) or remove the package `Krafs.Publicizer` from your project (you can do this from NuGet in Rider)

- Error: `System.ArgumentException: Undefined resource string ID:0x80070057`
  - Same as above, either enable publicizing or remove `Krafs.Publicizer` from your project
