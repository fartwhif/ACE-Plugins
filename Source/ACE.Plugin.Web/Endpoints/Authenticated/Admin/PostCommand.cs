using ACE.Entity.Enum;
using ACE.Plugin.Web.Model.Admin;
using ACE.Server.Command;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ACE.Plugin.Web
{
    static partial class WebEndpoints
    {
        public static IEndpointRouteBuilder PostCommand(IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapPost("PostCommand", async (HttpContext context, [FromBody] AdminCommandRequestModel request, IValidator<AdminCommandRequestModel> validator) =>
            {
                var account = context.User.ToACEAccount();

                // to-do: use an AuthorizationPolicy for this
                if (((AccessLevel)account.AccessLevel) != AccessLevel.Admin)
                {
                    return context.Unauthorized("");//you must have admin to use this
                }

                ValidationResult vr = await validator.ValidateAsync(request);
                if (!vr.IsValid)
                {
                    return context.BadRequest(vr);
                }

                CommandOverallResult? result = null;

                Gate.RunGatedAction(() =>
                {
                    log.Warn($"Executing command: {request.Command}");
                    result = CommandManager.DigestCommand(request.Command);
                });

                var resp = new AdminCommandResponseModel()
                {
                    Success = result.CommandResult == CommandDigestionResult.Success,
                    CommandDigestionResult = result.CommandResult.ToString(),
                    CommandHandlerResponse = result.CommandHandlerResponse?.ToString(),
                    SubmittedCommand = request.Command
                };

                return context.Ok(resp);

            }).RequireAuthorization();

            return endpoints;
        }
    }
}
