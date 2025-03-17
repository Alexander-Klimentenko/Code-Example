using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Api.Core.Commands.Settings;
using Api.Core.Commands.Settings.Optimization;
using Api.Core.Events.Settings;
using Api.Core.Models.Settings;
using Api.Core.Models.Settings.Automation;
using Api.Core.Models.Settings.Optimization;
using Api.Core.Services;
using AutoMapper;
using Core.Commands;
using Core.Interfaces;
using Core.Interfaces.Extensions;
using Infrastructure.Communication.Utilities;
using Infrastructure.Messaging.Utils;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Project.Infrastructure.Cache.Redis;
using IPublisher = Infrastructure.Messaging.Publisher.IPublisher;

namespace Api.Core.CommandHandlers
{
    public class TenantSettingsCommandHandler :
        IRequestHandler<UpdateTenantManualPickingOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateTenantPickingThresholdOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateTenantPickSlotOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateTenantQualityPickingRulesCommand, RequestResponse>,
        IRequestHandler<UpdateTenantLabelTemplateCommand, RequestResponse>,
        IRequestHandler<UpsertTenantMagicBtnOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateTenantPicklistOptionsCommand, RequestResponse>,
        IRequestHandler<DeleteTenantLabelTemplateCommand, RequestResponse>,
        IRequestHandler<ActivateForNewTenantCommand, RequestResponse>,
        IRequestHandler<UpsertTenantDeliverableAutomationOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateTenantSubstitutionLimitationsOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateTenantForcePrintWeightLabelOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateTenantOrderStatusOnDeviationsOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateTenantShowCustomerInfoOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateTenantForcePickAllLinesOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateTenantCancelOrderInWebOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateTenantApproveDeviationsOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateTenantRegisterLoadCarriersOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateTenantOverrideCustomerSubstitutionCommand, RequestResponse>,
        IRequestHandler<UpdateOrderStateTransitionRulesCommand, RequestResponse>,
        IRequestHandler<UpdateAllowAddOrderLineOptionsCommand, RequestResponse>,
        IRequestHandler<UpsertTenantExternalCarrierIdOptionsCommand, RequestResponse>,
        IRequestHandler<UpsertTenantTrolleyLocationsOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateLegacyLocationBarcodeSupportOptionsCommand, RequestResponse>
    {
        private readonly ILogger<TenantSettingsCommandHandler> logger;

        private readonly IMediatorHandler mediatorHandler;

        private readonly IRepositoryFactory repositoryFactory;

        private readonly IPublisher publisher;

        private readonly IMapper mapper;

        private readonly IMapperHelper mapperHelper;

        private readonly IRedisCache redisCache;

        public TenantSettingsCommandHandler(
            ILogger<TenantSettingsCommandHandler> logger,
            IMediatorHandler mediatorHandler,
            IRepositoryFactory repositoryFactory,
            IPublisher publisher,
            IMapper mapper,
            IMapperHelper mapperHelper,
            IRedisCache redisCache)
        {
            this.logger = logger;
            this.mediatorHandler = mediatorHandler;
            this.repositoryFactory = repositoryFactory;
            this.publisher = publisher;
            this.mapper = mapper;
            this.mapperHelper = mapperHelper;
            this.redisCache = redisCache;
        }

        public async Task<RequestResponse> Handle(UpdateTenantApproveDeviationsOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<ApproveDeviationsOptions>(cmd.TenantId);
            var options = mapperHelper.Map(cmd.ApproveDeviationsOptions, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantApproveDeviationsOptionsCommand)} handler could not upsert {nameof(ApproveDeviationsOptions)} to repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<ApproveDeviationsOptions>(
                               nameof(ApproveDeviationsOptions),
                               cmd.TenantId,
                               null,
                               cmd.Issuer,
                               DateTime.UtcNow,
                               options.UpdatedBy,
                               cmd.UserName,
                               options.UpdatedUtc,
                               options,
                               previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateTenantManualPickingOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<ManualPickingOptions>(cmd.TenantId);
            var options = mapperHelper.Map(cmd.ManualPickingOptions, cmd.TenantId, cmd.Issuer);

            if (string.IsNullOrWhiteSpace(options?.Id))
            {
                logger.LogError($"{nameof(UpdateTenantManualPickingOptionsCommand)} handler received a request but could not map {nameof(ManualPickingOptions)} from request to a core entity without Id from TenantId. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.BadRequest, publisher, cmd.TenantId, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantManualPickingOptionsCommand)} handler could not upsert {nameof(ManualPickingOptions)} to repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }
            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<ManualPickingOptions>(
                               nameof(ManualPickingOptions),
                               cmd.TenantId,
                               null,
                               cmd.Issuer,
                               DateTime.UtcNow,
                               options.UpdatedBy,
                               cmd.UserName,
                               options.UpdatedUtc,
                               options,
                               previousOptions));
            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateTenantPickingThresholdOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<PickingThresholdOptions>(cmd.TenantId);
            var options = mapperHelper.Map(cmd.PickingThresholdOptions, cmd.TenantId, cmd.Issuer);

            if (string.IsNullOrWhiteSpace(options?.Id))
            {
                logger.LogError($"{nameof(UpdateTenantPickingThresholdOptionsCommand)} handler received a request but could not map {nameof(PickingThresholdOptions)} from request to a core entity without Id from TenantId. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.BadRequest, publisher, cmd.TenantId, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantPickingThresholdOptionsCommand)} handler could not upsert {nameof(PickingThresholdOptions)} to repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<PickingThresholdOptions>(
                               nameof(PickingThresholdOptions),
                               cmd.TenantId,
                               null,
                               cmd.Issuer,
                               DateTime.UtcNow,
                               options.UpdatedBy,
                               cmd.UserName,
                               options.UpdatedUtc,
                               options,
                               previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateTenantPicklistOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<Models.Settings.Optimization.PicklistOptions>(cmd.TenantId);
            var options = new Models.Settings.Optimization.PicklistOptions(
                cmd.TenantId,
                cmd.PicklistOptions.PreparationStatesInactivityThreshold,
                cmd.PicklistOptions.InProgressStatesInactivityThreshold,
                cmd.PicklistOptions.ReleaseInactivePicklist,
                cmd.Issuer);

            if (string.IsNullOrWhiteSpace(options?.Id))
            {
                logger.LogError($"{nameof(UpdateTenantPickSlotOptionsCommand)} handler received a request but could not map {nameof(PickSlotOptions)} from request to a core entity without Id from TenantId. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.BadRequest, publisher, cmd.TenantId, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantPickSlotOptionsCommand)} handler could not upsert {nameof(PickSlotOptions)} to repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            await redisCache.Remove($"{cmd.TenantId}|PicklistOptions", true);

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<Models.Settings.Optimization.PicklistOptions>(
                nameof(PicklistOptions),
                cmd.TenantId,
                null,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc, options, previousOptions
            ));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateTenantPickSlotOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<PickSlotOptions>(cmd.TenantId);
            var options = new PickSlotOptions(
                        cmd.TenantId,
                        cmd.PickSlotOptions.DefaultPickSlotTimeSpan,
                        cmd.PickSlotOptions.DefaultPickSlotMargin,
                        cmd.PickSlotOptions.AutoUpdatePickSlot,
                        cmd.Issuer);

            if (string.IsNullOrWhiteSpace(options?.Id))
            {
                logger.LogError($"{nameof(UpdateTenantPickSlotOptionsCommand)} handler received a request but could not map {nameof(PickSlotOptions)} from request to a core entity without Id from TenantId. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.BadRequest, publisher, cmd.TenantId, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantPickSlotOptionsCommand)} handler could not upsert {nameof(PickSlotOptions)} to repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<PickSlotOptions>(
                nameof(PickSlotOptions),
                cmd.TenantId,
                null,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc, options, previousOptions
            ));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateTenantQualityPickingRulesCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<QualityPickingRules>(cmd.TenantId);
            var options = mapperHelper.Map(cmd.QualityPickingRules, cmd.TenantId, cmd.Issuer);

            if (string.IsNullOrWhiteSpace(options?.Id))
            {
                logger.LogError($"{nameof(UpdateTenantQualityPickingRulesCommand)} handler received a request but could not map {nameof(QualityPickingRules)} from request to a core entity without Id from TenantId. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.BadRequest, publisher, cmd.TenantId, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantQualityPickingRulesCommand)} handler could not upsert {nameof(QualityPickingRules)} to repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            var redisKey = $"{cmd.TenantId}|qualityPickingRules";
            await redisCache.Remove(redisKey, true);

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<QualityPickingRules>(
                               nameof(QualityPickingRules),
                               cmd.TenantId,
                               null,
                               cmd.Issuer,
                               DateTime.UtcNow,
                               options.UpdatedBy,
                               cmd.UserName,
                               options.UpdatedUtc,
                               options,
                               previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateTenantLabelTemplateCommand request, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<LabelTemplate>(request.TenantId);

            var template = new LabelTemplate(
                string.IsNullOrWhiteSpace(request.LabelId) ? Guid.NewGuid().ToString("N") : request.LabelId,
                request.Name,
                request.Body,
                request.Type,
                request.Dpmm,
                request.Width,
                request.Height);

            var upserted = await repo.Upsert(template.Id, template);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantLabelTemplateCommand)} handler received a request but could not store {nameof(LabelTemplate)} to repository for TenantId: {request.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.InternalServerError, publisher, request.TenantId, request.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.OK, publisher, request.TenantId, request.TenantId);
            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(DeleteTenantLabelTemplateCommand request, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<LabelTemplate>(request.TenantId);

            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", request.LabelId);
            var template = await repo.QueryItem<LabelTemplate>(queryDefinition, cancellationToken);

            if (template != null)
            {
                var deleted = await repo.Delete(template);
                if (!deleted.IsSuccessful())
                {
                    logger.LogError($"{nameof(DeleteTenantLabelTemplateCommand)} handler received a request for {nameof(LabelTemplate)} with Id: {request.LabelId} for TenantId: {request.TenantId} but could delete from repository. Request failed.");
                    _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.InternalServerError, publisher, request.TenantId, request.TenantId);
                    return RequestResponse.RequestFailed();
                }
            }

            _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.OK, publisher, request.TenantId, request.TenantId);
            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(ActivateForNewTenantCommand request, CancellationToken cancellationToken)
        {
            if (!request.Modules.HasFlag(Project.Models.Shared.Core_v1_0.ProjectModules.PnC))
            {
                return RequestResponse.Dismissed();
            }

            var repo = repositoryFactory.GetRepository<PnCActivated>();
            var queryDefinition = new QueryDefinition($"SELECT * FROM c");
            var response = await repo.QueryItems<PnCActivated>(queryDefinition, cancellationToken);

            if (response?.Any() ?? true)
            {
                logger.LogError($"{nameof(ActivateForNewTenantCommand)} handler received a request but pnc is already activated for TenantId: {request.TenantId}. Request Dismissed.");
                return RequestResponse.Dismissed();
            }

            var labelRepo = repositoryFactory.GetRepository<LabelTemplate>();
            var templates = await labelRepo.QueryItems<LabelTemplate>(queryDefinition, cancellationToken);

            if (!templates?.Any() ?? true)
            {
                logger.LogError($"{nameof(ActivateForNewTenantCommand)} handler received a request but could not get the default label templates for TenantId: {request.TenantId}. Request failed.");
                return RequestResponse.RequestFailed();
            }

            var tenantLabelRepo = repositoryFactory.GetTenantRepository<LabelTemplate>(request.TenantId);

            templates = templates.Select(l => new LabelTemplate(Guid.NewGuid().ToString("N"), l.Name, l.Body, l.Type, l.Dpmm, l.Width, l.Height)).ToList();

            var teplatesResponse = await tenantLabelRepo.BulkAdd(templates);

            if (teplatesResponse.NumberOfDocumentsImported != templates.Count())
            {
                logger.LogError("{command} handler received a request but could not create default labels for TenantId: {tenantId}. Request failed.", nameof(ActivateForNewTenantCommand), request.TenantId);
                return RequestResponse.RequestFailed();
            }

            var printerTenantRepo = repositoryFactory.GetTenantRepository<Printer>(request.TenantId);

            var printer = new Printer(Guid.NewGuid().ToString("N"), "Default", "Default system printer", true, true, templates.Select(t => new PncCore.Shared.Models.Settings.PrinterLabel(t.Id, t.Name, t.Type)), Project.Models.Shared.Core_v1_0.PrinterManufacturer.Zebra, Project.Models.Shared.Core_v1_0.PrinterConnectionType.Bluetooth);

            var added = await printerTenantRepo.Add(printer);

            if (!added.IsSuccessful())
            {
                logger.LogError("{command} handler received a request but could not create default printer for TenantId: {tenantId}. Request failed.", nameof(ActivateForNewTenantCommand), request.TenantId);
                return RequestResponse.RequestFailed();
            }

            await repo.Add(new PnCActivated(Guid.NewGuid().ToString("N"), DateTime.UtcNow));
            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpsertTenantDeliverableAutomationOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<DeliverableAutomationOptions>(cmd.TenantId);

            var options = new DeliverableAutomationOptions(
                cmd.Options.Allow,
                cmd.Options.MaxSubstitutedProductsPercentage,
                cmd.Options.MaxUnpickedProductsPercentage,
                cmd.Options.MaxWeightDeviationPercentage,
                cmd.Options.MaxSubstitutionWeightDeviationPercentage,
                cmd.Options.MaxSubstitutionDeviationPiecesPerPiece,
                cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError("{command} handler could not upsert {model} to repository for TenantId: {tenantId}. Request failed.", nameof(UpsertTenantDeliverableAutomationOptionsCommand), nameof(DeliverableAutomationOptions), cmd.TenantId);
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<DeliverableAutomationOptions>(
                nameof(DeliverableAutomationOptions),
                cmd.TenantId,
                null,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc, options, previousOptions
            ));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateTenantSubstitutionLimitationsOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<SubstitutionLimitationsOptions>(cmd.TenantId);
            var options = new SubstitutionLimitationsOptions(cmd.Mode, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantSubstitutionLimitationsOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(SubstitutionLimitationsOptions)} to repository for TenantId:{cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<SubstitutionLimitationsOptions>(
                               nameof(SubstitutionLimitationsOptions),
                               cmd.TenantId,
                               null,
                               cmd.Issuer,
                               DateTime.UtcNow,
                               options.UpdatedBy,
                               cmd.UserName,
                               options.UpdatedUtc,
                               options,
                               previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateTenantForcePrintWeightLabelOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<ForcePrintWeightLabelOptions>(cmd.TenantId);
            var options = new ForcePrintWeightLabelOptions(cmd.ForcePrintWeightLabel, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantForcePrintWeightLabelOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(ForcePrintWeightLabelOptions)} to repository for TenantId:{cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<ForcePrintWeightLabelOptions>(
                               nameof(ForcePrintWeightLabelOptions),
                               cmd.TenantId,
                               null,
                               cmd.Issuer,
                               DateTime.UtcNow,
                               options.UpdatedBy,
                               cmd.UserName,
                               options.UpdatedUtc,
                               options,
                               previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateTenantOrderStatusOnDeviationsOptionsCommand cmd, CancellationToken cancellationToken)
        {
            if (cmd.Options == null)
            {
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.BadRequest, publisher, cmd.TenantId);
                return RequestResponse.Dismissed();
            }

            var repo = repositoryFactory.GetTenantRepository<OrderStatusOnDeviationsOptions>(cmd.TenantId);

            var options = new OrderStatusOnDeviationsOptions(
                cmd.Options.MaxWeightDeviation,
                cmd.Options.MaxSubstitutionWeightDeviation,
                cmd.Options.MaxSubstitutionDeviationPiecesPerPiece,
                cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantOrderStatusOnDeviationsOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(OrderStatusOnDeviationsOptions)} to repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<OrderStatusOnDeviationsOptions>(
                               nameof(OrderStatusOnDeviationsOptions),
                               cmd.TenantId,
                               null,
                               cmd.Issuer,
                               DateTime.UtcNow,
                               options.UpdatedBy,
                               cmd.UserName,
                               options.UpdatedUtc,
                               options,
                               previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpsertTenantMagicBtnOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<MagicButtonOptions>(cmd.TenantId);

            var options = new MagicButtonOptions
            (
                cmd.SelectionFilters,
                cmd.PrioritizationFilter,
                cmd.PrioritizationFilterPrios,
                cmd.CapacityFilters,
                cmd.SortingFilter,
                cmd.ZoneContextSetting,
                cmd.FilterOptions,
                cmd.Issuer
            );

            var previousOpts = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpsertTenantMagicBtnOptionsCommand)} handler could not upsert {nameof(MagicButtonOptions)} to repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<MagicButtonOptions>(
                        nameof(MagicButtonOptions),
                        cmd.TenantId,
                        null,
                        cmd.Issuer,
                        DateTime.UtcNow,
                        options.UpdatedBy,
                        cmd.UserName,
                        options.UpdatedUtc,
                        options,
                        previousOpts));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateTenantShowCustomerInfoOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<ShowCustomerInfoOptions>(cmd.TenantId);
            var options = new ShowCustomerInfoOptions(cmd.ShowCustomerInfo, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantShowCustomerInfoOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(ShowCustomerInfoOptions)} to repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<ShowCustomerInfoOptions>(
                               nameof(ShowCustomerInfoOptions),
                               cmd.TenantId,
                               null,
                               cmd.Issuer,
                               DateTime.UtcNow,
                               options.UpdatedBy,
                               cmd.UserName,
                               options.UpdatedUtc,
                               options,
                               previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateTenantForcePickAllLinesOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<ForcePickAllLinesOptions>(cmd.TenantId);
            var options = new ForcePickAllLinesOptions(cmd.PickAllLines, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantForcePickAllLinesOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(ForcePickAllLinesOptions)} to repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<ForcePickAllLinesOptions>(
                               nameof(ForcePickAllLinesOptions),
                               cmd.TenantId,
                               null,
                               cmd.Issuer,
                               DateTime.UtcNow,
                               options.UpdatedBy,
                               cmd.UserName,
                               options.UpdatedUtc,
                               options,
                               previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateTenantCancelOrderInWebOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<CancelOrderInWebOptions>(cmd.TenantId);
            var options = new CancelOrderInWebOptions(cmd.CancelOrderInWeb, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantCancelOrderInWebOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(CancelOrderInWebOptions)} to a repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }
            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<CancelOrderInWebOptions>(
                               nameof(CancelOrderInWebOptions),
                               cmd.TenantId,
                               null,
                               cmd.Issuer,
                               DateTime.UtcNow,
                               options.UpdatedBy,
                               cmd.UserName,
                               options.UpdatedUtc,
                               options,
                               previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateTenantRegisterLoadCarriersOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<RegisterLoadCarriersOptions>(cmd.TenantId);
            var options = new RegisterLoadCarriersOptions(cmd.RegisterLoadCarriersOptions.RegisterLoadCarriersOptionsEnabled, cmd.RegisterLoadCarriersOptions.RegisterLoadCarriersOptionsOptional, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantRegisterLoadCarriersOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(RegisterLoadCarriersOptions)} to a repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }
            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<RegisterLoadCarriersOptions>(
                               nameof(RegisterLoadCarriersOptions),
                               cmd.TenantId,
                               null,
                               cmd.Issuer,
                               DateTime.UtcNow,
                               options.UpdatedBy,
                               cmd.UserName,
                               options.UpdatedUtc,
                               options,
                               previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateTenantOverrideCustomerSubstitutionCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<OverrideCustomerSubstitutionSettingOptions>(cmd.TenantId);
            var options = new OverrideCustomerSubstitutionSettingOptions(cmd.Options.Active, cmd.Options.Roles, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantOverrideCustomerSubstitutionCommand)} handler could not {nameof(repo.Upsert)} {nameof(OverrideCustomerSubstitutionSettingOptions)} to a repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<OverrideCustomerSubstitutionSettingOptions>(
                   nameof(OverrideCustomerSubstitutionSettingOptions),
                   cmd.TenantId,
                   null,
                   cmd.Issuer,
                   DateTime.UtcNow,
                   options.UpdatedBy,
                   cmd.UserName,
                   options.UpdatedUtc,
                   options,
                   previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateOrderStateTransitionRulesCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<OrderStateTransitionRules>(cmd.TenantId);
            var options = new OrderStateTransitionRules(
                                    cmd.OrderStateTransitionRules.BlockNew,
                                    cmd.OrderStateTransitionRules.BlockDeliverable,
                                    cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError("{command} handler could not upsert {model} to repository for TenantId: {tenantId}. Request failed.", nameof(UpdateOrderStateTransitionRulesCommand), nameof(OrderStateTransitionRules), cmd.TenantId);
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<OrderStateTransitionRules>(
                nameof(OrderStateTransitionRules),
                cmd.TenantId,
                null,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions
            ));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateAllowAddOrderLineOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<AllowAddOrderLineOptions>(cmd.TenantId);
            var options = new AllowAddOrderLineOptions(cmd.AllowAddOrderLineOptions.AllowAddOrderLine, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError("{command} handler could not upsert {model} to repository for TenantId: {tenantId}. Request failed.", nameof(UpdateAllowAddOrderLineOptionsCommand), nameof(AllowAddOrderLineOptions), cmd.TenantId);
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<AllowAddOrderLineOptions>(
                               nameof(AllowAddOrderLineOptions),
                               cmd.TenantId,
                               null,
                               cmd.Issuer,
                               DateTime.UtcNow,
                               options.UpdatedBy,
                               cmd.UserName,
                               options.UpdatedUtc,
                               options,
                               previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpsertTenantExternalCarrierIdOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<ExternalCarrierIdOptions>(cmd.TenantId);

            var options = new ExternalCarrierIdOptions(cmd.Options.UseExternalCarrierId, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upsertedStatus = await repo.Upsert(options.Id, options);

            if (!upsertedStatus.IsSuccessful())
            {
                logger.LogError($"{nameof(UpsertTenantExternalCarrierIdOptionsCommand)} handler could not upsert {nameof(ExternalCarrierIdOptions)}to repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<ExternalCarrierIdOptions>(
                              nameof(ExternalCarrierIdOptions),
                              cmd.TenantId,
                              null,
                              cmd.Issuer,
                              DateTime.UtcNow,
                              options.UpdatedBy,
                              cmd.UserName,
                              options.UpdatedUtc,
                              options,
                              previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpsertTenantTrolleyLocationsOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<TrolleyLocationsOptions>(cmd.TenantId);

            var options = new TrolleyLocationsOptions(cmd.Options.UseTrolleyLocations, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upsertedStatus = await repo.Upsert(options.Id, options);

            if (!upsertedStatus.IsSuccessful())
            {
                logger.LogError($"{nameof(UpsertTenantTrolleyLocationsOptionsCommand)} handler could not upsert {nameof(TrolleyLocationsOptions)}to repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }
            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<TrolleyLocationsOptions>(
                              nameof(TrolleyLocationsOptions),
                              cmd.TenantId,
                              null,
                              cmd.Issuer,
                              DateTime.UtcNow,
                              options.UpdatedBy,
                              cmd.UserName,
                              options.UpdatedUtc,
                              options,
                              previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateLegacyLocationBarcodeSupportOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetTenantRepository<LegacyLocationBarcodeSupportOptions>(cmd.TenantId);
            var options = new LegacyLocationBarcodeSupportOptions(cmd.Options.UseLegacyLocationBarcodeSupport, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateLegacyLocationBarcodeSupportOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(LegacyLocationBarcodeSupportOptions)} to repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }
            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<LegacyLocationBarcodeSupportOptions>(
                               nameof(LegacyLocationBarcodeSupportOptions),
                               cmd.TenantId,
                               null,
                               cmd.Issuer,
                               DateTime.UtcNow,
                               options.UpdatedBy,
                               cmd.UserName,
                               options.UpdatedUtc,
                               options,
                               previousOptions));

            return RequestResponse.Ok();
        }
    }
}