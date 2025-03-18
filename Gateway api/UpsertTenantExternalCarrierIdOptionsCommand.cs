using PncCore.Shared.Interfaces.Commands.Settings;
using PncCore.Shared.Models.Settings;

namespace Gateway.Core.Commands.Tenant.Settings.Optimization;

public record UpsertTenantExternalCarrierIdOptionsCommand(
    string TenantId,
    string TaskId,
    string Issuer,
    string UserName,
    ExternalCarrierIdOptionsUpdateModel Options) : IUpsertTenantExternalCarrierIdOptionsCommand;