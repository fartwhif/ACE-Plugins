using ACE.Plugin.Web.Model;
using ACE.Plugin.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ACE.Plugin.Web
{
    static partial class Endpoints
    {
        public static IEndpointRouteBuilder GetAccessToken(IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("GetAccessToken", async (IAccountService accountService, HttpContext context, [FromBody] Login login) =>
            {
                if (login == null)
                {
                    return context.Unauthorized("invalid credentials");
                }
                var result = await accountService.GetAuthTokens(login);
                if (result == null)
                {
                    return context.Unauthorized("invalid credentials");
                }
                return context.Ok(result);
            }).AllowAnonymous();

            return endpoints;
        }
    }
}
