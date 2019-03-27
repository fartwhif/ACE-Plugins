using ACE.Entity.Enum;
using ACE.Plugin.Transfer.Enums;
using ACE.Plugin.Transfer.Managers;
using ACE.Plugin.Transfer.Model.Character;
using ACE.Plugin.Transfer.Model.Character.Migration;
using ACE.Plugin.WebAPI;
using ACE.Plugin.WebAPI.Common;
using ACE.Plugin.WebAPI.Modules;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using System;
using System.IO;

namespace ACE.Plugin.Transfer.Modules
{
    public class AnyAuthenticatedModule : BaseAuthenticatedModule
    {
        public AnyAuthenticatedModule()
        {
            this.RequiresAuthentication();

            this.RequiresAnyClaim(
                k => k.Type == AccessLevel.Admin.ToString(),
                k => k.Type == AccessLevel.Advocate.ToString(),
                k => k.Type == AccessLevel.Developer.ToString(),
                k => k.Type == AccessLevel.Envoy.ToString(),
                k => k.Type == AccessLevel.Player.ToString(),
                k => k.Type == AccessLevel.Sentinel.ToString());


            Get("/api/character/backup", async (_) =>
            {
                CharacterBackupRequestModel request = this.BindAndValidate<CharacterBackupRequestModel>();
                if (!ModelValidationResult.IsValid)
                {
                    return Negotiate.WithModel(ModelValidationResult).WithStatusCode(HttpStatusCode.BadRequest);
                }
                PackageMetadata metadata = new PackageMetadata
                {
                    CharacterId = request.CharacterId,
                    AccountId = uint.Parse(Context.CurrentUser.FindFirst("AccountId").Value),
                    PackageType = PackageType.Backup
                };
                Gate.RunGatedAction(() =>
                {
                    metadata = TransferManager.CreatePackage(metadata).Result;
                });
                if (metadata == null)
                {
                    return new CharacterBackupResponseModel
                    {
                        Success = false,
                        CharacterId = request.CharacterId
                    }.AsJsonWebResponse();
                }
                if (!File.Exists(metadata.FilePath))
                {
                    return new CharacterBackupResponseModel
                    {
                        Success = false,
                        CharacterId = request.CharacterId
                    }.AsJsonWebResponse();
                }
                CharacterBackupResponseModel resp = new CharacterBackupResponseModel
                {
                    SnapshotPackage = File.ReadAllBytes(metadata.FilePath),
                    Success = true,
                    CharacterId = request.CharacterId
                };
                File.Delete(metadata.FilePath);
                return resp.AsJsonWebResponse();
            });

            Get("/api/character/migrationBegin", async (_) =>
            {
                CharacterMigrationBeginRequestModel request = this.BindAndValidate<CharacterMigrationBeginRequestModel>();
                if (!ModelValidationResult.IsValid)
                {
                    return Negotiate.WithModel(ModelValidationResult).WithStatusCode(HttpStatusCode.BadRequest);
                }
                PackageMetadata metadata = new PackageMetadata
                {
                    CharacterId = request.CharacterId,
                    AccountId = uint.Parse(Context.CurrentUser.FindFirst("AccountId").Value),
                    PackageType = PackageType.Migrate
                };
                Gate.RunGatedAction(() =>
                {
                    metadata = TransferManager.CreatePackage(metadata).Result;
                });
                if (metadata == null)
                {
                    return new CharacterMigrationBeginResponseModel()
                    {
                        CharacterId = request.CharacterId,
                        Success = false
                    }.AsJsonWebResponse();
                }
                if (!File.Exists(metadata.FilePath))
                {
                    return new CharacterMigrationBeginResponseModel()
                    {
                        CharacterId = request.CharacterId,
                        Success = false
                    }.AsJsonWebResponse();
                }
                CharacterMigrationBeginResponseModel resp = new CharacterMigrationBeginResponseModel
                {
                    BaseURL = $"https://{WebAPIConfigManager.Config.ExternalIPAddressOrDNSName}:{WebAPIConfigManager.Config.ExternalPort}",
                    Cookie = metadata.Cookie,
                    Success = true,
                    CharacterId = request.CharacterId
                };
                return resp.AsJsonWebResponse();
            });

            Get("/api/character/migrationCancel", async (_) =>
            {
                CharacterMigrationCancelRequestModel request = this.BindAndValidate<CharacterMigrationCancelRequestModel>();
                if (!ModelValidationResult.IsValid)
                {
                    return Negotiate.WithModel(ModelValidationResult).WithStatusCode(HttpStatusCode.BadRequest);
                }
                PackageMetadata metadata = new PackageMetadata
                {
                    Cookie = request.Cookie,
                    AccountId = uint.Parse(Context.CurrentUser.FindFirst("AccountId").Value)
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
                return resp.AsJsonWebResponse();
            });

            Get("/api/character/migrationComplete", async (_) =>
            {
                CharacterMigrationCompleteRequestModel request = this.BindAndValidate<CharacterMigrationCompleteRequestModel>();
                if (!ModelValidationResult.IsValid)
                {
                    return Negotiate.WithModel(ModelValidationResult).WithStatusCode(HttpStatusCode.BadRequest);
                }
                PackageMetadata metadata = new PackageMetadata
                {
                    NewCharacterName = request.NewCharacterName.Trim(),
                    Cookie = request.Cookie,
                    AccountId = uint.Parse(Context.CurrentUser.FindFirst("AccountId").Value),
                    ImportUrl = new Uri(request.BaseURL),
                    PackageType = PackageType.Migrate
                };
                ImportAndMigrateResult result = null;
                Gate.RunGatedAction(() =>
                {
                    result = TransferManager.ImportAndMigrate(metadata);
                });
                return new CharacterMigrationCompleteResponseModel()
                {
                    Cookie = request.Cookie,
                    Success = result.Success,
                    CharacterName = result.NewCharacterName,
                    FailureReason = result.Success ? null : result.FailReason.ToString(),
                    CharacterId = result.NewCharacterId
                }.AsJsonWebResponse();
            });

            Post("/api/character/import", async (_) =>
            {
                CharacterImportRequestModel request = this.BindAndValidate<CharacterImportRequestModel>();
                if (!ModelValidationResult.IsValid)
                {
                    return Negotiate.WithModel(ModelValidationResult).WithStatusCode(HttpStatusCode.BadRequest);
                }
                PackageMetadata metadata = new PackageMetadata
                {
                    NewCharacterName = request.NewCharacterName.Trim(),
                    AccountId = uint.Parse(Context.CurrentUser.FindFirst("AccountId").Value),
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
                    return new CharacterImportResponseModel()
                    {
                        Success = false,
                        FailureReason = "SnapshotPackageBase64 is not valid Base64 encoded data."
                    }.AsJsonWebResponse();
                }
                Gate.RunGatedAction(() =>
                {
                    result = TransferManager.ImportAndMigrate(metadata, fileData);
                });
                return new CharacterImportResponseModel()
                {
                    Success = result.Success,
                    CharacterName = result.NewCharacterName,
                    CharacterId = result.NewCharacterId,
                    FailureReason = result.Success ? null : result.FailReason.ToString()
                }.AsJsonWebResponse();
            });
        }
    }
}
