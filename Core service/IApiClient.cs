using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using PncCore.Shared.Models;
using PncCore.Shared.Models.BarcodePosIntegration;
using PncCore.Shared.Models.PickingStrategy;
using PncCore.Shared.Models.Products;
using PncCore.Shared.Models.QrCode;
using PncCore.Shared.Models.Reasons;
using PncCore.Shared.Models.Requests;
using PncCore.Shared.Models.Responses;
using PncCore.Shared.Models.Settings;
using PncCore.Shared.Models.Settings.AppFeatures;
using PncCore.Shared.Models.Settings.AppUI;
using PncCore.Shared.Models.Settings.Optimization;
using Project.Models.Shared.PncCore;
using Project.Models.Shared.Product_v1_27;
using Refit;
using PickOrderLineState = Project.Models.Shared.PncCore.PickOrderLineState;

#nullable enable

namespace PncCore.Shared.Interfaces
{
    public partial interface IApiClient
    {
        [Obsolete]
        [Get("/api/Picklists")]
        Task<QueryResult<Picklist>> GetPicklists([Query] string tenantId, [Query] string siteKey, [Query] DateTime? fromDate = null, [Query] DateTime? toDate = null, [Query(CollectionFormat.Multi)] PickinglistState[]? states = null, [Query(CollectionFormat.Multi)] string[]? userIds = null, [Query] int limit = 10, [Query] int offset = 0, CancellationToken cancellationToken = default);

        [Get("/api/Picklists/count")]
        Task<int> GetPicklistsCount([Query] string tenantId, [Query] string siteKey, [Query] DateTime? fromDate = null, [Query] DateTime? toDate = null, [Query(CollectionFormat.Multi)] PickinglistState[]? states = null, [Query(CollectionFormat.Multi)] string[]? userIds = null, CancellationToken cancellationToken = default);

        [Obsolete]
        [Get("/api/Picklists/{picklistId}")]
        Task<Picklist> GetPicklist(string picklistId, [Query] string tenantId, [Query] string siteKey);

        [Get("/api/Picklists/{id}/uievents")]
        Task<IEnumerable<Project.Models.Shared.Core_v1_0.UILogEntry>> GetPickListUIEvents(string id, [Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Obsolete]
        [Get("/api/PickOrder/order/{pickorderId}")]
        Task<PickOrder> GetPickOrder(string pickorderId, [Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Obsolete]
        [Get("/api/PickOrder/orders")]
        Task<IEnumerable<PickOrder>> GetPickOrders([Query(CollectionFormat.Multi)] string[] pickorderIds, [Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Obsolete]
        [Post("/api/PickOrder/search")]
        Task<QueryResult<PickOrder>> SearchPickOrders([Query] string tenantId, [Query] string siteKey, [Body] PickOrderSearchBody searchBody, [Query] int limit = 10, [Query] int offset = 0, CancellationToken cancellationToken = default);

        [Get("/api/PickOrder/{orderId}/result")]
        Task<PickOrderResult> GetPickOrderResult(string orderId, [Query] string siteKey, [Query] string tenantId);

        [Get("/api/PickOrder/results")]
        Task<IEnumerable<PickOrderResult>> GetPickOrderResults([Query(CollectionFormat.Multi)] string[] id, [Query] string siteKey, [Query] string tenantId);

        [Get("/api/PickOrder/{orderId}/summary")]
        Task<PickOrderScanSummary> GetPickOrderScanSummary(string orderId, [Query] string siteKey, [Query] string tenantId);

        [Get("/api/PickOrder/summaries")]
        Task<IEnumerable<PickOrderScanSummary>> GetPickOrderScanSummaries([Query(CollectionFormat.Multi)] string[] id, [Query] string siteKey, [Query] string tenantId);

        [Get("/api/PickOrder/{id}/uievents")]
        Task<IEnumerable<Project.Models.Shared.Core_v1_0.UILogEntry>> GetPickOrderUIEvents(string id, [Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Get("/api/PickOrder/{pickorderId}/connectedloadcarriers")]
        Task<IEnumerable<ConnectedLoadCarrier>> GetPickOrderConnectedLoadCarriers(string pickorderId, [Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Patch("/api/PickOrder/{pickorderId}/loadcarrierseries")]
        Task<int?> PatchPickOrderLoadCarrierSeries(string pickorderId, [Query] string tenantId, [Query] string siteKey, [Query] string userId);

        [Post("/api/PickOrder/LoadCarrierEstimation")]
        Task<IEnumerable<LoadCarrierEstimationResponse>> GetLoadCarrierEstimations([Query] string tenantId, [Query] string siteKey, [Body] LoadCarrierEstimationRequest request, CancellationToken cancellationToken = default);

        [Get("/api/PickOrderLines/Search")]
        Task<PickOrderLineSearch> SearchPickOrderLines(
            [Query] string tenantId,
            [Query] string siteKey,
            [Query(CollectionFormat.Multi)] string[]? zoneId = null,
            [Query(CollectionFormat.Multi)] PickOrderLineState[]? state = null,
            [Query] DateTime? fromDateTime = null,
            [Query] DateTime? toDateTime = null,
            [Query] string? supplier = null,
            [Query(CollectionFormat.Multi)] string[]? category = null,
            [Query] string? searchString = null,
            [Query] int limit = 10,
            [Query] int offset = 0, CancellationToken cancellationToken = default);

        [Post("/api/PickOrderLines/SearchDeviations")]
        Task<DeviatingPickOrderLineSearch> SearchDeviatingPickOrderLines(
            [Query] string tenantId,
            [Query] string siteKey,
            [Body] DeviatingPickOrderLineSearchBody body,
            [Query] int limit = 10,
            [Query] int offset = 0, CancellationToken cancellationToken = default);

        [Post("/api/PickOrderLines/Products")]
        Task<IEnumerable<PickOrderLineProduct>> GetOrderLineProducts(
            [Query] string tenantId,
            [Query] string siteKey,
            [Body] PickOrderLinesProductRequest body,
            CancellationToken cancellationToken = default);

        [Post("/api/PickOrderLines/ZoneTotalQuantities")]
        Task<IEnumerable<ZoneTotalQuantityResult>> GetZoneTotalQuantities(
            [Query] string tenantId,
            [Query] string siteKey,
            [Body] PickOrderLinesZoneQuantityRequest body,
            CancellationToken cancellationToken = default);

        [Get("/api/PickOrderLines/Products/Suppliers")]
        Task<IEnumerable<string>> GetSuppliersFromDate([Query] string tenantId, [Query] string siteKey, [Query] DateTime date, CancellationToken cancellationToken = default);

        [Get("/api/PickOrderLines/PickSlot")]
        Task<PickOrderLinePickSlotQueryResult> GetAffectedPickOrderLinesFromPickSlotQuery([Query] string tenantId, [Query] string siteKey, [Query(CollectionFormat.Multi)] string[] slotAliases, [Query] DateTime pickSlotStartUtc, [Query] DateTime pickSlotCompleteUtc, [Query(CollectionFormat.Multi)] string[]? zoneId = default, [Query(CollectionFormat.Multi)] string[]? category = default, CancellationToken cancellationToken = default);

        [Get("/api/Settings/MagicButtonOptions")]
        Task<MagicButtonOptions> MagicButtonOptions([Query] string tenantId, [Query] string? siteKey = null);

        [Get("/api/Settings/ApproveDeviationsOptions")]
        Task<ApproveDeviationsOptions> GetApproveDeviationsOptions([Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/Settings/RegisterLoadCarriersOptions")]
        Task<RegisterLoadCarriersOptions> GetRegisterLoadCarriersOptions([Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/Settings/ManualPickingOptions")]
        Task<ManualPickingOptions> ManualPickingOptions([Query] string tenantId, [Query] string? siteKey = null);

        [Get("/api/Settings/pickSlotOptions")]
        Task<PickSlotOptions> PickSlotOptions([Query] string tenantId, [Query] string? siteKey = null);

        [Get("/api/Settings/picklistOptions")]
        Task<PicklistOptions> PicklistOptions([Query] string tenantId, [Query] string? siteKey = null);

        [Get("/api/Settings/sitePicklistOptions")]
        Task<IDictionary<string, PicklistOptions>> PicklistOptions([Query] string tenantId);

        [Get("/api/Settings/PickingThresholdOptions")]
        Task<PickingThresholdOptions> PickingThresholdOptions([Query] string tenantId, [Query] string? siteKey = null);

        [Get("/api/Settings/QualityPickingRules")]
        Task<QualityPickingRules> QualityPickingRules([Query] string tenantId, [Query] string? siteKey = null);

        [Get("/api/Settings/Labels")]
        Task<IEnumerable<LabelTemplate>> GetDefaultLabelTemplates();

        [Get("/api/Settings/Labels/selected")]
        Task<IEnumerable<LabelTemplate>> GetLabelTemplates([Query] string tenantId, [Query] string? siteKey = default, [Query] LabelType? type = default, CancellationToken cancellationToken = default);

        [Get("/api/Settings/DeliveryNotes")]
        Task<IEnumerable<string>> GetDeliveryNotes([Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Get("/api/Settings/DeliveryNote/{id}")]
        Task<DeliveryNoteSettings> GetDeliveryNote(string id, [Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Get("/api/Settings/DeliveryNote/Active")]
        Task<DeliveryNoteSettings> GetActiveDeliveryNote([Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Get("/api/Settings/TransportDocument")]
        Task<TransportDocumentSettings> GetTransportDocument([Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Get("/api/Settings/AppFlowOptions")]
        Task<AppFlowOptions> AppFlowOptions([Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Get("/api/Settings/AppUIOptions")]
        Task<AppUIOptions> AppUIOptions([Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/Settings/DeliverableAutomationOptions")]
        Task<DeliverableAutomationOptions> DeliverableAutomationOptions([Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/Settings/AppFeatureOptions")]
        Task<AppFeatureOptions> AppFeatureOptions([Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/Settings/OrderStateTransitionRules")]
        Task<OrderStateTransitionRules> OrderStateTransitionRules([Query] string tenantId, CancellationToken cancellationToken = default);

        [Get("/api/Settings/AllowAddOrderLineOptions")]
        Task<AllowAddOrderLineOptions> AllowAddOrderLineOptions([Query] string tenantId, CancellationToken cancellationToken = default);

        [Get("/api/Settings/OrderNotificationOptions")]
        Task<OrderNotificationOptions> OrderNotificationOptions([Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/Settings/SubstitutionLimitationsOptions")]
        Task<SubstitutionLimitationsOptions> SubstitutionLimitationsOptions([Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/Settings/OverrideCustomerSubstitutionSettingOptions")]
        Task<OverrideCustomerSubstitutionSettingOptions> OverrideCustomerSubstitutionSettingOptions([Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/Settings/ForcePrintWeightLabelOptions")]
        Task<ForcePrintWeightLabelOptions> ForcePrintWeightLabelOptions([Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/Settings/OrderStatusOnDeviationsOptions")]
        Task<OrderStatusOnDeviationsOptions> OrderStatusOnDeviationsOptions([Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/Settings/ShowCustomerInfoOptions")]
        Task<ShowCustomerInfoOptions> ShowCustomerInfoOptions([Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/Settings/ForcePickAllLinesOptions")]
        Task<ForcePickAllLinesOptions> ForcePickAllLinesOptions([Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/Settings/CancelOrderInWebOptions")]
        Task<CancelOrderInWebOptions> CancelOrderInWebOptions([Query] string tenantId, CancellationToken cancellationToken = default);

        [Get("/api/Settings/PriceToMasssOptions")]
        Task<PriceToMassOptions> PriceToMassOptions([Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/Settings/OverrideExpirationDateOptions")]
        Task<OverrideExpirationDateOptions> OverrideExpirationDateOptions([Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/Reasons/substitutions")]
        Task<IEnumerable<SubstitutionReason>> SubstitutionReasons();

        [Get("/api/Reasons/substitutions/{id}")]
        Task<SubstitutionReason> SubstitutionReason(string id);

        [Get("/api/Reasons/selected/substitutions")]
        Task<IEnumerable<SubstitutionReason>> SelectedSubstitutionReasons([Query] string tenantId, [Query] string? siteKey = null);

        [Get("/api/Reasons/deviations")]
        Task<IEnumerable<PickingDeviationReason>> DeviationReasons();

        [Get("/api/Reasons/deviations/{id}")]
        Task<PickingDeviationReason> DeviationReason(string id);

        [Get("/api/Reasons/selected/deviations")]
        Task<IEnumerable<PickingDeviationReason>> SelectedDeviationReason([Query] string tenantId, [Query] string? siteKey = null);

        [Get("/api/Slots")]
        Task<IEnumerable<Project.Models.Shared.Core_v1_0.Slot>> GetOrderSlots([Query] string siteKey, [Query] string tenantId, [Query] DateTimeOffset start, [Query] DateTimeOffset? end, [Query] string timeZoneId = "W. Europe Standard Time", CancellationToken cancellationToken = default);

        [Get("/api/Slots/PickSlotQuery")]
        Task<IEnumerable<Project.Models.Shared.Core_v1_0.Slot>> GetOrderSlotsMatchingPickSlot([Query] DateTime startUtc, [Query] DateTime completeUtc, [Query] string siteKey, [Query] string tenantId, CancellationToken cancellationToken = default);

        [Get("/api/PickSlots")]
        Task<IEnumerable<Project.Models.Shared.Core_v1_0.PickSlot>> GetPickSlots([Query] string siteKey, [Query] string tenantId, [Query] DateTimeOffset start, [Query] DateTimeOffset? end, [Query] string timeZoneId = "W. Europe Standard Time", CancellationToken cancellationToken = default);

        [Get("/api/PickSlotsData")]
        Task<IEnumerable<PickSlotData>> GetPickSlotsData([Query] string siteKey, [Query] string tenantId, [Query] DateTimeOffset start, [Query] DateTimeOffset? end, [Query] string timeZoneId = "W. Europe Standard Time", CancellationToken cancellationToken = default);

        [Get("/api/PickSlots/OrderSlotQuery")]
        Task<IEnumerable<Project.Models.Shared.Core_v1_0.PickSlot>> GetPickSlotsMatchingOrderSlot([Query] DateTime startUtc, [Query] DateTime endUtc, [Query] string siteKey, [Query] string tenantId, CancellationToken cancellationToken = default);

        [Get("/api/CollectSlots")]
        Task<IEnumerable<CollectSlot>> GetCollectSlots([Query] string siteKey, [Query] string tenantId, [Query] DateTimeOffset start, [Query] DateTimeOffset? end, [Query] string timeZoneId = "W. Europe Standard Time", CancellationToken cancellationToken = default);

        [Obsolete]
        [Get("/api/TransportSlotsForOrderSlot")]
        Task<IEnumerable<TransportSlotPreview>> GetTransportSlotsForOrderSlot([Query] string siteKey, [Query] string tenantId, [Query] string alias, [Query] DateTime slotStartUtc, CancellationToken cancellationToken = default);

        [Obsolete]
        [Get("/api/TransportSlots")]
        Task<IEnumerable<TransportSlotPreview>> GetTransportSlots([Query] string siteKey, [Query] string tenantId, [Query] DateTimeOffset start, [Query] DateTimeOffset? end, [Query] string timeZoneId = "W. Europe Standard Time", CancellationToken cancellationToken = default);

        [Get("/api/TransportSlots/orders")]
        Task<QueryResult<TransportSlotOrderPreview>> TransportPickOrders([Query] string tenantId, [Query] string siteKey, [Query] DateTime departure, [Query] string name, [Query] int limit = 10, [Query] int offset = 0, CancellationToken cancellationToken = default);

        [Get("/api/Printers")]
        Task<IEnumerable<Printer>> Printers([Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/Printers/{id}")]
        Task<Printer> Printer(string id, [Query] string tenantId, [Query] string? siteKey = null, CancellationToken cancellationToken = default);

        [Get("/api/PickOrder/{orderId}/orderlines/{orderlineId}")]
        Task<PickOrderLine> GetPickOrderLine(string orderId, string orderlineId, [Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Get("/api/PickOrder/orders/qrcodes")]
        Task<QrCodeResponse> GetPickOrdersQrCodes([Query(CollectionFormat.Multi)] string[] pickOrderIds, [Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Get("/api/PickOrder/orders/posintegrationbarcodes")]
        Task<BarcodePosIntegrationResponse> GetPosIntegrationBarcodes([Query(CollectionFormat.Multi)] string[] pickOrderIds, [Query] string tenantId, [Query] string siteKey, [Query] string? timeZoneId, CancellationToken cancellationToken = default);

        [Get("/api/PickingStrategy/strategies")]
        Task<IEnumerable<PickingStrategy>> GetPickingStrategies([Query] string tenantId, [Query] string siteKey, [Query] string? layoutId = null, CancellationToken cancellationToken = default);

        [Get("/api/PickingStrategy/strategies/{pickingStrategyId}")]
        Task<PickingStrategy> GetPickingStrategy([Query] string tenantId, [Query] string siteKey, string pickingStrategyId, [Query] string? layoutId = null, CancellationToken cancellationToken = default);

        [Get("/api/PickingStrategy/{layoutId}/tagranking")]
        Task<TagRanking?> GetTagRanking([Query] string tenantId, [Query] string siteKey, string layoutId, CancellationToken cancellationToken = default);

        [Get("/api/PickingStrategy/tagranking")]
        Task<IEnumerable<TagRanking>> GetAllTagRankings([Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Get("/api/Collectlists")]
        Task<Collectlist> GetCollectlist([Query] string tenantId, [Query] string siteKey, [Query] string collectlistId, CancellationToken cancellationToken = default);

        [Get("/api/Packlists")]
        Task<Packlist> GetPacklist([Query] string tenantId, [Query] string siteKey, [Query] string packlistId, CancellationToken cancellationToken = default);

        [Get("/api/Pickinglists")]
        Task<QueryResult<PickinglistPreview>> GetPickinglistPreviews([Query] string tenantId, [Query] string siteKey, [Query(CollectionFormat.Multi)] string[]? listTypes = null, [Query] DateTime? fromDate = null, [Query] DateTime? toDate = null, [Query(CollectionFormat.Multi)] PickinglistState[]? states = null, [Query(CollectionFormat.Multi)] string[]? userIds = null, [Query] int limit = 10, [Query] int offset = 0, CancellationToken cancellationToken = default);

        [Post("/api/PickOrderLines/Products/Missinglocation")]
        Task<QueryResult<ProductInfo>> GetProductsWithMissingLocation([Query] string tenantId, [Query] string siteKey, [Query] DateTime? fromPickSlotStart = null, [Query] DateTime? toPickSlotEnd = null, [Body] IEnumerable<ProductCategory>? categories = null, [Query] int offset = 0, [Query] int limit = 50, CancellationToken cancellationToken = default);

        [Get("/api/Settings/RequireLoadCarrierOptions")]
        Task<RequireLoadCarrierOptions> RequireLoadCarrierOptions([Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Get("/api/PickingStrategy/strategies/sortings")]
        Task<IEnumerable<string>> SortingIdsInUse([Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Get("/api/PickingStrategy/strategies/sortings/{sortingId}")]
        Task<bool> SortingInUse([Query] string tenantId, [Query] string siteKey, [Query] string sortingId, CancellationToken cancellationToken = default);

        [Get("/api/Products/PickingProperties")]
        Task<IEnumerable<ProductPickingProperties>> GetProductPickingProperties([Query] string tenantId, [Query] string siteKey, bool onlyExpiredCountryOfOrigins = false, CancellationToken cancellationToken = default);


        [Get("/api/Traininglists/{traininglistId}")]
        Task<Traininglist> GetTraininglist([Query] string tenantId, [Query] string siteKey, [Query] string traininglistId, CancellationToken cancellationToken = default);

        [Get("/api/Traininglists")]
        Task<QueryResult<Traininglist>> GetTraininglists([Query] string tenantId, [Query] string siteKey, [Query(CollectionFormat.Multi)] IEnumerable<PickinglistState>? states = null, [Query] int limit = 20, [Query] int offset = 0, CancellationToken cancellationToken = default);

        [Post("/api/Substitutions")]
        Task<IEnumerable<GlobalSubstitution>> GetGlobalSubstitutions([Query] string tenantId, [Query] string siteKey, [Body] IEnumerable<string>? productIds = null, [Query] GlobalSubstitutionQueryType type = GlobalSubstitutionQueryType.Any, [Query] bool onlyActive = true, CancellationToken cancellationToken = default);

        [Post("/api/PickOrderLines/OrderedQuantity")]
        Task<IEnumerable<ProductQuantityResult>> GetOrderedQuantity([Query] string tenantId, [Query] string siteKey, [Query] DateTime fromPickSlotStart, [Query] DateTime toPickSlotStart, [Body] IEnumerable<string>? productIds = null, [Query] bool onlySubstitutionAllowed = false, [Query] bool onlyWithoutPersonalSubstitutions = false, [Query] PickOrderLineState maxState = PickOrderLineState.OnHold, CancellationToken cancellationToken = default);

        [Get("/api/Settings/RequireCompileOptions")]
        Task<RequireCompileOptions> GetRequireCompileOptions([Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Get("/api/Settings/CustomProductLocationOptions")]
        Task<CustomProductLocationOptions> GetCustomProductLocationOptions([Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Get("/api/Settings/ExternalCarrierIdOptions")]
        Task<ExternalCarrierIdOptions> GetExternalCarrierIdOptions([Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Get("/api/Settings/TrolleyLocationsOptions")]
        Task<TrolleyLocationsOptions> GetTrolleyLocationsOptions([Query] string tenantId, [Query] string siteKey, CancellationToken cancellationToken = default);

        [Get("/api/Settings/LegacyLocationBarcodeSupportOptions")]
        Task<LegacyLocationBarcodeSupportOptions> GetLegacyLocationBarcodeSupportOptions([Query] string tenantId, CancellationToken cancellationToken = default);
    }
}