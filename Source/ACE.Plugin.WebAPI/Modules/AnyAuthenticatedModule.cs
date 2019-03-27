using ACE.Database;
using ACE.Database.Models.Shard;
using ACE.Entity.Enum;
using ACE.Plugin.WebAPI.Model.Character;
using ACE.Server.Managers;
using Nancy.Security;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ACE.Plugin.WebAPI.Modules
{
    public class AnyAuthenticatedModule : BaseAuthenticatedModule
    {
        public async Task<CharacterListModel> GetModelCharacterListAsync()
        {
            CharacterListModel model = new CharacterListModel
            {
                Account = BaseModel
            };
            TaskCompletionSource<object> tsc = new TaskCompletionSource<object>();
            Gate.RunGatedAction(() =>
            {
                DatabaseManager.Shard.GetCharacters(uint.Parse(Context.CurrentUser.FindFirst("AccountId").Value), true, (chars) =>
                {
                    model.Characters = chars;
                    tsc.SetResult(new object());
                });
            });
            await tsc.Task;
            return model;
        }
        public AnyAuthenticatedModule()
        {
            this.RequiresAuthentication();

            this.RequiresAnyClaim(
                k => k.Type == AccessLevel.Admin.ToString(),
                k => k.Type == AccessLevel.Advocate.ToString(),
                k => k.Type == AccessLevel.Developer.ToString(),
                k => k.Type == AccessLevel.Envoy.ToString(),
                k => k.Type == AccessLevel.Player.ToString(),
                k => k.Type == AccessLevel.Sentinel.ToString());

            Get("/api/character", async (_) => { return (await GetModelCharacterListAsync()).AsJsonWebResponse(); });

            Get("/api/friends", async (_) => { return GetOnlineFriends().AsJsonWebResponse(); });

        }
        private string[] GetOnlineFriends()
        {
            string[] onlineFriends = null;
            Gate.RunGatedAction(() =>
            {
                TaskCompletionSource<object> tsc = new TaskCompletionSource<object>();
                List<Character> characters = null;
                DatabaseManager.Shard.GetCharacters(uint.Parse(Context.CurrentUser.FindFirst("AccountId").Value), true, (chars) =>
                {
                    characters = chars;
                    tsc.SetResult(new object());
                });
                tsc.Task.Wait();

                List<uint> allFriends = new List<uint>();
                ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(); // not currently useful
                characters.ForEach(k => allFriends.AddRange(k.GetFriendsIds(rwLock)));
                allFriends = allFriends.Distinct().ToList();

                onlineFriends = PlayerManager.GetAllOnline()
                    .Where(k => allFriends.Contains(k.Character.Id))
                    .Select(k => k.Name)
                    .ToArray();
            });
            return onlineFriends;
        }
    }
}
