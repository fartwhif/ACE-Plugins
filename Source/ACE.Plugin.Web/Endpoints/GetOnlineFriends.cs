using ACE.Database;
using ACE.Database.Models.Shard;
using ACE.Server.Managers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ACE.Plugin.Web
{
    static partial class Endpoints
    {
        public static IEndpointRouteBuilder GetOnlineFriends(IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("GetOnlineFriends", (Func<HttpContext, Task<string>>)(async (context) =>
            {
                var account = context.User.ToACEAccount();
                string[]? onlineFriends = null;
                Gate.RunGatedAction(() =>
                {
                    TaskCompletionSource<object> tsc = new TaskCompletionSource<object>();
                    List<Character>? characters = null;
                    DatabaseManager.Shard.GetCharacters(account.AccountId, true, (chars) =>
                    {
                        characters = chars;
                        tsc.SetResult(new object());
                    });
                    tsc.Task.Wait();

                    List<uint> allFriends = new();
                    ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(); // not currently useful
                    //characters.ForEach(k => allFriends.AddRange(k.GetFriendsIds(rwLock)));
                    allFriends = allFriends.Distinct().ToList();

                    onlineFriends = PlayerManager.GetAllOnline()
                        .Where(k => allFriends.Contains(k.Character.Id))
                        .Select(k => k.Name)
                        .ToArray();
                });
                return context.Ok(onlineFriends);
            })).RequireAuthorization();
            return endpoints;
        }
    }
}
