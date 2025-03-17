using PncCore.Shared.Interfaces.Commands.Settings;
using PncCore.Shared.Models.Settings;

namespace Gateway.Core.Commands.Site.Settings.Optimization;

public record UpsertSiteExternalCarrierIdOptionsCommand(
    string TenantId,
    string SiteKey,
    string TaskId,
    string Issuer,
    string UserName,
    ExternalCarrierIdOptionsUpdateModel Options) : IUpsertSiteExternalCarrierIdOptionsCommand;