using System;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;

using ACE.Common;
using ACE.Database;
using ACE.Database.Adapter;
using ACE.Database.Models.World;
using ACE.Entity;
using ACE.Server.Factories;
using ACE.Server.Managers;
using ACE.Server.WorldObjects;

using log4net;

namespace ACE.Plugin.PortalPlugin
{
    /// <summary>
    /// Portal plugin - spawns a custom portal in Rithwic and registers a collide hook.
    /// </summary>
    public class Init : IACEPlugin
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

            try
            {
                SpawnRithwicPortal();
                ResultOfInitSink.SetResult(true);
            }
            catch (Exception ex)
            {
                log.Error("Failed to spawn Rithwic portal", ex);
                ResultOfInitSink.SetResult(false);
            }
        }

        /// <summary>
        /// Spawns a portal in Rithwic and registers a collide object hook.
        /// </summary>
        private void SpawnRithwicPortal()
        {
            log.Info("[Portal] Spawning portal in Rithwic...");

            // Portal coordinates
            uint landblockId = 0xC98D0021; // indoor location

            var position = new Position(
                blockCellID: landblockId,
                newPositionX: 102.307503f,
                newPositionY: 15.664568f,
                newPositionZ: 22.004999f,
                newRotationX: -0.191985f,
                newRotationY: 0.000000f,
                newRotationZ: 0.000000f,
                newRotationW: 0.981398f
            );

            // portalrithwic WeenieClassId = 1955
            uint portalWcid = 1955;

            Weenie weenie = DatabaseManager.World.GetWeenie(portalWcid);
            if (weenie == null)
            {
                log.Error($"[Portal] Could not find weenie with WCID {portalWcid} (portalrithwic)");
                return;
            }

            var entityWeenie = WeenieConverter.ConvertToEntityWeenie(weenie);

            // Generate a GUID in the static range for this landblock
            uint guid = 0x70000000 | ((uint)0xC98D << 12) | 0x100;

            Portal portal = WorldObjectFactory.CreateWorldObject(entityWeenie, new ObjectGuid(guid)) as Portal;
            if (portal == null)
            {
                log.Error($"[Portal] Failed to create portal object for WCID {portalWcid}");
                return;
            }

            // Set the portal position
            portal.Location = position;

            // Set the portal destination
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

            // Ensure the portal is stuck so it doesn't move
            portal.Stuck = true;

            // Register the collide object hook BEFORE entering the world
            portal.warp_AddCollideObjectHook(OnPortalCollide);

            // Add to the world
            bool success = LandblockManager.AddObject(portal, loadAdjacents: true);

            if (success)
            {
                log.Info($"[Portal] Successfully spawned Rithwic portal (0x{portal.Guid:X8}) at {portal.Location.ToLOCString()}");
            }
            else
            {
                log.Error($"[Portal] Failed to add Rithwic portal to the world");
            }
        }

        /// <summary>
        /// Called when a player collides with the portal.
        /// </summary>
        private static void OnPortalCollide(Portal portal, Player player)
        {
            log.Info($"[Portal] Player {player.Name} collided with portal {portal.Name} (0x{portal.Guid:X8})");

            // Send a message to the player
            player.Session.Network.EnqueueSend(
                new ACE.Server.Network.GameMessages.Messages.GameMessageSystemChat(
                    "You have entered the Rithwic portal zone.",
                    ACE.Entity.Enum.ChatMessageType.System));
        }
    }
}
