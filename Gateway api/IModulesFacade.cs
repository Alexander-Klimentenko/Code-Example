using AutoMapper;
using Core.Interfaces;
using Gateway.Core.Commands;
using Gateway.Core.Commands.Site.Reasons;
using Gateway.Core.Commands.Site.Settings;
using Gateway.Core.Commands.Tenant;
using Gateway.Core.Commands.Tenant.Reasons;
using Gateway.Core.Commands.Tenant.Settings;
using Gateway.Core.Helpers;
using Gateway.Facades;
using Gateway.Shared.Models;
using Gateway.Shared.Models.PnC.Reasons;
using Gateway.Shared.Models.PnC.Settings;
using Gateway.Shared.Models.PnC.Settings.AppFeatures;
using Gateway.Shared.Models.PnC.Settings.AppUI;
using Gateway.Shared.Models.PnC.Settings.Optimization;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Publisher;
using Microsoft.Extensions.Logging;
using PncCore.Shared.Models.Settings.AppFeatures;
using PncCore.Shared.Models.Settings.AppUI;
using ProductCore.Shared.Interfaces;
using Project.Models.Shared.PncCore;
using Project.Models.Shared.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Gateway.Core.Commands.Site.Settings.Optimization;
using Gateway.Core.Commands.Tenant.Settings.Optimization;
using Gateway.Shared.Requests;
using PncCore.Shared.Models.Settings;
using PncCore.Shared.Models.Settings.Optimization;
using ProductCore.Shared.Models;
using AllowAddOrderLineOptions = Gateway.Shared.Models.AllowAddOrderLineOptions;
using AppFeatureOptions = Gateway.Shared.Models.PnC.Settings.AppFeatures.AppFeatureOptions;
using ApproveDeviationsOptions = Gateway.Shared.Models.ApproveDeviationsOptions;
using AppUIOptions = Gateway.Shared.Models.PnC.Settings.AppUI.AppUIOptions;
using CancelOrderInWebOptions = Gateway.Shared.Models.CancelOrderInWebOptions;
using CustomProductLocationOptions = Gateway.Shared.Models.PnC.Settings.CustomProductLocationOptions;
using DeliverableAutomationOptions = Gateway.Shared.Models.DeliverableAutomationOptions;
using ExternalCarrierIdOptions = Gateway.Shared.Models.PnC.Settings.ExternalCarrierIdOptions;
using TrolleyLocationsOptions = Gateway.Shared.Models.PnC.Settings.Optimization.TrolleyLocationsOptions;
using ForcePickAllLinesOptions = Gateway.Shared.Models.ForcePickAllLinesOptions;
using ForcePrintWeightLabelOptions = Gateway.Shared.Models.ForcePrintWeightLabelOptions;
using LabelTemplate = Gateway.Shared.Models.PnC.Settings.LabelTemplate;
using MagicButtonOptions = Gateway.Shared.Models.PnC.Settings.MagicButtonOptions;
using ManualPickingMode = Gateway.Shared.Models.PnC.Settings.ManualPickingMode;
using ManualPickingOptions = Gateway.Shared.Models.PnC.Settings.ManualPickingOptions;
using OrderNotificationOptions = Gateway.Shared.Models.OrderNotificationOptions;
using OrderStateTransitionRules = Gateway.Shared.Models.OrderStateTransitionRules;
using OrderStatusOnDeviationsOptions = Gateway.Shared.Models.PnC.Settings.OrderStatusOnDeviationsOptions;
using OverrideCustomerSubstitutionSettingOptions = Gateway.Shared.Models.PnC.Settings.OverrideCustomerSubstitutionSettingOptions;
using OverrideExpirationDateOptions = Gateway.Shared.Models.PnC.Settings.OverrideExpirationDateOptions;
using PickingThresholdOptions = Gateway.Shared.Models.PnC.Settings.PickingThresholdOptions;
using PickSlotOptions = Gateway.Shared.Models.PnC.Settings.Optimization.PickSlotOptions;
using PriceToMassOptions = Gateway.Shared.Models.PriceToMassOptions;
using Printer = Gateway.Shared.Models.PnC.Settings.Printer;
using QualityPickingRules = Gateway.Shared.Models.PnC.Settings.QualityPickingRules;
using RegisterLoadCarriersOptions = Gateway.Shared.Models.RegisterLoadCarriersOptions;
using RequireCompileOptions = Gateway.Shared.Models.PnC.Settings.RequireCompileOptions;
using RequireLoadCarrierOptions = Gateway.Shared.Models.PnC.Settings.RequireLoadCarrierOptions;
using ShowCustomerInfoOptions = Gateway.Shared.Models.ShowCustomerInfoOptions;
using SubstitutionLimitationsOptions = Gateway.Shared.Models.SubstitutionLimitationsOptions;
using SubstitutionRuleOptions = Gateway.Shared.Models.SubstitutionRuleOptions;
using LegacyLocationBarcodeSupportOptions = Gateway.Shared.Models.PnC.Settings.Optimization.LegacyLocationBarcodeSupportOptions;

namespace Api.Facades
{
    public interface IModulesFacade
    {
        Task<IEnumerable<Module>> GetModules();

        Task<MagicButtonOptions> MagicBtnOptions(string tenantId, string siteKey, CancellationToken cancellationToken);

        Task<HttpStatusCode> UpsertMagicBtnOptions(string tenantId, string siteKey, MagicButtonOptionsUpdateRequest request, string tokenUser);

        Task<PickSlotOptions> GetPickSlotOptions(string tenantId, string siteKey = null);
        Task<Gateway.Shared.Models.PnC.Settings.Optimization.PicklistOptions> GetPicklistOptions(string tenantId, string siteKey = null);

        Task<HttpStatusCode> UpdatePickSlotOptions(string issuer, string tenantId, PickSlotOptionsUpdateRequest request, string siteKey = null);
        Task<HttpStatusCode> UpdatePicklistOptions(string issuer, string tenantId, PicklistOptionsUpdateRequest request, string siteKey = null);

        Task<QualityPickingRules> GetQualityPickingRules(string tenantId, string siteKey = null);

        Task<HttpStatusCode> UpdateQualityPickingRules(string issuer, string tenantId, QualityPickingRulesUpdateRequest request, string siteKey = null);

        Task<AppUIOptions> GetAppUIOptions(string tenantId, string? siteKey = null);

        Task<HttpStatusCode> UpdateAppUIOptions(string issuer, string tenantId, AppUIOptionsUpdateRequest request, string siteKey = null);

        Task<PickingThresholdOptions> GetPickingThresholdOptions(string tenantId, string siteKey = null);

        Task<HttpStatusCode> UpdatePickingThresholdOptions(string issuer, string tenantId, PickingThresholdOptionsUpdateRequest request, string siteKey = null);

        Task<ManualPickingOptions> GetManualPickingOptions(string tenantId, string siteKey = null);

        Task<HttpStatusCode> UpdateManualPickingOptions(string issuer, string tenantId, ManualPickingOptionsUpdateRequest request, string siteKey = null);

        Task<IEnumerable<SubstitutionRuleOptions>> GetSubstitutionRuleOptions(string tenantId, string siteKey = null);

        Task<HttpStatusCode> UpsertProductSubstitutionRuleOptions(string issuer, string tenantId, IEnumerable<SubstitutionRuleOptionsUpdateRequest> request, string siteKey = null);

        Task<HttpStatusCode> DeleteAllProductSubstitutionRuleOptions(string issuer, string tenantId, string siteKey = null);

        Task<HttpStatusCode> UpsertTenantDeliverableAutomationOptions(string tenantId, string issuer, DeliverableAutomationOptionsUpdateRequest request);

        Task<HttpStatusCode> UpsertSiteDeliverableAutomationOptions(string tenantId, string siteKey, string issuer, DeliverableAutomationOptionsUpdateRequest request);

        Task<DeliverableAutomationOptions> GetDeliverableAutomationOptions(string tenantId, string siteKey);

        Task<HttpStatusCode> UpdateAppFeatureOptions(string tenantId, string issuer, AppFeatureOptionsUpdateRequest request, string siteKey = null);

        Task<AppFeatureOptions> GetAppFeatureOptions(string tenantId, string siteKey = null);

        Task<OrderStateTransitionRules> GetOrderStateTransitionRules(string tenantId);

        Task<HttpStatusCode> UpsertOrderStateTransitionRules(string tenantId, string issuer, OrderStateTransitionRulesUpdateRequest request);

        Task<AllowAddOrderLineOptions> GetAllowAddOrderLineOptions(string tenantId);

        Task<HttpStatusCode> UpsertAllowAddOrderLineOptions(string tenantId, string issuer, AllowAddOrderLineOptionsUpdateRequest request);

        Task<IEnumerable<LabelTemplate>> GetDefaultLabelTemplates();

        Task<LabelPreviewResponse> GetLabelPreview(LabelPreviewRequest labelPreviewRequest);

        Task<IEnumerable<LabelTemplate>> GetSelectedLabelTemplates(string tenantId, LabelType? type = default, string siteKey = default);

        Task<HttpStatusCode> UpdateLabelTemplate(string tenantId, LabelTemplate labelTemplate, string issuer, string siteKey = default);

        Task<HttpStatusCode> DeleteLabelTemplate(string tenantId, string labelId, string issuer, string siteKey = default);

        Task<IEnumerable<ProjectSubstitutionReason>> GetSubstitutionReasons(string culture);

        Task<IEnumerable<SubstitutionReason>> GetSelectedSubstitutionReasons(string tenantId, string siteKey = null, string culture = "en");

        Task<bool> UpdateSelectedSubstitutionReasons(string issuer, string tenantId, IEnumerable<SubstitutionReason> reasons, string siteKey = null);

        Task<HttpStatusCode> DeleteSelectedSubstitutionReason(string issuer, string tenantId, string code, string siteKey = null);

        Task<IEnumerable<ProjectPickingDeviationReason>> GetPickingDeviationReasons(string culture);

        Task<IEnumerable<PickingDeviationReason>> GetSelectedDeviationReasons(string tenantId, string siteKey = null, string culture = "en");

        Task<bool> UpdateSelectedDeviationReasons(string issuer, string tenantId, IEnumerable<PickingDeviationReason> reasons, string siteKey = null);

        Task<HttpStatusCode> DeleteSelectedDeviationReason(string issuer, string tenantId, string code, string siteKey = null);

        Task<IEnumerable<Printer>> GetPrinters(string tenantId, string siteKey);

        Task<Printer> GetPrinter(string tenantId, string siteKey, string id);

        Task<HttpStatusCode> UpsertPrinter(string tenantId, string siteKey, string id, Printer printer, string userId);

        Task DeletePrinter(string tenantId, string siteKey, string id, string userId);

        Task<OrderNotificationOptions> ReadOrderNotificationOptions(string tenantId, string siteKey);

        Task<HttpStatusCode> UpsertOrderNotificationOptions(string tenantId, string siteKey, string tokenUser, OrderNotificationOptions options);

        Task<SubstitutionLimitationsOptions> SubstitutionLimitationsOptions(string tenantId, string siteKey);

        Task<HttpStatusCode> UpsertSubstitutionLimitationsOptions(string tenantId, string siteKey, string issuer, SubstitutionLimitationsOptionsUpdateRequest updateRequest);

        Task<ForcePrintWeightLabelOptions> ForcePrintWeightLabelOptions(string tenantId, string siteKey);

        Task<HttpStatusCode> UpsertForcePrintWeightLabelOptions(string tenantId, string siteKey, string issuer, ForcePrintWeightLabelOptionsUpdateRequest request);

        Task<OrderStatusOnDeviationsOptions> OrderStatusOnDeviationsOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<HttpStatusCode> UpdateOrderStatusOnDeviationsOptions(string tenantid, string siteKey, string issuer, OrderStatusOnDeviationsOptionsUpdateRequest request);

        Task<OverrideCustomerSubstitutionSettingOptions> OverrideCustomerSubstitutionSettingOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<HttpStatusCode> UpdateOverrideCustomerSubstitutionSettingOptions(string tenantid, string siteKey, string issuer, OverrideCustomerSubstitutionSettingOptionsUpdateRequest request);

        Task<ShowCustomerInfoOptions> ShowCustomerInfoOptions(string tenantId, string siteKey);

        Task<HttpStatusCode> UpsertShowCustomerInfoOptions(string tenantId, string siteKey, string issuer, ShowCustomerInfoOptionsUpdateRequest request);

        Task<ForcePickAllLinesOptions> ForcePickAllLinesOptions(string tenantId, string siteKey);

        Task<HttpStatusCode> UpsertForcePickAllLinesOptions(string tenantId, string siteKey, string issuer, ForcePickAllLinesOptionsUpdateRequest request);

        Task<CancelOrderInWebOptions> CancelOrderInWebOptions(string tenantId);

        Task<HttpStatusCode> UpsertCancelOrderInWebOptions(string tenantId, string issuer, CancelOrderInWebOptionsUpdateRequest request);

        Task<ApproveDeviationsOptions> GetApproveDeviationsOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<HttpStatusCode> UpdateApproveDeviationsOptions(string tenantId, string siteKey, ApproveDeviationsOptionsUpdateRequest request, string tokenUser);

        Task<PriceToMassOptions> PriceToMassOptions(string tenantId, string siteKey);

        Task<HttpStatusCode> UpsertPriceToMassOptions(string tenantId, string siteKey, string tokenUser, PriceToMassOptions opt, string issuer);

        Task<AppEventOption> GetAppEventOption(string tenantId);

        Task<OverrideExpirationDateOptions> OverrideExpirationDateOptions(string tenantId, string siteKey);

        Task<HttpStatusCode> UpsertOverrideExpirationDateOptions(string tenantId, string siteKey, string tokenUser, OverrideExpirationDateOptionsUpdateRequest request, string issuer);

        Task<RegisterLoadCarriersOptions?> GetRegisterLoadCarriersOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<HttpStatusCode> UpdateRegisterLoadCarriersOptions(string tenantId, string siteKey, RegisterLoadCarriersOptionsUpdateRequest request, string tokenUser);

        Task<RequireLoadCarrierOptions> GetRequireLoadCarrierOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<HttpStatusCode> UpdateRequireLoadCarrierOptions(string tenantId, string siteKey, RequireLoadCarrierOptionsUpdateRequest request, string tokenUser);

        Task<RequireCompileOptions> GetRequireCompileOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<HttpStatusCode> UpdateRequireCompileOptions(string tenantId, string siteKey, RequireCompileOptionsUpdateRequest request, string tokenUser);

        Task<CustomProductLocationOptions> GetCustomProductLocationOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<HttpStatusCode> UpdateCustomProductLocationOptions(string tenantId, string siteKey, string tokenUser, CustomProductLocationOptionsUpdateRequest request);

        Task<ExternalCarrierIdOptions> GetExternalCarrierIdOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<HttpStatusCode> UpsertSiteExternalCarrierIdOptions(string tenantId, string siteKey, string tokenUser, ExternalCarrierIdOptionsUpdateRequest request);

        Task<HttpStatusCode> UpsertTenantExternalCarrierIdOptions(string tenantId, string tokenUser, ExternalCarrierIdOptionsUpdateRequest request);

        Task<TrolleyLocationsOptions> GetTrolleyLocationsOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<HttpStatusCode> UpsertSiteTrolleyLocationsOptions(string tenantId, string siteKey, string tokenUser, TrolleyLocationsOptionsUpsertRequest request);

        Task<HttpStatusCode> UpsertTenantTrolleyLocationsOptions(string tenantId, string tokenUser, TrolleyLocationsOptionsUpsertRequest request);

        Task<LegacyLocationBarcodeSupportOptions> GetLegacyLocationBarcodeSupportOptions(string tenantId, CancellationToken cancellationToken = default);

        Task<HttpStatusCode> UpdateLegacyLocationBarcodeSupportOptions(string tenantId, string tokenUser, UpdateLegacyLocationBarcodeSupportOptionsRequest request);
    }

    public class ModulesFacade : CommandFacade, IModulesFacade
    {
        private const string AllSiteKey = "*";

        private readonly IMapper mapper;
        private readonly IMapperHelper mapperHelper;

        private readonly ITaskCommandSynchronizer taskCommandSynchronizer;

        private readonly PncCore.Shared.Interfaces.IClient pncClient;
        private readonly IClient productClient;
        private readonly IUsersFacade usersFacade;
        private readonly Operation.Service.Shared.Interfaces.IClient operationsClient;
        private readonly Tenant.Service.Shared.Interfaces.IClient tenantClient;

        private readonly ILabelPreviewClient labelPreviewClient;

        public ModulesFacade(
            ILogger<ModulesFacade> logger,
            IMapper mapper,
            IPublisher publisher,
            ITaskCommandSynchronizer taskCommandSynchronizer,
            PncCore.Shared.Interfaces.IClient pncClient,
            IClient productClient,
            Operation.Service.Shared.Interfaces.IClient operationsClient,
            Tenant.Service.Shared.Interfaces.IClient tenantClient,
            ILabelPreviewClient labelPreviewClient,
            IMapperHelper mapperHelper, IUsersFacade usersFacade)
            : base(publisher, logger)
        {
            this.mapper = mapper;
            this.mapperHelper = mapperHelper;
            this.usersFacade = usersFacade;
            this.taskCommandSynchronizer = taskCommandSynchronizer;
            this.pncClient = pncClient;
            this.productClient = productClient;
            this.operationsClient = operationsClient;
            this.tenantClient = tenantClient;
            this.labelPreviewClient = labelPreviewClient;
        }

        public async Task<IEnumerable<Module>> GetModules()
        {
            var response = await operationsClient.GetModules();
            return response.Failed || !response.Data.Any()
                ? Array.Empty<Module>()
                : mapper.Map<IEnumerable<Module>>(response.Data);
        }

        public async Task<ApproveDeviationsOptions?> GetApproveDeviationsOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey == null)
            {
                var result = await pncClient.GetApproveDeviationsOptions(tenantId, siteKey, cancellationToken);
                return result.Failed
                    ? default
                    : new ApproveDeviationsOptions(
                        result.Data.ApproveDeviations,
                        result.Data.ApproveDeviationsOptional,
                        result.Data.ApproveSubstitutions,
                        result.Data.ApproveSubstitutionsOptional,
                        result.Data.AllowForceOfDeviations,
                        result.Data.AllowForceOfSubstitutions,
                        result.Data.UpdatedBy,
                        result.Data.UpdatedUtc);
            }

            var approveDeviationsPolicyTask = tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.ApproveDeviations);
            var approveSubstitutionsPolicyTask = tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.ApproveSubstitutions);
            var allowForceOfDeviationsPolicyTask = tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.AllowForceOfDeviations);
            var allowForceOfSubstitutionsPolicyTask = tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.AllowForceOfSubstitutions);
            await Task.WhenAll(new Task[]
            {
                approveDeviationsPolicyTask,
                approveSubstitutionsPolicyTask,
                allowForceOfDeviationsPolicyTask,
                allowForceOfSubstitutionsPolicyTask,
            });

            var approveDeviationsIsOverridable = approveDeviationsPolicyTask.IsFaulted || approveDeviationsPolicyTask.Result.Failed || approveDeviationsPolicyTask.Result.Data == null;
            var approveSubstitutionsIsOverridable = approveSubstitutionsPolicyTask.IsFaulted || approveSubstitutionsPolicyTask.Result.Failed || approveSubstitutionsPolicyTask.Result.Data == null;
            var allowForceOfDeviationsIsOverridable = allowForceOfDeviationsPolicyTask.IsFaulted || allowForceOfDeviationsPolicyTask.Result.Failed || allowForceOfDeviationsPolicyTask.Result.Data == null;
            var allowForceOfSubstitutionsIsOverridable = allowForceOfSubstitutionsPolicyTask.IsFaulted || allowForceOfSubstitutionsPolicyTask.Result.Failed || allowForceOfSubstitutionsPolicyTask.Result.Data == null;

            PncCore.Shared.Models.Settings.ApproveDeviationsOptions tenantOptions = null;
            var shouldFetchTenantSettings = !approveDeviationsIsOverridable || !approveSubstitutionsIsOverridable || !allowForceOfDeviationsIsOverridable || !allowForceOfSubstitutionsIsOverridable;
            if (shouldFetchTenantSettings)
            {
                var tenantOptionsResponse = await pncClient.GetApproveDeviationsOptions(tenantId, null, cancellationToken);
                if (!tenantOptionsResponse.Failed)
                    tenantOptions = tenantOptionsResponse.Data;
            }

            PncCore.Shared.Models.Settings.ApproveDeviationsOptions siteOptions = null;
            var shouldFetchSiteSettings = approveDeviationsIsOverridable || approveSubstitutionsIsOverridable || allowForceOfDeviationsIsOverridable || allowForceOfSubstitutionsIsOverridable;
            if (shouldFetchSiteSettings)
            {
                var siteOptionsResponse = await pncClient.GetApproveDeviationsOptions(tenantId, siteKey, cancellationToken);
                if (!siteOptionsResponse.Failed)
                    siteOptions = siteOptionsResponse.Data;
            }

            if (tenantOptions == null && siteOptions == null)
                return default;

            var approveDeviations = approveDeviationsIsOverridable ? siteOptions?.ApproveDeviations : tenantOptions?.ApproveDeviations;
            var approveDeviationsOptional = approveDeviationsIsOverridable ? siteOptions?.ApproveDeviationsOptional : tenantOptions?.ApproveDeviationsOptional;

            var approveSubstitutions = approveSubstitutionsIsOverridable ? siteOptions?.ApproveSubstitutions : tenantOptions?.ApproveSubstitutions;
            var approveSubstitutionsOptional = approveSubstitutionsIsOverridable ? siteOptions?.ApproveSubstitutionsOptional : tenantOptions?.ApproveSubstitutionsOptional;

            var allowForceOfDeviations = allowForceOfDeviationsIsOverridable ? siteOptions?.AllowForceOfDeviations : tenantOptions?.AllowForceOfDeviations;
            var allowForceOfSubstitutions = allowForceOfSubstitutionsIsOverridable ? siteOptions?.AllowForceOfSubstitutions : tenantOptions?.AllowForceOfSubstitutions;

            return new ApproveDeviationsOptions(
                approveDeviations ?? default,
                approveDeviationsOptional,
                approveSubstitutions ?? default,
                approveSubstitutionsOptional,
                allowForceOfDeviations ?? default,
                allowForceOfSubstitutions ?? default,
                siteOptions?.UpdatedBy,
                siteOptions?.UpdatedUtc);
        }

        public async Task<HttpStatusCode> UpdateApproveDeviationsOptions(string tenantId, string siteKey, ApproveDeviationsOptionsUpdateRequest request, string tokenUser)
        {
            var opts = new ApproveDeviationsOptionsUpdateModel(
                request.ApproveDeviations,
                request.ApproveDeviationsOptional,
                request.ApproveSubstitutions,
                request.ApproveSubstitutionsOptional,
                request.AllowForceOfDeviations,
                request.AllowForceOfSubstitutions);

            ITaskCommand cmd;

            string session = tenantId;

            var userName = await usersFacade.ResolveUserIdToName(tenantId, tokenUser);

            if (AllSiteKey.Equals(siteKey))
            {
                cmd = new UpdateTenantApproveDeviationsOptionsCommand(
                    Guid.NewGuid().ToString("N"),
                    tokenUser,
                    userName,
                    tenantId,
                    opts);
            }
            else
            {
                var approveDeviations = tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.ApproveDeviations);
                var approveSubstitutions = tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.ApproveSubstitutions);
                var AllowForceOfDeviations = tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.AllowForceOfDeviations);
                var AllowForceOfSubstitutions = tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.AllowForceOfSubstitutions);

                await Task.WhenAll(approveDeviations, approveSubstitutions, AllowForceOfDeviations, AllowForceOfSubstitutions);

                if (approveDeviations.Result.Failed || approveSubstitutions.Result.Failed || AllowForceOfDeviations.Result.Failed || AllowForceOfSubstitutions.Result.Failed)
                {
                    return HttpStatusCode.InternalServerError;
                }

                List<string> activePolicies = new();

                if (approveDeviations.Result.Data != null)
                    activePolicies.Add(approveDeviations.Result.Data.Id);
                if (approveSubstitutions.Result.Data != null)
                    activePolicies.Add(approveSubstitutions.Result.Data.Id);
                if (AllowForceOfDeviations.Result.Data != null)
                    activePolicies.Add(AllowForceOfDeviations.Result.Data.Id);
                if (AllowForceOfSubstitutions.Result.Data != null)
                    activePolicies.Add(AllowForceOfSubstitutions.Result.Data.Id);


                if (activePolicies.Any())
                {
                    var tenantSettings = await pncClient.GetApproveDeviationsOptions(tenantId);
                    if (tenantSettings.Failed || tenantSettings.Data == null)
                    {
                        return HttpStatusCode.InternalServerError;
                    }

                    opts = new ApproveDeviationsOptionsUpdateModel(
                        activePolicies.Contains(ManagementPolicyEntityType.ApproveDeviations.ToString()) ? tenantSettings.Data.ApproveDeviations : request.ApproveDeviations,
                        activePolicies.Contains(ManagementPolicyEntityType.ApproveDeviations.ToString()) ? tenantSettings.Data.ApproveDeviationsOptional : request.ApproveDeviationsOptional,
                        activePolicies.Contains(ManagementPolicyEntityType.ApproveSubstitutions.ToString()) ? tenantSettings.Data.ApproveSubstitutions : request.ApproveSubstitutions,
                        activePolicies.Contains(ManagementPolicyEntityType.ApproveSubstitutions.ToString()) ? tenantSettings.Data.ApproveSubstitutionsOptional : request.ApproveSubstitutionsOptional,
                        activePolicies.Contains(ManagementPolicyEntityType.AllowForceOfDeviations.ToString()) ? tenantSettings.Data.AllowForceOfDeviations : request.AllowForceOfDeviations,
                        activePolicies.Contains(ManagementPolicyEntityType.AllowForceOfSubstitutions.ToString()) ? tenantSettings.Data.AllowForceOfSubstitutions : request.AllowForceOfSubstitutions);
                }

                cmd = new UpdateSiteApproveDeviationsOptionsCommand(
                    Guid.NewGuid().ToString("N"),
                    tokenUser,
                    userName,
                    tenantId,
                    siteKey,
                    opts);

                session += $"|{siteKey}";
            }

            return await taskCommandSynchronizer.Execute(cmd, tenantId, session);
        }

        public async Task<MagicButtonOptions> MagicBtnOptions(string tenantId, string siteKey, CancellationToken cancellationToken)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            var response = await pncClient.MagicButtonOptions(tenantId, siteKey);

            // Custom mapper since automapper fails this task.
            return response.Failed || response.Data == null
                ? default
                : new MagicButtonOptions(
                    response.Data.SelectionFilters,
                    response.Data.PrioritizationFilter,
                    response.Data.PrioritizationFilterPrios,
                    response.Data.CapacityFilters,
                    response.Data.SortingFilter,
                    response.Data.ZoneContextSetting,
                    Configuration.MappingProfiles.MagicProfile.ConvertToClientOptions(response.Data.FilterOptions),
                    response.Data.UpdatedBy,
                    response.Data.UpdatedUtc);
        }

        public async Task<HttpStatusCode> UpsertMagicBtnOptions(string tenantId, string siteKey, MagicButtonOptionsUpdateRequest request, string tokenUser)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, tokenUser);
            var opts = Configuration.MappingProfiles.MagicProfile.ConvertToPncOptions(request.FilterOptions);

            ITaskCommand cmd;

            if (AllSiteKey.Equals(siteKey))
            {
                cmd = new UpsertTenantMagicBtnOptionsCommand(
                    tenantId,
                    Guid.NewGuid().ToString("N"),
                    tokenUser,
                    userName,
                    request.SelectionFilters,
                    request.CapacityFilters,
                    request.PrioritizationFilter,
                    request.PrioritizationFilterPrios,
                    request.SortingFilter,
                    request.ZoneContextSetting,
                    opts);
            }
            else
            {
                cmd = new UpsertSiteMagicBtnOptionsCommand(
                    tenantId,
                    siteKey,
                    Guid.NewGuid().ToString("N"),
                    tokenUser,
                    userName,
                    request.SelectionFilters,
                    request.CapacityFilters,
                    request.PrioritizationFilter,
                    request.PrioritizationFilterPrios,
                    request.SortingFilter,
                    request.ZoneContextSetting,
                    opts);
            }

            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{cmd.GetType().Name}");
        }

        public async Task<PickSlotOptions> GetPickSlotOptions(string tenantId, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            var response = await pncClient.GetPickSlotOptions(tenantId, siteKey);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<PickSlotOptions>(response.Data);
        }

        public async Task<Gateway.Shared.Models.PnC.Settings.Optimization.PicklistOptions> GetPicklistOptions(string tenantId, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.InactivePicklist);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.GetPicklistOptions(tenantId, siteKey);
            return response.Failed || response.Data == null
                ? null
                : new Gateway.Shared.Models.PnC.Settings.Optimization.PicklistOptions(
                    response.Data.ReleaseInactivePicklist,
                    response.Data.PreparationStatesInactivityThreshold,
                    response.Data.InProgressStatesInactivityThreshold,
                    response.Data.UpdatedBy,
                    response.Data.UpdatedUtc);
        }

        public Task<HttpStatusCode> UpdatePicklistOptions(string issuer, string tenantId, PicklistOptionsUpdateRequest request, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                return UpdateTenantPicklistOptions(issuer, tenantId, request);
            }

            return UpdateSitePicklistOptions(issuer, siteKey, tenantId, request);
        }

        public Task<HttpStatusCode> UpdatePickSlotOptions(string issuer, string tenantId, PickSlotOptionsUpdateRequest request, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                return UpdateTenantPickSlotOptions(issuer, tenantId, request);
            }

            return UpdateSitePickSlotOptions(issuer, siteKey, tenantId, request);
        }

        private async Task<HttpStatusCode> UpdateTenantPickSlotOptions(string issuer, string tenantId, PickSlotOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var pickSlotOptions = new PickSlotOptionsUpdateModel(
                request.DefaultPickSlotTimeSpan,
                request.DefaultPickSlotMargin,
                request.AutoUpdatePickSlot);
            var cmd = new UpdateTenantPickSlotOptionsCommand(
                Guid.NewGuid().ToString("N"),
                issuer,
                userName,
                tenantId,
                pickSlotOptions);
            return await taskCommandSynchronizer.Execute(cmd, cmd.TenantId, cmd.TenantId);
        }

        private async Task<HttpStatusCode> UpdateSitePicklistOptions(string issuer, string siteKey, string tenantId, PicklistOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var pickSlotOptions = new PicklistOptionsUpdateModel(
                request.PreparationStatesInactivityThreshold,
                request.InProgressStatesInactivityThreshold,
                request.ReleaseInactivePicklist);

            var cmd = new UpdateSitePicklistOptionsCommand(
                issuer,
                userName,
                Guid.NewGuid().ToString("N"),
                tenantId,
                siteKey,
                pickSlotOptions);

            return await taskCommandSynchronizer.Execute(cmd, cmd.TenantId, $"{tenantId}|{siteKey}");
        }

        private async Task<HttpStatusCode> UpdateTenantPicklistOptions(string issuer, string tenantId, PicklistOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var options = new PicklistOptionsUpdateModel(
                request.PreparationStatesInactivityThreshold,
                request.InProgressStatesInactivityThreshold,
                request.ReleaseInactivePicklist);

            var cmd = new UpdateTenantPicklistOptionsCommand(
                issuer,
                userName,
                Guid.NewGuid().ToString("N"),
                tenantId,
                options);

            return await taskCommandSynchronizer.Execute(cmd, cmd.TenantId, cmd.TenantId);
        }

        private async Task<HttpStatusCode> UpdateSitePickSlotOptions(string issuer, string siteKey, string tenantId, PickSlotOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var pickSlotOptions = new PickSlotOptionsUpdateModel(
                request.DefaultPickSlotTimeSpan,
                request.DefaultPickSlotMargin,
                request.AutoUpdatePickSlot);
            var cmd = new UpdateSitePickSlotOptionsCommand(
                Guid.NewGuid().ToString("N"),
                issuer,
                userName,
                siteKey,
                tenantId,
                pickSlotOptions);
            return await taskCommandSynchronizer.Execute(cmd, cmd.TenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<QualityPickingRules> GetQualityPickingRules(string tenantId, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.QualityPicking);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.GetQualityPickingRules(tenantId, siteKey);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<QualityPickingRules>(response.Data);
        }

        public Task<HttpStatusCode> UpdateQualityPickingRules(string issuer, string tenantId, QualityPickingRulesUpdateRequest request, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                return UpdateTenantQualityPickingRules(issuer, tenantId, request);
            }

            return UpdateSiteQualityPickingRules(issuer, siteKey, tenantId, request);
        }

        private async Task<HttpStatusCode> UpdateTenantQualityPickingRules(string issuer, string tenantId, QualityPickingRulesUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var model = new QualityPickingRulesUpdateModel(request.Active, request.AllowCloseWithoutScan, request.Threshold, request.LoadCarrierTracking);

            var cmd = new UpdateTenantQualityPickingRulesCommand(
                Guid.NewGuid().ToString("N"),
                issuer,
                userName,
                tenantId,
                model);

            return await taskCommandSynchronizer.Execute(cmd, cmd.TenantId, cmd.TenantId);
        }

        private async Task<HttpStatusCode> UpdateSiteQualityPickingRules(string issuer, string siteKey, string tenantId, QualityPickingRulesUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var model = new QualityPickingRulesUpdateModel(request.Active, request.AllowCloseWithoutScan, request.Threshold, request.LoadCarrierTracking);

            var cmd = new UpdateSiteQualityPickingRulesCommand(
                Guid.NewGuid().ToString("N"),
                issuer,
                userName,
                siteKey,
                tenantId,
                model);

            return await taskCommandSynchronizer.Execute(cmd, cmd.TenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<AppUIOptions> GetAppUIOptions(string tenantId, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.OrderPickingAppUI);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.AppUIOptions(tenantId, siteKey);
            return response.Failed || response.Data == null
                ? default
                : mapperHelper.Map(response.Data);
        }

        public async Task<HttpStatusCode> UpdateAppUIOptions(string issuer, string tenantId, AppUIOptionsUpdateRequest request, string siteKey = null)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var data = new AppUIOptionsUpdateModel(mapperHelper.Map(request.PickingView), mapperHelper.Map(request.HomeView));

            var cmd = new UpdateAppUIOptionsCommand(
                tenantId,
                siteKey,
                Guid.NewGuid().ToString("N"),
                issuer,
                userName,
                options: data);

            return await taskCommandSynchronizer.Execute(cmd, cmd.TenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<PickingThresholdOptions> GetPickingThresholdOptions(string tenantId, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.PickingDeviationLimits);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.GetPickingThresholdOptions(tenantId, siteKey);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<PickingThresholdOptions>(response.Data);
        }

        public Task<HttpStatusCode> UpdatePickingThresholdOptions(string issuer, string tenantId, PickingThresholdOptionsUpdateRequest request, string siteKey = null)
        {
            var opts = new PickingThresholdOptionsUpdateModel(request.UnderThresholdLimit, request.UnderThresholdWarning, request.OverThresholdLimit, request.OverThresholdWarning);

            if (AllSiteKey.Equals(siteKey))
            {
                return UpdateTenantPickingThresholdOptions(issuer, tenantId, opts);
            }

            return UpdateSitePickingThresholdOptions(issuer, siteKey, tenantId, opts);
        }

        private async Task<HttpStatusCode> UpdateTenantPickingThresholdOptions(string issuer, string tenantId, PickingThresholdOptionsUpdateModel pickingThresholdOptions)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var cmd = new UpdateTenantPickingThresholdOptionsCommand(
                Guid.NewGuid().ToString("N"),
                issuer,
                userName,
                tenantId,
                pickingThresholdOptions);
            return await taskCommandSynchronizer.Execute(cmd, cmd.TenantId, cmd.TenantId);
        }

        private async Task<HttpStatusCode> UpdateSitePickingThresholdOptions(string issuer, string siteKey, string tenantId, PickingThresholdOptionsUpdateModel pickingThresholdOptions)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var cmd = new UpdateSitePickingThresholdOptionsCommand(
                Guid.NewGuid().ToString("N"),
                issuer,
                userName,
                tenantId,
                siteKey,
                pickingThresholdOptions);
            return await taskCommandSynchronizer.Execute(cmd, cmd.TenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<ManualPickingOptions> GetManualPickingOptions(string tenantId, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.ManualPicking);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.GetManualPickingOptions(tenantId, siteKey);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<ManualPickingOptions>(response.Data);
        }

        public Task<HttpStatusCode> UpdateManualPickingOptions(string issuer, string tenantId, ManualPickingOptionsUpdateRequest request, string siteKey = null)
        {
            var opts = new ManualPickingOptionsUpdateModel(mapper.Map<PncCore.Shared.Models.Settings.ManualPickingMode>(request.ManualPickingMode), request.ForceScanOfAlias);

            if (AllSiteKey.Equals(siteKey))
            {
                return UpdateTenantManualPickingOptions(issuer, tenantId, opts);
            }

            return UpdateSiteManualPickingOptions(issuer, siteKey, tenantId, opts);
        }

        private async Task<HttpStatusCode> UpdateTenantManualPickingOptions(string issuer, string tenantId, ManualPickingOptionsUpdateModel options)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);

            var cmd = new UpdateTenantManualPickingOptionsCommand(
                Guid.NewGuid().ToString("N"),
                issuer,
                userName,
                tenantId,
                options);

            return await taskCommandSynchronizer.Execute(cmd, cmd.TenantId, cmd.TenantId);
        }

        private async Task<HttpStatusCode> UpdateSiteManualPickingOptions(string issuer, string siteKey, string tenantId, ManualPickingOptionsUpdateModel options)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);

            var cmd = new UpdateSiteManualPickingOptionsCommand(
                Guid.NewGuid().ToString("N"),
                issuer,
                userName,
                siteKey,
                tenantId,
                options);

            return await taskCommandSynchronizer.Execute(cmd, cmd.TenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<IEnumerable<SubstitutionRuleOptions>> GetSubstitutionRuleOptions(string tenantId, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            var response = await productClient.RuleOptions(tenantId, siteKey);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<IEnumerable<SubstitutionRuleOptions>>(response.Data);
        }

        public async Task<HttpStatusCode> UpsertProductSubstitutionRuleOptions(string issuer, string tenantId, IEnumerable<SubstitutionRuleOptionsUpdateRequest> request, string siteKey = null)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var model = request.Select(x => new SubstitutionRuleOptionsUpdateModel(x.RuleName, x.RuleType, x.Filter));
            var cmd = new UpsertProductSubstitutionRuleCommand(issuer, tenantId, siteKey, model, Guid.NewGuid().ToString("N"), userName);

            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public Task<HttpStatusCode> DeleteAllProductSubstitutionRuleOptions(string issuer, string tenantId, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                // todo: is this something we want to have implemented?
                return Task.FromResult(HttpStatusCode.NotImplemented);
            }

            return DeleteAllSiteProductSubstitutionRuleOptions(issuer, siteKey, tenantId);
        }

        private Task<HttpStatusCode> DeleteAllSiteProductSubstitutionRuleOptions(string issuer, string siteKey, string tenantId)
        {
            var cmd = new DeleteAllProductSubstitutionRuleCommand(
                issuer,
                tenantId,
                siteKey,
                Guid.NewGuid().ToString("N"));
            return taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<HttpStatusCode> UpsertTenantDeliverableAutomationOptions(string tenantId, string issuer, DeliverableAutomationOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var mappedOpts = mapper.Map<DeliverableAutomationOptionsUpdateModel>(request);

            var cmd = new UpsertTenantDeliverableAutomationOptionsCommand(
                tenantId,
                Guid.NewGuid().ToString("N"),
                issuer,
                userName,
                mappedOpts);

            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}");
        }

        public async Task<HttpStatusCode> UpsertSiteDeliverableAutomationOptions(string tenantId, string siteKey, string issuer, DeliverableAutomationOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var mappedOpts = mapper.Map<DeliverableAutomationOptionsUpdateModel>(request);

            var cmd = new UpsertSiteDeliverableAutomationOptionsCommand(
                tenantId,
                siteKey,
                Guid.NewGuid().ToString("N"),
                issuer,
                userName,
                mappedOpts);

            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<DeliverableAutomationOptions> GetDeliverableAutomationOptions(string tenantId, string siteKey)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.Automation);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.DeliverableAutomationOptions(tenantId, siteKey);
            return response.Failed || response.Data == null
                ? new DeliverableAutomationOptions()
                : mapper.Map<DeliverableAutomationOptions>(response.Data);
        }

        public async Task<HttpStatusCode> UpdateAppFeatureOptions(string tenantId, string issuer, AppFeatureOptionsUpdateRequest request, string siteKey = null)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);

            var featureOptions = new AppFeatureOptionsUpdateModel(
                request.DisableSearchByTextOrScanWhenSubstituting,
                request.DisableUserCommentForDeviation,
                request.DisableManualSiteSelection);

            var cmd = new UpdateAppFeatureOptionsCommand(
                tenantId,
                siteKey,
                Guid.NewGuid().ToString("N"),
                issuer,
                userName,
                featureOptions);

            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}");
        }

        public async Task<AppFeatureOptions> GetAppFeatureOptions(string tenantId, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.OrderPickingAppFeatures);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.AppFeatureOptions(tenantId, siteKey);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<AppFeatureOptions>(response.Data);
        }

        public async Task<OrderStateTransitionRules> GetOrderStateTransitionRules(string tenantId)
        {
            var response = await pncClient.OrderStateTransitionRules(tenantId);
            return response.Failed || response.Data == null
                ? new OrderStateTransitionRules()
                : mapper.Map<OrderStateTransitionRules>(response.Data);
        }

        public async Task<HttpStatusCode> UpsertOrderStateTransitionRules(string tenantId, string issuer, OrderStateTransitionRulesUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var mappedOpts = mapper.Map<OrderStateTransitionRulesUpdateModel>(request);

            var cmd = new UpdateOrderStateTransitionRulesCommand(
                tenantId,
                Guid.NewGuid().ToString("N"),
                issuer,
                userName,
                mappedOpts);

            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}");
        }

        public async Task<AllowAddOrderLineOptions> GetAllowAddOrderLineOptions(string tenantId)
        {
            var response = await pncClient.AllowAddOrderLineOptions(tenantId);
            return response.Failed || response.Data == null
                ? new AllowAddOrderLineOptions()
                : mapper.Map<AllowAddOrderLineOptions>(response.Data);
        }

        public async Task<HttpStatusCode> UpsertAllowAddOrderLineOptions(string tenantId, string issuer, AllowAddOrderLineOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var opts = new AllowAddOrderLineOptionsUpdateModel(request.AllowAddOrderLine);

            var cmd = new UpdateAllowAddOrderLineOptionsCommand(
                tenantId,
                opts,
                Guid.NewGuid().ToString("N"),
                issuer,
                userName
            );

            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}");
        }

        public async Task<OrderNotificationOptions> ReadOrderNotificationOptions(string tenantId, string siteKey)
        {
            var response = await pncClient.OrderNotificationOptions(tenantId, siteKey);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<OrderNotificationOptions>(response.Data);
        }

        public Task<HttpStatusCode> UpsertOrderNotificationOptions(string tenantId, string siteKey, string issuer, OrderNotificationOptions options)
        {
            var cmd = new UpsertOrderNotificationOptionsCommand(
                tenantId,
                siteKey,
                options.NewOrderTtlInSeconds,
                Guid.NewGuid().ToString("N"),
                issuer
            );

            return taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<SubstitutionLimitationsOptions> SubstitutionLimitationsOptions(string tenantId, string siteKey)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.SubstitutionLimitations);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.SubstitutionLimitationsOptions(tenantId, siteKey);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<SubstitutionLimitationsOptions>(response.Data);
        }

        public Task<HttpStatusCode> UpsertSubstitutionLimitationsOptions(string tenantId, string siteKey, string issuer, SubstitutionLimitationsOptionsUpdateRequest updateRequest)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                return UpsertTenantSubstitutionLimitationsOptions(tenantId, issuer, updateRequest);
            }

            return UpsertSiteSubstitutionLimitationsOptions(tenantId, siteKey, issuer, updateRequest);
        }

        private async Task<HttpStatusCode> UpsertTenantSubstitutionLimitationsOptions(string tenantId, string issuer, SubstitutionLimitationsOptionsUpdateRequest updateRequest)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var mode = mapper.Map<SubstitutionLimitationMode>(updateRequest.Mode);
            var cmd = new UpdateTenantSubstitutionLimitationsOptionsCommand(mode, tenantId, Guid.NewGuid().ToString("N"), issuer, userName);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, tenantId);
        }

        private async Task<HttpStatusCode> UpsertSiteSubstitutionLimitationsOptions(string tenantId, string siteKey, string issuer, SubstitutionLimitationsOptionsUpdateRequest updateRequest)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var mode = mapper.Map<SubstitutionLimitationMode>(updateRequest.Mode);
            var cmd = new UpdateSiteSubstitutionLimitationsOptionsCommand(mode, siteKey, tenantId, Guid.NewGuid().ToString("N"), issuer, userName);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<ForcePrintWeightLabelOptions> ForcePrintWeightLabelOptions(string tenantId, string siteKey)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.ScaleSettings);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.ForcePrintWeightLabelOptions(tenantId, siteKey);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<ForcePrintWeightLabelOptions>(response.Data);
        }

        public Task<HttpStatusCode> UpsertForcePrintWeightLabelOptions(string tenantId, string siteKey, string issuer, ForcePrintWeightLabelOptionsUpdateRequest request)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                return UpsertTenantForcePrintWeightLabelOptions(tenantId, issuer, request);
            }

            return UpsertSiteForcePrintWeightLabelOptions(tenantId, siteKey, issuer, request);
        }

        private async Task<HttpStatusCode> UpsertTenantForcePrintWeightLabelOptions(string tenantId, string issuer, ForcePrintWeightLabelOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var cmd = new UpdateTenantForcePrintWeightLabelOptionsCommand(request.ForcePrintWeightLabel, tenantId, Guid.NewGuid().ToString("N"), issuer, userName);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, tenantId);
        }

        private async Task<HttpStatusCode> UpsertSiteForcePrintWeightLabelOptions(string tenantId, string siteKey, string issuer, ForcePrintWeightLabelOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var cmd = new UpdateSiteForcePrintWeightLabelOptionsCommand(request.ForcePrintWeightLabel, siteKey, tenantId, Guid.NewGuid().ToString("N"), issuer, userName);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<OrderStatusOnDeviationsOptions> OrderStatusOnDeviationsOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.OrderStatusOnDeviations);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.OrderStatusOnDeviationsOptions(tenantId, siteKey, cancellationToken);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<OrderStatusOnDeviationsOptions>(response.Data);
        }

        public Task<HttpStatusCode> UpdateOrderStatusOnDeviationsOptions(string tenantId, string siteKey, string issuer, OrderStatusOnDeviationsOptionsUpdateRequest request)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                return UpdateTenantOrderStatusOnDeviationsOptions(tenantId, issuer, request);
            }

            return UpdateSiteOrderStatusOnDeviationsOptions(tenantId, siteKey, issuer, request);
        }

        private async Task<HttpStatusCode> UpdateTenantOrderStatusOnDeviationsOptions(string tenantId, string issuer, OrderStatusOnDeviationsOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var opts = new OrderStatusOnDeviationsOptionsUpdateModel(
                request.MaxWeightDeviation,
                request.MaxSubstitutionWeightDeviation,
                request.MaxSubstitutionDeviationPiecesPerPiece);

            var cmd = new UpdateTenantOrderStatusOnDeviationsOptionsCommand(opts, tenantId, Guid.NewGuid().ToString("N"), issuer, userName);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, tenantId);
        }

        private async Task<HttpStatusCode> UpdateSiteOrderStatusOnDeviationsOptions(string tenantId, string siteKey, string issuer, OrderStatusOnDeviationsOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var opts = new OrderStatusOnDeviationsOptionsUpdateModel(
                request.MaxWeightDeviation,
                request.MaxSubstitutionWeightDeviation,
                request.MaxSubstitutionDeviationPiecesPerPiece);

            var cmd = new UpdateSiteOrderStatusOnDeviationsOptionsCommand(opts, siteKey, tenantId, Guid.NewGuid().ToString("N"), issuer, userName);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<OverrideCustomerSubstitutionSettingOptions> OverrideCustomerSubstitutionSettingOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.AllowOverrideCustomerSubstitutionSetting);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.OverrideCustomerSubstitutionSettingOptions(tenantId, siteKey, cancellationToken);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<OverrideCustomerSubstitutionSettingOptions>(response.Data);
        }

        public Task<HttpStatusCode> UpdateOverrideCustomerSubstitutionSettingOptions(string tenantId, string siteKey, string issuer, OverrideCustomerSubstitutionSettingOptionsUpdateRequest request)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                return UpdateTenantOverrideCustomerSubstitutionSettingOptions(tenantId, issuer, request);
            }

            return UpdateSiteOverrideCustomerSubstitutionSettingOptions(tenantId, siteKey, issuer, request);
        }

        private async Task<HttpStatusCode> UpdateSiteOverrideCustomerSubstitutionSettingOptions(string tenantId, string siteKey, string issuer, OverrideCustomerSubstitutionSettingOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var opts = new OverrideCustomerSubstitutionSettingOptionsUpdateModel(
                request.Active,
                request.Roles);

            var cmd = new UpdateSiteOverrideCustomerSubstitutionCommand(opts, siteKey, tenantId, Guid.NewGuid().ToString("N"), issuer, userName);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        private async Task<HttpStatusCode> UpdateTenantOverrideCustomerSubstitutionSettingOptions(string tenantId, string issuer, OverrideCustomerSubstitutionSettingOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var opts = new OverrideCustomerSubstitutionSettingOptionsUpdateModel(
                request.Active,
                request.Roles);

            var cmd = new UpdateTenantOverrideCustomerSubstitutionCommand(opts, tenantId, Guid.NewGuid().ToString("N"), issuer, userName);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, tenantId);
        }

        public async Task<ShowCustomerInfoOptions> ShowCustomerInfoOptions(string tenantId, string siteKey)
        {
            if (AllSiteKey.Equals(siteKey))
                siteKey = null;
            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.CustomerInfo);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.ShowCustomerInfoOptions(tenantId, siteKey);
            return (response.Failed || response.Data == null) ? default : mapper.Map<ShowCustomerInfoOptions>(response.Data);
        }

        public async Task<HttpStatusCode> UpsertShowCustomerInfoOptions(string tenantId, string siteKey, string issuer, ShowCustomerInfoOptionsUpdateRequest request)
        {
            var opt = request ?? new ShowCustomerInfoOptionsUpdateRequest();

            if (AllSiteKey.Equals(siteKey))
            {
                return await UpsertTenantShowCustomerInfoOptions(tenantId, issuer, opt);
            }

            return await UpsertSiteForceShowCustomerInfoOptions(tenantId, siteKey, issuer, opt);
        }

        private async Task<HttpStatusCode> UpsertTenantShowCustomerInfoOptions(string tenantId, string issuer, ShowCustomerInfoOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var opt = request ?? new ShowCustomerInfoOptionsUpdateRequest();
            var cmd = new UpdateTenantShowCustomerInfoOptionsCommand(opt.ShowCustomerInfo, tenantId, Guid.NewGuid().ToString("N"), issuer, userName);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, tenantId);
        }

        private async Task<HttpStatusCode> UpsertSiteForceShowCustomerInfoOptions(string tenantId, string siteKey, string issuer, ShowCustomerInfoOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var opt = request ?? new ShowCustomerInfoOptionsUpdateRequest();
            var cmd = new UpdateSiteShowCustomerInfoOptionsCommand(opt.ShowCustomerInfo, siteKey, tenantId, Guid.NewGuid().ToString("N"), issuer, userName);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<ForcePickAllLinesOptions> ForcePickAllLinesOptions(string tenantId, string siteKey)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.ForcePickAllOrderLines);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.ForcePickAllLinesOptions(tenantId, siteKey);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<ForcePickAllLinesOptions>(response.Data);
        }

        public Task<HttpStatusCode> UpsertForcePickAllLinesOptions(string tenantId, string siteKey, string issuer, ForcePickAllLinesOptionsUpdateRequest request)
        {
            var opt = request ?? new ForcePickAllLinesOptionsUpdateRequest();

            if (AllSiteKey.Equals(siteKey))
            {
                return UpsertTenantForcePickAllLinesOptions(tenantId, issuer, opt);
            }

            return UpsertSiteForcePickAllLinesOptions(tenantId, siteKey, issuer, opt);
        }

        private async Task<HttpStatusCode> UpsertTenantForcePickAllLinesOptions(string tenantId, string issuer, ForcePickAllLinesOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var opt = request ?? new ForcePickAllLinesOptionsUpdateRequest();
            var cmd = new UpdateTenantForcePickAllLinesOptionsCommand(opt.PickAllLines, tenantId, Guid.NewGuid().ToString("N"), issuer, userName);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, tenantId);
        }

        private async Task<HttpStatusCode> UpsertSiteForcePickAllLinesOptions(string tenantId, string siteKey, string issuer, ForcePickAllLinesOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var opt = request ?? new ForcePickAllLinesOptionsUpdateRequest();
            var cmd = new UpdateSiteForcePickAllLinesOptionsCommand(opt.PickAllLines, siteKey, tenantId, Guid.NewGuid().ToString("N"), issuer, userName);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<CancelOrderInWebOptions> CancelOrderInWebOptions(string tenantId)
        {
            var response = await pncClient.CancelOrderInWebOptions(tenantId);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<CancelOrderInWebOptions>(response.Data);
        }

        public Task<HttpStatusCode> UpsertCancelOrderInWebOptions(string tenantId, string issuer, CancelOrderInWebOptionsUpdateRequest request)
        {
            var opt = request ?? new CancelOrderInWebOptionsUpdateRequest();
            return UpsertTenantCancelOrderInWebOptions(tenantId, issuer, opt);
        }

        private async Task<HttpStatusCode> UpsertTenantCancelOrderInWebOptions(string tenantId, string issuer, CancelOrderInWebOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, issuer);
            var cmd = new UpdateTenantCancelOrderInWebOptionsCommand(request.CancelOrderInWeb, tenantId, Guid.NewGuid().ToString("N"), issuer, userName);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, tenantId);
        }

        public async Task<IEnumerable<LabelTemplate>> GetDefaultLabelTemplates()
        {
            var response = await pncClient.GetDefaultLabelTemplates();
            return response.Failed || response.Data == null
                ? Enumerable.Empty<LabelTemplate>()
                : mapper.Map<IEnumerable<LabelTemplate>>(response.Data);
        }

        public async Task<LabelPreviewResponse> GetLabelPreview(LabelPreviewRequest labelPreviewRequest)
        {
            try
            {
                var response = await labelPreviewClient.GetZPLPreview(labelPreviewRequest.Zpl, labelPreviewRequest.Dpmm, labelPreviewRequest.Width, labelPreviewRequest.Height);
                return new LabelPreviewResponse($"data:{response.Headers.ContentType}; charset=utf-8; base64, {Convert.ToBase64String(await response.ReadAsByteArrayAsync())}");
            }
            catch
            {
                return default;
            }
        }

        public async Task<IEnumerable<LabelTemplate>> GetSelectedLabelTemplates(string tenantId, LabelType? type = default, string siteKey = default)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.Labels);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.GetLabelTemplates(tenantId, siteKey, type);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<IEnumerable<LabelTemplate>>(response.Data);
        }

        public Task<HttpStatusCode> UpdateLabelTemplate(string tenantId, LabelTemplate labelTemplate, string issuer, string siteKey = default)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                return UpdateTenantLabelTemplate(tenantId, issuer, labelTemplate);
            }

            return UpdateSiteLabelTemplate(siteKey, tenantId, issuer, labelTemplate);
        }

        private Task<HttpStatusCode> UpdateTenantLabelTemplate(string tenantId, string issuer, LabelTemplate labelTemplate)
        {
            var cmd = new UpdateTenantLabelTemplateCommand(
                labelTemplate.Name,
                labelTemplate.Body,
                labelTemplate.Type,
                labelTemplate.Dpmm,
                labelTemplate.Height,
                labelTemplate.Width,
                tenantId,
                Guid.NewGuid().ToString("N"),
                issuer,
                labelTemplate.Id);
            return taskCommandSynchronizer.Execute(cmd, tenantId, tenantId);
        }

        private Task<HttpStatusCode> UpdateSiteLabelTemplate(string siteKey, string tenantId, string issuer, LabelTemplate labelTemplate)
        {
            var cmd = new UpdateSiteLabelTemplateCommand(
                labelTemplate.Name,
                labelTemplate.Body,
                labelTemplate.Type,
                labelTemplate.Dpmm,
                labelTemplate.Height,
                labelTemplate.Width,
                siteKey,
                tenantId,
                Guid.NewGuid().ToString("N"),
                issuer,
                labelTemplate.Id);
            return taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public Task<HttpStatusCode> DeleteLabelTemplate(string tenantId, string labelId, string issuer, string siteKey = default)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                return DeleteTenantLabelTemplate(tenantId, labelId, issuer);
            }

            return DeleteSiteLabelTemplate(siteKey, tenantId, labelId, issuer);
        }

        private Task<HttpStatusCode> DeleteTenantLabelTemplate(string tenantId, string labelId, string issuer)
        {
            var cmd = new DeleteTenantLabelTemplateCommand(labelId, tenantId, Guid.NewGuid().ToString("N"), issuer);
            return taskCommandSynchronizer.Execute(cmd, tenantId, tenantId);
        }

        private Task<HttpStatusCode> DeleteSiteLabelTemplate(string siteKey, string tenantId, string labelId, string issuer)
        {
            var cmd = new DeleteSiteLabelTemplateCommand(siteKey, labelId, tenantId, Guid.NewGuid().ToString("N"), issuer);
            return taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<IEnumerable<ProjectSubstitutionReason>> GetSubstitutionReasons(string culture)
        {
            var response = await pncClient.GetSubstitutionReasons();

            if (response.Failed || !response.Data.Any())
            {
                return Array.Empty<ProjectSubstitutionReason>();
            }

            return mapper.Map<IEnumerable<ProjectSubstitutionReason>>(response.Data, opts => opts.Items["culture"] = culture);
        }

        public async Task<IEnumerable<SubstitutionReason>> GetSelectedSubstitutionReasons(string tenantId, string siteKey = null, string culture = "en")
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            var response = await pncClient.GetSelectedSubstitutionReasons(tenantId, siteKey);
            return response.Failed || !response.Data.Any()
                ? Array.Empty<SubstitutionReason>()
                : mapper.Map<IEnumerable<SubstitutionReason>>(response.Data, opts => opts.Items["culture"] = culture);
        }

        public Task<bool> UpdateSelectedSubstitutionReasons(string issuer, string tenantId, IEnumerable<SubstitutionReason> reasons, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                return UpdateTenantSubstitutionReasons(issuer, tenantId, reasons);
            }

            return UpdateSiteSubstitutionReasons(issuer, siteKey, tenantId, reasons);
        }

        private async Task<bool> UpdateTenantSubstitutionReasons(string issuer, string tenantId, IEnumerable<SubstitutionReason> reasons)
        {
            var tasks = new List<Task<bool>>();
            foreach (var reason in reasons)
            {
                tasks.Add(
                    SendToQueue(
                        new UpdateTenantSubstitutionReasonsCommand(
                            Guid.NewGuid().ToString("N"),
                            issuer,
                            tenantId,
                            new PncCore.Shared.Models.Reasons.SelectedReason(reason.Code, reason.Name, reason.Active)), null, tenantId));
            }

            var result = await Task.WhenAll(tasks);
            return result.All(r => r == true);
        }

        private async Task<bool> UpdateSiteSubstitutionReasons(string issuer, string siteKey, string tenantId, IEnumerable<SubstitutionReason> reasons)
        {
            var tasks = new List<Task<bool>>();
            foreach (var reason in reasons)
            {
                tasks.Add(
                    SendToQueue(
                        new UpdateSiteSubstitutionReasonsCommand(
                            Guid.NewGuid().ToString("N"),
                            issuer,
                            siteKey,
                            tenantId,
                            new PncCore.Shared.Models.Reasons.SelectedReason(
                                reason.Code,
                                reason.Name,
                                reason.Active))));
            }

            var result = await Task.WhenAll(tasks);
            return result.All(r => r == true);
        }

        public Task<HttpStatusCode> DeleteSelectedSubstitutionReason(string issuer, string tenantId, string code, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                return DeleteTenantSubstitutionReason(issuer, tenantId, code);
            }

            return DeleteSiteSubstitutionReason(issuer, siteKey, tenantId, code);
        }

        private Task<HttpStatusCode> DeleteTenantSubstitutionReason(string issuer, string tenantId, string code)
        {
            var cmd = new DeleteTenantSubstitutionReasonCommand(
                Guid.NewGuid().ToString("N"),
                issuer,
                tenantId,
                code);
            return taskCommandSynchronizer.Execute(cmd, cmd.TenantId, cmd.TenantId);
        }

        private Task<HttpStatusCode> DeleteSiteSubstitutionReason(string issuer, string siteKey, string tenantId, string code)
        {
            var cmd = new DeleteSiteSubstitutionReasonCommand(
                Guid.NewGuid().ToString("N"),
                issuer,
                siteKey,
                tenantId,
                code);
            return taskCommandSynchronizer.Execute(cmd, cmd.TenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<IEnumerable<ProjectPickingDeviationReason>> GetPickingDeviationReasons(string culture)
        {
            var response = await pncClient.GetDeviationReasons();
            if (response.Failed || !response.Data.Any())
            {
                return Array.Empty<ProjectPickingDeviationReason>();
            }

            return mapper.Map<IEnumerable<ProjectPickingDeviationReason>>(response.Data, opts => opts.Items["culture"] = culture);
        }

        public async Task<IEnumerable<PickingDeviationReason>> GetSelectedDeviationReasons(string tenantId, string siteKey = null, string culture = "en")
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            var response = await pncClient.GetSelectedDeviationReason(tenantId, siteKey);
            return response.Failed || !response.Data.Any()
                ? Array.Empty<PickingDeviationReason>()
                : mapper.Map<IEnumerable<PickingDeviationReason>>(response.Data, opts => opts.Items["culture"] = culture);
        }

        public Task<bool> UpdateSelectedDeviationReasons(string issuer, string tenantId, IEnumerable<PickingDeviationReason> reasons, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                return UpdateTenantDeviationReasons(issuer, tenantId, reasons);
            }

            return UpdateSiteDeviationReasons(issuer, siteKey, tenantId, reasons);
        }

        private async Task<bool> UpdateTenantDeviationReasons(string issuer, string tenantId, IEnumerable<PickingDeviationReason> reasons)
        {
            var tasks = new List<Task<bool>>();
            foreach (var reason in reasons)
            {
                tasks.Add(
                    SendToQueue(
                        new UpdateTenantPickingDeviationReasonsCommand(
                            Guid.NewGuid().ToString("N"),
                            issuer,
                            tenantId,
                            new PncCore.Shared.Models.Reasons.SelectedReason(reason.Code, reason.Name, reason.Active)), null, tenantId));
            }

            var result = await Task.WhenAll(tasks);
            return result.All(r => r == true);
        }

        private async Task<bool> UpdateSiteDeviationReasons(string issuer, string siteKey, string tenantId, IEnumerable<PickingDeviationReason> reasons)
        {
            var tasks = new List<Task<bool>>();
            foreach (var reason in reasons)
            {
                tasks.Add(
                    SendToQueue(
                        new UpdateSitePickingDeviationReasonsCommand(
                            Guid.NewGuid().ToString("N"),
                            issuer,
                            siteKey,
                            tenantId,
                            new PncCore.Shared.Models.Reasons.SelectedReason(
                                reason.Code,
                                reason.Name,
                                reason.Active))));
            }

            var result = await Task.WhenAll(tasks);
            return result.All(r => r == true);
        }

        public Task<HttpStatusCode> DeleteSelectedDeviationReason(string issuer, string tenantId, string code, string siteKey = null)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                return DeleteTenantPickingDeviationReason(issuer, tenantId, code);
            }

            return DeleteSitePickingDeviationReason(issuer, siteKey, tenantId, code);
        }

        private Task<HttpStatusCode> DeleteTenantPickingDeviationReason(string issuer, string tenantId, string code)
        {
            var cmd = new DeleteTenantPickingDeviationReasonCommand(
                Guid.NewGuid().ToString("N"),
                issuer,
                tenantId,
                code);
            return taskCommandSynchronizer.Execute(cmd, cmd.TenantId, cmd.TenantId);
        }

        private Task<HttpStatusCode> DeleteSitePickingDeviationReason(string issuer, string siteKey, string tenantId, string code)
        {
            var cmd = new DeleteSitePickingDeviationReasonCommand(
                Guid.NewGuid().ToString("N"),
                issuer,
                siteKey,
                tenantId,
                code);
            return taskCommandSynchronizer.Execute(cmd, cmd.TenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<IEnumerable<Printer>> GetPrinters(string tenantId, string siteKey)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.Printers);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.Printers(tenantId, siteKey);

            return response.Failed || !response.Data.Any()
                ? Array.Empty<Printer>()
                : mapper.Map<IEnumerable<Printer>>(response.Data);
        }

        public async Task<Printer> GetPrinter(string tenantId, string siteKey, string id)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            var response = await pncClient.Printer(id, tenantId, siteKey);

            return response.Failed
                ? default
                : mapper.Map<Printer>(response.Data);
        }

        public Task<HttpStatusCode> UpsertPrinter(string tenantId, string siteKey, string id, Printer printer, string userId)
        {
            var p = mapper.Map<PncCore.Shared.Models.Settings.Printer>(printer);

            var cmd = new UpsertPrinterCommand(
                tenantId,
                siteKey,
                p,
                Guid.NewGuid().ToString("N"),
                userId);

            return taskCommandSynchronizer.Execute(cmd, cmd.TenantId, $"{tenantId}|{siteKey}");
        }

        public Task DeletePrinter(string tenantId, string siteKey, string id, string userId)
        {
            var cmd = new DeletePrinterCommand(
                tenantId,
                siteKey,
                id,
                Guid.NewGuid().ToString("N"),
                userId);

            return taskCommandSynchronizer.Execute(cmd, cmd.TenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<PriceToMassOptions> PriceToMassOptions(string tenantId, string siteKey)
        {
            var response = await pncClient.PriceToMassOptions(tenantId, siteKey);
            return response.Failed || response.Data == null
                ? default
                : new PriceToMassOptions(response.Data.Expression, response.Data.Active);
        }

        public Task<HttpStatusCode> UpsertPriceToMassOptions(string tenantId, string siteKey, string tokenUser, PriceToMassOptions opt, string issuer)
        {
            var cmd = new UpdatePriceToMassOptionsCommand(
                tenantId,
                siteKey,
                opt.Expression,
                opt.Active,
                Guid.NewGuid().ToString("N"),
                issuer
            );

            return taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<AppEventOption> GetAppEventOption(string tenantId)
        {
            var response = await tenantClient.EventGridOption(tenantId);

            return response.Failed || response.Data == null
                ? new AppEventOption(false)
                : new AppEventOption(response.Data.UsesAppEvents);
        }

        public async Task<ConfirmExpirationDateOptions> ConfirmExpirationDateOptions(string tenantId, string siteKey)
        {
            return new ConfirmExpirationDateOptions(false);
        }

        public async Task<OverrideExpirationDateOptions> OverrideExpirationDateOptions(string tenantId, string siteKey)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.ConfirmExpirationDate);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.OverrideExpirationDateOptions(tenantId, siteKey);
            return response.Failed || response.Data == null
                ? default
                : new OverrideExpirationDateOptions(response.Data.OverrideExpirationDate, response.Data.UpdatedBy, response.Data.UpdatedUtc);
        }

        public async Task<HttpStatusCode> UpsertOverrideExpirationDateOptions(string tenantId, string siteKey, string tokenUser, OverrideExpirationDateOptionsUpdateRequest request, string issuer)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, tokenUser);
            var cmd = new UpdateOverrideExpirationDateOptionsCommand(
                tenantId,
                siteKey,
                request.OverrideExpirationDate,
                Guid.NewGuid().ToString("N"),
                issuer,
                userName
            );

            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<RegisterLoadCarriersOptions> GetRegisterLoadCarriersOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default)
        {
            if (AllSiteKey.Equals(siteKey))
            {
                siteKey = null;
            }

            if (siteKey != null)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.RegisterLoadCarriers);
                var isOverridable = policy.Failed || policy.Data == null;
                if (!isOverridable)
                    siteKey = null;
            }

            var response = await pncClient.GetRegisterLoadCarriersOptions(tenantId, siteKey, cancellationToken);
            return response.Failed || response.Data == null
                ? default
                : new RegisterLoadCarriersOptions(response.Data.RegisterLoadCarriersOptionsEnabled, response.Data.RegisterLoadCarriersOptionsOptional, response.Data.UpdatedBy, response.Data.UpdatedUtc);
        }

        public async Task<HttpStatusCode> UpdateRegisterLoadCarriersOptions(string tenantId, string siteKey, RegisterLoadCarriersOptionsUpdateRequest request, string tokenUser)
        {
            var opts = new RegisterLoadCarriersOptionsUpdateModel(request.RegisterLoadCarriersOptionsEnabled, request.RegisterLoadCarriersOptionsOptional);

            ITaskCommand cmd;

            string session = tenantId;

            var userName = await usersFacade.ResolveUserIdToName(tenantId, tokenUser);

            if (AllSiteKey.Equals(siteKey))
            {
                cmd = new UpdateTenantRegisterLoadCarriersOptionsCommand(
                    Guid.NewGuid().ToString("N"),
                    tokenUser,
                    userName,
                    tenantId,
                    opts);
            }
            else
            {
                var registerLoadCarriers = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.RegisterLoadCarriers);

                if (registerLoadCarriers.Failed)
                {
                    return HttpStatusCode.InternalServerError;
                }

                List<string> activePolicies = new();

                if (registerLoadCarriers.Data != null)
                    activePolicies.Add(registerLoadCarriers.Data.Id);


                if (activePolicies.Any())
                {
                    var tenantSettings = await pncClient.GetRegisterLoadCarriersOptions(tenantId);
                    if (tenantSettings.Failed || tenantSettings.Data == null)
                    {
                        return HttpStatusCode.InternalServerError;
                    }

                    opts = new RegisterLoadCarriersOptionsUpdateModel(
                        activePolicies.Contains(ManagementPolicyEntityType.RegisterLoadCarriers.ToString()) ? tenantSettings.Data.RegisterLoadCarriersOptionsEnabled : request.RegisterLoadCarriersOptionsEnabled,
                        activePolicies.Contains(ManagementPolicyEntityType.RegisterLoadCarriers.ToString()) ? tenantSettings.Data.RegisterLoadCarriersOptionsOptional : request.RegisterLoadCarriersOptionsOptional);
                }

                cmd = new UpdateSiteRegisterLoadCarriersOptionsCommand(
                    Guid.NewGuid().ToString("N"),
                    tokenUser,
                    userName,
                    tenantId,
                    siteKey,
                    opts);

                session += $"|{siteKey}";
            }

            return await taskCommandSynchronizer.Execute(cmd, tenantId, session);
        }

        public async Task<RequireLoadCarrierOptions> GetRequireLoadCarrierOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default)
        {
            if (siteKey != AllSiteKey)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.RequireLoadCarrier);
                var isOverridable = !policy.Failed && policy.Data == null;
                if (!isOverridable)
                    siteKey = AllSiteKey;
            }

            var response = await pncClient.RequireLoadCarrierOptions(tenantId, siteKey, cancellationToken);
            return response.Failed || response.Data == null
                ? default
                : new RequireLoadCarrierOptions(response.Data.LoadCarrierRequirement, response.Data.UpdatedBy, response.Data.UpdatedUtc);
        }

        public async Task<HttpStatusCode> UpdateRequireLoadCarrierOptions(string tenantId, string siteKey, RequireLoadCarrierOptionsUpdateRequest request, string tokenUser)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, tokenUser);
            var cmd = new UpdateRequireLoadCarrierOptionsCommand(tenantId, siteKey, request.LoadCarrierRequirement, Guid.NewGuid().ToString("N"), tokenUser, userName);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<RequireCompileOptions> GetRequireCompileOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default)
        {
            if (siteKey != AllSiteKey)
            {
                var policy = await tenantClient.GetTenantManagementPolicy(tenantId, ManagementPolicyEntityType.RequireCompile);
                var isOverridable = !policy.Failed && policy.Data == null;
                if (!isOverridable)
                    siteKey = AllSiteKey;
            }

            var response = await pncClient.GetRequireCompileOptions(tenantId, siteKey, cancellationToken);
            return response.Failed || response.Data == null
                ? default
                : new RequireCompileOptions(response.Data.RequireCompile, response.Data.UpdatedBy, response.Data.UpdatedUtc);
        }

        public async Task<HttpStatusCode> UpdateRequireCompileOptions(string tenantId, string siteKey, RequireCompileOptionsUpdateRequest request, string tokenUser)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, tokenUser);
            var cmd = new UpdateRequireCompileOptionsCommand(tenantId, siteKey, request.RequireCompile, Guid.NewGuid().ToString("N"), tokenUser, userName);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<CustomProductLocationOptions> GetCustomProductLocationOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default)
        {
            var response = await pncClient.GetCustomProductLocationOptions(tenantId, siteKey, cancellationToken);
            return response.Failed || response.Data == null
                ? default
                : new CustomProductLocationOptions(
                    response.Data.UseCustomProductLocations,
                    response.Data.UpdatedBy,
                    response.Data.UpdatedUtc);
        }

        public async Task<HttpStatusCode> UpdateCustomProductLocationOptions(string tenantId, string siteKey, string tokenUser, CustomProductLocationOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, tokenUser);

            var optionsToUpdate = new CustomProductLocationOptionsUpdateModel(
                request.UseCustomProductLocations);
            var cmd = new UpdateCustomProductLocationOptionsCommand(
                tenantId,
                siteKey,
                Guid.NewGuid().ToString("N"),
                tokenUser,
                userName,
                optionsToUpdate);
            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<ExternalCarrierIdOptions> GetExternalCarrierIdOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default)
        {
            var response = await pncClient.GetExternalCarrierIdOptions(tenantId, siteKey, cancellationToken);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<ExternalCarrierIdOptions>(response.Data);
        }

        public async Task<HttpStatusCode> UpsertSiteExternalCarrierIdOptions(string tenantId, string siteKey, string tokenUser, ExternalCarrierIdOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, tokenUser);
            var optionsToUpdate = mapper.Map<ExternalCarrierIdOptionsUpdateModel>(request);

            var cmd = new UpsertSiteExternalCarrierIdOptionsCommand(
                tenantId,
                siteKey,
                Guid.NewGuid().ToString("N"),
                tokenUser,
                userName,
                optionsToUpdate);

            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<HttpStatusCode> UpsertTenantExternalCarrierIdOptions(string tenantId, string tokenUser, ExternalCarrierIdOptionsUpdateRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, tokenUser);
            var optionsToUpdate = mapper.Map<ExternalCarrierIdOptionsUpdateModel>(request);

            var cmd = new UpsertTenantExternalCarrierIdOptionsCommand(
                tenantId,
                Guid.NewGuid().ToString("N"),
                tokenUser,
                userName,
                optionsToUpdate);

            return await taskCommandSynchronizer.Execute(cmd, tenantId, tenantId);
        }

        public async Task<TrolleyLocationsOptions> GetTrolleyLocationsOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default)
        {
            var response = await pncClient.GetTrolleyLocationsOptions(tenantId, siteKey, cancellationToken);
            return response.Failed || response.Data == null
                ? default
                : mapper.Map<TrolleyLocationsOptions>(response.Data);
        }

        public async Task<HttpStatusCode> UpsertSiteTrolleyLocationsOptions(string tenantId, string siteKey, string tokenUser, TrolleyLocationsOptionsUpsertRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, tokenUser);
            var optionsToUpdate = new TrolleyLocationsOptionsUpdateModel(request.UseTrolleyLocations);

            var cmd = new UpsertSiteTrolleyLocationsOptionsCommand(
                tenantId,
                siteKey,
                Guid.NewGuid().ToString("N"),
                tokenUser,
                userName,
                optionsToUpdate);

            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}|{siteKey}");
        }

        public async Task<HttpStatusCode> UpsertTenantTrolleyLocationsOptions(string tenantId, string tokenUser, TrolleyLocationsOptionsUpsertRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, tokenUser);
            var optionsToUpdate = new TrolleyLocationsOptionsUpdateModel(request.UseTrolleyLocations);

            var cmd = new UpsertTenantTrolleyLocationsOptionsCommand(
                tenantId,
                Guid.NewGuid().ToString("N"),
                tokenUser,
                userName,
                optionsToUpdate);

            return await taskCommandSynchronizer.Execute(cmd, tenantId, tenantId);
        }

        public async Task<LegacyLocationBarcodeSupportOptions> GetLegacyLocationBarcodeSupportOptions(string tenantId, CancellationToken cancellationToken = default)
        {
            var response = await pncClient.GetLegacyLocationBarcodeSupportOptions(tenantId);
            return response.Failed || response.Data == null
                ? default
                : new LegacyLocationBarcodeSupportOptions(response.Data.UseLegacyLocationBarcodeSupport, response.Data.UpdatedBy, response.Data.UpdatedUtc);
        }

        public async Task<HttpStatusCode> UpdateLegacyLocationBarcodeSupportOptions(string tenantId, string tokenUser, UpdateLegacyLocationBarcodeSupportOptionsRequest request)
        {
            var userName = await usersFacade.ResolveUserIdToName(tenantId, tokenUser);
            var optionsToUpdate = new LegacyLocationBarcodeSupportOptionsUpdateModel(request.UseLegacyLocationBarcodeSupport);

            var cmd = new UpdateLegacyLocationBarcodeSupportOptionsCommand(
                tenantId,
                Guid.NewGuid().ToString("N"),
                tokenUser,
                userName,
                optionsToUpdate);

            return await taskCommandSynchronizer.Execute(cmd, tenantId, $"{tenantId}");
        }
    }
}
