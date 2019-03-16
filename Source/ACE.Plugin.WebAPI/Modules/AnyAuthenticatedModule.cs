using ACE.Database;
using ACE.Entity.Enum;
using ACE.Plugin.WebAPI.Model.Character;
using Nancy.Security;
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
        }
    }
}
