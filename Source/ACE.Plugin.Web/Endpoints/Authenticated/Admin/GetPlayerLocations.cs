using ACE.Entity.Enum;
using ACE.Plugin.Web.Model;
using ACE.Server.Managers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ACE.Plugin.Web
{
    static partial class WebEndpoints
    {
        public static IEndpointRouteBuilder GetPlayerLocations(IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("GetPlayerLocations", async (HttpContext context) =>
            {
                var account = context.User.ToACEAccount();

                // to-do: use an AuthorizationPolicy for this
                if (((AccessLevel)account.AccessLevel) != AccessLevel.Admin)
                {
                    return context.Unauthorized("");//you must have admin to use this
                }

                PlayerLocationsResponseModel resp = new PlayerLocationsResponseModel();
                Gate.RunGatedAction(() => resp.Locations = PlayerManager.GetAllOnline().Select(k => new PlayerNameAndLocation() { Location = k.Location.ToString(), Name = k.Name }).ToList());

                return context.Ok(resp);

            }).RequireAuthorization();

            return endpoints;
        }
    }
}
