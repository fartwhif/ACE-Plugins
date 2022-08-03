// using ACE.Plugin.Web.Modules;
using System.Collections.Generic;

namespace ACE.Plugin.Web.Model.Character
{
    public class CharacterListModel
    {
        public BaseAuthenticatedModel Account { get; set; }
        public List<Database.Models.Shard.Character> Characters { get; set; }
    }
}
