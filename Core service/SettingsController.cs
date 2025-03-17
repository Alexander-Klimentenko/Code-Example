using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Api.Core.Facades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PncCore.Shared.Models.Settings;
using PncCore.Shared.Models.Settings.AppFeatures;
using PncCore.Shared.Models.Settings.Optimization;
using Project.Infrastructure.Cache.Redis;
using Project.Models.Shared.PncCore;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [Produces("application/json")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public class SettingsController(ISettingsFacade facade, IRedisCache redisCache) : ControllerBase
    {
        [HttpGet("MagicButtonOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MagicButtonOptions))]
        public async Task<IActionResult> MagicButtonOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null)
        {
            return Ok(await facade.GetOption<MagicButtonOptions>(tenantId, siteKey));
        }

        [HttpGet("ApproveDeviationsOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApproveDeviationsOptions))]
        public async Task<IActionResult> ApproveDeviationsOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<ApproveDeviationsOptions>(tenantId, siteKey, cancellationToken));
        }

        [HttpGet("ManualPickingOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ManualPickingOptions))]
        public async Task<IActionResult> ManualPickingOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null)
        {
            return Ok(await facade.GetOption<ManualPickingOptions>(tenantId, siteKey));
        }

        [HttpGet("PicklistOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PickSlotOptions))]
        public async Task<IActionResult> PicklistOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null)
        {
            var result = await facade.GetOption<PicklistOptions>(tenantId, siteKey);

            var cacheKey = string.IsNullOrEmpty(siteKey) ?
                $"{tenantId}|PicklistOptions" :
                $"{tenantId}|{siteKey}|PicklistOptions";
            _ = redisCache.Set(cacheKey, result, (int)TimeSpan.FromDays(1).TotalSeconds);

            return Ok(result);
        }

        [HttpGet("sitePicklistOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PickSlotOptions))]
        public async Task<IActionResult> PicklistOptions([Required] [FromQuery] string tenantId)
        {
            var options = (await facade.GetAllSitePicklistOptions(tenantId)).ToArray();

            var mapped = options.ToDictionary(key => key.Id,
                src => new PicklistOptions(src.PreparationStatesInactivityThreshold, src.InProgressStatesInactivityThreshold,
                    src.ReleaseInactivePicklist, src.UpdatedBy, src.UpdatedUtc));

            return Ok(mapped);
        }

        [HttpGet("PickSlotOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PickSlotOptions))]
        public async Task<IActionResult> PickSlotOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null)
        {
            return Ok(await facade.GetOption<PickSlotOptions>(tenantId, siteKey));
        }

        [HttpGet("PickingThresholdOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PickingThresholdOptions))]
        public async Task<IActionResult> PickingThresholdOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null)
        {
            return Ok(await facade.GetOption<PickingThresholdOptions>(tenantId, siteKey));
        }

        [HttpGet("QualityPickingRules")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(QualityPickingRules))]
        public async Task<IActionResult> QualityPickingRules([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null)
        {
            var result = await facade.GetOption<QualityPickingRules>(tenantId, siteKey);
            var cacheKey = string.IsNullOrEmpty(siteKey) ? $"{tenantId}|qualityPickingRules" : $"{tenantId}|{siteKey}|qualityPickingRules";
            _ = redisCache.Set(cacheKey, result, (int)TimeSpan.FromDays(1).TotalSeconds);
            return Ok(result);
        }


        [HttpGet("Labels")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<LabelTemplate>))]
        public async Task<IActionResult> GetDefaultLabelTemplates()
        {
            return Ok(await facade.GetDefaultLabelTemplates());
        }

        [HttpGet("Labels/Selected")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<LabelTemplate>))]
        public async Task<IActionResult> GetLabelTemplates([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = default, [FromQuery] LabelType? type = default, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetLabelTemplates(tenantId, siteKey, type, cancellationToken));
        }

        [HttpGet("AppFlowOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AppFlowOptions))]
        public async Task<IActionResult> AppFlowOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null)
        {
            return Ok(await facade.GetOption<AppFlowOptions>(tenantId, siteKey));
        }

        [HttpGet("AppUIOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PncCore.Shared.Models.Settings.AppUI.AppUIOptions))]
        public async Task<IActionResult> AppUIOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null)
        {
            return Ok(await facade.GetOption<PncCore.Shared.Models.Settings.AppUI.AppUIOptions>(tenantId, siteKey));
        }

        [HttpGet("DeliverableAutomationOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeliverableAutomationOptions))]
        public async Task<IActionResult> DeliverableAutomationOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null)
        {
            return Ok(await facade.GetOption<DeliverableAutomationOptions>(tenantId, siteKey));
        }

        [HttpGet("AppFeatureOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AppFeatureOptions))]
        public async Task<IActionResult> AppFeatureOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null)
        {
            return Ok(await facade.GetOption<AppFeatureOptions>(tenantId, siteKey));
        }

        [HttpGet("OrderStateTransitionRules")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderStateTransitionRules))]
        public async Task<IActionResult> OrderStateTransitionRules([Required] [FromQuery] string tenantId, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<OrderStateTransitionRules>(tenantId, default, cancellationToken));
        }

        [HttpGet("AllowAddOrderLineOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AllowAddOrderLineOptions))]
        public async Task<IActionResult> AllowAddOrderLineOptions([Required] [FromQuery] string tenantId, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<AllowAddOrderLineOptions>(tenantId, default, cancellationToken));
        }

        [HttpGet("OrderNotificationOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderNotificationOptions))]
        public async Task<IActionResult> OrderNotificationOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null)
        {
            return Ok(await facade.GetOption<OrderNotificationOptions>(tenantId, siteKey));
        }

        [HttpGet("SubstitutionLimitationsOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(SubstitutionLimitationsOptions))]
        public async Task<IActionResult> SubstitutionLimitationsOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<SubstitutionLimitationsOptions>(tenantId, siteKey, cancellationToken));
        }

        [HttpGet("OverrideCustomerSubstitutionSettingOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OverrideCustomerSubstitutionSettingOptions))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> OverrideCustomerSubstitutionSettingOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<OverrideCustomerSubstitutionSettingOptions>(tenantId, siteKey, cancellationToken));
        }

        [HttpGet("ForcePrintWeightLabelOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ForcePrintWeightLabelOptions))]
        public async Task<IActionResult> ForcePrintWeightLabelOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<ForcePrintWeightLabelOptions>(tenantId, siteKey, cancellationToken));
        }

        [HttpGet("DeliveryNotes")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetDeliveryNotes([Required] string tenantId, [Required] string siteKey, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetDeliveryNotes(tenantId, siteKey, cancellationToken));
        }

        [HttpGet("DeliveryNote/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeliveryNoteSettings))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetDeliveryNote([FromRoute] string id, [Required] string tenantId, [Required] string siteKey, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetDeliveryNote(id, tenantId, siteKey, cancellationToken));
        }

        [HttpGet("DeliveryNote/Active")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeliveryNoteSettings))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetActiveDeliveryNote([Required] string tenantId, [Required] string siteKey, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetActiveDeliveryNote(tenantId, siteKey, cancellationToken));
        }

        [HttpGet("TransportDocument")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(TransportDocumentSettings))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetTransportDocument([Required] string tenantId, [Required] string siteKey, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetTransportDocument(tenantId, siteKey, cancellationToken));
        }

        [HttpGet("OrderStatusOnDeviationsOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderStatusOnDeviationsOptions))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetUpdateOrderStatusOnDeviationsOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<OrderStatusOnDeviationsOptions>(tenantId, siteKey, cancellationToken));
        }

        [HttpGet("ShowCustomerInfoOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ShowCustomerInfoOptions))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> ShowCustomerInfoOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<ShowCustomerInfoOptions>(tenantId, siteKey, cancellationToken));
        }

        [HttpGet("ForcePickAllLinesOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ForcePickAllLinesOptions))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> ForcePickAllLinesOptions([Required] [FromQuery] string tenantId, [FromQuery] string siteKey = null, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<ForcePickAllLinesOptions>(tenantId, siteKey, cancellationToken));
        }

        [HttpGet("CancelOrderInWebOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CancelOrderInWebOptions))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CancelOrderOrderInWebOptions([Required] [FromQuery] string tenantId, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<CancelOrderInWebOptions>(tenantId, cancellationToken: cancellationToken));
        }

        [HttpGet("PriceToMasssOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PriceToMassOptions))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> PriceToMasssOptions([Required] [FromQuery] string tenantId, string siteKey = null, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<PriceToMassOptions>(tenantId, siteKey ?? "*", cancellationToken));
        }

        [HttpGet("OverrideExpirationDateOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OverrideExpirationDateOptions))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> OverrideExpirationDateOptions([Required] [FromQuery] string tenantId, string siteKey = null, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<OverrideExpirationDateOptions>(tenantId, siteKey ?? "*", cancellationToken));
        }

        [HttpGet("RegisterLoadCarriersOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RegisterLoadCarriersOptions))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetRegisterLoadCarriersOptions([Required] [FromQuery] string tenantId, string siteKey = null, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<RegisterLoadCarriersOptions>(tenantId, siteKey ?? "*", cancellationToken));
        }

        [HttpGet("RequireLoadCarrierOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RequireLoadCarrierOptions))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> RequireLoadCarrierOptions([Required] [FromQuery] string tenantId, string siteKey = null, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<RequireLoadCarrierOptions>(tenantId, siteKey, cancellationToken));
        }

        [HttpGet("RequireCompileOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RequireCompileOptions))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> RequireCompileOptions([Required] [FromQuery] string tenantId, string siteKey = null, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<RequireCompileOptions>(tenantId, siteKey, cancellationToken));
        }

        [HttpGet("CustomProductLocationOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomProductLocationOptions))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CustomProductLocationOptions([Required] [FromQuery] string tenantId, string siteKey = null, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<CustomProductLocationOptions>(tenantId, siteKey, cancellationToken));
        }

        [HttpGet("ExternalCarrierIdOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ExternalCarrierIdOptions))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> ExternalCarrierIdOptions([Required] [FromQuery] string tenantId, string siteKey = null, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<ExternalCarrierIdOptions>(tenantId, siteKey, cancellationToken));
        }

        [HttpGet("TrolleyLocationsOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(TrolleyLocationsOptions))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> TrolleyLocationsOptions([Required] [FromQuery] string tenantId, string siteKey = null, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<TrolleyLocationsOptions>(tenantId, siteKey, cancellationToken));
        }

        [HttpGet("LegacyLocationBarcodeSupportOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LegacyLocationBarcodeSupportOptions))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> LegacyLocationBarcodeSupportOptions([Required][FromQuery] string tenantId, CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetOption<LegacyLocationBarcodeSupportOptions>(tenantId, cancellationToken: cancellationToken));
        }
    }
}