using ACE.Plugin.Transfer.Enums;
using ACE.Plugin.Transfer.Managers;
using ACE.Plugin.Transfer.Model.Character.Migration;
using ACE.Plugin.Web;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace ACE.Plugin.Transfer
{
    static partial class WebEndpoints
    {
        public static IEndpointRouteBuilder MigrationDownload(IEndpointRouteBuilder endpoints)
        {
            static async Task<string> endpointAsync(HttpContext context,
                [FromQuery] string Cookie,
                IValidator<CharacterMigrationDownloadRequestModel> validator)
            {
                var request = new CharacterMigrationDownloadRequestModel()
                {
                    Cookie = Cookie
                };
                ValidationResult vr = await validator.ValidateAsync(request);
                if (!vr.IsValid)
                {
                    return context.BadRequest(vr);
                }
                PackageMetadata metadata = new PackageMetadata
                {
                    Cookie = request.Cookie
                };
                MigrateCloseResult result = null;
                Gate.RunGatedAction(() =>
                {
                    result = TransferManager.CloseMigration(metadata, MigrationCloseType.Download);
                }, 1); // inter-server request must use a different queue
                WebApiCharacterMigrationDownloadResponseModel resp = new WebApiCharacterMigrationDownloadResponseModel()
                {
                    Cookie = request.Cookie,
                    SnapshotPackage = result.SnapshotPackage,
                    Success = result.Success
                };
                return context.Ok(resp);
            };

            _ = endpoints.MapGet("/api/character/migrationDownload", endpointAsync).AllowAnonymous();

            return endpoints;
        }
    }
}
