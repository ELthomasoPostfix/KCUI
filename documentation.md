# Documentation

This section provides some documentation for the classes that result from decompiling `Assembly-CSharp.dll` of *Kingdom: Classic* using dnSpy.

**Note to self**: This section should not copy any code snippets that result from the decompilation.

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

## Chest

The `Chest::RetrieveData` method returns a `ChestData` object that contains a public `ChestData.coins` integer member.

## DevTools

The `DebugTools` class exists in `Assembly-CSharp.dll` of both *Kingdom: Classic* and *Kingdom: New Lands*. It represents the dev tools used by the admin/dev panel for testing purposes.

For the *Kingdom God* v3 mod, Owmince seems to have modified the `Game::Update` method by appending a case that checks with the Rewired input controller whether keycode TAB has been pressed, and enables the debug menu if it has. Else, when the debug menu is invisible, with `Game::OnGUI` Owmince instead displays a custom banner at the bottom of the screen, to inform the player that TAB toggles the debug menu.
