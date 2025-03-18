using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.Commands.Collectlist;
using Api.Core.Commands.Locations;
using Api.Core.Commands.Packlist;
using Api.Core.Commands.PickingStrategy;
using Api.Core.Commands.Picklist;
using Api.Core.Commands.PickOrder;
using Api.Core.Commands.Products;
using Api.Core.Commands.Reasons;
using Api.Core.Commands.Settings;
using Api.Core.Commands.Settings.Optimization;
using Api.Core.Commands.Substitutions;
using Api.Core.Commands.Traininglist;
using Api.Core.Commands.Zones;
using Api.Core.Events.Collectlist;
using Api.Core.Events.Layouts;
using Api.Core.Events.Locations;
using Api.Core.Events.Packlist;
using Api.Core.Events.PicklistEvents;
using Api.Core.Events.PickOrderEvents;
using Api.Core.Events.PickOrderLineEvents;
using Api.Core.Events.ProductPlacement;
using Api.Core.Events.Products;
using Api.Core.Events.Settings;
using Api.Core.Events.Zones;
using ConfigurationService.Shared;
using Core.Models;
using Infrastructure.Messaging.Consumer;
using Infrastructure.Messaging.Models;
using Infrastructure.Messaging.TaskSynchronizer;
using PncCore.Shared.Interfaces.Events.Picking;

namespace Api.Configuration
{
    public static class RegistratorExtension
    {
        public static async Task Initialize(this MessageBusConfigurator messageBusConfigurator, string serviceName, ISupportedCommands commands, ISupportedEvents events)
        {
            var commandChannelName = $"{serviceName}|command";
            var eventChannelName = $"{serviceName}|event";

            var maps = new List<MessageHandlerMap>();
            foreach (var cmd in commands)
            {
                maps.Add(
                    new MessageHandlerMap(
                        cmd.Name,
                        cmd.CommandType,
                        cmd.ContentType));
            }

            var siteEventMaps = new[]
            {
                new MessageHandlerMap(
                    nameof(LocationsUpdatedEvent),
                    typeof(UpdateLocationsCommand)),

                new MessageHandlerMap(
                    nameof(LocationsDeletedEvent),
                    typeof(DeleteLocationsCommand)),

                new MessageHandlerMap(
                    nameof(ZonesDeletedEvent),
                    typeof(DeleteZonesCommand)),

                new MessageHandlerMap(
                    nameof(SiteLayoutChangedEvent),
                    typeof(SiteLayoutChangedEvent)),

                new MessageHandlerMap(
                    nameof(SiteLayoutDeletedEvent),
                    typeof(SiteLayoutDeletedEvent)),

                new MessageHandlerMap(
                    nameof(ProductPlacementsAddedEvent),
                    typeof(ProductPlacementsAddedEvent)),

                new MessageHandlerMap(
                    nameof(ProductPlacementsUpdatedEvent),
                    typeof(ProductPlacementsUpdatedEvent)),

                new MessageHandlerMap(
                    nameof(ProductPlacementsDeletedEvent),
                    typeof(ProductPlacementsDeletedEvent)),
            };

            var tenantEventMaps = new[]
            {
                new MessageHandlerMap(
                    nameof(TenantModulesUpdatedEvent),
                    typeof(ActivateForNewTenantCommand))
            };

            var productEventMaps = new[]
           {
                new MessageHandlerMap(
                    nameof(ProductRecallCreatedEvent),
                    typeof(ProductRecallCreatedEvent)),

                new MessageHandlerMap(
                    nameof(ProductRecallDeletedEvent),
                    typeof(ProductRecallDeletedEvent)),

                 new MessageHandlerMap(
                    nameof(SiteProductRecallUpdatedEvent),
                    typeof(SiteProductRecallUpdatedEvent)),

                 new MessageHandlerMap(
                     nameof(SiteProductsCreatedEvent),
                     typeof(SiteProductsCreatedEvent)),

                 new MessageHandlerMap(
                     nameof(SiteProductsPatchEvent),
                     typeof(SiteProductsPatchEvent)),
            };

            var orderCore = new[]
            {
                new MessageHandlerMap(
                    nameof(TaskEvent),
                    typeof(TaskEvent)),
                new MessageHandlerMap(
                    nameof(TaskResultEvent),
                    typeof(TaskResultEvent)),
            };

            await messageBusConfigurator.RegisterSupportedContent(commands, events.Select(e => e.Name));

            messageBusConfigurator.RegisterChannelsToPublishTo(
            [
                commandChannelName,
                    eventChannelName,
                    "ordercore|command",
                    "messagecore|command"
            ]);

            await messageBusConfigurator.RegisterChannelsToSubcribeTo(new (string, IEnumerable<MessageHandlerMap>)[]
            {
                ("pnccore|command", maps),
                ("sitecore|event", siteEventMaps),
                ("tenantcore|event", tenantEventMaps),
                ("productcore|event", productEventMaps),
                ("ordercore|event", orderCore),
            });
        }
    }

    public class SupportedCommands : ISupportedCommands
    {
        public IEnumerator<SupportedContentCommand> GetEnumerator()
        {
            yield return SupportedContentCommand.Default(typeof(CreatePickOrderCommand));

            yield return SupportedContentCommand.Default(typeof(UpdatePickOrderLinesPickSlotCommand));
            yield return SupportedContentCommand.Default(typeof(ReSyncPickOrdersCommand));
            yield return SupportedContentCommand.Default(typeof(ReSyncPickOrderLineProductsCommand));
            yield return SupportedContentCommand.Default(typeof(CancelOrderCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTransportSlotCommand));
            yield return SupportedContentCommand.Default(typeof(UpdatePickOrderConnectedLoadCarriersCommand));
            yield return SupportedContentCommand.Default(typeof(OrderReadyForDeliveryCommand));
            yield return SupportedContentCommand.Default(typeof(OrdersReadyForDeliveryCommand));
            yield return SupportedContentCommand.Default(typeof(CreatePicklistFromOrderLinesCommand));
            yield return SupportedContentCommand.Default(typeof(RequestPicklistCommand));
            yield return SupportedContentCommand.Default(typeof(RequestPicklistCommandv2));
            yield return SupportedContentCommand.Default(typeof(AssignPicklistCommand));
            yield return SupportedContentCommand.Default(typeof(UnassignPicklistCommand));
            yield return SupportedContentCommand.Default(typeof(UnassignPicklistsCommand));
            yield return SupportedContentCommand.Default(typeof(RestartPicklistCommand));
            yield return SupportedContentCommand.Default(typeof(CancelPicklistCommand));
            yield return SupportedContentCommand.Default(typeof(CancelPicklistsCommand));
            yield return SupportedContentCommand.Default(typeof(CompletePicklistCommand));
            yield return SupportedContentCommand.Default(typeof(CloseOrderCommand));
            yield return SupportedContentCommand.Default(typeof(CloseDeliveryOrderCommand));
            yield return SupportedContentCommand.Default(typeof(StartPicklistCommand));
            yield return SupportedContentCommand.Default(typeof(AddOnHoldCommand));
            yield return SupportedContentCommand.Default(typeof(RemoveOnHoldCommand));
            yield return SupportedContentCommand.Default(typeof(ApproveDeviationsCommand));
            yield return SupportedContentCommand.Default(typeof(PreparePicklistCommand));
            yield return SupportedContentCommand.Default(typeof(ResetPickOrderLinesCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateProductPickingPropertiesCommand));
            yield return SupportedContentCommand.Default(typeof(ApprovePendingDeliverableCommand));
            yield return SupportedContentCommand.Default(typeof(ApprovePendingOrderCommand));
            yield return SupportedContentCommand.Default(typeof(OrderReadyToPickCommand));
            yield return SupportedContentCommand.Default(typeof(AddOrderLineCommand));
            yield return SupportedContentCommand.Default(typeof(ReleaseInactivePicklistsCommand));


            // Collectlist
            yield return SupportedContentCommand.Default(typeof(CreateCollectlistFromOrderLinesCommand));
            yield return SupportedContentCommand.Default(typeof(AssignCollectlistCommand));
            yield return SupportedContentCommand.Default(typeof(UnassignCollectlistCommand));
            yield return SupportedContentCommand.Default(typeof(UnassignCollectlistsCommand));
            yield return SupportedContentCommand.Default(typeof(StartCollectlistCommand));
            yield return SupportedContentCommand.Default(typeof(RestartCollectlistCommand));
            yield return SupportedContentCommand.Default(typeof(CancelCollectlistCommand));
            yield return SupportedContentCommand.Default(typeof(CancelCollectlistsCommand));
            yield return SupportedContentCommand.Default(typeof(CompleteCollectlistCommand));


            // Packlist
            yield return SupportedContentCommand.Default(typeof(CreatePacklistFromCollectResultCommand));
            yield return SupportedContentCommand.Default(typeof(AssignPacklistCommand));
            yield return SupportedContentCommand.Default(typeof(UnassignPacklistCommand));
            yield return SupportedContentCommand.Default(typeof(UnassignPacklistsCommand));
            yield return SupportedContentCommand.Default(typeof(StartPacklistCommand));
            yield return SupportedContentCommand.Default(typeof(RestartPacklistCommand));
            yield return SupportedContentCommand.Default(typeof(CancelPacklistCommand));
            yield return SupportedContentCommand.Default(typeof(CancelPacklistsCommand));
            yield return SupportedContentCommand.Default(typeof(CompletePacklistCommand));

            // Settings
            yield return SupportedContentCommand.Default(typeof(UpsertTenantMagicBtnOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpsertSiteMagicBtnOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantManualPickingOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantPickSlotOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantQualityPickingRulesCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantPickingThresholdOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantLabelTemplateCommand));
            yield return SupportedContentCommand.Default(typeof(DeleteTenantLabelTemplateCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSiteManualPickingOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSitePickingThresholdOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSitePickSlotOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSiteQualityPickingRulesCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSiteLabelTemplateCommand));
            yield return SupportedContentCommand.Default(typeof(DeleteSiteLabelTemplateCommand));
            yield return SupportedContentCommand.Default(typeof(UpsertDeliveryNoteCommand));
            yield return SupportedContentCommand.Default(typeof(UpsertTransportDocumentCommand));
            yield return SupportedContentCommand.Default(typeof(DeleteDeliveryNoteCommand));
            yield return SupportedContentCommand.Default(typeof(SetActiveDeliveryNoteCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateAppFlowOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateAppUIOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpsertTenantDeliverableAutomationOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpsertSiteDeliverableAutomationOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateAppFeatureOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpsertOrderNotificationOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantSubstitutionLimitationsOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSiteSubstitutionLimitationsOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantForcePrintWeightLabelOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSiteForcePrintWeightLabelOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantOrderStatusOnDeviationsOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSiteOrderStatusOnDeviationsOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantShowCustomerInfoOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSiteShowCustomerInfoOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantForcePickAllLinesOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSiteForcePickAllLinesOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantCancelOrderInWebOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantApproveDeviationsOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSiteApproveDeviationsOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdatePriceToMassOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateOverrideExpirationDateOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSiteRegisterLoadCarriersOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantRegisterLoadCarriersOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateRequireLoadCarrierOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantOverrideCustomerSubstitutionCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSiteOverrideCustomerSubstitutionCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateRequireCompileOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateOrderStateTransitionRulesCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateCustomProductLocationOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateAllowAddOrderLineOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpsertSiteExternalCarrierIdOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpsertTenantExternalCarrierIdOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSitePicklistOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantPicklistOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpsertSiteTrolleyLocationsOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpsertTenantTrolleyLocationsOptionsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateLegacyLocationBarcodeSupportOptionsCommand));

            // Reasons
            yield return SupportedContentCommand.Default(typeof(UpdateTenantSubstitutionReasonsCommand));
            yield return SupportedContentCommand.Default(typeof(DeleteTenantSubstitutionReasonCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSiteSubstitutionReasonsCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateTenantPickingDeviationReasonsCommand));
            yield return SupportedContentCommand.Default(typeof(DeleteTenantPickingDeviationReasonCommand));
            yield return SupportedContentCommand.Default(typeof(UpdateSitePickingDeviationReasonsCommand));
            yield return SupportedContentCommand.Default(typeof(DeleteSitePickingDeviationReasonCommand));

            // Printers
            yield return SupportedContentCommand.Default(typeof(UpsertPrinterCommand));
            yield return SupportedContentCommand.Default(typeof(DeletePrinterCommand));

            // AutoDeliverable
            yield return SupportedContentCommand.Default(typeof(AutoDeliverableOrderCommand));

            // PickingStategy
            yield return SupportedContentCommand.Default(typeof(UpsertPickingStrategiesCommand));
            yield return SupportedContentCommand.Default(typeof(DeletePickingStrategiesCommand));
            yield return SupportedContentCommand.Default(typeof(UpsertTagRankingCommand));

            // Traininglist
            yield return SupportedContentCommand.Default(typeof(CreateTraininglistFromOrderLinesCommand));
            yield return SupportedContentCommand.Default(typeof(AssignTraininglistCommand));
            yield return SupportedContentCommand.Default(typeof(UnassignTraininglistCommand));
            yield return SupportedContentCommand.Default(typeof(CancelTraininglistCommand));
            yield return SupportedContentCommand.Default(typeof(CompleteTraininglistCommand));
            yield return SupportedContentCommand.Default(typeof(RequestTraininglistCommand));

            // Substitutions
            yield return SupportedContentCommand.Default(typeof(UpsertGlobalSubstitutionCommand));
            yield return SupportedContentCommand.Default(typeof(DeleteGlobalSubstitutionCommand));
            yield return SupportedContentCommand.Default(typeof(ReSyncGlobalSubstitutionsCommand));

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class SupportedEvents : ISupportedEvents
    {
        public IEnumerator<Type> GetEnumerator()
        {
            yield return typeof(PicklistAssignedEvent);
            yield return typeof(PicklistNotAssignedEvent);
            yield return typeof(InProgressPicklistCanceledEvent);
            yield return typeof(InProgressOrderCanceledEvent);
            yield return typeof(PicklistCompletedEvent);
            yield return typeof(TaskEvent);
            yield return typeof(TaskResultEvent);
            yield return typeof(PickOrderCreatedEvent);
            yield return typeof(PickOrderImportedEvent);
            yield return typeof(PickOrderStateChangedEvent);
            yield return typeof(PickOrderPartialPickResultAddedEvent);
            yield return typeof(PickOrderUpdatedEvent);
            yield return typeof(FailedToCreatePickOrderEvent);
            yield return typeof(PickOrderIsPendingEvent);
            yield return typeof(PickOrderIsDeliverableEvent);
            yield return typeof(PicklistStartedEvent);
            yield return typeof(PicklistCanceledEvent);
            yield return typeof(PickOrderSnapshotUpdatedEvent);
            yield return typeof(PicklistRequestCommandCompletedEvent);
            yield return typeof(PickOrderLineAddedEvent);
            yield return typeof(ExpiryDateOverriddenEvent);
            yield return typeof(PickOrderLineOnHoldAddedEvent);
            yield return typeof(PickOrderLineOnHoldRemovedEvent);

            yield return typeof(PickOrderLineDeviationApprovedEvent);
            yield return typeof(PickOrderLineResetEvent);

            yield return typeof(CollectlistCreatedEvent);
            yield return typeof(CollectlistAssignedEvent);
            yield return typeof(CollectlistUnAssignedEvent);
            yield return typeof(CollectlistStartedEvent);
            yield return typeof(CollectlistRestartedEvent);
            yield return typeof(CollectlistCompletedEvent);
            yield return typeof(CollectlistCanceledEvent);


            yield return typeof(PacklistCreatedEvent);
            yield return typeof(PacklistAssignedEvent);
            yield return typeof(PacklistUnAssignedEvent);
            yield return typeof(PacklistStartedEvent);
            yield return typeof(PacklistRestartedEvent);
            yield return typeof(PacklistCompletedEvent);
            yield return typeof(PacklistCanceledEvent);
            yield return typeof(PicklistPreparedEvent);
            yield return typeof(PicklistInactivityCancellationEvent);

            yield return typeof(SettingsUpdatedEvent<>);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class PickOrderSnapshotUpdatedEvent
        {
        }
    }
}