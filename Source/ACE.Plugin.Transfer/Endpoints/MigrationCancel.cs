using ACE.Plugin.Transfer.Enums;
using ACE.Plugin.Transfer.Managers;
using ACE.Plugin.Transfer.Model.Character.Migration;
using ACE.Plugin.Web;
using ACE.Plugin.Web.Common;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.IO;
using System.Threading.Tasks;

namespace ACE.Plugin.Transfer
{
    static partial class WebEndpoints
    {
        public static IEndpointRouteBuilder MigrationCancel(IEndpointRouteBuilder endpoints)
        {
            static async Task<string> endpointAsync(HttpContext context,
                [FromBody] CharacterMigrationCancelRequestModel request,
                IValidator<CharacterMigrationCancelRequestModel> validator)
            {
                ValidationResult vr = await validator.ValidateAsync(request);
                if (!vr.IsValid)
                {
                    return context.BadRequest(vr);
                }
                var account = context.User.ToACEAccount();
                PackageMetadata metadata = new PackageMetadata
                {
                    Cookie = request.Cookie,
                    AccountId = account.AccountId
                };
                MigrateCloseResult result = null;
                Gate.RunGatedAction(() =>
                {
                    result = TransferManager.CloseMigration(metadata, MigrationCloseType.Cancel);
                });
                CharacterMigrationCancelResponseModel resp = new CharacterMigrationCancelResponseModel()
                {
                    Cookie = request.Cookie,
                    Success = result.Success
                };
                return context.Ok(resp);
            };

            _ = endpoints.MapGet("api/character/migrationCancel", endpointAsync).RequireAuthorization();

            return endpoints;
        }
    }
}
