using PncCore.Shared.Interfaces.Commands.Settings;
using PncCore.Shared.Models.Settings;

namespace Api.Core.Commands.Settings;

public record UpsertSiteExternalCarrierIdOptionsCommand(
    string TenantId,
    string SiteKey,
    string TaskId,
    string Issuer,
    string UserName,
    ExternalCarrierIdOptionsUpdateModel Options) : IUpsertSiteExternalCarrierIdOptionsCommand;