using FluentValidation;
using FluentValidation.Results;

namespace ACE.Plugin.Transfer.Model.Character.Migration
{
    public class CharacterMigrationCancelRequestModel
    {
        public string Cookie { get; set; }
    }
    public class CharacterMigrationCancelRequestModelValidator : AbstractValidator<CharacterMigrationCancelRequestModel>
    {
        public CharacterMigrationCancelRequestModelValidator()
        {
            RuleFor(request => request.Cookie).NotEmpty().WithMessage("You must specify the character migration cookie.");
            RuleFor(request => request.Cookie).Custom((str, _) =>
            {
                if (!string.IsNullOrWhiteSpace(str) && TransferManagerUtil.StringContainsInvalidChars(TransferManagerConstants.CookieChars, str))
                {
                    _.AddFailure("The cookie contains invalid characters.");
                }
            });
            RuleFor(request => request.Cookie).Length(TransferManagerConstants.CookieLength).WithMessage($"Cookie must be {TransferManagerConstants.CookieLength} characters in length.");
        }
        protected override bool PreValidate(ValidationContext<CharacterMigrationCancelRequestModel> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", "a request body must be supplied"));
                return false;
            }
            return true;
        }
    }
    public class CharacterMigrationCancelResponseModel
    {
        public bool Success { get; set; }
        public string Cookie { get; set; }
    }
}
