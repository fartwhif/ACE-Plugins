using FluentValidation;
using FluentValidation.Results;

namespace ACE.Plugin.Transfer.Model.Character.Migration
{
    public class CharacterMigrationBeginRequestModel
    {
        public uint CharacterId { get; set; }
    }
    public class CharacterMigrationBeginRequestModelValidator : AbstractValidator<CharacterMigrationBeginRequestModel>
    {
        public CharacterMigrationBeginRequestModelValidator()
        {
            RuleFor(request => request.CharacterId).NotEmpty().WithMessage("You must specify the character Id.");
            RuleFor(request => request.CharacterId).GreaterThan((uint)0).WithMessage("You must specify a valid character Id.");
        }
        protected override bool PreValidate(ValidationContext<CharacterMigrationBeginRequestModel> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", "a request body must be supplied"));
                return false;
            }
            return true;
        }
    }
    public class CharacterMigrationBeginResponseModel
    {
        public bool Success { get; set; }
        public uint CharacterId { get; set; }
        public string Cookie { get; set; }
        public string BaseURL { get; set; }
    }
}
