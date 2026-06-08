# Rithwic Warp Plugin

## Overview

ACE.Plugin.Warp spawns a custom portal in Rithwic and registers a collide object hook
using the `warp_AddCollideObjectHook` API from ACE.Server.WorldObjects.Portal.

## What it does

1. **Spawns a portal** in Rithwic (landblock 0x00CD, position 96, 96, 0)
   - Uses WeenieClassId 1025 (portalrithwic)
   - Static GUID: 0x700CD100

2. **Registers a collide hook** via `portal.warp_AddCollideObjectHook(OnPortalCollide)`
   - When a player collides with the portal, the hook fires
   - Logs the collision event
   - Sends "You have entered the Rithwic portal zone." to the player

## Files

- `Init.cs` - Plugin entry point implementing IACEPlugin
- `ACE.Plugin.Warp.csproj` - Project file

## Deployment

1. Build: `dotnet build ACE.Plugin.Warp.csproj -c Release`
2. Copy DLL to: `/ace/Plugins/ACE.Plugin.Warp/ACE.Plugin.Warp.dll`
3. Add to Config.js Plugins list: `"ACE.Plugin.Warp"`

## warp_AddCollideObjectHook API

Defined in `ACE.Server.WorldObjects.Portal`:

```csharp
public void warp_AddCollideObjectHook(Action<Portal, Player> action)
```

The hook is called from `Portal.OnCollideObject(Player player)` which invokes
all registered hooks before calling `OnActivate(player)`.

## Coordinates

- Landblock: 0x00CD (Rithwic area)
- Position: (96, 96, 0) within landblock
- Rotation: (0, 0, 0, 1) - facing north
