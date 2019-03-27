namespace ACE.Plugin.Transfer
{
    public class MigrateCloseResult
    {
        public bool Success { get; set; } = false;
        public byte[] SnapshotPackage { get; set; }
    }
}
