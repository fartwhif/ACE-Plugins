using ACE.Common;
using ACE.Plugin.WebAPI.Model;
using ACE.Server.Command.Handlers;
using ACE.Server.Entity;
using ACE.Server.Managers;
using ACE.Server.Network;
using Nancy;

namespace ACE.Plugin.WebAPI.Modules
{
    public class UnauthenticatedModule : NancyModule
    {
        public UnauthenticatedModule()
        {
            Get("/api/playerCount", async (_) =>
            {
                PlayerCountResponseModel resp = null;
                Gate.RunGatedAction(() => resp = new PlayerCountResponseModel()
                {
                    Online = PlayerManager.GetAllOnline().Count,
                    Offline = PlayerManager.GetAllOffline().Count
                });
                return resp.AsJsonWebResponse();
            });

            Get("/api/networkStats", async (_) =>
            {
                return new NetworkStatsResponseModel()
                {
                    C2S_CRCErrors_Aggregate = NetworkStatistics.C2S_CRCErrors_Aggregate,
                    C2S_Packets_Aggregate = NetworkStatistics.C2S_Packets_Aggregate,
                    C2S_RequestsForRetransmit_Aggregate = NetworkStatistics.C2S_RequestsForRetransmit_Aggregate,
                    S2C_Packets_Aggregate = NetworkStatistics.S2C_Packets_Aggregate,
                    S2C_RequestsForRetransmit_Aggregate = NetworkStatistics.S2C_RequestsForRetransmit_Aggregate,
                    Summary = NetworkStatistics.Summary()
                }.AsJsonWebResponse();
            });

            Get("/api/serverStatus", async (_) =>
            {
                string resp = null;
                Gate.RunGatedAction(() => resp = AdminCommands.GetServerStatus());
                return resp.AsJsonWebResponse();
            });

            Get("/api/serverInfo", async (_) =>
            {
                ServerInfoResponseModel resp = null;
                Gate.RunGatedAction(() => resp = new ServerInfoResponseModel()
                {
                    PlayerCount = new PlayerCountResponseModel()
                    {
                        Online = PlayerManager.GetAllOnline().Count,
                        Offline = PlayerManager.GetAllOffline().Count
                    },
                    Uptime = Timers.RunningTime,
                    WorldName = ConfigManager.Config.Server.WorldName,
                    AccountDefaults = ConfigManager.Config.Server.Accounts,
                    MaximumAllowedSessions = ConfigManager.Config.Server.Network.MaximumAllowedSessions,
                    ServerMotd = PropertyManager.GetString("server_motd").Item,
                    PopupMotd = PropertyManager.GetString("popup_motd").Item,
                });
                return resp.AsJsonWebResponse();
            });
        }
    }
}
