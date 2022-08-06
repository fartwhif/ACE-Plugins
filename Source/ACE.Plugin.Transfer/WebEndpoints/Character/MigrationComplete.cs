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
using System;
using System.IO;
using System.Threading.Tasks;

namespace ACE.Plugin.Transfer
{
    static partial class WebEndpoints
    {
        public static IEndpointRouteBuilder MigrationComplete(IEndpointRouteBuilder endpoints)
        {
            static async Task<string> endpointAsync(HttpContext context,
                [FromBody] CharacterMigrationCompleteRequestModel request,
                IValidator<CharacterMigrationCompleteRequestModel> validator)
            {
                ValidationResult vr = await validator.ValidateAsync(request);
                if (!vr.IsValid)
                {
                    return context.BadRequest(vr);
                }
                var account = context.User.ToACEAccount();
                PackageMetadata metadata = new PackageMetadata
                {
                    NewCharacterName = request.NewCharacterName.Trim(),
                    Cookie = request.Cookie,
                    AccountId = account.AccountId,
                    ImportUrl = new Uri(request.BaseURL),
                    PackageType = PackageType.Migrate
                };
                ImportAndMigrateResult result = null;
                Gate.RunGatedAction(() =>
                {
                    result = TransferManager.ImportAndMigrate(metadata);
                });
                return context.Ok(new CharacterMigrationCompleteResponseModel()
                {
                    Cookie = request.Cookie,
                    Success = result.Success,
                    CharacterName = result.NewCharacterName,
                    FailureReason = result.Success ? null : result.FailReason.ToString(),
                    CharacterId = result.NewCharacterId
                });
            };

            _ = endpoints.MapGet("api/character/migrationComplete", endpointAsync).RequireAuthorization();

            return endpoints;
        }
    }
}
