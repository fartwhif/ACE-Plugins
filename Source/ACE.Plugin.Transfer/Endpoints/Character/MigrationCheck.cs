using ACE.Plugin.Crypto.Managers;
using ACE.Plugin.Transfer.Common;
using ACE.Plugin.Transfer.Enums;
using ACE.Plugin.Transfer.Managers;
using ACE.Plugin.Transfer.Responses;
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
        public static IEndpointRouteBuilder MigrationCheck(IEndpointRouteBuilder endpoints)
        {
            static async Task<string> endpointAsync(HttpContext context,
                [FromQuery] string Cookie,
                [FromQuery] string Nonce,
                IValidator<MigrationCheckRequestModel> validator)
            {
                var request = new MigrationCheckRequestModel()
                {
                    Nonce = Nonce,
                    Cookie = Cookie
                };
                ValidationResult vr = await validator.ValidateAsync(request);
                if (!vr.IsValid)
                {
                    return context.BadRequest(vr);
                }
                MigrationReadyStatus readyStatus = MigrationReadyStatus.Unknown;
                Gate.RunGatedAction(() =>
                {
                    readyStatus = TransferManager.CheckReadyStatusOfMigration(request.Cookie);
                }, 1); // inter-server request must use a different queue

                MigrationCheckResponseModel payload = new MigrationCheckResponseModel()
                {
                    Config = new TransferConfigResponseModel()
                    {
                        MyThumbprint = CertificateManager.Thumbprint,
                        AllowImportFrom = TransferConfigManager.Config.AllowImportFrom,
                        AllowMigrationFrom = TransferConfigManager.Config.AllowMigrationFrom,
                        AllowBackup = TransferConfigManager.Config.AllowBackup,
                        AllowImport = TransferConfigManager.Config.AllowImport,
                        AllowMigrate = TransferConfigManager.Config.AllowMigrate,
                    },
                    Cookie = request.Cookie,
                    Nonce = request.Nonce,
                    Ready = readyStatus == MigrationReadyStatus.Ready,
                    ReadyStatus = readyStatus.ToString()
                };

                SignedMigrationCheckResponseModel model = new SignedMigrationCheckResponseModel()
                {
                    Result = payload,
                    Signature = CertificateManager.SignData(payload.ToJSON()),
                    Signer = CertificateManager.ExportCertAsBytes()
                };

                return context.Ok(model);
            };

            _ = endpoints.MapGet("api/character/migrationCheck", endpointAsync).AllowAnonymous();

            return endpoints;
        }
    }
}
