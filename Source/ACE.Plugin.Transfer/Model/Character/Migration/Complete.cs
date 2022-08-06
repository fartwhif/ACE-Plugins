using ACE.Common;
using FluentValidation;
using FluentValidation.Results;

namespace ACE.Plugin.Transfer.Model.Character.Migration
{
    public class CharacterMigrationCompleteRequestModel
    {
        public string Cookie { get; set; }
        public string NewCharacterName { get; set; }
        public string BaseURL { get; set; }
    }
    public class CharacterMigrationCompleteRequestModelValidator : AbstractValidator<CharacterMigrationCompleteRequestModel>
    {
        public CharacterMigrationCompleteRequestModelValidator()
        {
            RuleFor(request => request.BaseURL).NotEmpty().WithMessage("You must specify the base URL.");
            RuleFor(request => request.NewCharacterName).NotEmpty().WithMessage("You must specify the character name to use.");
            RuleFor(request => request.NewCharacterName).Custom((str, context) =>
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    if (TransferManagerUtil.StringContainsInvalidChars(GameConfiguration.AllowedCharacterNameCharacters, str))
                    {
                        context.AddFailure("The new character name contains invalid characters.");
                    }
                    var g = str.Trim();
                    if (str.Length > GameConfiguration.CharacterNameMaximumLength || str.Length < GameConfiguration.CharacterNameMinimumLength)
                    {
                        context.AddFailure($"The new character name must be {GameConfiguration.CharacterNameMinimumLength} to {GameConfiguration.CharacterNameMaximumLength} characters in length.");
                    }
                }
            });
            RuleFor(request => request.Cookie).NotEmpty().WithMessage("You must specify the character migration cookie.");
            RuleFor(request => request.Cookie).Custom((str, _) =>
            {
                if (!string.IsNullOrWhiteSpace(str) && TransferManagerUtil.StringContainsInvalidChars(TransferManagerConstants.CookieChars, str))
                {
                    _.AddFailure("The cookie contains invalid characters.");
                }
            });
            RuleFor(request => request.Cookie).Length(TransferManagerConstants.CookieLength)
                .WithMessage($"Cookie must be {TransferManagerConstants.CookieLength} characters in length.");
        }
        protected override bool PreValidate(ValidationContext<CharacterMigrationCompleteRequestModel> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", "a request body must be supplied"));
                return false;
            }
            return true;
        }
    }
    public class CharacterMigrationCompleteResponseModel
    {
        public bool Success { get; set; }
        public string FailureReason { get; set; }
        public string Cookie { get; set; }
        public string CharacterName { get; set; }
        public uint CharacterId { get; set; }
    }
}
