using Api.Shared.Interfaces;
using Api.Shared.Models;
using Api.Shared.Models.PickingStrategy;
using Gateway.Shared.Interfaces;
using Gateway.Shared.Models;
using Gateway.Shared.Models.PnC.Settings;
using Gateway.Shared.Models.PnC.Settings.AppFeatures;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Project.Models.Shared.PncCore;
using Spa.Client.Services.Toast;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gateway.Shared.Models.PnC.Settings.AppUI;
using Gateway.Shared.Models.PnC.Settings.Optimization;
using Gateway.Shared.Requests;
using MagicButtonOptions = Gateway.Shared.Models.PnC.Settings.MagicButtonOptions;

namespace Spa.Client.Facades
{
    public interface ISettingsFacade
    {
        Task<ApiResponse<AppFlowOptions>> GetAppFlowOptions();

        Task<ApiResponse<AppUIOptions>> GetAppUIOptions();

        Task<ApiResponse<AppFeatureOptions>> GetAppFeatureOptions();

        Task<ApiResponse> UpdateAppUIOptions(AppUIOptionsUpdateRequest request);

        Task<ApiResponse> UpdateAppFeatureOptions(bool disableSearchByTextOrScanWhenSubstituting, bool disableUserCommentForDeviation);

        Task<ApiResponse> UpsertAppFlowOptions(AppFlowOptionsUpdateRequest request);

        Task<ApiResponse<PickSlotOptions>> GetPickSlotOptions();

        Task<ApiResponse> UpdatePickSlotOptions(PickSlotOptionsUpdateRequest request);

        Task<ApiResponse<PicklistOptions>> GetPicklistOptions();

        Task<ApiResponse> UpdatePicklistOptions(PicklistOptionsUpdateRequest request);

        Task<ApiResponse<QualityPickingRules>> GetQualityPickingRules();

        Task<ApiResponse> UpdateQualityPickingRules(QualityPickingRules qualityPickingRules);

        Task<ApiResponse<ManualPickingOptions>> GetManualPickingOptions();

        Task<ApiResponse> UpdateManualPickingOptions(ManualPickingOptionsUpdateRequest request);

        Task<ApiResponse<PickingThresholdOptions>> GetPickingThresholdOptions();

        Task<ApiResponse> UpdatePickingThresholdOptions(PickingThresholdOptionsUpdateRequest request);

        Task<ApiResponse<IEnumerable<SubstitutionRuleOptions>>> GetSubstitutionRuleOptions();

        Task<ApiResponse> UpdateSubstitutionRules(IEnumerable<SubstitutionRuleOptions> options);

        Task<ApiResponse<IEnumerable<LabelTemplate>>> GetDefaultLabelTemplates();

        Task<ApiResponse<LabelPreviewResponse>> GetLabelPreview(LabelPreviewRequest labelPreviewRequest);

        Task<ApiResponse<IEnumerable<LabelTemplate>>> GetSelectedLabelTemplates(LabelType? type = default);

        Task<ApiResponse> UpdateLabelTemplate(LabelTemplate labelTemplate);

        Task<ApiResponse> DeleteLabelTemplate(string labelId);

        Task<ApiResponse<DeliverableAutomationOptions>> GetDeliverableAutomationOptions();

        Task<ApiResponse> UpdateDeliverableAutomationOptions(DeliverableAutomationOptionsUpdateRequest request);

        Task<ApiResponse<OrderNotificationOptions>> GetOrderNotificationOptions();

        Task<ApiResponse> UpdateOrderNotificationOptions(int newOrderTtl);

        Task<ApiResponse<SubstitutionLimitationsOptions>> GetSubstitutionLimitationsOptions();

        Task<ApiResponse> UpdateSubstitutionLimitationsOptions(SubstitutionLimitationsMode mode);

        Task<ApiResponse<MagicButtonOptions>> GetMagicButtonSettings();

        Task<ApiResponse> UpdateMagicButtonOptions(MagicButtonOptionsUpdateRequest request);

        Task<ApiResponse<ForcePrintWeightLabelOptions>> GetForcePrintWeightLabelOptions();

        Task<ApiResponse> UpdateForcePrintWeightLabelOptions(bool forcePrintWeightLabels);

        Task<ApiResponse<OrderStatusOnDeviationsOptions>> GetOrderStatusOnDeviationsOptions();

        Task<ApiResponse> UpdateOrderStatusOnDeviationsOptions(OrderStatusOnDeviationsOptionsUpdateRequest request);

        Task<ApiResponse<ShowCustomerInfoOptions>> GetShowCustomerInfoOptions();

        Task<ApiResponse> UpdateShowCustomerInfoOptions(bool showCustomerInfoOptions);

        Task<ApiResponse<ForcePickAllLinesOptions>> GetForcePickAllLinesOptions();

        Task<ApiResponse> UpdateForcePickAllLinesOptions(bool pickAllLines);

        Task<ApiResponse<IEnumerable<Gateway.Shared.Models.PnC.Reasons.PickingDeviationReason>>> GetDeviationReasons(string culture = "en");

        Task<ApiResponse<IEnumerable<Gateway.Shared.Models.PnC.Reasons.SubstitutionReason>>> GetSubstitutionReasons(string culture = "en");

        Task<ApiResponse<CancelOrderInWebOptions>> GetCancelOrderInWebOptions();

        Task<ApiResponse<ApproveDeviationsOptions>> GetApproveDeviationsOptions();

        Task<ApiResponse> UpdateApproveDeviationsOptions(ApproveDeviationsOptionsUpdateRequest request);


        #region PickingStrategies
#nullable enable

        Task<ApiResponse<IEnumerable<PickingStrategy>>> GetPickingStrategies(string? layoutId = default);

        Task<ApiResponse<PickingStrategy>> GetPickingStrategy(string pickingStrategyId, string? layoutId);

        Task<ApiResponse<TagRanking>> GetTagRanking(string layoutId);

        Task<ApiResponse<IEnumerable<TagRanking>>> GetAllTagRankings();

        Task<ApiResponse> UpsertPickingStrategies(IEnumerable<PickingStrategy> pickingStrategies);

        Task<ApiResponse> DeletePickingStrategies(IEnumerable<string> pickingStrategyIds);

        Task<ApiResponse> UpsertTagRanking(TagRanking tagRanking);

#nullable disable
        #endregion

        Task<ApiResponse<PriceToMassOptions>> GetPriceToMassOptions();

        Task<ApiResponse> UpdatePriceToMassOptions(PriceToMassOptions options);

        Task<ApiResponse<OverrideExpirationDateOptions>> GetOverrideExpirationDateOptions();

        Task<ApiResponse> UpdateOverrideExpirationDateOptions(bool overrideExpirationDate);

        Task<ApiResponse<RegisterLoadCarriersOptions>> GetRegisterLoadCarriersOptions();

        Task<ApiResponse> UpdateRegisterLoadCarriersOptions(RegisterLoadCarriersOptionsUpdateRequest request);

        Task<ApiResponse<RequireLoadCarrierOptions>> GetRequireLoadCarrierOptions();

        Task<ApiResponse> UpdateRequireLoadCarrierOptions(RequireLoadCarrierOptionsUpdateRequest request);

        Task<ApiResponse<OverrideCustomerSubstitutionSettingOptions>> GetOverrideCustomerSubstitutionSettings();

        Task<ApiResponse> UpdateOverrideCustomerSubstitutionSettings(OverrideCustomerSubstitutionSettingOptionsUpdateRequest request);

        Task<ApiResponse<RequireCompileOptions>> GetRequireCompileOptions();

        Task<ApiResponse> UpdateRequireCompileOptions(RequireCompileOptionsUpdateRequest request);

        Task<ApiResponse<CustomProductLocationOptions>> GetCustomProductLocationsOptions();

        Task<ApiResponse> UpdateCustomProductLocationsOptions(CustomProductLocationOptionsUpdateRequest request);

        Task<ApiResponse<ExternalCarrierIdOptions>> GetExternalCarrierIdOptions();

        Task<ApiResponse> UpsertExternalCarrierIdOptions(ExternalCarrierIdOptionsUpdateRequest request);

        Task<ApiResponse<TrolleyLocationsOptions>> GetTrolleyLocationsOptions();

        Task<ApiResponse> UpsertTrolleyLocationsOptions(TrolleyLocationsOptionsUpsertRequest request);

        Task<ApiResponse<AllowAddOrderLineOptions>> GetAllowAddOrderLineOptions();
    }

    public class SettingsFacade : ISettingsFacade
    {
        private readonly ILogger<SettingsFacade> logger;

        private readonly ToastService toastService;

        private readonly NavigationManager navigationManager;

        private readonly IPnCModuleClient client;

        private readonly Pnc.Api.Shared.Interfaces.ISiteClient settingsClient;

        private readonly IPickingStrategyClient strategyClient;

        public const string AddedOrderLineDeviationReasonCode = "DevReason4";

        public SettingsFacade(
            ILogger<SettingsFacade> logger,
            ToastService toastService,
            NavigationManager navigationManager,
            IPnCModuleClient client,
            Pnc.Api.Shared.Interfaces.ISiteClient settingsClient,
            IPickingStrategyClient strategyClient)
        {
            this.logger = logger;
            this.toastService = toastService;
            this.navigationManager = navigationManager;
            this.client = client;
            this.settingsClient = settingsClient;
            this.strategyClient = strategyClient;
        }

        public async Task<ApiResponse<ApproveDeviationsOptions>> GetApproveDeviationsOptions()
        {
            try
            {
                var opts = await client.GetApproveDeviationsOptions();
                return new ApiResponse<ApproveDeviationsOptions>(opts, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<ApproveDeviationsOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateApproveDeviationsOptions(ApproveDeviationsOptionsUpdateRequest request)
        {
            try
            {
                await client.UpdateApproveDeviationsOptions(request);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<PickSlotOptions>> GetPickSlotOptions()
        {
            try
            {
                var opts = await client.GetPickSlotOptions();
                return new ApiResponse<PickSlotOptions>(opts, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<PickSlotOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdatePickSlotOptions(PickSlotOptionsUpdateRequest request)
        {
            try
            {
                await client.UpdatePickSlotOptions(request);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<PicklistOptions>> GetPicklistOptions()
        {
            try
            {
                var opts = await client.GetPicklistOptions();
                return new ApiResponse<PicklistOptions>(opts, ApiResponseType.Ok);
            }
            catch (Exception apiEx)
            {
                return ApiErrorHandler<PicklistOptions>.Handle(apiEx, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdatePicklistOptions(PicklistOptionsUpdateRequest request)
        {
            try
            {
                await client.UpdatePicklistOptions(request);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception apiEx)
            {
                return ApiErrorHandler.Handle(apiEx, logger, toastService, navigationManager);
            }
        }


        public async Task<ApiResponse<QualityPickingRules>> GetQualityPickingRules()
        {
            try
            {
                var opts = await client.GetQualityPickingRules();
                return new ApiResponse<QualityPickingRules>(opts, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<QualityPickingRules>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateQualityPickingRules(QualityPickingRules qualityPickingRules)
        {
            try
            {
                await client.UpdateQualityPickingRules(qualityPickingRules);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<ManualPickingOptions>> GetManualPickingOptions()
        {
            try
            {
                var opts = await client.GetManualPickingOptions();
                return new ApiResponse<ManualPickingOptions>(opts, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<ManualPickingOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateManualPickingOptions(ManualPickingOptionsUpdateRequest request)
        {
            try
            {
                await client.UpdateManualPickingOptions(request);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<PickingThresholdOptions>> GetPickingThresholdOptions()
        {
            try
            {
                var opts = await client.GetPickingThresholdOptions();
                return new ApiResponse<PickingThresholdOptions>(opts, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<PickingThresholdOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdatePickingThresholdOptions(PickingThresholdOptionsUpdateRequest request)
        {
            try
            {
                await client.UpdatePickingThresholdOptions(request);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<IEnumerable<SubstitutionRuleOptions>>> GetSubstitutionRuleOptions()
        {
            try
            {
                var opts = await client.GetSubstitutionRuleOptions();
                return new ApiResponse<IEnumerable<SubstitutionRuleOptions>>(opts, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<IEnumerable<SubstitutionRuleOptions>>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateSubstitutionRules(IEnumerable<SubstitutionRuleOptions> options)
        {
            try
            {
                await client.UpdateSubstitutionRuleOptions(options);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<IEnumerable<LabelTemplate>>> GetDefaultLabelTemplates()
        {
            try
            {
                var templates = await client.GetDefaultLabelTemplates();
                return new ApiResponse<IEnumerable<LabelTemplate>>(templates, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<IEnumerable<LabelTemplate>>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<LabelPreviewResponse>> GetLabelPreview(LabelPreviewRequest labelPreviewRequest)
        {
            try
            {
                var response = await client.GetLabelPreview(labelPreviewRequest);
                return new ApiResponse<LabelPreviewResponse>(response, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<LabelPreviewResponse>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<IEnumerable<LabelTemplate>>> GetSelectedLabelTemplates(LabelType? type = default)
        {
            try
            {
                var templates = await client.GetLabelTemplates(type);
                return new ApiResponse<IEnumerable<LabelTemplate>>(templates, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<IEnumerable<LabelTemplate>>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateLabelTemplate(LabelTemplate labelTemplate)
        {
            try
            {
                await client.UpdateLabelTemplate(labelTemplate);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> DeleteLabelTemplate(string labelId)
        {
            try
            {
                await client.DeleteLabelTemplate(labelId);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<AppFlowOptions>> GetAppFlowOptions()
        {
            try
            {
                var opts = await settingsClient.GetAppFlowOptions();
                return new ApiResponse<AppFlowOptions>(opts, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<AppFlowOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpsertAppFlowOptions(AppFlowOptionsUpdateRequest request)
        {
            try
            {
                var updated = await settingsClient.UpsertAppFlowOptions(request);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateDeliverableAutomationOptions(DeliverableAutomationOptionsUpdateRequest request)
        {
            try
            {
                await client.UpdateDeliverableAutomationOptions(request);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<DeliverableAutomationOptions>> GetDeliverableAutomationOptions()
        {
            try
            {
                var options = await client.GetDeliverableAutomationOptions();
                return new ApiResponse<DeliverableAutomationOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<DeliverableAutomationOptions>.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<OrderNotificationOptions>> GetOrderNotificationOptions()
        {
            try
            {
                var options = await client.GetOrderNotificationOptions();
                return new ApiResponse<OrderNotificationOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<OrderNotificationOptions>.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateOrderNotificationOptions(int newOrderTtl)
        {
            try
            {
                await client.UpdateOrderNotificationOptions(new OrderNotificationOptions(newOrderTtl));
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<MagicButtonOptions>> GetMagicButtonSettings()
        {
            try
            {
                var options = await client.GetMagicBtnOptions();
                return new ApiResponse<MagicButtonOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<MagicButtonOptions>.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateMagicButtonOptions(MagicButtonOptionsUpdateRequest request)
        {
            try
            {
                await client.UpsertMagicBtnOptions(request);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<SubstitutionLimitationsOptions>> GetSubstitutionLimitationsOptions()
        {
            try
            {
                var options = await client.SubstitutionLimitationsOptions();
                return new ApiResponse<SubstitutionLimitationsOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<SubstitutionLimitationsOptions>.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateSubstitutionLimitationsOptions(SubstitutionLimitationsMode mode)
        {
            try
            {
                var request = new SubstitutionLimitationsOptionsUpdateRequest(mode);
                await client.UpdateSubstitutionLimitationsOptions(request);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<OrderStatusOnDeviationsOptions>> GetOrderStatusOnDeviationsOptions()
        {
            try
            {
                var options = await client.OrderStatusOnDeviationsOptions();
                return new ApiResponse<OrderStatusOnDeviationsOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<OrderStatusOnDeviationsOptions>.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateOrderStatusOnDeviationsOptions(OrderStatusOnDeviationsOptionsUpdateRequest request)
        {
            try
            {
                await client.UpdateOrderStatusOnDeviationsOptions(request);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<ForcePrintWeightLabelOptions>> GetForcePrintWeightLabelOptions()
        {
            try
            {
                var options = await client.ForcePrintWeightLabelOptions();
                return new ApiResponse<ForcePrintWeightLabelOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<ForcePrintWeightLabelOptions>.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateForcePrintWeightLabelOptions(bool forcePrintWeightLabels)
        {
            try
            {
                var options = new ForcePrintWeightLabelOptionsUpdateRequest(forcePrintWeightLabels);
                await client.UpdateForcePrintWeightLabelOptions(options);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<ShowCustomerInfoOptions>> GetShowCustomerInfoOptions()
        {
            try
            {
                var options = await client.ShowCustomerInfoOptions();
                return new ApiResponse<ShowCustomerInfoOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<ShowCustomerInfoOptions>.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateShowCustomerInfoOptions(bool showCustomerInfoOptions)
        {
            try
            {
                var options = new ShowCustomerInfoOptionsUpdateRequest(showCustomerInfoOptions);
                await client.UpdateShowCustomerInfoOptions(options);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<ForcePickAllLinesOptions>> GetForcePickAllLinesOptions()
        {
            try
            {
                var options = await client.ForcePickAllLinesOptions();
                return new ApiResponse<ForcePickAllLinesOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<ForcePickAllLinesOptions>.Handle(ex, logger, toastService, navigationManager);
            }
        }
        public async Task<ApiResponse<CancelOrderInWebOptions>> GetCancelOrderInWebOptions()
        {
            try
            {
                var options = await client.CancelOrderInWebOptions();
                return new ApiResponse<CancelOrderInWebOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<CancelOrderInWebOptions>.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateForcePickAllLinesOptions(bool pickAllLines)
        {
            try
            {
                var options = new ForcePickAllLinesOptionsUpdateRequest(pickAllLines);
                await client.UpdateForcePickAllLinesOptions(options);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler.Handle(ex, logger, toastService, navigationManager);
            }
        }



        public async Task<ApiResponse<IEnumerable<Gateway.Shared.Models.PnC.Reasons.PickingDeviationReason>>> GetDeviationReasons(string culture = "en")
        {
            try
            {
                var deviationReasons = await client.GetSelectedDeviationReasons(culture);
                return new ApiResponse<IEnumerable<Gateway.Shared.Models.PnC.Reasons.PickingDeviationReason>>(deviationReasons, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<IEnumerable<Gateway.Shared.Models.PnC.Reasons.PickingDeviationReason>>.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<IEnumerable<Gateway.Shared.Models.PnC.Reasons.SubstitutionReason>>> GetSubstitutionReasons(string culture = "en")
        {
            try
            {
                var substitutionReasons = await client.GetSelectedSubstitutionReasons(culture);
                return new ApiResponse<IEnumerable<Gateway.Shared.Models.PnC.Reasons.SubstitutionReason>>(substitutionReasons, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<IEnumerable<Gateway.Shared.Models.PnC.Reasons.SubstitutionReason>>.Handle(ex, logger, toastService, navigationManager);
            }
        }

        #region PickingStrategies
#nullable enable

        public async Task<ApiResponse<IEnumerable<PickingStrategy>>> GetPickingStrategies(string? layoutId = default)
        {
            try
            {
                var substitutionReasons = await strategyClient.GetPickingStrategies(layoutId);
                return new ApiResponse<IEnumerable<PickingStrategy>>(substitutionReasons, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<IEnumerable<PickingStrategy>>.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<PickingStrategy>> GetPickingStrategy(string pickingStrategyId, string? layoutId)
        {
            try
            {
                var substitutionReasons = await strategyClient.GetPickingStrategy(pickingStrategyId, layoutId);
                return new ApiResponse<PickingStrategy>(substitutionReasons, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<PickingStrategy>.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<TagRanking>> GetTagRanking(string layoutId)
        {
            try
            {
                var substitutionReasons = await strategyClient.GetTagRanking(layoutId);
                return new ApiResponse<TagRanking>(substitutionReasons, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<TagRanking>.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<IEnumerable<TagRanking>>> GetAllTagRankings()
        {
            try
            {
                var tagRankings = await strategyClient.GetAllTagRankings();
                return new ApiResponse<IEnumerable<TagRanking>>(tagRankings, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<IEnumerable<TagRanking>>.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpsertPickingStrategies(IEnumerable<PickingStrategy> pickingStrategies)
        {
            try
            {
                await strategyClient.UpsertPickingStrategies(pickingStrategies);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> DeletePickingStrategies(IEnumerable<string> pickingStrategyIds)
        {
            try
            {
                await strategyClient.DeletePickingStrategies(pickingStrategyIds);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpsertTagRanking(TagRanking tagRanking)
        {
            try
            {
                await strategyClient.UpsertTagRanking(tagRanking);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler.Handle(ex, logger, toastService, navigationManager);
            }
        }


#nullable disable
        #endregion

        public async Task<ApiResponse<PriceToMassOptions>> GetPriceToMassOptions()
        {
            try
            {
                var options = await client.PriceToMassOptions();
                return new ApiResponse<PriceToMassOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<PriceToMassOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdatePriceToMassOptions(PriceToMassOptions options)
        {
            try
            {
                await client.UpdatePriceToMassOptions(options);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<AppUIOptions>> GetAppUIOptions()
        {
            try
            {
                var options = await client.GetAppUIOptions();
                return new ApiResponse<AppUIOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<AppUIOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateAppUIOptions(AppUIOptionsUpdateRequest options)
        {
            try
            {
                await client.UpdateAppUIOptions(options);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<AppFeatureOptions>> GetAppFeatureOptions()
        {
            try
            {
                var options = await client.GetAppFeatureOptions();
                return new ApiResponse<AppFeatureOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler<AppFeatureOptions>.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateAppFeatureOptions(bool disableSearchByTextOrScanWhenSubstituting, bool disableUserCommentForDeviation)
        {
            try
            {
                await client.UpdateAppFeatureOptions(new AppFeatureOptionsUpdateRequest(disableSearchByTextOrScanWhenSubstituting, disableUserCommentForDeviation, default));
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception ex)
            {
                return ApiErrorHandler.Handle(ex, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<OverrideExpirationDateOptions>> GetOverrideExpirationDateOptions()
        {
            try
            {
                var options = await client.GetOverrideExpirationDateOptions();
                return new ApiResponse<OverrideExpirationDateOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<OverrideExpirationDateOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateOverrideExpirationDateOptions(bool overrideExpirationDate)
        {
            try
            {
                var options = new OverrideExpirationDateOptionsUpdateRequest(overrideExpirationDate);
                await client.UpdateOverrideExpirationDateOptions(options);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<RegisterLoadCarriersOptions>> GetRegisterLoadCarriersOptions()
        {
            try
            {
                var options = await client.GetRegisterLoadCarriersOptions();
                return new ApiResponse<RegisterLoadCarriersOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<RegisterLoadCarriersOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateRegisterLoadCarriersOptions(RegisterLoadCarriersOptionsUpdateRequest request)
        {
            try
            {
                await client.UpdateRegisterLoadCarriersOptions(request);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<RequireLoadCarrierOptions>> GetRequireLoadCarrierOptions()
        {
            try
            {
                var options = await client.GetRequireLoadCarrierOptions();
                return new ApiResponse<RequireLoadCarrierOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<RequireLoadCarrierOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateRequireLoadCarrierOptions(RequireLoadCarrierOptionsUpdateRequest request)
        {
            try
            {
                await client.UpdateRequireLoadCarrierOptions(request);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<OverrideCustomerSubstitutionSettingOptions>> GetOverrideCustomerSubstitutionSettings()
        {
            try
            {
                var options = await client.OverrideCustomerSubstitutionSettingOptions();
                return new ApiResponse<OverrideCustomerSubstitutionSettingOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<OverrideCustomerSubstitutionSettingOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateOverrideCustomerSubstitutionSettings(OverrideCustomerSubstitutionSettingOptionsUpdateRequest request)
        {
            try
            {
                await client.UpdateOverrideCustomerSubstitutionSettingOptions(request);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<RequireCompileOptions>> GetRequireCompileOptions()
        {
            try
            {
                var options = await client.GetRequireCompileOptions();
                return new ApiResponse<RequireCompileOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<RequireCompileOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateRequireCompileOptions(RequireCompileOptionsUpdateRequest request)
        {
            try
            {
                var response = await client.UpdateRequireCompileOptions(request);
                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse(ApiResponseType.Error);
                }
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<CustomProductLocationOptions>> GetCustomProductLocationsOptions()
        {
            try
            {
                var options = await client.GetCustomProductLocationOptions();
                return new ApiResponse<CustomProductLocationOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<CustomProductLocationOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpdateCustomProductLocationsOptions(CustomProductLocationOptionsUpdateRequest request)
        {
            try
            {
                await client.UpdateCustomProductLocationOptions(request);
                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<ExternalCarrierIdOptions>> GetExternalCarrierIdOptions()
        {
            try
            {
                var options = await client.GetExternalCarrierIdOptions();
                return new ApiResponse<ExternalCarrierIdOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<ExternalCarrierIdOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpsertExternalCarrierIdOptions(ExternalCarrierIdOptionsUpdateRequest request)
        {
            try
            {
                await client.UpsertExternalCarrierIdOptions(request);

                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<TrolleyLocationsOptions>> GetTrolleyLocationsOptions()
        {
            try
            {
                var options = await client.GetTrolleyLocationsOptions();
                return new ApiResponse<TrolleyLocationsOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<TrolleyLocationsOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse> UpsertTrolleyLocationsOptions(TrolleyLocationsOptionsUpsertRequest request)
        {
            try
            {
                await client.UpsertTrolleyLocationsOptions(request);

                return new ApiResponse(ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler.Handle(e, logger, toastService, navigationManager);
            }
        }

        public async Task<ApiResponse<AllowAddOrderLineOptions>> GetAllowAddOrderLineOptions()
        {
            try
            {
                var options = await client.GetAllowAddOrderLineOptions();
                return new ApiResponse<AllowAddOrderLineOptions>(options, ApiResponseType.Ok);
            }
            catch (Exception e)
            {
                return ApiErrorHandler<AllowAddOrderLineOptions>.Handle(e, logger, toastService, navigationManager);
            }
        }
    }
}
