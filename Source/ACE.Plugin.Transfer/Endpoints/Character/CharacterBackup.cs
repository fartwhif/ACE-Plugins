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
using System.IO;
using System.Threading.Tasks;

namespace ACE.Plugin.Transfer
{
    static partial class WebEndpoints
    {
        public static IEndpointRouteBuilder CharacterBackup(IEndpointRouteBuilder endpoints)
        {
            static async Task<string> endpointAsync(HttpContext context,
                [FromBody] CharacterBackupRequestModel request,
                IValidator<CharacterBackupRequestModel> validator)
            {
                ValidationResult vr = await validator.ValidateAsync(request);
                if (!vr.IsValid)
                {
                    return context.BadRequest(vr);
                }
                var account = context.User.ToACEAccount();
                PackageMetadata metadata = new PackageMetadata
                {
                    CharacterId = request.CharacterId,
                    AccountId = account.AccountId,
                    PackageType = PackageType.Backup
                };
                Gate.RunGatedAction(() =>
                {
                    metadata = TransferManager.CreatePackage(metadata).Result;
                });
                if (metadata == null)
                {
                    return context.Ok(new CharacterBackupResponseModel
                    {
                        Success = false,
                        CharacterId = request.CharacterId
                    });
                }
                if (!File.Exists(metadata.FilePath))
                {
                    return context.Ok(new CharacterBackupResponseModel
                    {
                        Success = false,
                        CharacterId = request.CharacterId
                    });
                }
                CharacterBackupResponseModel resp = new CharacterBackupResponseModel
                {
                    SnapshotPackage = File.ReadAllBytes(metadata.FilePath),
                    Success = true,
                    CharacterId = request.CharacterId
                };
                File.Delete(metadata.FilePath);
                return context.Ok(resp);
            };

            _ = endpoints.MapGet("api/character/backup", endpointAsync).RequireAuthorization();

            return endpoints;
        }
    }
}
