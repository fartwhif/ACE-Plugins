namespace ACE.Plugin.Transfer.Responses
{
    public class SignedMigrationCheckResponseModel
    {
        public MigrationCheckResponseModel Result { get; set; }
        public byte[] Signature { get; set; }
        public byte[] Signer { get; set; }
    }
}
