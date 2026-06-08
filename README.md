[![Run in Postman](https://run.pstmn.io/button.svg)](https://app.getpostman.com/run-collection/22538867-7bce2470-b0ee-45a9-8207-e92afeb09616?action=collection%2Ffork&collection-url=entityId%3D22538867-7bce2470-b0ee-45a9-8207-e92afeb09616%26entityType%3Dcollection%26workspaceId%3D787de432-978a-4e40-8059-6af7d905d34b#?env%5BACE.Plugins%5D=W3sia2V5IjoidG9rZW4iLCJ2YWx1ZSI6IiIsImVuYWJsZWQiOnRydWUsInR5cGUiOiJhbnkiLCJzZXNzaW9uVmFsdWUiOiJleUpoYkdjaU9pSklVekkxTmlJc0luUjVjQ0k2SWtwWFZDSjkuZXlKb2RIUndPaTh2YzJOb1pXMWhjeTU0Yld4emIyRndMbTl5Wnk5M2N5OHlNREExTHpBMUwybGtaVzUwYVhSNUwyTnNZV2x0Y3k5dVlXMWxJam9pWVdSdGFXNGlMQ0pCWTJOdi4uLiIsInNlc3Npb25JbmRleCI6MH1d)

# ACE-Plugins

A collection of unofficial [ACEmulator](https://github.com/ACEmulator/ACE) plugins. ACE Plugins are instantiated and run from within the primary execution context of ACE.

## Plugins

| Plugin | Description | Dependencies |
|--------|-------------|--------------|
| **ACE.Plugin.Crypto** | Self-signing and startup services for certificates, data signing, certificate utilities. | None |
| **ACE.Plugin.Web** | Hosts a customizable website and various general API endpoints. | None |
| **ACE.Plugin.Transfer** | Character backup/restore, interserver character migration, character transfers between accounts. | ACE.Plugin.Crypto, optionally ACE.Plugin.Web |
| **ACE.Plugin.Warp** | Portal collision zone system with configurable collision messages. | None |

## Prerequisites

- **.NET 10 SDK** — plugins target `net10.0` to match ACE.Server
- **ACE.Server DLLs** — copied to `Dependencies/ACE/` for build references. Extract from a running ACE container:
  ```bash
  docker cp ace-server:/ace/ACE.Common.dll Dependencies/ACE/
  docker cp ace-server:/ace/ACE.Entity.dll Dependencies/ACE/
  docker cp ace-server:/ace/ACE.Server.dll Dependencies/ACE/
  docker cp ace-server:/ace/ACE.Database.dll Dependencies/ACE/
  docker cp ace-server:/ace/ACE.DatLoader.dll Dependencies/ACE/
  ```

## Building

```bash
cd Source
dotnet build ACE.Plugins.sln -c Release
```

Each plugin outputs to `Source/ACE.Plugin.<Name>/bin/Release/net10.0/`.

## Deploying to ACE Server

### Step 1: Copy plugin DLLs

Copy each plugin DLL to its own folder under `ACE/Plugins/`:

```
ACE/Plugins/
  ACE.Plugin.Crypto/
    ACE.Plugin.Crypto.dll
    crypto.js
  ACE.Plugin.Transfer/
    ACE.Plugin.Transfer.dll
    transfer.js
  ACE.Plugin.Web/
    ACE.Plugin.Web.dll
    web.js
  ACE.Plugin.Warp/
    ACE.Plugin.Warp.dll
    warp.js
```

### Step 2: Copy NuGet dependencies (Web & Transfer only)

The Web and Transfer plugins use ASP.NET Core. Copy all DLLs from the build output:

```bash
cp Source/ACE.Plugin.Web/bin/Release/net10.0/*.dll ACE/Plugins/ACE.Plugin.Web/
cp Source/ACE.Plugin.Transfer/bin/Release/net10.0/*.dll ACE/Plugins/ACE.Plugin.Transfer/
```

### Step 3: Copy shared framework DLLs (Web & Transfer only)

ACE's PluginManager uses `Assembly.LoadFile()` which cannot resolve shared framework assemblies. Copy ALL shared framework DLLs into each plugin directory:

```bash
# On the host or inside the container:
cp /usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.*/\*.dll ACE/Plugins/ACE.Plugin.Web/
cp /usr/share/dotnet/shared/Microsoft.AspNetCore.App/10.0.*/\*.dll ACE/Plugins/ACE.Plugin.Transfer/
cp /usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.*/System.IO.Pipelines.dll ACE/Plugins/ACE.Plugin.Web/
cp /usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.*/System.Text.Json.dll ACE/Plugins/ACE.Plugin.Web/
cp /usr/share/dotnet/shared/Microsoft.NETCore.App/10.0.*/System.Text.Encodings.Web.dll ACE/Plugins/ACE.Plugin.Web/
```

### Step 4: Merge config fragments into Config.js

Each plugin has a `.js` config file that defines a top-level JSON key. These MUST be merged into the main `Config.js`:

| Plugin | Config file | Top-level key in Config.js |
|--------|------------|---------------------------|
| Crypto | `crypto.js` | `CryptoConfiguration` |
| Transfer | `transfer.js` | `TransferConfiguration` |
| Web | `web.js` | `WebConfiguration` |
| Warp | `warp.js` | `WarpConfiguration` |

Plugins read from both their local `.js` file AND the main `Config.js` — Config.js values take precedence.

Example for Warp:
```json
  "WarpConfiguration": {
    "CollisionMessages": [
      "You have entered the Rithwic portal zone.",
      "The shimmering portal envelops you."
    ]
  }
```

### Step 5: Enable plugins in Config.js

Add plugin names to the `Plugins.Plugins` array in `Config.js`:

```json
"Plugins": {
  "Enabled": true,
  "Plugins": [
    "ACE.Plugin.Crypto",
    "ACE.Plugin.Web",
    "ACE.Plugin.Transfer",
    "ACE.Plugin.Warp"
  ]
}
```

### Step 6: Docker setup

Ensure your `docker-compose.yml` mounts the Plugins directory:

```yaml
volumes:
  - ./Plugins:/ace/Plugins
```

And uses the `dotnet/aspnet` base image (not `dotnet/runtime`) so the shared framework is available:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble
```

### Step 7: Restart

```bash
cd /path/to/ACE
docker compose build
docker compose create --force-recreate && docker compose start
```

## Verifying deployment

Check server logs for the plugin initialization summary:

```
Plugin initialization summary: Success: [ ACE.Plugin.Crypto, ACE.Plugin.Web, ACE.Plugin.Transfer, ACE.Plugin.Warp ]
```

Each plugin should show `Success`. If a plugin fails, check for:
- Missing config file in the plugin directory
- Missing shared framework DLLs (Web/Transfer)
- Framework version mismatch (must be `net10.0`)

## Configuration

Each plugin uses `Log4netEnabledPluginConfigManager<T>` to load a `.js` JSON config file from the plugin root. The config file name matches the plugin name in lowercase.

### Crypto (`crypto.js`)
- `FilePathKeyAndCertBundleWeb` — path to PKCS#12 cert for web services (null = auto-generate)
- `FilePathKeyAndCertBundleDataSigner` — path to PKCS#12 cert for character data signing (null = auto-generate)
- Password fields for encrypted PKCS#12 files

### Transfer (`transfer.js`)
- `AllowBackup` — allow character backup exports
- `AllowImport` — allow character imports
- `AllowMigrate` — allow character migration between servers
- `AllowMigrationFrom` — list of trusted server thumbprints for incoming migrations
- `KeepMigrationsForDays` — days to keep pending migration files (-1 = indefinitely)

### Web (`web.js`)
- `Host` — IP address to listen on (default: `0.0.0.0`)
- `Port` — HTTP port (default: `9002`)
- `ExternalIPAddressOrDNSName` — public hostname for inter-server URLs
- `ExternalPort` — public port for inter-server URLs

### Warp (`warp.js`)
- `CollisionMessages` — array of messages shown when players collide with warp portals

## Thread safety

Plugin `Start()` runs on the **main thread**. Landblock operations must be marshaled through `WorldManager.EnqueueAction()` to run on the UpdateWorld thread. See the [ACEmulator documentation](https://github.com/ACEmulator/ACE) for details.

## Building on WSL

- Clean `obj/` directories before building (Windows NuGet cache paths cause conflicts)
- Remove Windows-only `<Target Name="PostBuild">` sections from csproj files
- Match package versions to what ACE.Server uses (log4net 2.0.17, etc.)
