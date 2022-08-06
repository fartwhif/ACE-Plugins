using ACE.Plugin.Transfer.Responses;
using FluentValidation;
using FluentValidation.Results;

namespace ACE.Plugin.Transfer.Model.Character.Migration
{
    public class TransferManagerMigrationCheckRequestModelValidator : AbstractValidator<MigrationCheckRequestModel>
    {
        public TransferManagerMigrationCheckRequestModelValidator()
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
            RuleFor(request => request.Nonce).NotEmpty().WithMessage("You must specify the nonce.");
            RuleFor(request => request.Nonce).Custom((str, _) =>
            {
                if (!string.IsNullOrWhiteSpace(str) && TransferManagerUtil.StringContainsInvalidChars(TransferManagerConstants.NonceChars, str))
                {
                    _.AddFailure("The nonce contains invalid characters.");
                }
            });
            RuleFor(request => request.Nonce).Length(TransferManagerConstants.NonceLength).WithMessage($"Nonce must be {TransferManagerConstants.NonceLength} characters in length.");
        }
        protected override bool PreValidate(ValidationContext<MigrationCheckRequestModel> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", "a request body must be supplied"));
                return false;
            }
            return true;
        }
    }
}
