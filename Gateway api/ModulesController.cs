using Api.Facades;
using Api.Filters;
using Gateway.Controllers;
using Gateway.Facades;
using Gateway.Shared.Models;
using Gateway.Shared.Models.PnC.Reasons;
using Gateway.Shared.Models.PnC.Settings;
using Gateway.Shared.Models.PnC.Settings.AppFeatures;
using Gateway.Shared.Models.PnC.Settings.AppUI;
using Gateway.Shared.Models.PnC.Settings.Optimization;
using Infrastructure.Communication.Http;
using Infrastructure.Communication.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Models.Shared.PncCore;
using Project.Models.Shared.Tenant;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Gateway.Shared.Requests;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("All endpoints available regarding modules")]
    public class ModulesController : ApiControllerBase
    {
        private readonly IModulesFacade facade;

        private readonly ITenantFacade tenantFacade;

        public ModulesController(
            IModulesFacade facade,
            ITenantFacade tenantFacade)
        {
            this.facade = facade;
            this.tenantFacade = tenantFacade;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "List all modules in Project")]
        [SwaggerResponse(
            (int)HttpStatusCode.OK,
            Type = typeof(IEnumerable<Module>))]
        //[ApiPermissionFilterFactory("read", "tenant")] - todo: Is this needed when this is a "system" resource? e.g: list all modules in Project
        public async Task<IActionResult> GetModules()
        {
            return Ok(await facade.GetModules());
        }

        [HttpGet("pnc/settings/magicBtnOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Gets the options for the magic button",
            Description = "Scope: read|site",
            OperationId = "GetMagicBtnOptions")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(MagicButtonOptions))]
        public async Task<IActionResult> GetMagicBtnOptions(CancellationToken cancellationToken = default)
        {
            return Ok(await facade.MagicBtnOptions(TenantId, SiteKey, cancellationToken));
        }

        [HttpPut("pnc/settings/magicBtnOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Update the options for the magic button",
            Description = "Scope: manage|site",
            OperationId = "UpsertMagicBtnOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.FailedDependency)]
        public async Task<IActionResult> UpsertMagicBtnOptions([FromBody] MagicButtonOptionsUpdateRequest request)
        {
            var queued = await facade.UpsertMagicBtnOptions(TenantId, SiteKey, request, TokenUser);
            return StatusCode((int)queued);
        }

        [HttpGet("pnc/settings/approveDeviationsOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Gets the options approve deviations",
            Description = "Scope: read|pncsettings",
            OperationId = "GetApproveDeviationsOptions")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ApproveDeviationsOptions))]
        public async Task<IActionResult> GetApproveDeviationsOptions(CancellationToken cancellationToken = default)
        {
            return Ok(await facade.GetApproveDeviationsOptions(TenantId, SiteKey, cancellationToken));
        }

        [HttpPut("pnc/settings/approveDeviationsOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "update the options for approve deviations",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpdateApproveDeviationsOptions")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateApproveDeviationsOptions([Required][FromBody] ApproveDeviationsOptionsUpdateRequest request)
        {
            return Ok(await facade.UpdateApproveDeviationsOptions(TenantId, SiteKey, request, TokenUser));
        }
        
        [HttpGet("pnc/settings/picklistOptions")]
        [SwaggerOperation(
            Summary = "Get Picklist Options for PnC module in the current tenant and site context",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse(
            (int)HttpStatusCode.OK,
            Type = typeof(PicklistOptions))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetPicklistOptions()
        {
            return Ok(await facade.GetPicklistOptions(TenantId, SiteKey));
        }

        [HttpPatch("pnc/settings/picklistOptions")]
        [SwaggerOperation(
            Summary = "Get Picklist Options for PnC module in the current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> UpdatePicklistOptions([Required] [FromBody] PicklistOptionsUpdateRequest request)
        {
            return StatusCode((int)await facade.UpdatePicklistOptions(TokenUser, TenantId, request, SiteKey));
        }

        [HttpGet("pnc/settings/PickSlotOptions")]
        [SwaggerOperation(
            Summary = "Get Pick Slot Options for PnC module in the current tenant and site context",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse(
            (int)HttpStatusCode.OK,
            Type = typeof(PickSlotOptions))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetPickSlotOptions()
        {
            return Ok(await facade.GetPickSlotOptions(TenantId, SiteKey));
        }

        [HttpPatch("pnc/settings/PickSlotOptions")]
        [SwaggerOperation(
            Summary = "Update Pick Slot Options for PnC module in the current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> UpdatePickSlotOptions([Required][FromBody] PickSlotOptionsUpdateRequest request)
        {
            return StatusCode((int)await facade.UpdatePickSlotOptions(TokenUser, TenantId, request, SiteKey));
        }

        [HttpGet("pnc/settings/QualityPickingRules")]
        [SwaggerOperation(
            Summary = "Get Quality Picking Rules for PnC module in the current tenant and site context",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse(
            (int)HttpStatusCode.OK,
            Type = typeof(QualityPickingRules))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetQualityPickingRules()
        {
            var rules = await facade.GetQualityPickingRules(TenantId, SiteKey);
            return Ok(rules);
        }

        [HttpPatch("pnc/settings/QualityPickingRules")]
        [SwaggerOperation(
            Summary = "Update Quality Picking Rules for PnC module in the current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> UpdateQualityPickingRules([Required][FromBody] QualityPickingRulesUpdateRequest request)
        {
            var updated = await facade.UpdateQualityPickingRules(TokenUser, TenantId, request, SiteKey);
            return StatusCode((int)updated);
        }

        [HttpGet("pnc/settings/AppUIOptions")]
        [SwaggerOperation(
            Summary = "Get UI options for PnC module's app in the current tenant and site context",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse(
            (int)HttpStatusCode.OK,
            Type = typeof(AppUIOptions))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetAppUIOptions()
        {
            return Ok(await facade.GetAppUIOptions(TenantId, SiteKey));
        }

        [HttpPatch("pnc/settings/AppUIOptions")]
        [SwaggerOperation(
            Summary = "Update UI options for PnC module's app in the current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> UpdateAppUIOptions([Required][FromBody] AppUIOptionsUpdateRequest request)
        {
            var updated = await facade.UpdateAppUIOptions(TokenUser, TenantId, request, SiteKey);
            return StatusCode((int)updated);
        }

        [HttpGet("pnc/settings/PickingThresholdOptions")]
        [SwaggerOperation(
            Summary = "Get Picking Threshold Options for PnC module in the current tenant and site context",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse(
            (int)HttpStatusCode.OK,
            Type = typeof(PickingThresholdOptions))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetPickingThresholdOptions()
        {
            return Ok(await facade.GetPickingThresholdOptions(TenantId, SiteKey));
        }

        [HttpPatch("pnc/settings/PickingThresholdOptions")]
        [SwaggerOperation(
            Summary = "Update Picking Threshold Options for PnC module in the current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> UpdatePickingThresholdOptions([Required][FromBody] PickingThresholdOptionsUpdateRequest request)
        {
            var updated = await facade.UpdatePickingThresholdOptions(TokenUser, TenantId, request, SiteKey);
            return StatusCode((int)updated);
        }

        [HttpGet("pnc/settings/ManualPickingOptions")]
        [SwaggerOperation(
            Summary = "Get Manual Picking Options for PnC module in the current tenant and site context",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse(
            (int)HttpStatusCode.OK,
            Type = typeof(ManualPickingOptions))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetManualPickingOptions()
        {
            return Ok(await facade.GetManualPickingOptions(TenantId, SiteKey));
        }

        [HttpPatch("pnc/settings/ManualPickingOptions")]
        [SwaggerOperation(
            Summary = "Update Manual Picking Options for PnC module in the current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> UpdateManualPickingOptions([Required][FromBody] ManualPickingOptionsUpdateRequest request)
        {
            var updated = await facade.UpdateManualPickingOptions(TokenUser, TenantId, request, SiteKey);
            return StatusCode((int)updated);
        }

        [HttpGet("pnc/settings/SubstitutionRuleOptions")]
        [SwaggerOperation(
            Summary = "Get Substitution Rule Options for PnC module in the current tenant and site context",
            Description = "Scope| read|pncsettings")]
        [SwaggerResponse(
            (int)HttpStatusCode.OK,
            Type = typeof(IEnumerable<SubstitutionRuleOptions>))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetSubstitutionRules()
        {
            return Ok(await facade.GetSubstitutionRuleOptions(TenantId, SiteKey));
        }

        [HttpPatch("pnc/settings/SubstitutionRuleOptions")]
        [SwaggerOperation(
            Summary = "Update Substitution Rule Options for PnC module in the current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> UpsertProductSubstitutionRuleOptions([FromBody] IEnumerable<SubstitutionRuleOptionsUpdateRequest> options)
        {
            var task = await facade.UpsertProductSubstitutionRuleOptions(TokenUser, TenantId, options, SiteKey);
            return task.IsSuccessful() ? Ok() : StatusCode((int)HttpStatusCode.FailedDependency);
        }

        [HttpDelete("pnc/settings/SubstitutionRuleOptions")]
        [SwaggerOperation(
            Summary = "Remove all current Substitution Rule Options for PnC module in current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        // TODO: What HttpStatusCode is returned for succesful deletion?
        public async Task<IActionResult> DeleteAllProductSubstitutionRuleOptions()
        {
            var response = await facade.DeleteAllProductSubstitutionRuleOptions(TenantId, SiteKey);
            return StatusCode((int)response);
        }

        [HttpPatch("pnc/settings/DeliverableAutomationOptions")]
        [SwaggerOperation(
            Summary = "Update setting how for automatic deliverable orders in PnC module in the current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> UpdateDeliverableAutomationOptions([Required][FromBody] DeliverableAutomationOptionsUpdateRequest options)
        {
            HttpStatusCode result;

            if (SiteKey == "*")
            {
                result = await facade.UpsertTenantDeliverableAutomationOptions(TenantId, TokenUser, options);
            }
            else
            {
                result = await facade.UpsertSiteDeliverableAutomationOptions(TenantId, SiteKey, TokenUser, options);
            }

            return StatusCode((int)result);
        }

        [HttpGet("pnc/settings/DeliverableAutomationOptions")]
        [SwaggerOperation(
            Summary = "Get automatic deliverable orders settings in PnC module in the current tenant and site context",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(DeliverableAutomationOptions))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetDeliverableAutomationOptions()
        {
            var result = await facade.GetDeliverableAutomationOptions(TenantId, SiteKey);
            return Ok(result);
        }

        [HttpPatch("pnc/settings/AppFeatureOptions")]
        [SwaggerOperation(
            Summary = "Update feature settings for the Order Picking Application in PnC module in the current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> UpdateAppFeatureOptions([Required][FromBody] AppFeatureOptionsUpdateRequest request)
        {
            var result = await facade.UpdateAppFeatureOptions(TenantId, TokenUser, request, SiteKey);
            return StatusCode((int)result);
        }

        [HttpGet("pnc/settings/AppFeatureOptions")]
        [SwaggerOperation(
            Summary = "Get Order Picking Application feature settings in PnC module in the current tenant and site context",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(AppFeatureOptions))]
        [ApiPermissionFilterFactory("read", "pncsettings", true)]
        public async Task<IActionResult> GetAppFeatureOptions()
        {
            var result = await facade.GetAppFeatureOptions(TenantId, SiteKey);
            return Ok(result);
        }

        [HttpGet("pnc/settings/OrderStateTransitionRules")]
        [SwaggerOperation(
            Summary = "Get order state transition rules in PnC module in the current tenant context",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(OrderStateTransitionRules))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetOrderStateTransitionRules()
        {
            var result = await facade.GetOrderStateTransitionRules(TenantId);
            return Ok(result);
        }

        [HttpPatch("pnc/settings/OrderStateTransitionRules")]
        [SwaggerOperation(
            Summary = "Update order state transition rules in PnC module in the current tenant context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> UpdateOrderStateTransitionRules([Required][FromBody] OrderStateTransitionRulesUpdateRequest request)
        {
            var result = await facade.UpsertOrderStateTransitionRules(TenantId, TokenUser, request);

            return StatusCode((int)result);
        }

        [HttpGet("pnc/settings/AllowAddOrderLineOptions")]
        [SwaggerOperation(
            Summary = "Get allow order line options in PnC module in the current tenant context",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(AllowAddOrderLineOptions))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetAllowAddOrderLineOptions()
        {
            var result = await facade.GetAllowAddOrderLineOptions(TenantId);
            return Ok(result);
        }

        [HttpPatch("pnc/settings/AllowAddOrderLineOptions")]
        [SwaggerOperation(
            Summary = "Update allow order line options in PnC module in the current tenant context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> UpdateAllowAddOrderLineOptions([Required][FromBody] AllowAddOrderLineOptionsUpdateRequest request)
        {
            var result = await facade.UpsertAllowAddOrderLineOptions(TenantId, TokenUser, request);

            return StatusCode((int)result);
        }

        [HttpGet("pnc/settings/OrderNotificationOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the order notification options ",
            Description = "Scope: read|pncsettings",
            OperationId = "ReadOrderNotificationOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderNotificationOptions))]
        public async Task<IActionResult> ReadOrderNotificationOptions()
        {
            return Ok(await facade.ReadOrderNotificationOptions(TenantId, SiteKey));
        }

        [HttpPatch("pnc/settings/OrderNotificationOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Update the order notification options ",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpsertOrderNotificationOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpsertOrderNotificationOptions([FromBody] OrderNotificationOptions options)
        {
            return StatusCode((int)await facade.UpsertOrderNotificationOptions(TenantId, SiteKey, TokenUser, options));
        }

        [HttpGet("pnc/settings/SubstitutionLimitationsOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the Substitution Limitations options",
            Description = "Scope: read|pncsettings",
            OperationId = "ReadSubstitutionLimitationsOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(SubstitutionLimitationsOptions))]
        public async Task<IActionResult> SubstitutionLimitationsOptions()
        {
            return Ok(await facade.SubstitutionLimitationsOptions(TenantId, SiteKey));
        }

        [HttpPatch("pnc/settings/SubstitutionLimitationsOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Update the Substitution Limitations options ",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpsertSubstitutionLimitationsOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SubstitutionLimitationsOptions([FromBody] SubstitutionLimitationsOptionsUpdateRequest updateRequest)
        {
            if (SiteKey != "*")
            {
                var policyResponse = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.SubstitutionLimitations);
                if (policyResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has substitution limitations management policy enabled.");
                }
            }

            return StatusCode((int)await facade.UpsertSubstitutionLimitationsOptions(TenantId, SiteKey, TokenUser, updateRequest));
        }

        [HttpGet("pnc/settings/ForcePrintWeightLabelOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the Force print weight label options",
            Description = "Scope: read|pncsettings",
            OperationId = "ReadSubstitutionLimitationsOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ForcePrintWeightLabelOptions))]
        public async Task<IActionResult> ForcePrintWeightLabelOptions()
        {
            return Ok(await facade.ForcePrintWeightLabelOptions(TenantId, SiteKey));
        }

        [HttpPatch("pnc/settings/ForcePrintWeightLabelOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Update the Force print weight label options",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpsertForcePrintWeightLabelOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> ForcePrintWeightLabelOptions([FromBody] ForcePrintWeightLabelOptionsUpdateRequest request)
        {
            if (SiteKey != "*")
            {
                var policyResponse = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.ScaleSettings);
                if (policyResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has force print weight label limitations management policy enabled.");
                }
            }

            return StatusCode((int)await facade.UpsertForcePrintWeightLabelOptions(TenantId, SiteKey, TokenUser, request));
        }

        [HttpGet("pnc/settings/ShowCustomerInfoOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the show customer info options",
            Description = "Scope: read|pncsettings",
            OperationId = "ReadShowCustomerInfoOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ShowCustomerInfoOptions))]
        public async Task<IActionResult> ShowCustomerInfoOptions()
        {
            return Ok(await facade.ShowCustomerInfoOptions(TenantId, SiteKey));
        }

        [HttpPatch("pnc/settings/ShowCustomerInfoOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Update the show customer info options",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpsertShowCustomerInfoOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> ShowCustomerInfoOptions([FromBody] ShowCustomerInfoOptionsUpdateRequest request)
        {
            if (SiteKey != "*")
            {
                var policyResponse = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.CustomerInfo);
                if (policyResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has show customer info limitations management policy enabled.");
                }
            }

            var opt = request ?? new ShowCustomerInfoOptionsUpdateRequest();

            return StatusCode((int)await facade.UpsertShowCustomerInfoOptions(TenantId, SiteKey, TokenUser, opt));
        }

        [HttpGet("pnc/settings/ForcePickAllLinesOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the force pick all orderlines options",
            Description = "Scope: read|pncsettings",
            OperationId = "ReadForcePickAllLinesOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ForcePickAllLinesOptions))]
        public async Task<IActionResult> ForcePickAllLinesOptions()
        {
            return Ok(await facade.ForcePickAllLinesOptions(TenantId, SiteKey));
        }

        [HttpPatch("pnc/settings/ForcePickAllLinesOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Update the force pick all orderlines options",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpsertForcePickAllLinesOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> ForcePickAllLinesOptions([FromBody] ForcePickAllLinesOptionsUpdateRequest request)
        {
            if (SiteKey != "*")
            {
                var policyResponse = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.ForcePickAllOrderLines);
                if (policyResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has show customer info limitations management policy enabled.");
                }
            }

            var opt = request ?? new ForcePickAllLinesOptionsUpdateRequest();

            return StatusCode((int)await facade.UpsertForcePickAllLinesOptions(TenantId, SiteKey, TokenUser, opt));
        }


        [HttpGet("pnc/settings/PriceToMassOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the price to mass formula options",
            Description = "Scope: read|pncsettings",
            OperationId = "PriceToMassOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PriceToMassOptions))]
        public async Task<IActionResult> PriceToMassOptions()
        {
            return Ok(await facade.PriceToMassOptions(TenantId, SiteKey));
        }

        [HttpPatch("pnc/settings/PriceToMassOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Update the price to mass formula options",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpsertPriceToMassOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpsertPriceToMassOptions([FromBody] PriceToMassOptions options)
        {
            if (SiteKey != "*")
            {
                var policyResponse = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.PriceToMass);
                if (policyResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has price to mass management policy enabled.");
                }
            }

            var stausCode = await facade.UpsertPriceToMassOptions(TenantId, SiteKey, TokenUser, new PriceToMassOptions(options?.Expression, options?.Active ?? false), TokenUser);

            return StatusCode((int)stausCode);
        }

        [HttpGet("pnc/settings/ConfirmExpirationDateOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the confirm expiration date options",
            Description = "Scope: read|pncsettings",
            OperationId = "ConfirmExpirationDateOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ConfirmExpirationDateOptions))]
        [Obsolete]
        public async Task<IActionResult> ConfirmExpirationDateOptions()
        {
            return Ok(new ConfirmExpirationDateOptions(false));
        }

        [HttpGet("pnc/settings/OverrideExpirationDateOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the override expiration date options",
            Description = "Scope: read|pncsettings",
            OperationId = "OverrideExpirationDateOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OverrideExpirationDateOptions))]
        public async Task<IActionResult> OverrideExpirationDateOptions()
        {
            return Ok(await facade.OverrideExpirationDateOptions(TenantId, SiteKey));
        }

        [HttpPut("pnc/settings/OverrideExpirationDateOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Update the override expiration date options",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpsertOverrideExpirationDateOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateOverrideExpirationDateOptions([FromBody] OverrideExpirationDateOptionsUpdateRequest request)
        {
            if (SiteKey != "*")
            {
                var policyResponse = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.ConfirmExpirationDate);
                if (policyResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has price to mass management policy enabled.");
                }
            }

            var stausCode = await facade.UpsertOverrideExpirationDateOptions(TenantId, SiteKey, TokenUser, request, TokenUser);

            return StatusCode((int)stausCode);
        }

        [HttpGet("pnc/settings/RegisterLoadCarriersOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the register load carrier options",
            Description = "Scope: read|pncsettings",
            OperationId = "RegisterLoadCarriersOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RegisterLoadCarriersOptions))]
        public async Task<IActionResult> GetRegisterLoadCarriersOptions()
        {
            return Ok(await facade.GetRegisterLoadCarriersOptions(TenantId, SiteKey));
        }

        [HttpPut("pnc/settings/RegisterLoadCarriersOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Update the register load carrier options",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpsertRegisterLoadCarriersOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateRegisterLoadCarriersOptions([FromBody] RegisterLoadCarriersOptionsUpdateRequest request)
        {
            if (SiteKey != "*")
            {
                var policyResponse = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.RegisterLoadCarriers);
                if (policyResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has register load carriers management policy enabled.");
                }
            }

            var stausCode = await facade.UpdateRegisterLoadCarriersOptions(TenantId, SiteKey, request, TokenUser);

            return StatusCode((int)stausCode);
        }

        [HttpGet("pnc/settings/RequireLoadCarrierOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the require loadcarrier options",
            Description = "Scope: read|pncsettings",
            OperationId = "RequireLoadCarrierOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RequireLoadCarrierOptions))]
        public async Task<IActionResult> GetRequireLoadCarrierOptions()
        {
            return Ok(await facade.GetRequireLoadCarrierOptions(TenantId, SiteKey));
        }

        [HttpPut("pnc/settings/RequireLoadCarrierOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Update the require load carrier options",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpsertRequireLoadCarrierOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateRequireLoadCarrierOptions([FromBody] RequireLoadCarrierOptionsUpdateRequest request)
        {
            if (SiteKey != "*")
            {
                var policy = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.RequireLoadCarrier);
                if (policy != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has require load carriers management policy enabled.");
                }
            }

            var statusCode = await facade.UpdateRequireLoadCarrierOptions(TenantId, SiteKey, request, TokenUser);

            return StatusCode((int)statusCode);
        }

        [HttpGet("pnc/settings/CancelOrderInWebOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the cancel order in web options",
            Description = "Scope: read|pncsettings",
            OperationId = "ReadCancelOrderInWebOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CancelOrderInWebOptions))]
        public async Task<IActionResult> CancelOrderInWebOptions()
        {
            return Ok(await facade.CancelOrderInWebOptions(TenantId));
        }

        [HttpPatch("pnc/settings/CancelOrderInWebOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Update the cancel order in web options",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpsertCancelOrderInWebOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CancelOrderInWeb([FromBody] CancelOrderInWebOptionsUpdateRequest options)
        {
            var opt = options ?? new CancelOrderInWebOptionsUpdateRequest();
            return StatusCode((int)await facade.UpsertCancelOrderInWebOptions(TenantId, TokenUser, opt));
        }

        [HttpGet("pnc/settings/OrderStatusOnDeviationsOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the Order Status on Deviations options",
            Description = "Scope: read|pncsettings",
            OperationId = "GetOrderStatusOnDeviationsOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrderStatusOnDeviationsOptions))]
        public async Task<IActionResult> GetOrderStatusOnDeviationsOptions()
        {
            return Ok(await facade.OrderStatusOnDeviationsOptions(TenantId, SiteKey));
        }

        [HttpPatch("pnc/settings/OrderStatusOnDeviationsOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Update the Order Status on Deviations options",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpdateOrderStatusOnDeviationsOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateOrderStatusOnDeviationsOptions([FromBody] OrderStatusOnDeviationsOptionsUpdateRequest request)
        {
            if (SiteKey != "*")
            {
                var policyResponse = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.OrderStatusOnDeviations);
                if (policyResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has Order Status on Deviations options management policy enabled.");
                }
            }

            return StatusCode((int)await facade.UpdateOrderStatusOnDeviationsOptions(TenantId, SiteKey, TokenUser, request));
        }

        [HttpGet("pnc/settings/OverrideCustomerSubstitutionSettingOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the Override Customer Substitution Setting options",
            Description = "Scope: read|pncsettings",
            OperationId = "GetOverrideCustomerSubstitutionSettingOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OverrideCustomerSubstitutionSettingOptions))]
        public async Task<IActionResult> GetOverrideCustomerSubstitutionSettingOptions()
        {
            return Ok(await facade.OverrideCustomerSubstitutionSettingOptions(TenantId, SiteKey));
        }

        [HttpPatch("pnc/settings/OverrideCustomerSubstitutionSettingOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Update the Override Customer Substitution Setting options",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpdateOverrideCustomerSubstitutionSettingOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateOverrideCustomerSubstitutionSettingOptions([FromBody] OverrideCustomerSubstitutionSettingOptionsUpdateRequest request)
        {
            if (SiteKey != "*")
            {
                var policyResponse = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.AllowOverrideCustomerSubstitutionSetting);
                if (policyResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has Override Customer Substitution Setting options management policy enabled.");
                }
            }

            return StatusCode((int)await facade.UpdateOverrideCustomerSubstitutionSettingOptions(TenantId, SiteKey, TokenUser, request));
        }

        [HttpGet("pnc/settings/RequireCompileOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the Compile Mandatory Options",
            Description = "Scope: read|pncsettings",
            OperationId = "GetRequireCompileOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RequireCompileOptions))]
        public async Task<IActionResult> GetRequireCompileOptions()
        {
            return Ok(await facade.GetRequireCompileOptions(TenantId, SiteKey));
        }

        [HttpPut("pnc/settings/RequireCompileOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Update the Compile Mandatory Options",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpdateRequireCompileOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateRequireCompileOptions([FromBody] RequireCompileOptionsUpdateRequest request)
        {
            if (SiteKey != "*")
            {
                var policyResponse = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.RequireCompile);
                if (policyResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has Override Compile Mandatory Setting options management policy enabled.");
                }
            }

            return StatusCode((int)await facade.UpdateRequireCompileOptions(TenantId, SiteKey, request, TokenUser));
        }

        [HttpGet("pnc/settings/CustomProductLocationOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the Custom Product Location Options",
            Description = "Scope: read|pncsettings",
            OperationId = "GetCustomProductLocationOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomProductLocationOptions))]
        public async Task<IActionResult> GetCustomProductLocationOptions()
        {
            return Ok(await facade.GetCustomProductLocationOptions(TenantId, SiteKey));
        }

        [HttpPut("pnc/settings/CustomProductLocationOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Update the Custom Product Location Options",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpdateCustomProductLocationOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateCustomProductLocationOptions([FromBody] CustomProductLocationOptionsUpdateRequest request)
        {
            if (SiteKey != "*")
            {
                var policyResponse = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.CustomProductLocations);
                if (policyResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has Override Compile Mandatory Setting options management policy enabled.");
                }
            }

            return StatusCode((int)await facade.UpdateCustomProductLocationOptions(TenantId, SiteKey, TokenUser, request));
        }

        [HttpGet("pnc/settings/ExternalCarrierIdOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the External carrier identifier options",
            Description = "Scope: read|pncsettings",
            OperationId = "GetExternalCarrierIdOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ExternalCarrierIdOptions))]
        public async Task<IActionResult> GetExternalCarrierIdOptions()
        {
            return Ok(await facade.GetExternalCarrierIdOptions(TenantId, SiteKey));
        }

        [HttpPut("pnc/settings/ExternalCarrierIdOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Upsert External carrier identifier options",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpsertExternalCarrierIdOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpsertExternalCarrierIdOptions([FromBody] ExternalCarrierIdOptionsUpdateRequest request)
        {
            HttpStatusCode result;
            if (SiteKey != "*")
            {
                var policyResponse = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.ExternalCarrierId);
                if (policyResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has Override External carrier id options management policy enabled.");
                }
                result = await facade.UpsertSiteExternalCarrierIdOptions(TenantId, SiteKey, TokenUser, request);
            }
            else
            {
                result = await facade.UpsertTenantExternalCarrierIdOptions(TenantId, TokenUser, request);
            }

            return StatusCode((int)result);
        }

        [HttpGet("pnc/settings/TrolleyLocationsOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the Trolley locations options",
            Description = "Scope: read|pncsettings",
            OperationId = "GetTrolleyLocationsOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(TrolleyLocationsOptions))]
        public async Task<IActionResult> GetTrolleyLocationsOptions()
        {
            return Ok(await facade.GetTrolleyLocationsOptions(TenantId, SiteKey));
        }

        [HttpPut("pnc/settings/TrolleyLocationsOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
            Summary = "Upsert Trolley locations options",
            Description = "Scope: manage|pncsettings",
            OperationId = "UpsertTrolleyLocationsOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpsertTolleyLocationsOptions([FromBody] TrolleyLocationsOptionsUpsertRequest request)
        {
            HttpStatusCode result;
            if (SiteKey != "*")
            {
                var policyResponse = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.TrolleyLocations);
                if (policyResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has Override Trolley locations options management policy enabled.");
                }
                result = await facade.UpsertSiteTrolleyLocationsOptions(TenantId, SiteKey, TokenUser, request);
            }
            else
            {
                result = await facade.UpsertTenantTrolleyLocationsOptions(TenantId, TokenUser, request);
            }

            return StatusCode((int)result);
        }

        [HttpGet("pnc/settings/LegacyLocationBarcodeSupportOptions")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
          Summary = "Get the legacy location barcode support options",
          Description = "Scope: read|pncsettings",
          OperationId = "GetLegacyLocationBarcodeSupportOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LegacyLocationBarcodeSupportOptions))]
        public async Task<IActionResult> GetLegacyLocationBarcodeSupportOptions()
        {
            return Ok(await facade.GetLegacyLocationBarcodeSupportOptions(TenantId));
        }

        [HttpPut("pnc/settings/LegacyLocationBarcodeSupportOptions")]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        [SwaggerOperation(
           Summary = "Update legacy location barcode support options",
           Description = "Scope: manage|pncsettings",
           OperationId = "UpdateLegacyLocationBarcodeSupportOptions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateLegacyLocationBarcodeSupportOptions([FromBody] UpdateLegacyLocationBarcodeSupportOptionsRequest request)
        {
            return StatusCode((int)await facade.UpdateLegacyLocationBarcodeSupportOptions(TenantId, TokenUser, request));
        }

        [HttpGet("pnc/settings/labels")]
        [SwaggerOperation(
            Summary = "Get all default LabelTemplates in Project",
            Description = "")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<LabelTemplate>))]
        public async Task<IActionResult> GetDefaultLabelTemplates()
        {
            return Ok(await facade.GetDefaultLabelTemplates());
        }

        [HttpPost("pnc/settings/labels/preview")]
        [SwaggerOperation(
            Summary = "Preview Label from ZPL",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(LabelPreviewResponse))]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetLabelPreview([Required][FromBody] LabelPreviewRequest labelPreviewRequest)
        {
            return Ok(await facade.GetLabelPreview(labelPreviewRequest));
        }

        [HttpGet("pnc/settings/labels/selected")]
        [SwaggerOperation(
            Summary = "Get LabelTemplate of Type in current tenant and site context",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<LabelTemplate>))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetSelectedLabelTemplates([FromQuery] LabelType? type = default)
        {
            return Ok(await facade.GetSelectedLabelTemplates(TenantId, type, SiteKey));
        }

        [HttpPut("pnc/settings/labels/selected")]
        [SwaggerOperation(
            Summary = "Update LabelTemplate of Type in current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> UpdateLabelTemplate([Required][FromBody] LabelTemplate labelTemplate)
        {
            return StatusCode((int)await facade.UpdateLabelTemplate(TenantId, labelTemplate, TokenUser, SiteKey));
        }

        [HttpDelete("pnc/settings/labels/selected/{labelId}")]
        [SwaggerOperation(
            Summary = "Delete LabelTemplate of Type in current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> DeleteLabelTemplate([Required][FromRoute] string labelId)
        {
            return StatusCode((int)await facade.DeleteLabelTemplate(TenantId, labelId, TokenUser, SiteKey));
        }

        [HttpGet("pnc/settings/printers")]
        [SwaggerOperation(
            Summary = "Get all printers in current tenant and site context",
            Description = "")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Printer>))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetPrinters()
        {
            return Ok(await facade.GetPrinters(TenantId, SiteKey));
        }

        [HttpGet("pnc/settings/printers/{id}")]
        [SwaggerOperation(
            Summary = "Get printer by id",
            Description = "")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Printer))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetPrinter([FromRoute] string id)
        {
            return Ok(await facade.GetPrinter(TenantId, SiteKey, id));
        }

        [HttpPut("pnc/settings/printers/{id}")]
        [SwaggerOperation(
            Summary = "Upsert a printer",
            Description = "")]
        [SwaggerResponse((int)HttpStatusCode.Accepted)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> UpsertPrinter([FromRoute] string id, [FromBody] Printer printer)
        {
            if (SiteKey != "*")
            {
                var policyResponse = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.Printers);
                if (policyResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has printer management policy enabled.");
                }
            }

            return StatusCode((int)await facade.UpsertPrinter(TenantId, SiteKey, id, printer, TokenUser));
        }

        [HttpDelete("pnc/settings/printers/{id}")]
        [SwaggerOperation(
            Summary = "Delete a printer",
            Description = "")]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> DeletePrinter([FromRoute] string id)
        {
            if (SiteKey != "*")
            {
                var policyResponse = await tenantFacade.GetManagementpolicy(TenantId, ManagementPolicyEntityType.Printers);
                if (policyResponse != null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, "Tenant has printer management policy enabled.");
                }
            }

            await facade.DeletePrinter(TenantId, SiteKey, id, TokenUser);
            return NoContent();
        }


        [HttpGet("pnc/settings/appevents")]
        [SwaggerOperation(
            Summary = "Get app event settings",
            Description = "")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(AppEventOption))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetAppEventOption()
        {
            return Ok(await facade.GetAppEventOption(TenantId));
        }


        [HttpGet("pnc/reasons/substitutions")]
        [SwaggerOperation(
            Summary = "List all Substitution Reasons in PnC module")]
        [SwaggerResponse(
            (int)HttpStatusCode.OK,
            Type = typeof(IEnumerable<ProjectSubstitutionReason>))]
        //[ApiPermissionFilterFactory("read", "tenant")] - todo: Is this needed when this is a "system" resource?
        public async Task<IActionResult> GetSubstitutionReasons(string culture = "en")
        {
            return Ok(await facade.GetSubstitutionReasons(culture));
        }

        [HttpGet("pnc/reasons/substitutions/selected")]
        [SwaggerOperation(
            Summary = "Get selected Substitution Reasons for PnC module in the current tenant and site context",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse(
            (int)HttpStatusCode.OK,
            Type = typeof(IEnumerable<SubstitutionReason>))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetSelectedSubstitutionReasons(string culture = "en")
        {
            return Ok(await facade.GetSelectedSubstitutionReasons(TenantId, SiteKey, culture));
        }

        [HttpPatch("pnc/reasons/substitutions/selected")]
        [SwaggerOperation(
            Summary = "Update selected Substitution Reasons for PnC module in the current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.Accepted,
            Type = typeof(bool))]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> UpdateSubstitutionReasons([FromBody] IEnumerable<SubstitutionReason> substitutionReasons)
        {
            return Accepted(await facade.UpdateSelectedSubstitutionReasons(TokenUser, TenantId, substitutionReasons, SiteKey));
        }

        [HttpDelete("pnc/reasons/substitutions/{code}")]
        [SwaggerOperation(
            Summary = "Delete selected Substitution Reason for PnC module in the current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        // TODO: What HttpStatusCode is returned for succesful deletion?
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> DeleteSelectedSubstitutionReason(string code)
        {
            var response = await facade.DeleteSelectedSubstitutionReason(TokenUser, TenantId, code, SiteKey);
            return StatusCode((int)response);
        }

        [HttpGet("pnc/reasons/deviations")]
        [SwaggerOperation(
            Summary = "List all Deviation Reasons in PnC module")]
        [SwaggerResponse(
            (int)HttpStatusCode.OK,
            Type = typeof(IEnumerable<ProjectPickingDeviationReason>))]
        //[ApiPermissionFilterFactory("read", "tenant")] - todo: Is this needed when this is a "system" resource?
        public async Task<IActionResult> GetPickingDeviationReasons(string culture = "en")
        {
            return Ok(await facade.GetPickingDeviationReasons(culture));
        }

        [HttpGet("pnc/reasons/deviations/selected")]
        [SwaggerOperation(
            Summary = "Get selected Deviation Reasons for PnC module in the current tenant and site context",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse(
            (int)HttpStatusCode.OK,
            Type = typeof(IEnumerable<PickingDeviationReason>))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetSelectedDeviationReasons(string culture = "en")
        {
            return Ok(await facade.GetSelectedDeviationReasons(TenantId, SiteKey, culture));
        }

        [HttpPatch("pnc/reasons/deviations/selected")]
        [SwaggerOperation(
            Summary = "Update selected Deviation Reasons for PnC module in the current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse(
            (int)HttpStatusCode.Accepted,
            Type = typeof(bool))]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> UpdatePickingDeviationReasons([FromBody] IEnumerable<PickingDeviationReason> pickingDeviationReasons)
        {
            return Accepted(await facade.UpdateSelectedDeviationReasons(TokenUser, TenantId, pickingDeviationReasons, SiteKey));
        }

        [HttpDelete("pnc/reasons/deviations/{code}")]
        [SwaggerOperation(
            Summary = "Delete selected Picking Deviation Reason for PnC module in the current tenant and site context",
            Description = "Scope: manage|pncsettings")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [ApiPermissionFilterFactory("manage", "pncsettings")]
        public async Task<IActionResult> DeleteSelectedDeviationReason(string code)
        {
            var response = await facade.DeleteSelectedDeviationReason(TokenUser, TenantId, code, SiteKey);
            return StatusCode((int)response);
        }

        [Obsolete("This is here for backwards compability from APP version 1.4.6, use magic button options")]
        [HttpGet("pnc/strategies/selection/selected")]
        [SwaggerOperation(
            Summary = "Get selected Selection Strategy for PnC module in the current tenant and site context",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse(
            (int)HttpStatusCode.OK,
            Type = typeof(SelectedStrategy))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetSelectedSelectionStrategy()
        {
            return Ok(await Task.FromResult(new SelectedStrategy("NotInUse", new Dictionary<string, object>())));
        }

        [Obsolete("This is here for backwards compability from APP version 1.4.6, use magic button options")]
        [HttpPatch("pnc/strategies/selection/selected")]
        [HttpGet("pnc/strategies/optimization/selected")]
        [SwaggerOperation(
            Summary = "Get selected Optimization Strategy for PnC module in the current tenant and site context",
            Description = "Scope: read|pncsettings")]
        [SwaggerResponse(
            (int)HttpStatusCode.OK,
            Type = typeof(SelectedStrategy))]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        public async Task<IActionResult> GetSelectedOptimizationStrategy()
        {
            return Ok(await Task.FromResult(new SelectedStrategy("TrolleyOptimizationStrategy", new Dictionary<string, object>())));
        }

        [Obsolete]
        [HttpGet("pnc/settings/PickResultQRCodeSettings")]
        [ApiPermissionFilterFactory("read", "pncsettings")]
        [SwaggerOperation(
            Summary = "Get the pick result QR code settings",
            Description = "Scope: read|pncsettings",
            OperationId = "ReadPickResultQRCodeSettings")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PickResultQRCodeSettings))]
        [ResponseCache(CacheProfileName = CachingProfileName.NoStore)]
        public async Task<IActionResult> PickResultQRCodeSettings()
        {
            var active = await tenantFacade.GetPosIntegrationActiveStatus(TenantId, SiteKey);

            var result = new PickResultQRCodeSettings("", "", "", "", "", 0, 0, 0, 0, 0, active);

            return Ok(result);
        }
    }
}
