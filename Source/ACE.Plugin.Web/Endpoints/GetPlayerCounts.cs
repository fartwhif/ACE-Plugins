using ACE.Plugin.Web.Model;
using ACE.Server.Managers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ACE.Plugin.Web
{
    static partial class Endpoints
    {
        public static IEndpointRouteBuilder GetPlayerCounts(IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("GetPlayerCounts", (Func<HttpContext, Task<string>>)(async (context) =>
            {
                PlayerCountResponseModel resp = null;
                Gate.RunGatedAction(() => resp = new PlayerCountResponseModel()
                {
                    Online = PlayerManager.GetAllOnline().Count,
                    Offline = PlayerManager.GetAllOffline().Count
                });
                return context.Ok(resp);
            })).AllowAnonymous();

            return endpoints;
        }
    }
}
