using ACE.Common;
using ACE.Plugin.Web.Model;
using ACE.Server.Entity;
using ACE.Server.Managers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ACE.Plugin.Web
{
    static partial class WebEndpoints
    {
        public static IEndpointRouteBuilder GetServerInfo(IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("GetServerInfo", (Func<HttpContext, Task<string>>)(async (context) =>
            {
                ServerInfoResponseModel? resp = null;
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
                return context.Ok(resp);
            })).AllowAnonymous();

            return endpoints;
        }
    }
}
