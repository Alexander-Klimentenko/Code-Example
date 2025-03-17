using PncCore.Shared.Interfaces.Commands.Settings;
using PncCore.Shared.Models.Settings;

namespace Api.Core.Commands.Settings;

public record UpsertTenantExternalCarrierIdOptionsCommand(
    string TenantId,
    string TaskId,
    string Issuer,
    string UserName,
    ExternalCarrierIdOptionsUpdateModel Options) : IUpsertTenantExternalCarrierIdOptionsCommand;