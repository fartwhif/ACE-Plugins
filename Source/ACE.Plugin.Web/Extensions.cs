using ACE.Database.Models.Auth;
using ACE.Entity.Enum;
using ACE.Plugin.Web.Model;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ACE.Plugin.Web
{
    public static class Extensions
    {
        public static string Ok(this HttpContext context, object obj)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            return context.JSONResult(obj);
        }
        public static string BadRequest(this HttpContext context, object obj)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return context.JSONResult(obj);
        }
        public static string Unauthorized(this HttpContext context, object obj)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return context.JSONResult(obj);
        }
        public static string JSONResult(this HttpContext context, object obj)
        {
            context.Response.ContentType = "application/json";
            return obj.ToJSON();
        }
        public static string ToJSON(this object obj)
        {
            return Util.ToJson(obj);
        }
        public static Account? ToACEAccount(this ClaimsPrincipal user)
        {
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return null;
            }
            var acct = new Account();
            foreach(var claim in user.Claims)
            {
                switch (claim.Type)
                {
                    case ClaimTypes.Name:
                        acct.AccountName = claim.Value;
                        break;
                    //case "AccessLevelName":
                    //    break;
                    case "AccountId":
                        acct.AccountId = uint.Parse(claim.Value);
                        break;
                    case "AccessLevelId":
                        acct.AccessLevel = uint.Parse(claim.Value);
                        break;
                    //case "Player":
                    //    break;
                    //case "Advocate":
                    //    break;
                    //case "Sentinel":
                    //    break;
                    //case "Envoy":
                    //    break;
                    //case "Developer":
                    //    break;
                    //case "Admin":
                    //    break;
                }
            }
            return acct;
        }
        public static BaseAuthenticatedModel? ToBaseAuth(this Account acct)
        {
            if (acct == null) { return null; }
            var auth = new BaseAuthenticatedModel
            {
                AccessLevelName = ((AccessLevel)acct.AccessLevel).ToString(),
                AccountName = acct.AccountName
            };
            return auth;
        }
    }
}
