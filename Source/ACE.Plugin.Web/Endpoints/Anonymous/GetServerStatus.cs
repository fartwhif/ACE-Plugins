using ACE.Database;
using ACE.DatLoader;
using ACE.Plugin.Web.Model;
using ACE.Server.Command.Handlers;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.WorldObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics;
using System.Text;

namespace ACE.Plugin.Web
{
    static partial class WebEndpoints
    {
        public static IEndpointRouteBuilder GetServerStatus(IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("GetServerStatus", (Func<HttpContext, Task<string>>)(async (context) =>
            {
                string? resp = null;
                Gate.RunGatedAction(() => resp = GetServerStatus());
                return context.Ok(resp);
            })).AllowAnonymous();

            return endpoints;
        }

        private static string GetServerStatus()
        {
            // Format comparable to GDL.

            var sb = new StringBuilder();

            var proc = Process.GetCurrentProcess();

            sb.Append($"Server Status:{'\n'}");

            var runTime = DateTime.Now - proc.StartTime;
            sb.Append($"Server Runtime: {(int)runTime.TotalHours}h {runTime.Minutes}m {runTime.Seconds}s{'\n'}");

            sb.Append($"Total CPU Time: {(int)proc.TotalProcessorTime.TotalHours}h {proc.TotalProcessorTime.Minutes}m {proc.TotalProcessorTime.Seconds}s, Threads: {proc.Threads.Count}{'\n'}");

            // todo, add actual system memory used/avail
            sb.Append($"{(proc.PrivateMemorySize64 >> 20):N0} MB used{'\n'}");  // sb.Append($"{(proc.PrivateMemorySize64 >> 20)} MB used, xxxx / yyyy MB physical mem free.{'\n'}");

            //sb.Append($"{WorldManager.GetSessionCount():N0} connections, {PlayerManager.GetAllOnline().Count:N0} players online{'\n'}");
            sb.Append($"Total Accounts Created: {DatabaseManager.Authentication.GetAccountCount():N0}, Total Characters Created: {(PlayerManager.GetAllOffline().Count + PlayerManager.GetAllOnline().Count):N0}{'\n'}");

            // 330 active objects, 1931 total objects(16777216 buckets.)

            // todo, expand this
            var activeLandblocks = LandblockManager.GetLoadedLandblocks();//.GetActiveLandblocks();
            int players = 0, creatures = 0, missiles = 0, other = 0, total = 0;
            foreach (var landblock in activeLandblocks)
            {
                foreach (var worldObject in landblock.GetAllWorldObjectsForDiagnostics())
                {
                    if (worldObject is Player)
                        players++;
                    else if (worldObject is Creature)
                        creatures++;
                    else if (worldObject.Missile ?? false)
                        missiles++;
                    else
                        other++;

                    total++;
                }
            }
            sb.Append($"{activeLandblocks.Count:N0} active landblocks - Players: {players:N0}, Creatures: {creatures:N0}, Missiles: {missiles:N0}, Other: {other:N0}, Total: {total:N0}.{'\n'}"); // 11 total blocks loaded. 11 active. 0 pending dormancy. 0 dormant. 314 unloaded.
            // 11 total blocks loaded. 11 active. 0 pending dormancy. 0 dormant. 314 unloaded.

            //sb.Append($"UpdateGameWorld {(DateTime.UtcNow - WorldManager.UpdateGameWorld5MinLastReset).TotalMinutes:N2} min - {WorldManager.UpdateGameWorld5MinRM}.{'\n'}");
            //sb.Append($"UpdateGameWorld {(DateTime.UtcNow - WorldManager.UpdateGameWorld60MinLastReset).TotalMinutes:N2} min - {WorldManager.UpdateGameWorld60MinRM}.{'\n'}");

            sb.Append($"World DB Cache Counts - Weenies: {DatabaseManager.World.GetWeenieCacheCount():N0}, LandblockInstances: {DatabaseManager.World.GetLandblockInstancesCacheCount():N0}, PointsOfInterest: {DatabaseManager.World.GetPointsOfInterestCacheCount():N0}, Cookbooks: {DatabaseManager.World.GetCookbookCacheCount():N0}, Spells: {DatabaseManager.World.GetSpellCacheCount():N0}, Encounters: {DatabaseManager.World.GetEncounterCacheCount():N0}, Events: {DatabaseManager.World.GetEventsCacheCount():N0}{'\n'}");
            //sb.Append($"Shard DB Counts - Biotas: {DatabaseManager.Shard.GetBiotaCount():N0}{'\n'}");

            sb.Append($"Portal.dat has {DatManager.PortalDat.FileCache.Count:N0} files cached of {DatManager.PortalDat.AllFiles.Count:N0} total{'\n'}");
            sb.Append($"Cell.dat has {DatManager.CellDat.FileCache.Count:N0} files cached of {DatManager.CellDat.AllFiles.Count:N0} total{'\n'}");

            return sb.ToString();
        }
    }
}
