using ACE.Entity.Enum;
using ACE.Plugin.Web.Model;
using ACE.Server.Managers;
using ACE.Server.WorldObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ACE.Plugin.Web
{
    static partial class WebEndpoints
    {
        public static IEndpointRouteBuilder GetLandblockStatus(IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("GetLandblockStatus", async (HttpContext context) =>
            {
                var account = context.User.ToACEAccount();

                // to-do: use an AuthorizationPolicy for this
                if (((AccessLevel)account.AccessLevel) != AccessLevel.Admin)
                {
                    return context.Unauthorized("");//you must have admin to use this
                }

                LandblockStatusResponseModel resp = new LandblockStatusResponseModel();
                Gate.RunGatedAction(() =>
                {
                    List<Server.Entity.Landblock> activeLandblocks = LandblockManager.GetLoadedLandblocks();
                    List<LandblockStatus> lbsl = new List<LandblockStatus>();
                    foreach (Server.Entity.Landblock landblock in activeLandblocks)
                    {
                        LandblockStatus lbs = new LandblockStatus()
                        {
                            Id = landblock.Id.ToString(),
                            Creatures = new List<WorldObjectStatus>(),
                            Missiles = new List<WorldObjectStatus>(),
                            Other = new List<WorldObjectStatus>(),
                            Players = new List<WorldObjectStatus>()
                        };
                        foreach (WorldObject worldObject in landblock.GetAllWorldObjectsForDiagnostics())
                        {
                            if (worldObject is Player)
                            {
                                lbs.Players.Add(new WorldObjectStatus() { Id = worldObject.Guid.Full, Name = worldObject.Name });
                            }
                            else if (worldObject is Creature)
                            {
                                lbs.Creatures.Add(new WorldObjectStatus() { Id = worldObject.Guid.Full, Name = worldObject.Name });
                            }
                            else if (worldObject.Missile ?? false)
                            {
                                lbs.Missiles.Add(new WorldObjectStatus() { Id = worldObject.Guid.Full, Name = worldObject.Name });
                            }
                            else
                            {
                                lbs.Other.Add(new WorldObjectStatus() { Id = worldObject.Guid.Full, Name = worldObject.Name });
                            }
                        }
                        lbsl.Add(lbs);
                    }
                    resp.Active = lbsl;
                });
                return context.Ok(resp);
            }).RequireAuthorization();

            return endpoints;
        }
    }
}
