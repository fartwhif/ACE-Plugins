using FluentValidation;

namespace ACE.Plugin.Web.Model.Admin
{
    public class AdminCommandRequestModel
    {
        public string? Command { get; set; }
    }
    public class AdminCommandRequestModelValidator : AbstractValidator<AdminCommandRequestModel>
    {
        public AdminCommandRequestModelValidator()
        {
            RuleFor(request => request.Command).NotEmpty().WithMessage("You must specify the command to execute.");
        }
    }
    public class AdminCommandResponseModel
    {
        public bool Success { get; set; }
        public string CommandDigestionResult { get; set; }
        public string CommandHandlerResponse { get; set; }
        public string SubmittedCommand { get; set; }
    }
}
