// using ACE.Plugin.Web.Modules;
using System.Collections.Generic;

namespace ACE.Plugin.Web.Model.Character
{
    public class CharacterListModel : BaseAuthenticatedModel
    {
        public BaseAuthenticatedModel Account { get; set; }
        public List<Database.Models.Shard.Character> Characters { get; set; }
    }
}
