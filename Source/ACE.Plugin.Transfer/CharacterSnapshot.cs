using ACE.Database.Entity;
using ACE.Database.Models.Shard;

namespace ACE.Plugin.Transfer
{
    public class CharacterSnapshot
    {
        public CharacterSnapshot() { }
        public Character Character { get; set; }
        public Biota Player { get; set; }
        public PossessedBiotas PossessedBiotas { get; set; }
    }
}
