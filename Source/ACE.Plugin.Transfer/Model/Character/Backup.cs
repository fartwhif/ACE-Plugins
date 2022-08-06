using FluentValidation;
using FluentValidation.Results;

namespace ACE.Plugin.Transfer.Model.Character
{
    public class CharacterBackupRequestModel
    {
        public uint CharacterId { get; set; }
    }
    public class CharacterBackupRequestModelValidator : AbstractValidator<CharacterBackupRequestModel>
    {
        public CharacterBackupRequestModelValidator()
        {
            RuleFor(request => request.CharacterId).NotEmpty().GreaterThan((uint)0).WithMessage("You must specify a valid character Id.");
        }

        protected override bool PreValidate(ValidationContext<CharacterBackupRequestModel> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", "a request body must be supplied"));
                return false;
            }
            return true;
        }

    }
    public class CharacterBackupResponseModel
    {
        public bool Success { get; set; }
        public uint CharacterId { get; set; }
        public byte[] SnapshotPackage { get; set; }
    }
}
