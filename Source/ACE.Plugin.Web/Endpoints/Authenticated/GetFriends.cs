using ACE.Database;
using ACE.Database.Models.Shard;
using ACE.Plugin.Web.Model;
using ACE.Server.Managers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ACE.Plugin.Web
{
    static partial class WebEndpoints
    {
        public static IEndpointRouteBuilder GetFriends(IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("GetFriends", (Func<HttpContext, Task<string>>)(async (context) =>
            {
                var account = context.User.ToACEAccount();
                FriendsList frens = new();
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
                    ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
                    List<CharacterPropertiesFriendList> friends = new List<CharacterPropertiesFriendList>();
                    foreach (var c in characters)
                    {
                        friends.AddRange(c.GetFriends(rwLock));
                    }
                    friends = friends.GroupBy(k => k.CharacterId).Select(k => k.First()).ToList();
                    foreach (var f in friends)
                    {
                        var player = PlayerManager.FindByGuid(f.FriendId, out var isOnline);
                        if (isOnline)
                        {
                            frens.Online.Add(player.Name);
                        }
                        else
                        {
                            frens.Offline.Add(player.Name);
                        }
                    }
                });
                return context.Ok(frens);
            })).RequireAuthorization();
            return endpoints;
        }
    }
}
