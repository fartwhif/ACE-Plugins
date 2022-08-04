using ACE.Plugin.Web.Model;
using ACE.Server.Managers;
using ACE.Server.Network;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ACE.Plugin.Web
{
    static partial class WebEndpoints
    {
        public static IEndpointRouteBuilder GetNetworkStats(IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("GetNetworkStats", (Func<HttpContext, Task<string>>)(async (context) =>
            {
                var resp = new NetworkStatsResponseModel()
                {
                    C2S_CRCErrors_Aggregate = NetworkStatistics.C2S_CRCErrors_Aggregate,
                    C2S_Packets_Aggregate = NetworkStatistics.C2S_Packets_Aggregate,
                    C2S_RequestsForRetransmit_Aggregate = NetworkStatistics.C2S_RequestsForRetransmit_Aggregate,
                    S2C_Packets_Aggregate = NetworkStatistics.S2C_Packets_Aggregate,
                    S2C_RequestsForRetransmit_Aggregate = NetworkStatistics.S2C_RequestsForRetransmit_Aggregate,
                    Summary = NetworkStatistics.Summary()
                };
                return context.Ok(resp);
            })).AllowAnonymous();

            return endpoints;
        }
    }
}
