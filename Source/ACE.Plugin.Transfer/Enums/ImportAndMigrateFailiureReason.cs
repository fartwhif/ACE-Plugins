namespace ACE.Plugin.Transfer.Enums
{
    public enum ImportAndMigrateFailiureReason
    {
        None,
        Unknown,
        OperationNotAllowed,
        UnverifiedSourceServerNotAllowed,
        UnverifiedSourceServerDoesntAllowMigrate,
        VerifiedSourceServerNotAllowed,
        VerifiedMigrationSourceThumbprintMismatch,
        PackInfoNotFound,
        CannotContactSourceServer,
        ProtocolError,
        SourceServerRejectedRequest,
        CharacterAlreadyPresent,
        WrongPackageType,
        PackageTypeNotAllowed,
        NameIsUnavailable,
        NameIsNaughty,
        NameContainsInvalidCharacters,
        NameTooShortOrTooLong,
        NoCharacterSlotsAvailable,
        FoundMoreThanOneCharacter,
        CannotFindCharacter,
        MalformedCharacterData,
        AddCharacterFailed,
        CharacterPackageSignatureInvalid,
        SignatureMismatch,
        SignatureInvalid,
        MigrationCheckFailed,
        NonceInvalid,
        PackageUnsigned,
        CookieAlreadyUsed
    }
}