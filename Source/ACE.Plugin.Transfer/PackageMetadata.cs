using ACE.Plugin.Transfer.Enums;
using System;

namespace ACE.Plugin.Transfer
{
    public class PackageMetadata
    {
        public PackageType PackageType { get; set; }
        public string Cookie { get; set; }
        public uint CharacterId { get; set; }
        public uint AccountId { get; set; }
        public string FilePath { get; set; }
        public string NewCharacterName { get; set; }
        public Uri ImportUrl { get; set; }
    }
}
