using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Api.Core.Commands.Settings;
using Api.Core.Commands.Settings.Optimization;
using Api.Core.Events.Settings;
using Api.Core.Models.Settings;
using Api.Core.Models.Settings.AppFeatures;
using Api.Core.Models.Settings.AppUI;
using Api.Core.Models.Settings.Automation;
using Api.Core.Models.Settings.Optimization;
using Api.Core.Services;
using AutoMapper;
using Core.Commands;
using Core.Interfaces;
using Core.Interfaces.Extensions;
using Core.Models;
using Infrastructure.Communication.Utilities;
using Infrastructure.Data.Repositories.AzureStorageAccount;
using Infrastructure.Messaging.Utils;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Project.Infrastructure.Cache.Redis;
using IPublisher = Infrastructure.Messaging.Publisher.IPublisher;

namespace Api.Core.CommandHandlers
{
    public class SiteSettingsCommandHandler :
        IRequestHandler<UpdateSiteManualPickingOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateSitePickingThresholdOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateSitePickSlotOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateSiteQualityPickingRulesCommand, RequestResponse>,
        IRequestHandler<UpdateSiteLabelTemplateCommand, RequestResponse>,
        IRequestHandler<DeleteSiteLabelTemplateCommand, RequestResponse>,
        IRequestHandler<UpsertDeliveryNoteCommand, RequestResponse>,
        IRequestHandler<UpsertTransportDocumentCommand, RequestResponse>,
        IRequestHandler<DeleteDeliveryNoteCommand, RequestResponse>,
        IRequestHandler<SetActiveDeliveryNoteCommand, RequestResponse>,
        IRequestHandler<UpdateAppFlowOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateAppUIOptionsCommand, RequestResponse>,
        IRequestHandler<UpsertSiteDeliverableAutomationOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateAppFeatureOptionsCommand, RequestResponse>,
        IRequestHandler<UpsertOrderNotificationOptionsCommand, RequestResponse>,
        IRequestHandler<UpsertSiteMagicBtnOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateSiteSubstitutionLimitationsOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateSiteForcePrintWeightLabelOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateSiteOrderStatusOnDeviationsOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateSiteShowCustomerInfoOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateSiteForcePickAllLinesOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateSiteApproveDeviationsOptionsCommand, RequestResponse>,
        IRequestHandler<UpdatePriceToMassOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateSitePicklistOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateOverrideExpirationDateOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateSiteRegisterLoadCarriersOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateRequireLoadCarrierOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateSiteOverrideCustomerSubstitutionCommand, RequestResponse>,
        IRequestHandler<UpdateRequireCompileOptionsCommand, RequestResponse>,
        IRequestHandler<UpdateCustomProductLocationOptionsCommand, RequestResponse>,
        IRequestHandler<UpsertSiteExternalCarrierIdOptionsCommand, RequestResponse>,
        IRequestHandler<UpsertSiteTrolleyLocationsOptionsCommand, RequestResponse>

    {
        private readonly ILogger<SiteSettingsCommandHandler> logger;

        private readonly IMediatorHandler mediatorHandler;

        private readonly IRepositoryFactory repositoryFactory;

        private readonly IImageRepositoryFactory imageRepositoryFactory;

        private readonly IPublisher publisher;

        private readonly IMapper mapper;

        private readonly IMapperHelper mapperHelper;

        private readonly IRedisCache redisCache;

        public SiteSettingsCommandHandler(
            ILogger<SiteSettingsCommandHandler> logger,
            IMediatorHandler mediatorHandler,
            IRepositoryFactory repositoryFactory,
            IImageRepositoryFactory imageRepositoryFactory,
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
            this.imageRepositoryFactory = imageRepositoryFactory;
            this.mapperHelper = mapperHelper;
            this.redisCache = redisCache;
        }

        public async Task<RequestResponse> Handle(UpdateSiteApproveDeviationsOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<ApproveDeviationsOptions>(cmd.TenantId, cmd.SiteKey);
            var options = mapperHelper.Map(cmd.ApproveDeviationsOptions, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateSiteApproveDeviationsOptionsCommand)} handler could not upsert {nameof(ApproveDeviationsOptions)} to repository for TenantId: {cmd.TenantId} and Site: {cmd.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<ApproveDeviationsOptions>(
                nameof(ApproveDeviationsOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateSiteManualPickingOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<ManualPickingOptions>(cmd.TenantId, cmd.SiteKey);
            var options = mapperHelper.Map(cmd.ManualPickingOptions, cmd.SiteKey, cmd.Issuer);

            if (string.IsNullOrWhiteSpace(options.Id))
            {
                logger.LogError($"{nameof(UpdateSiteManualPickingOptionsCommand)} handler received a request but could not map {nameof(ManualPickingOptions)} from request to a core entity without Id from SiteKey. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.BadRequest, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateSiteManualPickingOptionsCommand)} handler could not upsert {nameof(ManualPickingOptions)} to repository for TenantId: {cmd.TenantId} and SiteKey: {cmd.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<ManualPickingOptions>(
                nameof(ManualPickingOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateSitePickingThresholdOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<PickingThresholdOptions>(cmd.TenantId, cmd.SiteKey);
            var options = mapperHelper.Map(cmd.PickingThresholdOptions, cmd.SiteKey, cmd.Issuer);

            if (string.IsNullOrWhiteSpace(options.Id))
            {
                logger.LogError($"{nameof(UpdateSitePickingThresholdOptionsCommand)} handler received a request but could not map {nameof(PickingThresholdOptions)} from request to a core entity without Id from SiteKey. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.BadRequest, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateSitePickingThresholdOptionsCommand)} handler could not upsert {nameof(PickingThresholdOptions)} to repository for TenantId: {cmd.TenantId} and SiteKey: {cmd.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<PickingThresholdOptions>(
                nameof(PickingThresholdOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateSitePicklistOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<Models.Settings.Optimization.PicklistOptions>(cmd.TenantId, cmd.SiteKey);
            var options = new Models.Settings.Optimization.PicklistOptions(
                cmd.SiteKey,
                cmd.PicklistOptions.PreparationStatesInactivityThreshold,
                cmd.PicklistOptions.InProgressStatesInactivityThreshold,
                cmd.PicklistOptions.ReleaseInactivePicklist,
                cmd.Issuer);

            if (string.IsNullOrWhiteSpace(options?.Id))
            {
                logger.LogError($"{nameof(UpdateSitePicklistOptionsCommand)} handler received a request but could not map {nameof(PicklistOptions)} from request to a core entity without Id from SiteKey: {cmd.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.BadRequest, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateTenantPickSlotOptionsCommand)} handler could not upsert {nameof(PickSlotOptions)} to repository for SiteKey: {cmd.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.BadRequest, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            var picklistOptionsKey = $"{cmd.TenantId}|{cmd.SiteKey}|PicklistOptions";
            var allSitesOptionsKey = $"SitePicklistOptions|{cmd.TenantId}";

            await redisCache.Remove(picklistOptionsKey, true);
            await redisCache.Remove(allSitesOptionsKey, true);

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<Models.Settings.Optimization.PicklistOptions>(
                nameof(PicklistOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc, options, previousOptions
            ));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateSitePickSlotOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<PickSlotOptions>(cmd.TenantId, cmd.SiteKey);
            var options = mapper.Map<PickSlotOptions>(cmd.PickSlotOptions, opts =>
            {
                opts.Items.Add(MapperKeys.Id, cmd.SiteKey);
                opts.Items.Add(MapperKeys.UpdatedBy, cmd.Issuer);
            });

            if (string.IsNullOrWhiteSpace(options.Id))
            {
                logger.LogError($"{nameof(UpdateSitePickSlotOptionsCommand)} handler received a request but could not map {nameof(PickSlotOptions)} from request to a core entity without Id from SiteKey. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.BadRequest, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateSitePickSlotOptionsCommand)} handler could not upsert {nameof(PickSlotOptions)} to repository for TenantId: {cmd.TenantId} and SiteKey: {cmd.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<PickSlotOptions>(
                nameof(PickSlotOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc, options, previousOptions
            ));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateSiteQualityPickingRulesCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<QualityPickingRules>(cmd.TenantId, cmd.SiteKey);
            var options = mapperHelper.Map(cmd.QualityPickingRules, cmd.SiteKey, cmd.Issuer);

            if (string.IsNullOrWhiteSpace(options.Id))
            {
                logger.LogError($"{nameof(UpdateSiteQualityPickingRulesCommand)} handler received a request but could not map {nameof(QualityPickingRules)} from request to a core entity without Id from SiteKey. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.BadRequest, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateSiteQualityPickingRulesCommand)} handler could not upsert {nameof(QualityPickingRules)} to repository for TenantId: {cmd.TenantId} and SiteKey: {cmd.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            var redisKey = $"{cmd.TenantId}|{cmd.SiteKey}|qualityPickingRules";
            await redisCache.Remove(redisKey, true);

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<QualityPickingRules>(
                nameof(QualityPickingRules),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateSiteLabelTemplateCommand request, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<LabelTemplate>(request.TenantId, request.SiteKey);

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
                logger.LogError($"{nameof(UpdateSiteLabelTemplateCommand)} handler received a request but could not save new {nameof(LabelTemplate)} to repository for TenantId: {request.TenantId} and SiteKey: {request.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.InternalServerError, publisher, request.TenantId, $"{request.TenantId}|{request.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.OK, publisher, request.TenantId, $"{request.TenantId}|{request.SiteKey}");
            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(DeleteSiteLabelTemplateCommand request, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<LabelTemplate>(request.TenantId, request.SiteKey);

            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", request.LabelId);
            var template = await repo.QueryItem<LabelTemplate>(queryDefinition, cancellationToken);

            if (template != null)
            {
                var deleted = await repo.Delete(template);
                if (!deleted.IsSuccessful())
                {
                    logger.LogError($"{nameof(DeleteSiteLabelTemplateCommand)} handler received a request for {nameof(LabelTemplate)} with Id: {request.LabelId} for TenantId: {request.TenantId} and SiteKey: {request.SiteKey} but could delete from repository. Request failed.");
                    _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.InternalServerError, publisher, request.TenantId, $"{request.TenantId}|{request.SiteKey}");
                    return RequestResponse.RequestFailed();
                }
            }

            _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.OK, publisher, request.TenantId, $"{request.TenantId}|{request.SiteKey}");
            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpsertDeliveryNoteCommand request, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<DeliveryNoteSettings>(request.TenantId, request.SiteKey);

            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", request.Id);
            var exists = await repo.QueryItem<DeliveryNoteSettings>(queryDefinition, cancellationToken);

            DeliveryNoteSettings note;

            if (exists != null && exists?.Logo != request.Logo)
            {
                var imgUrl = await UploadImage(request.Logo, request.TenantId, request.SiteKey);
                note = new DeliveryNoteSettings(request.Id, request.NumberOfBags, request.ShowPickers, request.ShowSignature, request.NumberOfDeliveryNotes, request.Header, request.Footer, request.Css, imgUrl);
            }
            else
            {
                note = new DeliveryNoteSettings(request.Id, request.NumberOfBags, request.ShowPickers, request.ShowSignature, request.NumberOfDeliveryNotes, request.Header, request.Footer, request.Css, request.Logo);
            }

            var upserted = await repo.Upsert(note.Id, note);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpsertDeliveryNoteCommand)} handler could not upsert {nameof(DeliveryNoteSettings)} to repository for TenantId: {request.TenantId} and SiteKey: {request.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.InternalServerError, publisher, request.TenantId, $"{request.TenantId}|{request.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.OK, publisher, request.TenantId, $"{request.TenantId}|{request.SiteKey}");
            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpsertTransportDocumentCommand request, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<TransportDocumentSettings>(request.TenantId, request.SiteKey);
            var exists = await repo.ById(nameof(TransportDocumentSettings), cancellationToken);

            TransportDocumentSettings transportDocument;

            if (!string.IsNullOrEmpty(request.Image) && (exists == null || exists.Image != request.Image))
            {
                var imgUrl = await UploadImage(request.Image, request.TenantId, request.SiteKey);
                transportDocument = new TransportDocumentSettings(imgUrl, request.DocumentType, request.Issuer);
            }
            else
            {
                transportDocument = new TransportDocumentSettings(request.Image, request.DocumentType, request.Issuer);
            }

            var upserted = await repo.Upsert(transportDocument.Id, transportDocument);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpsertTransportDocumentCommand)} handler could not upsert {nameof(TransportDocumentSettings)} to repository for TenantId: {request.TenantId} and SiteKey: {request.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.InternalServerError, publisher, request.TenantId, $"{request.TenantId}|{request.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.OK, publisher, request.TenantId, $"{request.TenantId}|{request.SiteKey}");
            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(DeleteDeliveryNoteCommand request, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<DeliveryNoteSettings>(request.TenantId, request.SiteKey);

            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", request.Id);
            var note = await repo.QueryItem<DeliveryNoteSettings>(queryDefinition, cancellationToken);

            if (note != null)
            {
                var activeRepo = repositoryFactory.GetSiteRepository<ActiveDeliveryNote>(request.TenantId, request.SiteKey);

                var queryDefinitionActive = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                    .WithParameter("@id", request.SiteKey);
                var active = await activeRepo.QueryItem<ActiveDeliveryNote>(queryDefinitionActive, cancellationToken);

                if (active?.DeliveryNoteId == request.Id)
                {
                    var deleteActive = await activeRepo.Delete(active);
                    if (!deleteActive.IsSuccessful())
                    {
                        logger.LogError($"{nameof(DeleteDeliveryNoteCommand)} handler received a request for {nameof(DeliveryNoteSettings)} with Id:{request.Id} for TenantId: {request.TenantId} and SiteKey: {request.SiteKey} but could delete the active delivery note from repository. Request failed.");
                        _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.InternalServerError, publisher, request.TenantId, $"{request.TenantId}|{request.SiteKey}");
                        return RequestResponse.RequestFailed();
                    }
                }

                var deleted = await repo.Delete(note);
                if (!deleted.IsSuccessful())
                {
                    logger.LogError($"{nameof(DeleteDeliveryNoteCommand)} handler received a request for {nameof(DeliveryNoteSettings)} with Id:{request.Id} for TenantId: {request.TenantId} and SiteKey: {request.SiteKey} but could delete from repository. Request failed.");
                    _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.InternalServerError, publisher, request.TenantId, $"{request.TenantId}|{request.SiteKey}");
                    return RequestResponse.RequestFailed();
                }
            }

            _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.OK, publisher, request.TenantId, $"{request.TenantId}|{request.SiteKey}");
            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(SetActiveDeliveryNoteCommand request, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<ActiveDeliveryNote>(request.TenantId, request.SiteKey);

            var note = await repo.Upsert(request.SiteKey, new ActiveDeliveryNote(request.SiteKey, request.Id));
            if (!note.IsSuccessful())
            {
                logger.LogError($"{nameof(SetActiveDeliveryNoteCommand)} handler received a request for {nameof(ActiveDeliveryNote)} with Id:{request.Id} for TenantId: {request.TenantId} and SiteKey: {request.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.InternalServerError, publisher, request.TenantId, $"{request.TenantId}|{request.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.OK, publisher, request.TenantId, $"{request.TenantId}|{request.SiteKey}");
            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateAppFlowOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<AppFlowOptions>(cmd.TenantId, cmd.SiteKey);
            var options = new AppFlowOptions(cmd.AppFlowOptions.UsePrinterCentricWorkflow, cmd.AppFlowOptions.RegisterTypePostPick, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upsertedStatus = await repo.Upsert(options.Id, options);

            if (!upsertedStatus.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateAppFlowOptionsCommand)} handler could not upsert {nameof(AppFlowOptions)} to repository for TenantId: {cmd.TenantId} and SiteKey: {cmd.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, upsertedStatus, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<AppFlowOptions>(
                nameof(AppFlowOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateAppUIOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = GetRepository<AppUIOptions>(cmd.TenantId, cmd.SiteKey);

            var options = mapperHelper.Map(cmd.Options, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateAppUIOptionsCommand)} handler received a request but could not store {nameof(Models.Settings.AppUI.AppUIOptions)} to repository for TenantId: {cmd.TenantId}, SiteKey: {cmd.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, upserted, publisher);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<AppUIOptions>(
                nameof(AppUIOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpsertSiteDeliverableAutomationOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<DeliverableAutomationOptions>(cmd.TenantId, cmd.SiteKey);

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
                logger.LogError($"{nameof(UpsertSiteDeliverableAutomationOptionsCommand)} handler could not upsert {nameof(DeliverableAutomationOptions)} to repository for TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<DeliverableAutomationOptions>(
                nameof(DeliverableAutomationOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc, options, previousOptions
            ));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateAppFeatureOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = GetRepository<AppFeatureOptions>(cmd.TenantId, cmd.SiteKey);

            var options = new AppFeatureOptions(
                cmd.Options.DisableSearchByTextOrScanWhenSubstituting,
                cmd.Options.DisableUserCommentForDeviation,
                cmd.Options.DisableManualSiteSelection,
                cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateAppFeatureOptionsCommand)} handler could not upsert {nameof(AppFeatureOptions)} to repository for TenantId:{cmd.TenantId} and SiteKey:{cmd.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<AppFeatureOptions>(
                nameof(AppFeatureOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpsertOrderNotificationOptionsCommand request, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<OrderNotificationOptions>(request.TenantId, request.SiteKey);

            var opts = new OrderNotificationOptions(request.NewOrderTtlInSeconds);

            var upserted = await repo.Upsert(opts.Id, opts);
            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpsertOrderNotificationOptionsCommand)} handler could not upsert {nameof(OrderNotificationOptions)} to repository for TenantId: {request.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.InternalServerError, publisher);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.OK, publisher, request.TenantId);

            return RequestResponse.Ok();
        }


        public async Task<RequestResponse> Handle(UpsertSiteMagicBtnOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<MagicButtonOptions>(cmd.TenantId, cmd.SiteKey);

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
                logger.LogError($"{nameof(UpsertSiteMagicBtnOptionsCommand)} handler could not upsert {nameof(MagicButtonOptions)} to repository for TenantId: {cmd.TenantId} and SiteKey: {cmd.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<MagicButtonOptions>(
                nameof(MagicButtonOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOpts));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateSiteSubstitutionLimitationsOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<SubstitutionLimitationsOptions>(cmd.TenantId, cmd.SiteKey);
            var options = new SubstitutionLimitationsOptions(cmd.Mode, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateSiteSubstitutionLimitationsOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(SubstitutionLimitationsOptions)} to repository for SiteKey: {cmd.SiteKey} and TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, upserted, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<SubstitutionLimitationsOptions>(
                nameof(SubstitutionLimitationsOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateSiteForcePrintWeightLabelOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<ForcePrintWeightLabelOptions>(cmd.TenantId, cmd.SiteKey);
            var options = new ForcePrintWeightLabelOptions(cmd.ForcePrintWeightLabel, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateSiteForcePrintWeightLabelOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(ForcePrintWeightLabelOptions)} to repository for SiteKey: {cmd.SiteKey} and TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, upserted, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<ForcePrintWeightLabelOptions>(
                nameof(ForcePrintWeightLabelOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateSiteOrderStatusOnDeviationsOptionsCommand cmd, CancellationToken cancellationToken)
        {
            if (cmd.Options == null)
            {
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.BadRequest, publisher, cmd.TenantId);
                return RequestResponse.Dismissed();
            }

            var repo = repositoryFactory.GetSiteRepository<OrderStatusOnDeviationsOptions>(cmd.TenantId, cmd.SiteKey);

            var options = new OrderStatusOnDeviationsOptions(
                cmd.Options.MaxWeightDeviation,
                cmd.Options.MaxSubstitutionWeightDeviation,
                cmd.Options.MaxSubstitutionDeviationPiecesPerPiece,
                cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateSiteOrderStatusOnDeviationsOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(OrderStatusOnDeviationsOptions)} to repository for SiteKey: {cmd.SiteKey} and TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<OrderStatusOnDeviationsOptions>(
                nameof(OrderStatusOnDeviationsOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateSiteShowCustomerInfoOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<ShowCustomerInfoOptions>(cmd.TenantId, cmd.SiteKey);
            var options = new ShowCustomerInfoOptions(cmd.ShowCustomerInfo, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);
            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateSiteShowCustomerInfoOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(ShowCustomerInfoOptions)} to repository for SiteKey: {cmd.SiteKey} and TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, upserted, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<ShowCustomerInfoOptions>(
                nameof(ShowCustomerInfoOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateSiteForcePickAllLinesOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<ForcePickAllLinesOptions>(cmd.TenantId, cmd.SiteKey);
            var options = new ForcePickAllLinesOptions(cmd.PickAllLines, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateSiteForcePickAllLinesOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(ForcePickAllLinesOptions)} to repository for SiteKey: {cmd.SiteKey} and TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, upserted, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<ForcePickAllLinesOptions>(
                nameof(ForcePickAllLinesOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdatePriceToMassOptionsCommand request, CancellationToken cancellationToken)
        {
            var repo = GetRepository<PriceToMassOptions>(request.TenantId, request.SiteKey);

            var options = new PriceToMassOptions(request.Expression, request.Active);
            var saveStatus = HttpStatusCode.OK;

            if (string.IsNullOrEmpty(request.Expression))
            {
                saveStatus = await repo.Delete(options);
            }
            else
            {
                saveStatus = await repo.Upsert(options.Id, options);
            }

            if (!saveStatus.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdatePriceToMassOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(PriceToMassOptions)} to repository for SiteKey: {request.SiteKey} and TenantId: {request.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(request, saveStatus, publisher, request.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(request, HttpStatusCode.OK, publisher, request.TenantId);
            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateOverrideExpirationDateOptionsCommand cmd, CancellationToken cancellationToken)
        {
            IRepository<OverrideExpirationDateOptions> repo;

            if (cmd.SiteKey == "*" || string.IsNullOrEmpty(cmd.SiteKey))
            {
                repo = repositoryFactory.GetTenantRepository<OverrideExpirationDateOptions>(cmd.TenantId);
            }
            else
            {
                repo = repositoryFactory.GetSiteRepository<OverrideExpirationDateOptions>(cmd.TenantId, cmd.SiteKey);
            }

            var options = new OverrideExpirationDateOptions(cmd.OverrideExpirationDate, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var saveStatus = await repo.Upsert(options.Id, options);

            if (!saveStatus.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateOverrideExpirationDateOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(OverrideExpirationDateOptions)} to repository for SiteKey: {cmd.SiteKey} and TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, saveStatus, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<OverrideExpirationDateOptions>(
                nameof(OverrideExpirationDateOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateSiteRegisterLoadCarriersOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<RegisterLoadCarriersOptions>(cmd.TenantId, cmd.SiteKey);
            var options = new RegisterLoadCarriersOptions(cmd.RegisterLoadCarriersOptions.RegisterLoadCarriersOptionsEnabled, cmd.RegisterLoadCarriersOptions.RegisterLoadCarriersOptionsOptional, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateSiteRegisterLoadCarriersOptionsCommand)} handler could not upsert {nameof(RegisterLoadCarriersOptions)} to repository for TenantId: {cmd.TenantId} and Site: {cmd.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<RegisterLoadCarriersOptions>(
                nameof(RegisterLoadCarriersOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateRequireLoadCarrierOptionsCommand cmd, CancellationToken cancellationToken)
        {
            IRepository<RequireLoadCarrierOptions> repo;

            if (cmd.SiteKey == "*")
                repo = repositoryFactory.GetTenantRepository<RequireLoadCarrierOptions>(cmd.TenantId);
            else
                repo = repositoryFactory.GetSiteRepository<RequireLoadCarrierOptions>(cmd.TenantId, cmd.SiteKey);

            var options = new RequireLoadCarrierOptions(cmd.LoadCarrierRequirement, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var result = await repo.Upsert(options.Id, options);

            if (!result.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateRequireLoadCarrierOptionsCommand)} handler could not {nameof(repo.Upsert)} {nameof(RequireLoadCarrierOptions)} to repository for SiteKey: {cmd.SiteKey} and TenantId: {cmd.TenantId}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, result, publisher, cmd.TenantId);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<RequireLoadCarrierOptions>(
                nameof(RequireLoadCarrierOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateSiteOverrideCustomerSubstitutionCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<OverrideCustomerSubstitutionSettingOptions>(cmd.TenantId, cmd.SiteKey);
            var options = new OverrideCustomerSubstitutionSettingOptions(cmd.Options.Active, cmd.Options.Roles, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateSiteOverrideCustomerSubstitutionCommand)} handler could not upsert {nameof(OverrideCustomerSubstitutionSettingOptions)} to repository for TenantId: {cmd.TenantId} and Site: {cmd.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<OverrideCustomerSubstitutionSettingOptions>(
                nameof(OverrideCustomerSubstitutionSettingOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateRequireCompileOptionsCommand cmd, CancellationToken cancellationToken)
        {
            IRepository<RequireCompileOptions> repo;

            if (string.IsNullOrEmpty(cmd.SiteKey) || cmd.SiteKey == "*")
                repo = repositoryFactory.GetTenantRepository<RequireCompileOptions>(cmd.TenantId);
            else
                repo = repositoryFactory.GetSiteRepository<RequireCompileOptions>(cmd.TenantId, cmd.SiteKey);

            var options = new RequireCompileOptions(cmd.RequireCompile, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upserted = await repo.Upsert(options.Id, options);

            if (!upserted.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateRequireCompileOptionsCommand)} handler could not upsert {nameof(RequireCompileOptions)} to repository for TenantId:{cmd.TenantId}{( string.IsNullOrEmpty(cmd.SiteKey) || cmd.SiteKey == "*" ? "" : $" and Site: {cmd.SiteKey}" )}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher);
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId);
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<RequireCompileOptions>(
                nameof(RequireCompileOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc,
                options,
                previousOptions));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpdateCustomProductLocationOptionsCommand cmd, CancellationToken cancellationToken)
        {
            IRepository<CustomProductLocationOptions> repo;

            if (string.IsNullOrEmpty(cmd.SiteKey) || cmd.SiteKey == "*")
            {
                repo = repositoryFactory.GetTenantRepository<CustomProductLocationOptions>(cmd.TenantId);
            }
            else
            {
                repo = repositoryFactory.GetSiteRepository<CustomProductLocationOptions>(cmd.TenantId, cmd.SiteKey);
            }

            var options = new CustomProductLocationOptions(cmd.Options.UseCustomProductLocations, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upsertedStatus = await repo.Upsert(options.Id, options);

            if (!upsertedStatus.IsSuccessful())
            {
                logger.LogError($"{nameof(UpdateCustomProductLocationOptionsCommand)} handler could not upsert {nameof(CustomProductLocationOptions)}to repository for TenantId: {cmd.TenantId}{( string.IsNullOrEmpty(cmd.SiteKey) || cmd.SiteKey == "*" ? "" : $" and Site: {cmd.SiteKey}" )}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<CustomProductLocationOptions>(
                nameof(CustomProductLocationOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc, options, previousOptions
            ));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpsertSiteExternalCarrierIdOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<ExternalCarrierIdOptions>(cmd.TenantId, cmd.SiteKey);

            var options = new ExternalCarrierIdOptions(cmd.Options.UseExternalCarrierId, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upsertedStatus = await repo.Upsert(options.Id, options);

            if (!upsertedStatus.IsSuccessful())
            {
                logger.LogError($"{nameof(UpsertSiteExternalCarrierIdOptionsCommand)} handler could not upsert {nameof(ExternalCarrierIdOptions)}to repository for TenantId: {cmd.TenantId} and Site: {cmd.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<ExternalCarrierIdOptions>(
                nameof(ExternalCarrierIdOptions),
                cmd.TenantId,
                cmd.SiteKey,
                cmd.Issuer,
                DateTime.UtcNow,
                options.UpdatedBy,
                cmd.UserName,
                options.UpdatedUtc, options, previousOptions
            ));

            return RequestResponse.Ok();
        }

        public async Task<RequestResponse> Handle(UpsertSiteTrolleyLocationsOptionsCommand cmd, CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.GetSiteRepository<TrolleyLocationsOptions>(cmd.TenantId, cmd.SiteKey);

            var options = new TrolleyLocationsOptions(cmd.Options.UseTrolleyLocations, cmd.Issuer);

            var previousOptions = await repo.ById(options.Id, cancellationToken);
            var upsertedStatus = await repo.Upsert(options.Id, options);

            if (!upsertedStatus.IsSuccessful())
            {
                logger.LogError($"{nameof(UpsertSiteTrolleyLocationsOptionsCommand)} handler could not upsert {nameof(TrolleyLocationsOptions)}to repository for TenantId: {cmd.TenantId} and Site: {cmd.SiteKey}. Request failed.");
                _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.InternalServerError, publisher, cmd.TenantId, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
                return RequestResponse.RequestFailed();
            }

            _ = MessageUtil.SendTaskEvent(cmd, HttpStatusCode.OK, publisher, cmd.TenantId, cmd.TenantId, $"{cmd.TenantId}|{cmd.SiteKey}");
            _ = mediatorHandler.RaiseEvent(new SettingsUpdatedEvent<TrolleyLocationsOptions>(
                nameof(TrolleyLocationsOptions),
                cmd.TenantId,
                cmd.SiteKey,
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
        private Task<string> UploadImage(string image, string tenantId, string siteKey)
        {
            var imageRepository = imageRepositoryFactory.GetSiteImageRepository(tenantId, siteKey);
            var ext = image.Substring(image.IndexOf('/') + 1, image.IndexOf(';') - image.IndexOf('/') - 1);
            return imageRepository.UploadImage(image, ext);
        }

        private IRepository<T> GetRepository<T>(string tenantId, string siteKey = null) where T : Entity
        {
            if (ValidSiteKey(siteKey))
            {
                return repositoryFactory.GetSiteRepository<T>(tenantId, siteKey);
            }

            return repositoryFactory.GetTenantRepository<T>(tenantId);
        }

        private static bool ValidSiteKey(string siteKey)
        {
            if (string.IsNullOrEmpty(siteKey))
                return false;

            if (siteKey == "*")
                return false;

            return true;
        }
    }
}