namespace ACE.Plugin.Transfer.Enums
{
    public enum MigrationReadyStatus
    {
        Unknown,
        Ready,
        NonExistant,
        NotMigration,
        AlreadyDownloaded,
        AlreadyCancelled,
        CharNotFound,
        InvalidCharState,
        PackageFileMissing
    }
}
