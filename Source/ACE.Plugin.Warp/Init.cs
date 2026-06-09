using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;

using ACE.Common;
using ACE.Database;
using ACE.Database.Adapter;
using ACE.Database.Models.World;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Plugin.Transfer;
using ACE.Plugin.Transfer.Enums;
using ACE.Plugin.Transfer.Managers;
using ACE.Plugin.Warp.Common;
using ACE.Server.Entity.Actions;
using ACE.Server.Factories;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.Network.Enum;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.WorldObjects;

using log4net;

namespace ACE.Plugin.WarpPlugin
{
    /// <summary>
    /// Warp plugin - spawns a custom portal in Rithwic and registers a collide hook.
    /// Handles cross-server migration via the Transfer plugin when the decal client plugin is present.
    /// </summary>
    public class Init : IACEPlugin
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Random random = new Random();

        /// <summary>
        /// PropertyInt index used to verify the decal client plugin is present.
        /// The decal plugin sets this to its version number on login.
        /// </summary>
        private const int DecalVersionPropertyId = 37;

        public void AllPluginsStarted(TaskCompletionSource<bool> AllPluginsStartedSink)
        {
            AllPluginsStartedSink.SetResult(true);
        }

        public void Start(TaskCompletionSource<bool> ResultOfInitSink)
        {
            if (!Assembly.GetCallingAssembly().GetName().FullName.StartsWith("ACE.Server,"))
            {
                log.Fatal("Invalid startup method. This is an ACEmulator plugin.");
                return;
            }

            // Load configuration from warp.js
            try
            {
                WarpConfigManager.Initialize();
                var messageCount = WarpConfigManager.Config.CollisionMessages?.Count ?? 0;
                log.Info($"[Warp] Configuration loaded. {messageCount} collision message(s) available.");
                
                var destHost = WarpConfigManager.Config.DestinationServerHost;
                var destPort = WarpConfigManager.Config.DestinationServerPort;
                var destUrl = WarpConfigManager.Config.DestinationServerUrl;
                log.Info($"[Warp] Destination: {destHost}:{destPort} (API: {destUrl})");
            }
            catch (Exception ex)
            {
                log.Warn("[Warp] Failed to load configuration from warp.js, using defaults.", ex);
            }

            // Enqueue portal spawn onto the UpdateWorld thread to avoid cross-thread errors.
            WorldManager.EnqueueAction(new ActionEventDelegate(() =>
            {
                try
                {
                    SpawnRithwicPortal();
                }
                catch (Exception ex)
                {
                    log.Error("Failed to spawn Rithwic warp", ex);
                    ResultOfInitSink.SetResult(false);
                    return;
                }
                ResultOfInitSink.SetResult(true);
            }));
        }

        /// <summary>
        /// Spawns a portal in Rithwic and registers a collide object hook.
        /// </summary>
        private void SpawnRithwicPortal()
        {
            log.Info("[Warp] Spawning portal in Rithwic...");

            uint landblockId = 0xC98D0021;

            var position = new Position(
                blockCellID: landblockId,
                newPositionX: 102.307503f,
                newPositionY: 15.664568f,
                newPositionZ: 22.004999f,
                newRotationX: 0.000000f,
                newRotationY: 0.000000f,
                newRotationZ: 0.000000f,
                newRotationW: 1.000000f
            );

            uint portalWcid = 1025;

            Weenie weenie = DatabaseManager.World.GetWeenie(portalWcid);
            if (weenie == null)
            {
                log.Error($"[Warp] Could not find weenie with WCID {portalWcid} (portalrithwic)");
                return;
            }

            var entityWeenie = WeenieConverter.ConvertToEntityWeenie(weenie);
            uint guid = 0x70000000 | ((uint)0xC98D << 12) | 0x100;

            Portal portal = WorldObjectFactory.CreateWorldObject(entityWeenie, new ObjectGuid(guid)) as Portal;
            if (portal == null)
            {
                log.Error($"[Warp] Failed to create portal object for WCID {portalWcid}");
                return;
            }

            portal.Location = position;

            var destPosition = new Position(
                blockCellID: 0xC98D0033,
                newPositionX: 158.927948f,
                newPositionY: 66.506470f,
                newPositionZ: 23.936876f,
                newRotationX: 0.000000f,
                newRotationY: 0.000000f,
                newRotationZ: 0.000000f,
                newRotationW: 0.000000f
            );
            portal.Destination = destPosition;
            portal.Stuck = true;
            portal.ReportCollisions = true;
            portal.IgnoreCollisions = false;

            portal.warp_AddCollideObjectHook(OnPortalCollide);

            bool success = LandblockManager.AddObject(portal, loadAdjacents: true);

            if (success)
            {
                log.Info($"[Warp] Successfully spawned Rithwic portal (0x{portal.Guid:X8}) at {portal.Location.ToLOCString()}");
            }
            else
            {
                log.Error($"[Warp] Failed to add Rithwic portal to the world");
            }
        }

        /// <summary>
        /// Called when a player collides with the portal.
        /// Checks for decal plugin, then triggers cross-server migration.
        /// </summary>
        private static void OnPortalCollide(Portal portal, Player player)
        {
            try
            {
                var config = WarpConfigManager.Config;
                log.Info($"[Warp] Player {player.Name} (0x{player.Guid:X8}) collided with portal {portal.Name} (0x{portal.Guid:X8})");

                // Pick a random message from the config
                string message;
                var messages = config.CollisionMessages;
                if (messages != null && messages.Count > 0)
                {
                    message = messages[random.Next(messages.Count)];
                }
                else
                {
                    message = "You have entered the Rithwic portal zone.";
                }

                // Send the portal message
                player.Session.Network.EnqueueSend(
                    new GameMessageSystemChat(message, ChatMessageType.System));

                // Check for decal plugin presence via PropertyInt
                // The decal plugin sets PropertyInt 37 to its version number on login
                int? decalVersion = player.GetProperty((PropertyInt)DecalVersionPropertyId);
                
                if (decalVersion == null || decalVersion < 1)
                {
                    log.Info($"[Warp] Player {player.Name} lacks decal plugin (version: {decalVersion}). Blocking migration.");
                    
                    player.Session.Network.EnqueueSend(
                        new GameMessageSystemChat(
                            "Cross-world migration requires the Veilrend Decal client plugin.",
                            ChatMessageType.System));
                    player.Session.Network.EnqueueSend(
                        new GameMessageSystemChat(
                            "Download it from the landing page to travel between worlds.",
                            ChatMessageType.System));
                    
                    return; // Block migration - let normal portal teleport proceed
                }

                log.Info($"[Warp] Player {player.Name} has decal plugin v{decalVersion}. Initiating cross-server migration.");

                // Notify player
                player.Session.Network.EnqueueSend(
                    new GameMessageSystemChat(
                        $"Initiating transfer to {config.DestinationServer}...",
                        ChatMessageType.System));

                // Start async migration - don't block the game thread
                Task.Run(async () => await MigratePlayer(player, config));
            }
            catch (Exception ex)
            {
                log.Error($"[Warp] Error in collision handler for player {player.Name}", ex);
            }
        }

        /// <summary>
        /// Performs the cross-server migration: begin on source, complete on target, then handoff.
        /// </summary>
        private static async Task MigratePlayer(Player player, WarpConfiguration config)
        {
            try
            {
                log.Info($"[Warp] Starting migration for {player.Name} to {config.DestinationServer}");

                // Step 1: Begin migration on source server (create signed package)
                var packageMetadata = new PackageMetadata
                {
                    CharacterId = player.Guid.Full,
                    AccountId = player.Session.AccountId,
                    PackageType = PackageType.Migrate
                };

                PackageMetadata resultMetadata = null;
                
                // Run on the game thread via EnqueueAction
                await EnqueueOnGameThread(() =>
                {
                    resultMetadata = TransferManager.CreatePackage(packageMetadata).Result;
                });

                if (resultMetadata == null || !System.IO.File.Exists(resultMetadata.FilePath))
                {
                    log.Error($"[Warp] Failed to create migration package for {player.Name}");
                    player.Session.Network.EnqueueSend(
                        new GameMessageSystemChat(
                            "Migration package creation failed. Please try again.",
                            ChatMessageType.System));
                    return;
                }

                log.Info($"[Warp] Migration package created for {player.Name} (cookie: {resultMetadata.Cookie})");

                // Step 2: Complete migration on destination server via HTTP API
                string migrationToken = null;
                
                try
                {
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        // Get source server's external URL from WebConfig
                        string sourceBaseUrl = config.DestinationServerUrl;
                        
                        // POST to destination server's migration complete endpoint
                        var requestContent = new System.Net.Http.StringContent(
                            $"{{\"Cookie\":\"{resultMetadata.Cookie}\",\"AccountId\":{player.Session.AccountId}," +
                            $"\"NewCharacterName\":\"{player.Name}\",\"BaseURL\":\"{sourceBaseUrl}\"}}",
                            System.Text.Encoding.UTF8,
                            "application/json");

                        var response = await client.PostAsync(
                            $"{config.DestinationServerUrl}/api/character/migrationComplete",
                            requestContent);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            log.Info($"[Warp] Destination server response: {responseContent}");
                            
                            migrationToken = resultMetadata.Cookie;
                        }
                        else
                        {
                            log.Error($"[Warp] Destination server returned {response.StatusCode}");
                            player.Session.Network.EnqueueSend(
                                new GameMessageSystemChat(
                                    "Destination server rejected the migration. Please try again.",
                                    ChatMessageType.System));
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error($"[Warp] Failed to contact destination server", ex);
                    player.Session.Network.EnqueueSend(
                        new GameMessageSystemChat(
                            "Connection to destination server failed. Please try again.",
                            ChatMessageType.System));
                    return;
                }

                // Step 3: Log out the player (removes from landblock, prevents portal teleport)
                log.Info($"[Warp] Logging out {player.Name} from source server");
                player.LogOut();

                // Wait for logout animation
                await Task.Delay(7000);

                // Step 4: Send ServerHandoff message to client.
                // The DecalPlugin intercepts this custom opcode (0xF7CF), saves target
                // info to handoff.json, and exits the client. The DecalLauncher then
                // relaunches the AC client pointing to the new server.
                //
                // Custom opcodes work because the DecalPlugin registers a handler for
                // opcode 0xF7CF via Harmony patch on the client's message dispatch.
                // See: https://gitlab.com/trevis/acserverclientplugindemo
                var handoffToken = migrationToken ?? resultMetadata.Cookie;
                
                log.Info($"[Warp] Sending ServerHandoff to {player.Name} -> {config.DestinationServerHost}:{config.DestinationServerPort}");
                
                player.Session.Network.EnqueueSend(
                    new GameMessageServerHandoff(
                        config.DestinationServerHost,
                        (ushort)config.DestinationServerPort,
                        handoffToken));

                // Step 5: Boot the session (disconnects the client)
                // Give the plugin a moment to process the handoff message before we kill the session
                await Task.Delay(500);
                
                player.Session.Terminate(
                    SessionTerminationReason.AccountBooted,
                    new GameMessageBootAccount(
                        $" because your character was transferred to {config.DestinationServer}."),
                    extraReason: "Cross-server migration complete");

                log.Info($"[Warp] Migration complete for {player.Name}. Client will reconnect to {config.DestinationServer}.");
            }
            catch (Exception ex)
            {
                log.Error($"[Warp] Migration failed for {player.Name}", ex);
                player.Session.Network.EnqueueSend(
                    new GameMessageSystemChat(
                        "Migration failed. Your character is safe on this server.",
                        ChatMessageType.System));
            }
        }

        /// <summary>
        /// Runs an action on the game thread via WorldManager.EnqueueAction.
        /// </summary>
        private static Task EnqueueOnGameThread(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();
            WorldManager.EnqueueAction(new ActionEventDelegate(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }
    }
}
