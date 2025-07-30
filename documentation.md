# Documentation

This section provides some documentation for the classes that result from decompiling `Assembly-CSharp.dll` of *Kingdom: Classic* using dnSpy.

**Note to self**: Never copy any code snippets that result from the decompilation.

## Unity Prefabs

Prefabs in Unity form the templates from which GameObjects can be instantiated so that they have common default attributes.

AssetRipper can be used to determine which prefabs the game depends on. Use AssetRipper to import `\home\user\.local\share\Steam\steamapps\common\Kingdom\Kingdom_Data\`. Then, under `Bundles > Generated Hierarchy Assets > Collections > Generated Prefabs` you will find a list of all prefabs that AssetRipper extracted. Note that you should indeed import the entire `Kingdom_Data/` directory, because AssetRipper requires the meta-data stored in the bundled assets to determine what the *original path* used to be before compilation. When not working with the Unity editor, but instead creating pure c# modifications of the decompiled source code, you may not always have easy access to a prefab reference as you would with the Unity inspector. Thus, knowing the original path of the prefabs at runtime allows us to manually load any prefab.

For example, if we only import the file `Kingdom_Data/resources.assets` into AssetRipper then `Bundles > Generated Hierarchy Assets > Collections > Generated Prefabs` lists `Archer` as a prefab. However, due to lacking metadata the prefab only specifies `Override Path: Assets/GameObject/Archer` but does not specify a `Original Path` attribute. On the othe hand, importing the entire `Kingdom_Data/` directory into AssetRipper reveals that `Original Path: Assets/Resources/prefabs/characters/Archer` for the `Archer` prefab. In other words, the meta-data stored by Unity reveals that we may obtain the archer prefab at runtime by making use of the `Resources.Load` function as follows.

```c#
Resources.Load<Archer>("prefabs/characters/Archer")
```

Note that the load function is always relative to all subdirectories named `Resources` of the `Assets` directory. Thus, we pass `"prefabs/characters/Archer"` to `Resources.Load` because `"Assets/Resources/"` is implicitly prepended to the search path. See [the Unity docs](https://docs.unity3d.com/ScriptReference/Resources.Load.html).


## World

The `World` class contains and manages the forests, grasslands and world bounds.

## Managers

The `Managers` class contains all manager type objects. It serves as a globally accessible (static) container of manager singletons.

### AchievementManager

Some Steam achievement descriptions for *Kingdom: Classic* are somewhat ambiguous. Refer to [Wild's Steam achievement guide](https://steamcommunity.com/sharedfiles/filedetails/?id=2173013207) for the general outline of how to complete all achievements.

The `AchievementManager` class instantiates a list of runtime achievements.

Additionally, it defines a `Counter` enum and a `Flag` enum. Both enums are used to typecast the enum elements to indexes of respectively the `AchievementManager._counters` array and `AchievementManager._flags` array.

Enum | Element Name (Array Index)
-- | --
Counter | None (0), Day (1), HuntedRabbits (2), Archers (3), HuntedDeers (4), KilledEnemies (5), DestroyedPortals (6), ClearedLand (7)
Flag | None (0), WorkerAcquired (1), FreeWallBuilt (2), LitUpFire (3), Galloped (4), CoinDroppedOnGround (5), WallBuilt (6), PortalsDestroyed (7), CoinFellOut (8), Won (9)


#### Achievement

The `Achievement` class implements all of the Steam achievements as public methods.
- All "Safe in X" achievements are implemented by the `Achievement.SafeInX` method.
- All "Survive X days" achievements are implemented by the `Achievement.SurvivedXDays` method.
- Each "on the X-th day I ..." achievement is implemented as a separate method.

As a practical example, the line
```c#
int current_day = Managers.achievements.counters[1];
```
obtains the counter representing the `Counter.Day` enum element, which corresponds to the current day number.

## EnemyManager

The `EnemyManager` class provides an interface for spawning a `Wave` of enemies.

The `EnemyManager` class specifies only four `Enemy` prefabs: `trollPrefab, ogrePrefab, squidPrefab, bossPrefab`. There is no separate prefabs for the three troll types, all troll objects are instantiated from the single troll prefab. nor for the killer boss type.

### EnemyType

The `EnemyType` Enum contains the elements `TrollWeak, TrollMedium, ToughTroll, Ogre, Squid, Boss, KillerBoss`.

The method `List<EnemyBlueprint> EnemyManager::GetEnemies(Wave, int, bool)` is a good starting point for understanding the `EnemyType` enum.

We differentiate between the enemy types in code and [on the wiki](https://kingdomthegame.fandom.com/wiki/Category:Monsters).

code class | wiki name
-- | --
Troll | Greedling
Squid | Floater
Boss | Breeder
Ogre | Legacy Breeder
Killer Boss | ?

The Ogre is a Legacy implementation of the Breeder, which can throw rocks but not spawn greedlings. The current Breeder can do both.

**TODO**: See Kingdom-GOD and spawn in these entities. Backup the current .dll !

**TODO**: See Kingdom-GOD and spawn in these entities. Backup the current .dll !

**TODO**: See Kingdom-GOD and spawn in these entities. Backup the current .dll !

**TODO**: See Kingdom-GOD and spawn in these entities. Backup the current .dll !

**TODO**: See Kingdom-GOD and spawn in these entities. Backup the current .dll !

**TODO**: See Kingdom-GOD and spawn in these entities. Backup the current .dll !


#### Unpacking


#### Boss

The *Boss* class represents the breeders? They scan for builders to throw?

#### KillerBoss



#### Squid

The *Squid* class represents the floaters? They only scan for "citizens"?

#### Ogre

???

#### Troll

The *Troll* class represents the basic greeds?

## Wave

The `Wave` class has many notable members. 

Type | Members
-- | --
AnimationCurve | `numTrolls, numToughTrolls, numSquids, numOgres, numBosses, numKillerBosses`
bool | `spawnOnBothSides, useOuterSpawners, special`

A `Wave` object simply specifies the counts of each enemy type that should be spawned in a particular wave. Actually instantiating the required enemy objects seems to happen by invoking `List<EnemyBlueprint> EnemyManager::GetEnemies(Wave, int, bool)` given a `Wave` object as a recipe, to generate a list `List<EnemyBlueprint>` of actual enemy objects. Though at that point, the enemies have not yet been added to the scene.

Namely, the method `void EnemyManager::SpawnWave(Wave, int, Side)` is the API for spawning a `Wave`, meaning it takes a `Wave` as input, then invokes `EnemyManager::GetEnemies` given that wave to instantiate the enemies, and passes the resulting list to a particular `Portal` object (the closest portal given a left/right kingdom side) by invoking the spawner function `void Portal::SpawnEnemies(ICollection<EnemyBlueprint>, bool)` to queue that list of enemies for spawning. Note that `Portal::HandleOnDeath` and `Portal::SpawnDefenseWave` directly invoke `Portal::SpawnEnemies`, bypassing `EnemyManager::SpawnWave`.

A wave is either spawned on both sides simultaneously, or on one side at a time. The function `EnemyManager::SpawnWave` is invoked by two functions to implement spawning a wave on one or two sides.

1. `DirectorEvent::Execute(float)` invokes `EnemyManager::SpawnWave` iff. `DirectorEvent.type == DirectorEvent.Type.SpawnWave`. The method `List<DirectorEvent> Day::EventsWave(Wave,int)` returns a list containing an event of type `DirectorEvent.Type.ScheduleWave` for each side that a wave should be spawned on.
    - **Both sides**: If `Wave.spawnOnBothSides == true` then `Day::EventsWave(Wave,int)` returns a list of *two* `DirectorEvent.Type.ScheduleWave` events so that a wave is scheduled for both the left and right side simultaneously.
    - **One side**: If `Wave.spawnOnBothSides == false` then `Day::EventsWave(Wave,int)` returns a list of *one* `DirectorEvent.Type.ScheduleWave` event. It performs a 50/50 coin flip to randomly, uniformly decide which of the two sides (right xor left) the wave should spawn at.
1. `EnemyManager::SpawnWaveImmediate(Wave)` always invokes `EnemyManager::SpawnWave` directly instead of relying on `DirectorEvent` objects!
    - **Both sides**: If `Wave.spawnOnBothSides == true` then `void EnemyManager::SpawnWaveImmediate(Wave)` spawns a wave on both the left and right sides simultaneously.
    - **One side**: If `Wave.spawnOnBothSides == false` then `void EnemyManager::SpawnWaveImmediate(Wave)` performs a 50/50 coin flip to randomly, uniformly decide which of the two sides (right xor left) the wave should spawn at.


The results of the coin flips are indeed uniformly distributed, because they are implemented as the check `Random.value < 0.5f`, and [Random.value](https://docs.unity3d.com/ScriptReference/Random-value.html) is a Unity api that samples a float in the continuous range $[0, 1]$ for which "*Any given float value between 0.0 and 1.0, including both 0.0 and 1.0, will appear on average approximately once every ten million random samples.*"

## Day

The method `List<DirectorEvent> Day::GenerateEvents(int)` is a central method of the `Day` class. Namely, it schedules a wave iff. its `Day.wave != null`.

The method `void Director::AddCycle()` actually implements the logic of scheduling normal days, bloodmoon days (called "bossDays"), and the safe days after a bloodmoon (called "recoveryDays"). Namely, the method `void Director::AddCycle()` generates a list of days, and on each day individually invokes `void Director::AddDay(Day)`, which in turn calls `List<DirectorEvent> Day::GenerateEvents(int)` to schedule all necessary events for any given `Day` object.

**TODO**: Look deeper into the implementation of `void Director::AddCycle()` to figure out how normal days, bloodmoons and recovery days work! How to work a seed into this???



## Chest

The `Chest::RetrieveData` method returns a `ChestData` object that contains a public `ChestData.coins` integer member.

## DevTools

The `DebugTools` class exists in `Assembly-CSharp.dll` of both *Kingdom: Classic* and *Kingdom: New Lands*. It represents the dev tools used by the admin/dev panel for testing purposes.

For the *Kingdom God* v3 mod, Owmince seems to have modified the `Game::Update` method by appending a case that checks with the Rewired input controller whether keycode TAB has been pressed, and enables the debug menu if it has. Else, when the debug menu is invisible, with `Game::OnGUI` Owmince instead displays a custom banner at the bottom of the screen, to inform the player that TAB toggles the debug menu.
