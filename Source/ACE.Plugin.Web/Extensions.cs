using ACE.Database.Models.Auth;
using ACE.Entity.Enum;
using ACE.Plugin.Web.Model;
using System;
using System.Collections.Generic;

using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Plugin.Web
{
    internal static class Extensions
    {
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
