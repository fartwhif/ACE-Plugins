using ACE.Plugin.Transfer.Enums;
using ACE.Plugin.Transfer.Managers;
using ACE.Plugin.Transfer.Model.Character;
using ACE.Plugin.Web;
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
        public static IEndpointRouteBuilder CharacterImport(IEndpointRouteBuilder endpoints)
        {
            static async Task<string> endpointAsync(HttpContext context,
                [FromBody] CharacterImportRequestModel request,
                IValidator<CharacterImportRequestModel> validator)
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
                    AccountId = account.AccountId,
                    PackageType = PackageType.Backup
                };
                ImportAndMigrateResult result = null;
                byte[] fileData = null;
                try
                {
                    fileData = Convert.FromBase64String(request.SnapshotPackageBase64);
                }
                catch
                {
                    return context.Ok(new CharacterImportResponseModel()
                    {
                        Success = false,
                        FailureReason = "SnapshotPackageBase64 is not valid Base64 encoded data."
                    });
                }
                Gate.RunGatedAction(() =>
                {
                    result = TransferManager.ImportAndMigrate(metadata, fileData);
                });
                return context.Ok(new CharacterImportResponseModel()
                {
                    Success = result.Success,
                    CharacterName = result.NewCharacterName,
                    CharacterId = result.NewCharacterId,
                    FailureReason = result.Success ? null : result.FailReason.ToString()
                });
            };

            _ = endpoints.MapPost("api/character/import", endpointAsync).RequireAuthorization();

            return endpoints;
        }
    }
}
