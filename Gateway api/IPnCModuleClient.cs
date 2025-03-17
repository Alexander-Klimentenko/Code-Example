using Gateway.Shared.Models;
using Gateway.Shared.Models.PnC.Reasons;
using Gateway.Shared.Models.PnC.Settings;
using Gateway.Shared.Models.PnC.Settings.AppFeatures;
using Gateway.Shared.Models.PnC.Settings.AppUI;
using Gateway.Shared.Models.PnC.Settings.Optimization;
using Project.Models.Shared.PncCore;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Gateway.Shared.Requests;

namespace Gateway.Shared.Interfaces
{
    [Headers("Accept: application/json")]
    public interface IPnCModuleClient
    {
        [Get("/api/Modules/pnc/settings/magicBtnOptions")]
        Task<MagicButtonOptions> GetMagicBtnOptions(CancellationToken cancellationToken = default);

        [Put("/api/Modules/pnc/settings/magicBtnOptions")]
        Task UpsertMagicBtnOptions([Body] MagicButtonOptionsUpdateRequest request);
        
        [Get("/api/Modules/pnc/settings/PicklistOptions")]
        Task<PicklistOptions> GetPicklistOptions();

        [Patch("/api/Modules/pnc/settings/PicklistOptions")]
        Task UpdatePicklistOptions([Body] PicklistOptionsUpdateRequest request);

        [Get("/api/Modules/pnc/settings/PickSlotOptions")]
        Task<PickSlotOptions> GetPickSlotOptions();

        [Patch("/api/Modules/pnc/settings/PickSlotOptions")]
        Task UpdatePickSlotOptions([Body] PickSlotOptionsUpdateRequest request);

        [Get("/api/Modules/pnc/settings/QualityPickingRules")]
        Task<QualityPickingRules> GetQualityPickingRules();

        [Patch("/api/Modules/pnc/settings/QualityPickingRules")]
        Task UpdateQualityPickingRules([Body] QualityPickingRules qualityPickingRules);

        [Get("/api/Modules/pnc/settings/PickingThresholdOptions")]
        Task<PickingThresholdOptions> GetPickingThresholdOptions();

        [Patch("/api/Modules/pnc/settings/PickingThresholdOptions")]
        Task UpdatePickingThresholdOptions([Body] PickingThresholdOptionsUpdateRequest request);

        [Get("/api/Modules/pnc/settings/ManualPickingOptions")]
        Task<ManualPickingOptions> GetManualPickingOptions();

        [Patch("/api/Modules/pnc/settings/ManualPickingOptions")]
        Task UpdateManualPickingOptions([Body] ManualPickingOptionsUpdateRequest request);

        [Get("/api/Modules/pnc/settings/SubstitutionRuleOptions")]
        Task<IEnumerable<SubstitutionRuleOptions>> GetSubstitutionRuleOptions();

        [Patch("/api/Modules/pnc/settings/SubstitutionRuleOptions")]
        Task UpdateSubstitutionRuleOptions(IEnumerable<SubstitutionRuleOptions> options);

        [Get("/api/Modules/pnc/settings/labels")]
        Task<IEnumerable<LabelTemplate>> GetDefaultLabelTemplates();

        [Post("/api/Modules/pnc/settings/labels/preview")]
        Task<LabelPreviewResponse> GetLabelPreview(LabelPreviewRequest labelPreviewRequest);

        [Get("/api/Modules/pnc/settings/labels/selected")]
        Task<IEnumerable<LabelTemplate>> GetLabelTemplates(LabelType? type = default);

        [Put("/api/Modules/pnc/settings/labels/selected")]
        Task UpdateLabelTemplate([Body] LabelTemplate labelTemplate);

        [Delete("/api/Modules/pnc/settings/labels/selected/{labelId}")]
        Task DeleteLabelTemplate(string labelId);


        [Get("/api/Modules/pnc/settings/printers")]
        Task<IEnumerable<Printer>> GetPrinters();

        [Get("/api/Modules/pnc/settings/printers/{id}")]
        Task<Printer> GetPrinter(string id);

        [Put("/api/Modules/pnc/settings/printers/{id}")]
        Task UpsertPrinter(string id, [Body] Printer printer);

        [Delete("/api/Modules/pnc/settings/printers/{id}")]
        Task DeletePrinter(string id);


        [Patch("/api/Modules/pnc/settings/DeliverableAutomationOptions")]
        Task UpdateDeliverableAutomationOptions([Body] DeliverableAutomationOptionsUpdateRequest options);

        [Get("/api/Modules/pnc/settings/DeliverableAutomationOptions")]
        Task<DeliverableAutomationOptions> GetDeliverableAutomationOptions();

        [Patch("/api/Modules/pnc/settings/AppFeatureOptions")]
        Task UpdateAppFeatureOptions([Body] AppFeatureOptionsUpdateRequest request);

        [Get("/api/Modules/pnc/settings/AppFeatureOptions")]
        Task<AppFeatureOptions> GetAppFeatureOptions();

        [Get("/api/Modules/pnc/settings/OrderStateTransitionRules")]
        Task<OrderStateTransitionRules> GetOrderStateTransitionRules();

        [Patch("/api/Modules/pnc/settings/OrderStateTransitionRules")]
        Task UpdateOrderStateTransitionRules([Body] OrderStateTransitionRulesUpdateRequest options);

        [Get("/api/Modules/pnc/settings/AllowAddOrderLineOptions")]
        Task<AllowAddOrderLineOptions> GetAllowAddOrderLineOptions();

        [Patch("/api/Modules/pnc/settings/AllowAddOrderLineOptions")]
        Task UpdateAllowAddOrderLineOptions([Body] AllowAddOrderLineOptionsUpdateRequest request);


        [Get("/api/Modules/pnc/settings/OrderNotificationOptions")]
        Task<OrderNotificationOptions> GetOrderNotificationOptions();

        [Patch("/api/Modules/pnc/settings/OrderNotificationOptions")]
        Task UpdateOrderNotificationOptions([Body] OrderNotificationOptions options);


        [Get("/api/Modules/pnc/settings/SubstitutionLimitationsOptions")]
        Task<SubstitutionLimitationsOptions> SubstitutionLimitationsOptions();

        [Patch("/api/Modules/pnc/settings/SubstitutionLimitationsOptions")]
        Task UpdateSubstitutionLimitationsOptions([Body] SubstitutionLimitationsOptionsUpdateRequest request);

        [Get("/api/Modules/pnc/settings/OverrideCustomerSubstitutionSettingOptions")]
        Task<OverrideCustomerSubstitutionSettingOptions> OverrideCustomerSubstitutionSettingOptions();

        [Patch("/api/Modules/pnc/settings/OverrideCustomerSubstitutionSettingOptions")]
        Task UpdateOverrideCustomerSubstitutionSettingOptions([Body] OverrideCustomerSubstitutionSettingOptionsUpdateRequest request);


        [Get("/api/Modules/pnc/settings/ForcePrintWeightLabelOptions")]
        Task<ForcePrintWeightLabelOptions> ForcePrintWeightLabelOptions();

        [Patch("/api/Modules/pnc/settings/ForcePrintWeightLabelOptions")]
        Task UpdateForcePrintWeightLabelOptions([Body] ForcePrintWeightLabelOptionsUpdateRequest request);


        [Get("/api/Modules/pnc/settings/OrderStatusOnDeviationsOptions")]
        Task<OrderStatusOnDeviationsOptions> OrderStatusOnDeviationsOptions();

        [Patch("/api/Modules/pnc/settings/OrderStatusOnDeviationsOptions")]
        Task UpdateOrderStatusOnDeviationsOptions([Body] OrderStatusOnDeviationsOptionsUpdateRequest request);

        [Get("/api/Modules/pnc/settings/ShowCustomerInfoOptions")]
        Task<ShowCustomerInfoOptions> ShowCustomerInfoOptions();

        [Patch("/api/Modules/pnc/settings/ShowCustomerInfoOptions")]
        Task UpdateShowCustomerInfoOptions([Body] ShowCustomerInfoOptionsUpdateRequest request);

        [Get("/api/Modules/pnc/settings/ForcePickAllLinesOptions")]
        Task<ForcePickAllLinesOptions> ForcePickAllLinesOptions();

        [Patch("/api/Modules/pnc/settings/ForcePickAllLinesOptions")]
        Task UpdateForcePickAllLinesOptions([Body] ForcePickAllLinesOptionsUpdateRequest request);

        [Get("/api/Modules/pnc/settings/PriceToMassOptions")]
        Task<PriceToMassOptions> PriceToMassOptions();

        [Patch("/api/Modules/pnc/settings/PriceToMassOptions")]
        Task UpdatePriceToMassOptions([Body] PriceToMassOptions options);


        [Get("/api/Modules/pnc/settings/CancelOrderInWebOptions")]
        Task<CancelOrderInWebOptions> CancelOrderInWebOptions();

        [Patch("/api/Modules/pnc/settings/CancelOrderInWebOptions")]
        Task UpdateCancelOrderInWebOptions([Body] CancelOrderInWebOptionsUpdateRequest request);

        [Get("/api/Modules/pnc/settings/appevents")]
        Task<AppEventOption> GetAppEventOption();

        [Get("/api/Modules/pnc/settings/AppUIOptions")]
        Task<AppUIOptions> GetAppUIOptions();

        [Patch("/api/Modules/pnc/settings/AppUIOptions")]
        Task UpdateAppUIOptions([Body] AppUIOptionsUpdateRequest request);


        [Get("/api/Modules/pnc/reasons/substitutions")]
        Task<IEnumerable<ProjectSubstitutionReason>> GetSubstitutionReasons(string culture = "en");

        [Get("/api/Modules/pnc/reasons/substitutions/selected")]
        Task<IEnumerable<SubstitutionReason>> GetSelectedSubstitutionReasons(string culture = "en");

        [Patch("/api/Modules/pnc/reasons/substitutions/selected")]
        Task<bool> UpdateSelectedSubstitutionReasons([Body] IEnumerable<SubstitutionReason> substitutionReasons);

        [Delete("/api/Modules/pnc/reasons/substitutions/{code}")]
        Task DeleteSelectedSubstitutionReason(string code);

        [Get("/api/Modules/pnc/reasons/deviations")]
        Task<IEnumerable<ProjectPickingDeviationReason>> GetDeviationReasons(string culture = "en");

        [Get("/api/Modules/pnc/reasons/deviations/selected")]
        Task<IEnumerable<PickingDeviationReason>> GetSelectedDeviationReasons(string culture = "en");

        [Patch("/api/Modules/pnc/reasons/deviations/selected")]
        Task<bool> UpdateSelectedDeviationReasons([Body] IEnumerable<PickingDeviationReason> pickingDeviationReasons);

        [Delete("/api/Modules/pnc/reasons/deviations/{code}")]
        Task DeleteSelectedDeviationReason(string code);

        [Get("/api/Modules/pnc/settings/approveDeviationsOptions")]
        Task<ApproveDeviationsOptions> GetApproveDeviationsOptions();

        [Put("/api/Modules/pnc/settings/approveDeviationsOptions")]
        Task UpdateApproveDeviationsOptions([Body] ApproveDeviationsOptionsUpdateRequest request);

        [Get("/api/Modules/pnc/settings/ConfirmExpirationDateOptions")]
        [Obsolete]
        Task<ConfirmExpirationDateOptions> GetConfirmExpirationDateOptions();

        [Get("/api/Modules/pnc/settings/OverrideExpirationDateOptions")]
        Task<OverrideExpirationDateOptions> GetOverrideExpirationDateOptions();

        [Put("/api/Modules/pnc/settings/OverrideExpirationDateOptions")]
        Task UpdateOverrideExpirationDateOptions([Body] OverrideExpirationDateOptionsUpdateRequest request);

        [Get("/api/Modules/pnc/settings/RegisterLoadCarriersOptions")]
        Task<RegisterLoadCarriersOptions> GetRegisterLoadCarriersOptions();

        [Put("/api/Modules/pnc/settings/RegisterLoadCarriersOptions")]
        Task UpdateRegisterLoadCarriersOptions([Body] RegisterLoadCarriersOptionsUpdateRequest request);
        
        [Get("/api/Modules/pnc/settings/RequireLoadCarrierOptions")]
        Task<RequireLoadCarrierOptions> GetRequireLoadCarrierOptions();

        [Put("/api/Modules/pnc/settings/RequireLoadCarrierOptions")]
        Task UpdateRequireLoadCarrierOptions([Body] RequireLoadCarrierOptionsUpdateRequest request);

        [Obsolete("This is here for backwards compability from APP version 1.4.6, use magic button options")]
        [Get("/api/Modules/pnc/strategies/selection/selected")]
        Task<SelectedStrategy> GetSelectedSelectionStrategy();

        [Obsolete("This is here for backwards compability from APP version 1.4.6, use magic button options")]
        [Get("/api/Modules/pnc/strategies/optimization/selected")]
        Task<SelectedStrategy> GetSelectedOptimizationStrategy();

        [Obsolete]
        [Get("/api/Modules/pnc/settings/PickResultQRCodeSettings")]
        Task<PickResultQRCodeSettings> PickResultQRCodeSettings();

        [Put("/api/Modules/pnc/settings/RequireCompileOptions")]
        Task<HttpResponseMessage> UpdateRequireCompileOptions([Body] RequireCompileOptionsUpdateRequest request);

        [Get("/api/Modules/pnc/settings/RequireCompileOptions")]
        Task<RequireCompileOptions> GetRequireCompileOptions();

        [Put("/api/Modules/pnc/settings/CustomProductLocationOptions")]
        Task<HttpResponseMessage> UpdateCustomProductLocationOptions([Body] CustomProductLocationOptionsUpdateRequest options);

        [Get("/api/Modules/pnc/settings/CustomProductLocationOptions")]
        Task<CustomProductLocationOptions> GetCustomProductLocationOptions();

        [Get("/api/Modules/pnc/settings/ExternalCarrierIdOptions")]
        Task<ExternalCarrierIdOptions> GetExternalCarrierIdOptions();

        [Put("/api/Modules/pnc/settings/ExternalCarrierIdOptions")]
        Task<HttpResponseMessage> UpsertExternalCarrierIdOptions([Body] ExternalCarrierIdOptionsUpdateRequest options);

        [Get("/api/Modules/pnc/settings/TrolleyLocationsOptions")]
        Task<TrolleyLocationsOptions> GetTrolleyLocationsOptions();

        [Put("/api/Modules/pnc/settings/TrolleyLocationsOptions")]
        Task<HttpResponseMessage> UpsertTrolleyLocationsOptions([Body] TrolleyLocationsOptionsUpsertRequest options);

        [Get("/api/Modules/pnc/settings/LegacyLocationBarcodeSupportOptions")]
        Task<LegacyLocationBarcodeSupportOptions> GetLegacyLocationBarcodeSupportOptions();

        [Put("/api/Modules/pnc/settings/LegacyLocationBarcodeSupportOptions")]
        Task<HttpResponseMessage> UpdateLegacyLocationBarcodeSupportOptions([Body] UpdateLegacyLocationBarcodeSupportOptionsRequest options);
    }
}
