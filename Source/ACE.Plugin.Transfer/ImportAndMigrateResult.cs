using ACE.Plugin.Transfer.Enums;

namespace ACE.Plugin.Transfer
{
    public class ImportAndMigrateResult
    {
        public ImportAndMigrateFailiureReason FailReason { get; set; } = ImportAndMigrateFailiureReason.Unknown;
        public bool Success { get; set; } = false;
        public string NewCharacterName { get; set; }
        public uint NewCharacterId { get; set; }
    }
}
