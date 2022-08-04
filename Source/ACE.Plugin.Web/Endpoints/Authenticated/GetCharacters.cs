using ACE.Database;
using ACE.Plugin.Web.Model.Character;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ACE.Plugin.Web
{
    static partial class WebEndpoints
    {
        public static IEndpointRouteBuilder GetCharacters(IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("GetCharacters", (Func<HttpContext, Task<string>>)(async (context) =>
            {
                var account = context.User.ToACEAccount();

                CharacterListModel model = new()
                {
                    Account = account.ToBaseAuth()
                };

                TaskCompletionSource<object> tsc = new TaskCompletionSource<object>();
                Gate.RunGatedAction(() =>
                {
                    DatabaseManager.Shard.GetCharacters(account.AccountId, true, (chars) =>
                    {
                        model.Characters = chars;
                        tsc.SetResult(new object());
                    });
                });
                await tsc.Task;
                
                return context.Ok(model);

            })).RequireAuthorization();

            return endpoints;
        }
    }
}
