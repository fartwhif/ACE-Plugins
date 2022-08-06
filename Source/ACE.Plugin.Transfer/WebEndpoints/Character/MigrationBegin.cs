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
        public static IEndpointRouteBuilder MigrationBegin(IEndpointRouteBuilder endpoints)
        {
            static async Task<string> endpointAsync(HttpContext context,
                [FromBody] CharacterMigrationBeginRequestModel request,
                IValidator<CharacterMigrationBeginRequestModel> validator)
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
                    PackageType = PackageType.Migrate
                };
                Gate.RunGatedAction(() =>
                {
                    metadata = TransferManager.CreatePackage(metadata).Result;
                });
                if (metadata == null)
                {
                    return context.Ok(new CharacterMigrationBeginResponseModel()
                    {
                        CharacterId = request.CharacterId,
                        Success = false
                    });
                }
                if (!File.Exists(metadata.FilePath))
                {
                    return context.Ok(new CharacterMigrationBeginResponseModel()
                    {
                        CharacterId = request.CharacterId,
                        Success = false
                    });
                }
                CharacterMigrationBeginResponseModel resp = new CharacterMigrationBeginResponseModel
                {
                    BaseURL = $"https://{WebConfigManager.Config.ExternalIPAddressOrDNSName}:{WebConfigManager.Config.ExternalPort}",
                    Cookie = metadata.Cookie,
                    Success = true,
                    CharacterId = request.CharacterId
                };
                return context.Ok(resp);
            };

            _ = endpoints.MapGet("api/character/migrationBegin", endpointAsync).RequireAuthorization();

            return endpoints;
        }
    }
}
