using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using Infrastructure.Communication.Models;
using PncCore.Shared.Models;
using PncCore.Shared.Models.BarcodePosIntegration;
using PncCore.Shared.Models.PickingStrategy;
using PncCore.Shared.Models.QrCode;
using PncCore.Shared.Models.Reasons;
using PncCore.Shared.Models.Settings;
using PickOrderLineState = Project.Models.Shared.PncCore.PickOrderLineState;
using PncCore.Shared.Models.Settings.AppUI;
using Project.Models.Shared.PncCore;
using Project.Models.Shared.Product_v1_27;
using PncCore.Shared.Models.Products;
using PncCore.Shared.Models.Requests;
using PncCore.Shared.Models.Responses;
using PncCore.Shared.Models.Settings.AppFeatures;
using PncCore.Shared.Models.Settings.Optimization;

#nullable enable

namespace PncCore.Shared.Interfaces
{
    public interface IClient
    {
        [Obsolete]
        Task<ClientResponse<QueryResult<Picklist>>> GetPicklists(string tenantId, string siteKey, DateTime? fromDate = null, DateTime? toDate = null, PickinglistState[]? states = null, string[]? userIds = null, int limit = 10, int offset = 0, CancellationToken cancellationToken = default);
        Task<ClientResponse<QueryResult<Models_v1_27.Picklist>>> GetPicklists_v1_1(string tenantId, string siteKey, DateTime? fromDate = null, DateTime? toDate = null, PickinglistState[]? states = null, string[]? userIds = null, int limit = 10, int offset = 0, CancellationToken cancellationToken = default);

        Task<ClientResponse<int>> GetPicklistsCount(string tenantId, string siteKey, DateTime? fromDate = null, DateTime? toDate = null, PickinglistState[]? states = null, string[]? userIds = null, CancellationToken cancellationToken = default);

        [Obsolete]
        Task<ClientResponse<Picklist>> GetPicklist(string picklistId, string tenantId, string siteKey);
        Task<ClientResponse<Models_v1_27.Picklist>> GetPicklist_v1_1(string picklistId, string tenantId, string siteKey);

        [Obsolete]
        Task<ClientResponse<PickOrder>> GetPickOrder(string pickorderId, string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<Models_v1_27.PickOrder>> GetPickOrderv1_1(string pickorderId, string tenantId, string siteKey, CancellationToken cancellationToken = default);

        [Obsolete]
        Task<ClientResponse<IEnumerable<PickOrder>>> GetPickOrders(string[] pickorderIds, string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<Models_v1_27.PickOrder>>> GetPickOrders_v1_1(string[] pickorderIds, string tenantId, string siteKey, CancellationToken cancellationToken = default);

        [Obsolete]
        Task<ClientResponse<QueryResult<PickOrder>>> SearchPickOrders(string tenantId, string siteKey, PickOrderSearchBody searchBody, int limit = 10, int offset = 0, CancellationToken cancellationToken = default);

        Task<ClientResponse<QueryResult<Models_v1_27.PickOrder>>> SearchPickOrders_v1_1(string tenantId, string siteKey, Models_v1_27.PickOrderSearchBody searchBody, int limit = 10, int offset = 0, CancellationToken cancellationToken = default);

        Task<ClientResponse<PickOrderResult>> GetPickOrderResult(string orderId, string siteKey, string tenantId);

        Task<ClientResponse<IEnumerable<PickOrderResult>>> GetPickOrderResults(string[] orderIds, string siteKey, string tenantId);

        Task<ClientResponse<PickOrderScanSummary>> GetPickOrderScanSummary(string orderId, string siteKey, string tenantId);

        Task<ClientResponse<IEnumerable<PickOrderScanSummary>>> GetPickOrderScanSummaries(string[] orderIds, string siteKey, string tenantId);

        Task<ClientResponse<IEnumerable<ConnectedLoadCarrier>>> GetPickOrderConnectedLoadCarriers(string pickorderId, string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<int?>> PatchPickOrderLoadCarrierSeries(string pickorderId, string tenantId, string siteKey, string userId);

        Task<ClientResponse<PickOrderLineSearch>> SearchPickOrderLines(
            string tenantId,
            string siteKey,
            string[]? zoneId = null,
            PickOrderLineState[]? state = null,
            DateTime? slotStart = null,
            DateTime? slotEnd = null,
            string? supplier = null,
            string[]? category = null,
            string? searchString = null,
            int limit = 10,
            int offset = 0,
            CancellationToken cancellationToken = default);

        Task<ClientResponse<DeviatingPickOrderLineSearch>> SearchDeviatingPickOrderLines(
            string tenantId,
            string siteKey,
            DeviatingPickOrderLineSearchBody body,
            int limit = 10,
            int offset = 0, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<PickOrderLineProduct>>> GetOrderLineProducts(
            string tenantId,
            string siteKey,
            PickOrderLinesProductRequest body,
            CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<ZoneTotalQuantityResult>>> GetZoneTotalQuantities(
            string tenantId,
            string siteKey,
            PickOrderLinesZoneQuantityRequest body,
            CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<string>>> GetSuppliersFromDate(string tenantId, string siteKey, DateTime date, CancellationToken cancellationToken = default);

        Task<ClientResponse<PickOrderLinePickSlotQueryResult>> GetAffectedPickOrderLinesFromPickSlotQuery(string tenantId, string siteKey, string[] slotAliases, DateTime pickSlotStartUtc, DateTime pickSlotCompleteUtc, string[]? zoneId = default, string[]? category = default, CancellationToken cancellationToken = default);

        Task<ClientResponse<MagicButtonOptions>> MagicButtonOptions(string tenantId, string? siteKey = null);

        Task<ClientResponse<ManualPickingOptions>> GetManualPickingOptions(string tenantId, string? siteKey = null);

        Task<ClientResponse<PickSlotOptions>> GetPickSlotOptions(string tenantId, string? siteKey = null);

        Task<ClientResponse<PickingThresholdOptions>> GetPickingThresholdOptions(string tenantId, string? siteKey = null);

        Task<ClientResponse<QualityPickingRules>> GetQualityPickingRules(string tenantId, string? siteKey = null);

        Task<ClientResponse<IEnumerable<LabelTemplate>>> GetDefaultLabelTemplates();

        Task<ClientResponse<IEnumerable<LabelTemplate>>> GetLabelTemplates(string tenantId, string? siteKey = default, LabelType? type = default, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<string>>> GetDeliveryNotes(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<DeliveryNoteSettings>> GetDeliveryNote(string id, string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<DeliveryNoteSettings>> GetActiveDeliveryNote(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<TransportDocumentSettings>> GetTransportDocument (string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<PicklistOptions>> GetPicklistOptions(string tenantId, string? siteKey = null);

        Task<ClientResponse<IDictionary<string, PicklistOptions>>> GetSitePicklistOptions(string tenantId);

        Task<ClientResponse<AppFlowOptions>> AppFlowOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<AppUIOptions>> AppUIOptions(string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<DeliverableAutomationOptions>> DeliverableAutomationOptions(string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<OrderStateTransitionRules>> OrderStateTransitionRules(string tenantId, CancellationToken cancellationToken = default);

        Task<ClientResponse<AllowAddOrderLineOptions>> AllowAddOrderLineOptions(string tenantId, CancellationToken cancellationToken = default);

        Task<ClientResponse<AppFeatureOptions>> AppFeatureOptions(string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<OrderNotificationOptions>> OrderNotificationOptions(string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<SubstitutionLimitationsOptions>> SubstitutionLimitationsOptions(string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<OverrideCustomerSubstitutionSettingOptions>> OverrideCustomerSubstitutionSettingOptions(string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<ForcePrintWeightLabelOptions>> ForcePrintWeightLabelOptions(string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<OrderStatusOnDeviationsOptions>> OrderStatusOnDeviationsOptions(string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<ShowCustomerInfoOptions>> ShowCustomerInfoOptions(string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<ForcePickAllLinesOptions>> ForcePickAllLinesOptions(string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<CancelOrderInWebOptions>> CancelOrderInWebOptions(string tenantId, CancellationToken cancellationToken = default);

        Task<ClientResponse<PriceToMassOptions>> PriceToMassOptions(string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<OverrideExpirationDateOptions>> OverrideExpirationDateOptions(string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<SubstitutionReason>>> GetSubstitutionReasons();

        Task<ClientResponse<SubstitutionReason>> GetSubstitutionReason(string id);

        Task<ClientResponse<IEnumerable<SubstitutionReason>>> GetSelectedSubstitutionReasons(string tenantId, string? siteKey = null);

        Task<ClientResponse<IEnumerable<PickingDeviationReason>>> GetDeviationReasons();

        Task<ClientResponse<PickingDeviationReason>> GetDeviationReason(string id);

        Task<ClientResponse<IEnumerable<PickingDeviationReason>>> GetSelectedDeviationReason(string tenantId, string? siteKey = null);

        Task<ClientResponse<IEnumerable<Project.Models.Shared.Core_v1_0.Slot>>> GetOrderSlots(string siteKey, string tenantId, DateTimeOffset start, DateTimeOffset? end, string timeZoneId = "W. Europe Standard Time", CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<Project.Models.Shared.Core_v1_0.Slot>>> GetOrderSlotsMatchingPickSlot(DateTime startUtc, DateTime completeUtc, string siteKey, string tenantId, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<Project.Models.Shared.Core_v1_0.PickSlot>>> GetPickSlots(string siteKey, string tenantId, DateTimeOffset start, DateTimeOffset? end, string timeZoneId = "W. Europe Standard Time", CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<PickSlotData>>> GetPickSlotsData(string siteKey, string tenantId, DateTimeOffset start, DateTimeOffset? end, string timeZoneId = "W. Europe Standard Time", CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<Project.Models.Shared.Core_v1_0.PickSlot>>> GetPickSlotsMatchingOrderSlot(DateTime startUtc, DateTime endUtc, string siteKey, string tenantId, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<CollectSlot>>> GetCollectSlots(string siteKey, string tenantId, DateTimeOffset start, DateTimeOffset? end, string timeZoneId = "W. Europe Standard Time", CancellationToken cancellationToken = default);

        [Obsolete]
        Task<ClientResponse<IEnumerable<TransportSlotPreview>>> GetTransportSlots(string siteKey, string tenantId, DateTimeOffset start, DateTimeOffset? end, string timeZoneId = "W. Europe Standard Time", CancellationToken cancellationToken = default);
        Task<ClientResponse<IEnumerable<Models_v1_27.TransportSlotPreview>>> GetTransportSlots_v1_1(string siteKey, string tenantId, DateTimeOffset start, DateTimeOffset? end, string timeZoneId = "W. Europe Standard Time", CancellationToken cancellationToken = default);

        [Obsolete]
        Task<ClientResponse<IEnumerable<TransportSlotPreview>>> GetTransportSlotsForOrderSlot(string siteKey, string tenantId, string alias, DateTime slotStartUtc, CancellationToken cancellationToken = default);
        Task<ClientResponse<IEnumerable<Models_v1_27.TransportSlotPreview>>> GetTransportSlotsForOrderSlot_v1_1(string siteKey, string tenantId, string alias, DateTime slotStartUtc, CancellationToken cancellationToken = default);

        Task<ClientResponse<QueryResult<TransportSlotOrderPreview>>> TransportPickOrders(string tenantId, string siteKey, DateTime departure, string name, int limit = 10, int offset = 0, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<Printer>>> Printers(string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<Printer>> Printer(string id, string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<PickOrderLine>> GetPickOrderLine(string orderId, string orderlineId, string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<Project.Models.Shared.Core_v1_0.UILogEntry>>> GetPickOrderUIEvents(string id, string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<Project.Models.Shared.Core_v1_0.UILogEntry>>> GetPickListUIEvents(string id, string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<QrCodeResponse>> GetPickOrdersQrCodes(string[] pickOrderIds, string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<BarcodePosIntegrationResponse>> GetPosIntegrationBarcodes(string[] pickOrderIds, string tenantId, string siteKey, string? timeZoneId, CancellationToken cancellationToken = default);

        Task<ClientResponse<ApproveDeviationsOptions>> GetApproveDeviationsOptions(string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<RegisterLoadCarriersOptions>> GetRegisterLoadCarriersOptions(string tenantId, string? siteKey = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<PickingStrategy>>> GetPickingStrategies(string tenantId, string siteKey, string? layoutId = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<PickingStrategy>> GetPickingStrategy(string tenantId, string siteKey, string pickingStrategyId, string? layoutId = null, CancellationToken cancellationToken = default);

        Task<ClientResponse<TagRanking?>> GetTagRanking(string tenantId, string siteKey, string layoutId, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<TagRanking>>> GetAllTagRankings(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<Collectlist>> GetCollectlist(string tenantId, string siteKey, string collectlistId, CancellationToken cancellationToken = default);

        Task<ClientResponse<Packlist>> GetPacklist(string tenantId, string siteKey, string packlistId, CancellationToken cancellationToken = default);

        Task<ClientResponse<QueryResult<PickinglistPreview>>> GetPickinglistPreviews(string tenantId, string siteKey, string[]? listTypes = null, DateTime? fromDate = null, DateTime? toDate = null, PickinglistState[]? states = null, string[]? userIds = null, int limit = 10, int offset = 0, CancellationToken cancellationToken = default);

        Task<ClientResponse<QueryResult<ProductInfo>>> GetProductsWithMissingLocation(string tenantId, string siteKey, DateTime? fromPickSlotStart = null, DateTime? toPickSlotEnd = null, IEnumerable<ProductCategory>? categories = null, int offset = 0, int limit = 50, CancellationToken cancellationToken = default);

        Task<ClientResponse<RequireLoadCarrierOptions>> RequireLoadCarrierOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<string>>> SortingIdsInUse(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<bool>> SortingInUse(string tenantId, string siteKey, string sortingId, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<ProductPickingProperties>>> GetProductPickingProperties(string tenantId, string siteKey, bool onlyExpiredCountryOfOrigins = false, CancellationToken cancellationToken = default);

        Task<ClientResponse<Traininglist>> GetTraininglist(string tenantId, string siteKey, string traininglistId, CancellationToken cancellationToken = default);

        Task<ClientResponse<QueryResult<Traininglist>>> GetTraininglists(string tenantId, string siteKey, IEnumerable<PickinglistState>? states = null, int limit = 20, int offset = 0, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<GlobalSubstitution>>> GetGlobalSubstitutions(string tenantId, string siteKey, IEnumerable<string>? productIds = null, GlobalSubstitutionQueryType type = GlobalSubstitutionQueryType.Any, bool onlyActive = true, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<ProductQuantityResult>>> GetOrderedQuantity(string tenantId,string siteKey, DateTime fromPickSlotStart, DateTime toPickSlotStart, IEnumerable<string>? productIds = null, bool onlySubstitutionAllowed = false, bool onlyWithoutPersonalSubstitutions = false, PickOrderLineState maxState = PickOrderLineState.OnHold, CancellationToken cancellationToken = default);

        Task<ClientResponse<RequireCompileOptions>> GetRequireCompileOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<CustomProductLocationOptions>> GetCustomProductLocationOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<ExternalCarrierIdOptions>> GetExternalCarrierIdOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<TrolleyLocationsOptions>> GetTrolleyLocationsOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<ClientResponse<LegacyLocationBarcodeSupportOptions>> GetLegacyLocationBarcodeSupportOptions(string tenantId, CancellationToken cancellationToken = default);

        Task<ClientResponse<IEnumerable<LoadCarrierEstimationResponse>>> GetLoadCarrierEstimations(string tenantId, string siteKey, LoadCarrierEstimationRequest request, CancellationToken cancellationToken = default);
    }
}