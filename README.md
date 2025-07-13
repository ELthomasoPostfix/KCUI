# KCUI

A basic UI mod for *Kingdom: Classic* (v1.2.0 (R290)), to display hidden in-game parameters.

*Kingdom: Classic* is the first game in the *Kingdom* series of games. It has fewer features that are more basic than those of its successors, with limited end-game content and a basic winning strategy: archer spam. This leaves room for simplistic, but still meaningful mods. Additionally, according to the [wiki](https://kingdomthegame.fandom.com/wiki/Patch_notes_for_Classic) "*1.2.0 (R290) is the last and definitive update to Kingdom: Classic. Scheduled to 22 March 2016, it was released on 7 April 2016, and its logs were published on 11 April 2016.*" This ensures long term stability of any mods, because the game will supposedly not receive breaking changes in the future.

Note: *Kingdom Two Crowns* is a much more feature rich game than *Kingdom: Classic*. If you are just looking for a game to play, probably prefer *Kingdom Two Crowns* over modding *Kingdom: Classic*.

## Disclaimer of Non-Affiliation

*KCUI* is an independent, fan-created modification for *Kingdom: Classic*. It is not affiliated with, endorsed by, or approved in any way by Raw Fury, Licorice, or any other parties involved in the development or publishing of *Kingdom: Classic*.

All trademarks, registered trademarks, product names, and company names or logos mentioned in this project are the property of their respective owners. Use of these names, trademarks, and brands does not imply endorsement.

*KCUI* does not contain or distribute any copyrighted game files from *Kingdom: Classic*. This project contains only original code and content authored by the creator(s) of *KCUI*. Any references to game binaries (such as `Assembly-CSharp.dll`) are strictly for interoperability purposes and must be supplied by the user from their own legally purchased copy of *Kingdom: Classic*.

At the time of writing (10 July 2025) the *Kingdom* series fan content policy was last updated on 16 February 2023.
You can find an archive of this version [here](https://web.archive.org/web/20250620160343/https://www.kingdomthegame.com/fancontentpolicy).

# Development

This project is developed using JetBrains Rider and MelonLoader on Windows 10. Configure the Rider project for the .NET framework v3.5 as target framework.
On Windows, you may install the .NET framework v3.5 via the "Turn Windows features on or off" feature. 
This version is required because *Kingdom: Classic* was developed using Unity 5.3.x and compiled for the .NET framework 3.5 runtime.
Note that the .NET framework is a Windows specific implementation, and is not the same thing as .NET Core.

We require **MelonLoader v0.5.7** to be set up for *Kingdom: Classic*. We tested some newer and or older versions of MelonLoader, but they seemed to not be compatible.
The basic workflow of modding is as follows.
1) Install the MelonLoader installer program.
2) Install MelonLoader v0.5.7 for *Kingdom: Classic*.
3) Launch *Kingdom: Classic* once via Steam to properly setup MelonLoader for the game.
4) Copy and Paste the mod `.dll` into the `Kingdom/Mods`.
5) Launch *Kingdom: Classic* again via steam. The mod should now be loaded.

Then, to configure Rider, add the following references to `Dependencies/Assemblies` of the Rider Solution tab's file tree, by right clicking on `Assemblies` and selecting the `Reference...` button. For the sake of consistency during developent (i.e. the `.csproj` file depends on it), copy the following `.dll` files to the `assemblies/` directory in this project. Note that the `.gitignore` should exclude the `assemblies/` directory from commits to the repo! We should not (or are not allowed to) host these assemblies.
```bash

# Add MelonLoader dependencies, for patching the original .dll.
/home/user/.local/share/Steam/steamapps/common/Kingdom/MelonLoader/net6/MelonLoader.dll
/home/user/.local/share/Steam/steamapps/common/Kingdom/MelonLoader/net6/0Harmony.dll

# Add the Kingdom internal logic dependency.
/home/user/.local/share/Steam/steamapps/common/Kingdom/Kingdom_Data/Managed/Assembly-CSharp.dll

# Add UnityEngine dependencies.
/home/user/.local/share/Steam/steamapps/common/Kingdom/Kingdom_Data/Managed/UnityEngine.dll
/home/user/.local/share/Steam/steamapps/common/Kingdom/Kingdom_Data/Managed/UnityEngine-UI.dll
/home/user/.local/share/Steam/steamapps/common/Kingdom/Kingdom_Data/Managed/Rewired_Core.dll
```
Note that `Rewired_Core.dll` takes the place of `UnityEngine.InputModule.dll` and *Kingdom: Classic* does not use `UnityEngine.CoreModule.dll` because it was built using an older version of Unity.

Also see [this tutorial](https://steamcommunity.com/sharedfiles/filedetails/?id=2968763665) on steam and [this tutorial](https://www.youtube.com/watch?v=_8B80owys4w) on youtube.

# dnSpy + AssetRipper

**Warning**: install and use dnSpy and AssetRipper at your own peril. Always beware of malware.

Because *Kingdom: Classic*, as available on steam, was developed using Unity and compiled for the Mono runtime, we can simply use [dnSpy](https://github.com/dnSpy/dnSpy) to decompile `Assembly-CSharp.dll`. This allows us to inspect the game's internals. With regards to the local Steam installation, dnSpy is specifically looking for `\home\user\.local\share\Steam\steamapps\common\Kingdom\Kingdom_Data\Managed\Assembly-CSharp.dll`.

Additionally, using [AssetRipper](https://github.com/AssetRipper/AssetRipper) we can uncompress the Unity asset files, to get a better idea which in-code classes correspond to which in-game entities, by comparing the naming scheme of the assets (mainly the `.png` sprite maps) with the class names. The decompilation process with dnSpy does not yield meaningful comments, so this extra step provides clarity. With regards to the local Steam installation, AssetRipper is specifically looking for all files located at `\home\user\.local\share\Steam\steamapps\common\Kingdom\Kingdom_Data\`. Particularly, the `.assets` files should contain the spite maps. run AssetRipper, and click `File > Open Folder` at the path mentioned above. Then click `View Loaded Files`, which should list `Sprite Data Storage` under the heading Collection. Now, clicking `Export > Export All Files > Export Primary Content` should dump all assets, including the sprites, to the chosen output directory.

This approach rests on lawful interoperability - creating a mod that works with the game - while respecting both DMCA and EU rules. As such, **we do not distribute any of the original game files, uncompressed assets, decompiled source code, nor parts thereof, all legally obtained by purchasing a copy of *Kingdom: Classic* through Steam, in this repository.**

Any further discussion targets the Steam version v1.2.0 (R290) (Build ID 1062187) of *Kingdom: Classic* (App ID 368230). Also see [SteamDB](https://steamdb.info/app/368230/patchnotes/).

# Existing Mods

For the sake of research, I looked into existing mods for Kingdom. A non-exhaustive list of references follows.

**[Kingdom GOD](https://owmince.com/kingdom-god/)** by owmince has separate versions for *Kingdom: Classic* and *Kingdom: New Lands*. The latest version of the mod is v3 for *Kingdom: New Lands* v1.2.8, and makes the dev panel available in-game. However, the Lite version of the mod for *Kingdom: Classic* v1.2.0 does not utilize the Dev panel. Instead, it adds an `Owmince` class to `Assembly-CSharp.dll` to implement a custom mod panel.

Another set of mods is available on [github](https://github.com/brooklinymym279/Kingdom-mods-experience-points) or equivalently on the [wemod](https://www.wemod.com/cheats/kingdom-classic-trainers) website.
