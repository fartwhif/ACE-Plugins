using ACE.Common;
using FluentValidation;
using FluentValidation.Results;

namespace ACE.Plugin.Transfer.Model.Character
{
    public class CharacterImportRequestModel
    {
        public string SnapshotPackageBase64 { get; set; }
        public string NewCharacterName { get; set; }
    }
    public class CharacterImportRequestModelValidator : AbstractValidator<CharacterImportRequestModel>
    {
        public CharacterImportRequestModelValidator()
        {
            RuleFor(request => request.SnapshotPackageBase64).NotEmpty().WithMessage("You must specify the snapshot package.");
            RuleFor(request => request.NewCharacterName).NotEmpty().WithMessage("You must specify the character name to use.");
            RuleFor(request => request.NewCharacterName).Custom((str, _) =>
            {
                if (!string.IsNullOrWhiteSpace(str) && TransferManagerUtil.StringContainsInvalidChars(GameConfiguration.AllowedCharacterNameCharacters, str))
                {
                    _.AddFailure("The new character name contains invalid characters.");
                }
            });
            RuleFor(request => request.NewCharacterName.Trim())
                .Length(GameConfiguration.CharacterNameMinimumLength, GameConfiguration.CharacterNameMaximumLength)
                .WithMessage("The new character name must be 1 to 32 characters in length.");
        }
        protected override bool PreValidate(ValidationContext<CharacterImportRequestModel> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", "a request body must be supplied"));
                return false;
            }
            return true;
        }
    }
    public class CharacterImportResponseModel
    {
        public bool Success { get; set; }
        public string FailureReason { get; set; }
        public string CharacterName { get; set; }
        public uint CharacterId { get; set; }
    }
}
