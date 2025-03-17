using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Core.Models.Settings.AppFeatures;
using Core.Interfaces;
using Core.Models;
using MapsterMapper;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using PncCore.Shared.Models.Settings;
using PncCore.Shared.Models.Settings.AppUI;
using PncCore.Shared.Models.Settings.Optimization;
using Project.Models.Shared.PncCore;
using Project.Models.Shared.Tenant;

namespace Api.Core.Facades
{
    public interface ISettingsFacade
    {
        Task<IEnumerable<LabelTemplate>> GetDefaultLabelTemplates();

        Task<IEnumerable<LabelTemplate>> GetLabelTemplates(string tenantId, string siteKey = default, LabelType? type = default, CancellationToken cancellationToken = default);

        Task<IEnumerable<Models.Settings.Optimization.PicklistOptions>> GetAllSitePicklistOptions(string tenantId, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> GetDeliveryNotes(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<DeliveryNoteSettings> GetDeliveryNote(string id, string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<DeliveryNoteSettings> GetActiveDeliveryNote(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<TransportDocumentSettings> GetTransportDocument(string tenantId, string siteKey, CancellationToken cancellationToken = default);

        Task<T> GetOption<T>(string tenantId, string siteKey = default, CancellationToken cancellationToken = default);
    }

    public class SettingsFacade : ISettingsFacade
    {
        private readonly ILogger<SettingsFacade> logger;

        private readonly IRepositoryFactory repositoryFactory;

        private readonly IMapper mapper;

        private readonly Tenant.Service.Shared.Interfaces.IClient tenantClient;

        public SettingsFacade(
            ILogger<SettingsFacade> logger,
            IRepositoryFactory repositoryFactory,
            IMapper mapper,
            Tenant.Service.Shared.Interfaces.IClient tenantClient)
        {
            this.logger = logger;
            this.repositoryFactory = repositoryFactory;
            this.mapper = mapper;
            this.tenantClient = tenantClient;
        }
        public async Task<T> GetOption<T>(string tenantId, string siteKey = default, CancellationToken cancellationToken = default)
        {
            if (typeof(T) == typeof(MagicButtonOptions))
            {
                var options = await GetMagicButtonOptions(tenantId, siteKey);
                return mapper.Map<T>(options) ?? mapper.Map<T>(MagicButtonOptions.Default());
            }
            if (typeof(T) == typeof(ApproveDeviationsOptions))
            {
                var options = await GetApproveDeviationsOptions(tenantId, siteKey, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new ApproveDeviationsOptions());
            }
            if (typeof(T) == typeof(ManualPickingOptions))
            {
                var options = await ManualPickingOptions(tenantId, siteKey);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new Models.Settings.ManualPickingOptions(string.Empty));
            }
            if (typeof(T) == typeof(PickSlotOptions))
            {
                var options = await PickSlotOptions(tenantId, siteKey);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new Models.Settings.Optimization.PickSlotOptions());
            }
            if (typeof(T) == typeof(PicklistOptions))
            {
                var options = await PicklistOptions(tenantId, siteKey);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new Models.Settings.Optimization.PicklistOptions());
            }
            if (typeof(T) == typeof(PickingThresholdOptions))
            {
                var options = await PickingThresholdOptions(tenantId, siteKey);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new Models.Settings.PickingThresholdOptions(string.Empty));
            }
            if (typeof(T) == typeof(QualityPickingRules))
            {
                var options = await QualityPickingRules(tenantId, siteKey);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new Models.Settings.QualityPickingRules(string.Empty));
            }
            if (typeof(T) == typeof(AppFlowOptions))
            {
                var options = await AppFlowOptions(tenantId, siteKey);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new Models.Settings.Optimization.AppFlowOptions());
            }
            if (typeof(T) == typeof(AppUIOptions))
            {
                var options = await AppUIOptions(tenantId, siteKey);
                return mapper.Map<T>(options) ?? mapper.Map<T>(Models.Settings.AppUI.AppUIOptions.Default);
            }
            if (typeof(T) == typeof(DeliverableAutomationOptions))
            {
                var options = await DeliverableAutomationOptions(tenantId, siteKey);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new Models.Settings.Automation.DeliverableAutomationOptions());
            }
            if (typeof(T) == typeof(PncCore.Shared.Models.Settings.AppFeatures.AppFeatureOptions))
            {
                var options = await AppFeatureOptions(tenantId, siteKey);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new AppFeatureOptions());
            }
            if (typeof(T) == typeof(OrderNotificationOptions))
            {
                var options = await OrderNotificationOptions(tenantId, siteKey);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new Models.Settings.OrderNotificationOptions((int)TimeSpan.FromMinutes(10).TotalSeconds));
            }
            if (typeof(T) == typeof(SubstitutionLimitationsOptions))
            {
                var options = await SubstitutionLimitationsOptions(tenantId, siteKey, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new SubstitutionLimitationsOptions());
            }
            if (typeof(T) == typeof(ForcePrintWeightLabelOptions))
            {
                var options = await ForcePrintWeightLabelOptions(tenantId, siteKey, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new ForcePrintWeightLabelOptions());
            }
            if (typeof(T) == typeof(OrderStatusOnDeviationsOptions))
            {
                var options = await OrderStatusOnDeviationsOptions(tenantId, siteKey, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new OrderStatusOnDeviationsOptions());
            }
            if (typeof(T) == typeof(ShowCustomerInfoOptions))
            {
                var options = await ShowCustomerInfoOptions(tenantId, siteKey, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new ShowCustomerInfoOptions());
            }
            if (typeof(T) == typeof(ForcePickAllLinesOptions))
            {
                var options = await ForcePickAllLinesOptions(tenantId, siteKey, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new ForcePickAllLinesOptions());
            }
            if (typeof(T) == typeof(CancelOrderInWebOptions))
            {
                var options = await CancelOrderInWebOptions(tenantId, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new CancelOrderInWebOptions());
            }
            if (typeof(T) == typeof(PriceToMassOptions))
            {
                var options = await PriceToMasssOptions(tenantId, siteKey, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new PriceToMassOptions());
            }
            if (typeof(T) == typeof(OverrideExpirationDateOptions))
            {
                var options = await OverrideExpirationDateOptions(tenantId, siteKey, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new OverrideExpirationDateOptions());
            }
            if (typeof(T) == typeof(RegisterLoadCarriersOptions))
            {
                var options = await GetRegisterLoadCarriersOptions(tenantId, siteKey, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new RegisterLoadCarriersOptions());
            }
            if (typeof(T) == typeof(RequireLoadCarrierOptions))
            {
                var options = await RequireLoadCarrierOptions(tenantId, siteKey, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new RequireLoadCarrierOptions());
            }
            if (typeof(T) == typeof(OverrideCustomerSubstitutionSettingOptions))
            {
                var options = await OverrideCustomerSubstitutionSettingOptions(tenantId, siteKey, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new OverrideCustomerSubstitutionSettingOptions());
            }
            if (typeof(T) == typeof(RequireCompileOptions))
            {
                var options = await RequireCompileOptions(tenantId, siteKey, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new RequireCompileOptions());
            }
            if (typeof(T) == typeof(CustomProductLocationOptions))
            {
                var options = await GetCustomProductLocationOptions(tenantId, siteKey, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new Models.Settings.Optimization.CustomProductLocationOptions());
            }
            if (typeof(T) == typeof(OrderStateTransitionRules))
            {
                var options = await OrderStateTransitionRules(tenantId, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new Models.Settings.Automation.OrderStateTransitionRules());
            }
            if (typeof(T) == typeof(AllowAddOrderLineOptions))
            {
                var options = await AllowAddOrderLineOptions(tenantId, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new AllowAddOrderLineOptions());
            }

            if (typeof(T) == typeof(ExternalCarrierIdOptions))
            {
                var options = await GetExternalCarrierIdOptions(tenantId, siteKey, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new Models.Settings.Optimization.ExternalCarrierIdOptions());
            }
            if (typeof(T) == typeof(TrolleyLocationsOptions))
            {
                var options = await GetTrolleyLocationsOptions(tenantId, siteKey, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new Models.Settings.Optimization.TrolleyLocationsOptions());
            }
            if (typeof(T) == typeof(LegacyLocationBarcodeSupportOptions))
            {
                var options = await GetLegacyLocationBarcodeSupportOptions(tenantId, cancellationToken);
                return mapper.Map<T>(options) ?? mapper.Map<T>(new Models.Settings.Optimization.LegacyLocationBarcodeSupportOptions());
            }
            return default;
        }


        // All these labetemplate methods has to get changed as well at some point, they were very "custom" however as they have less generic input parameters
        public async Task<IEnumerable<LabelTemplate>> GetDefaultLabelTemplates()
        {
            var repo = repositoryFactory.GetRepository<Models.Settings.LabelTemplate>();
            var queryDefinition = new QueryDefinition($"SELECT * FROM c");
            var templates = await repo.QueryItems<Models.Settings.LabelTemplate>(queryDefinition);

            return templates == null ? Enumerable.Empty<LabelTemplate>() : mapper.From(templates).AddParameters("context", LabelContext.System).AdaptToType<List<LabelTemplate>>();
        }

        public Task<IEnumerable<LabelTemplate>> GetLabelTemplates(string tenantId, string siteKey = default, LabelType? type = default, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(siteKey) || siteKey == "*")
            {
                return GetTenantLabelTemplates(tenantId, type, cancellationToken);
            }

            return GetSiteLabelTemplates(tenantId, siteKey, type, cancellationToken);
        }

        private async Task<IEnumerable<LabelTemplate>> GetTenantLabelTemplates(string tenantId, LabelType? type = default, CancellationToken cancellationToken = default)
        {
            var repo = repositoryFactory.GetTenantRepository<Models.Settings.LabelTemplate>(tenantId);

            IEnumerable<Models.Settings.LabelTemplate> templates;

            if (type.HasValue)
            {
                var queryDefinition = new QueryDefinition($"SELECT * FROM c WHERE c.Type = @typeValue").WithParameter("@typeValue", type.Value);
                templates = await repo.QueryItems<Models.Settings.LabelTemplate>(queryDefinition, cancellationToken);
            }
            else
            {
                var queryDefinition = new QueryDefinition($"SELECT * FROM c");
                templates = await repo.QueryItems<Models.Settings.LabelTemplate>(queryDefinition, cancellationToken);
            }

            return !templates.Any() ? Enumerable.Empty<LabelTemplate>() : mapper.From(templates).AddParameters("context", LabelContext.Tenant).AdaptToType<List<LabelTemplate>>();
        }

        private async Task<IEnumerable<LabelTemplate>> GetSiteLabelTemplates(string tenantId, string siteKey, LabelType? type = default, CancellationToken cancellationToken = default)
        {
            var tenantTemplates = GetTenantLabelTemplates(tenantId, type, cancellationToken);

            var repo = repositoryFactory.GetSiteRepository<Models.Settings.LabelTemplate>(tenantId, siteKey);

            IEnumerable<Models.Settings.LabelTemplate> siteTemplates;

            if (type.HasValue)
            {
                var queryDefinition = new QueryDefinition($"SELECT * FROM c WHERE c.Type = @typeValue").WithParameter("@typeValue", type.Value);
                siteTemplates = await repo.QueryItems<Models.Settings.LabelTemplate>(queryDefinition, cancellationToken);
            }
            else
            {
                var queryDefinition = new QueryDefinition($"SELECT * FROM c");
                siteTemplates = await repo.QueryItems<Models.Settings.LabelTemplate>(queryDefinition, cancellationToken);
            }
            var templates = mapper.From(siteTemplates).AddParameters("context", LabelContext.Site).AdaptToType<List<LabelTemplate>>();
            templates.AddRange(await tenantTemplates);

            return templates;
        }

        public async Task<IEnumerable<string>> GetDeliveryNotes(string tenantId, string siteKey, CancellationToken cancellationToken = default)
        {
            var repo = repositoryFactory.GetSiteRepository<Models.Settings.DeliveryNoteSettings>(tenantId, siteKey);
            var queryDefinition = new QueryDefinition($"SELECT * FROM c");
            var notes = await repo.QueryItems<Models.Settings.DeliveryNoteSettings>(queryDefinition, cancellationToken);

            return notes.Select(x => x.Id);
        }

        public async Task<DeliveryNoteSettings> GetDeliveryNote(string id, string tenantId, string siteKey, CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", id);
            var options = await GetOptionInternal<Models.Settings.DeliveryNoteSettings>(queryDefinition, tenantId, siteKey, cancellationToken);

            return mapper.Map<DeliveryNoteSettings>(options);
        }

        public async Task<DeliveryNoteSettings> GetActiveDeliveryNote(string tenantId, string siteKey, CancellationToken cancellationToken = default)
        {
            var activeRepo = repositoryFactory.GetSiteRepository<Models.Settings.ActiveDeliveryNote>(tenantId, siteKey);
            var queryDefinition = new QueryDefinition($"SELECT * FROM c WHERE c.id = @siteKey").WithParameter("@siteKey", siteKey);

            var active = await activeRepo.QueryItem<Models.Settings.ActiveDeliveryNote>(queryDefinition, cancellationToken);

            if (active != null)
            {
                var noteRepo = repositoryFactory.GetSiteRepository<Models.Settings.DeliveryNoteSettings>(tenantId, siteKey);
                var deliveryNoteQueryDefinition = new QueryDefinition($"SELECT * FROM c WHERE c.id = @deliveryNoteId").WithParameter("@deliveryNoteId", active.DeliveryNoteId);

                var note = await noteRepo.QueryItem<Models.Settings.DeliveryNoteSettings>(deliveryNoteQueryDefinition, cancellationToken);

                return mapper.Map<DeliveryNoteSettings>(note);
            }

            return null;
        }

        public async Task<TransportDocumentSettings> GetTransportDocument(string tenantId, string siteKey, CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($"SELECT * FROM c WHERE c.id = @id").WithParameter("@id", nameof(Models.Settings.TransportDocumentSettings));
            var options = await GetOptionInternal<Models.Settings.TransportDocumentSettings>(queryDefinition, tenantId, siteKey, cancellationToken);

            return options != null ? mapper.Map<TransportDocumentSettings>(options) : new TransportDocumentSettings();
        }

        #region PrivateOptionMethods
        private async Task<Models.Settings.MagicButtonOptions> GetMagicButtonOptions(string tenantId, string siteKey)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(MagicButtonOptions));
            var options = await GetOptionInternal<Models.Settings.MagicButtonOptions>(queryDefinition, tenantId, siteKey);
            return options;
        }

        private async Task<Models.Settings.ApproveDeviationsOptions> GetApproveDeviationsOptions(string tenantId, string siteKey, CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id").WithParameter("@id", nameof(Models.Settings.ApproveDeviationsOptions));
            var options = await GetOptionInternal<Models.Settings.ApproveDeviationsOptions>(queryDefinition, tenantId, siteKey, cancellationToken);
            return options;
        }

        private async Task<Models.Settings.ManualPickingOptions> ManualPickingOptions(string tenantId, string siteKey = null)
        {
            var queryDefinition = new QueryDefinition($"SELECT * FROM c");
            var options = await GetOptionInternal<Models.Settings.ManualPickingOptions>(queryDefinition, tenantId, siteKey);
            return options;
        }

        private async Task<Models.Settings.Optimization.PickSlotOptions> PickSlotOptions(string tenantId, string siteKey = null)
        {
            var queryDefinition = new QueryDefinition($"SELECT * FROM c");
            var options = await GetOptionInternal<Models.Settings.Optimization.PickSlotOptions>(queryDefinition, tenantId, siteKey);
            return options;
        }

        private async Task<Models.Settings.Optimization.PicklistOptions> PicklistOptions(string tenantId, string siteKey = null)
        {
            var queryDefinition = new QueryDefinition($"SELECT * FROM c");
            var options = await GetOptionInternal<Models.Settings.Optimization.PicklistOptions>(queryDefinition, tenantId, siteKey);
            return options;
        }

        private async Task<Models.Settings.PickingThresholdOptions> PickingThresholdOptions(string tenantId, string siteKey = null)
        {
            var queryDefinition = new QueryDefinition($"SELECT * FROM c");
            var options = await GetOptionInternal<Models.Settings.PickingThresholdOptions>(queryDefinition, tenantId, siteKey);
            return options;
        }

        private async Task<Models.Settings.QualityPickingRules> QualityPickingRules(string tenantId, string siteKey = null)
        {
            var queryDefinition = new QueryDefinition($"SELECT * FROM c");
            var options = await GetOptionInternal<Models.Settings.QualityPickingRules>(queryDefinition, tenantId, siteKey);
            return options;
        }

        private async Task<Models.Settings.Optimization.AppFlowOptions> AppFlowOptions(string tenantId, string siteKey)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.Optimization.AppFlowOptions));
            var options = await GetOptionInternal<Models.Settings.Optimization.AppFlowOptions>(queryDefinition, tenantId, siteKey);
            return options;
        }

        private async Task<Models.Settings.AppUI.AppUIOptions> AppUIOptions(string tenantId, string siteKey = null)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.AppUI.AppUIOptions));
            var options = await GetOptionInternal<Models.Settings.AppUI.AppUIOptions>(queryDefinition, tenantId, siteKey);
            return options;
        }

        private async Task<Models.Settings.Automation.DeliverableAutomationOptions> DeliverableAutomationOptions(string tenantId, string siteKey)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.Automation.DeliverableAutomationOptions));
            var options = await GetOptionInternal<Models.Settings.Automation.DeliverableAutomationOptions>(queryDefinition, tenantId, siteKey);
            return options;
        }

        private async Task<AppFeatureOptions> AppFeatureOptions(string tenantId, string siteKey)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.AppFeatures.AppFeatureOptions));
            var options = await GetOptionInternal<AppFeatureOptions>(queryDefinition, tenantId, siteKey);
            return options;
        }

        private async Task<Models.Settings.OrderNotificationOptions> OrderNotificationOptions(string tenantId, string siteKey)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.OrderNotificationOptions));
            var options = await GetOptionInternal<Models.Settings.OrderNotificationOptions>(queryDefinition, tenantId, siteKey);


            return options;
        }

        private async Task<Models.Settings.SubstitutionLimitationsOptions> SubstitutionLimitationsOptions(string tenantId, string siteKey = null, CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
            .WithParameter("@id", nameof(Models.Settings.SubstitutionLimitationsOptions));
            var options = await GetOptionInternal<Models.Settings.SubstitutionLimitationsOptions>(queryDefinition, tenantId, siteKey, cancellationToken);
            return options;
        }

        private async Task<Models.Settings.ForcePrintWeightLabelOptions> ForcePrintWeightLabelOptions(string tenantId, string siteKey = null, CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.ForcePrintWeightLabelOptions));
            var options = await GetOptionInternal<Models.Settings.ForcePrintWeightLabelOptions>(queryDefinition, tenantId, siteKey, cancellationToken);
            return options;
        }

        private async Task<Models.Settings.OrderStatusOnDeviationsOptions> OrderStatusOnDeviationsOptions(string tenantId, string siteKey = null, CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.OrderStatusOnDeviationsOptions));
            var options = await GetOptionInternal<Models.Settings.OrderStatusOnDeviationsOptions>(queryDefinition, tenantId, siteKey, cancellationToken);
            return options;
        }
        private async Task<Models.Settings.ShowCustomerInfoOptions> ShowCustomerInfoOptions(string tenantId, string siteKey = null, CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.ShowCustomerInfoOptions));
            var options = await GetOptionInternal<Models.Settings.ShowCustomerInfoOptions>(queryDefinition, tenantId, siteKey, cancellationToken);
            return options;
        }

        private async Task<Models.Settings.ForcePickAllLinesOptions> ForcePickAllLinesOptions(string tenantId, string siteKey = null, CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.ForcePickAllLinesOptions));
            var options = await GetOptionInternal<Models.Settings.ForcePickAllLinesOptions>(queryDefinition, tenantId, siteKey, cancellationToken);
            return options;
        }

        private async Task<Models.Settings.CancelOrderInWebOptions> CancelOrderInWebOptions(string tenantId, CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.CancelOrderInWebOptions));
            var options = await GetOptionInternal<Models.Settings.CancelOrderInWebOptions>(queryDefinition, tenantId, string.Empty, cancellationToken);
            return options;
        }
        private async Task<Models.Settings.PriceToMassOptions> PriceToMasssOptions(string tenantId, string siteKey, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.PriceToMassOptions));
            var options = await GetOptionInternal<Models.Settings.PriceToMassOptions>(queryDefinition, tenantId, siteKey, cancellationToken);
            return options;
        }

        private async Task<Models.Settings.OverrideExpirationDateOptions> OverrideExpirationDateOptions(string tenantId, string siteKey, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.OverrideExpirationDateOptions));
            var options = await GetOptionInternal<Models.Settings.OverrideExpirationDateOptions>(queryDefinition, tenantId, siteKey, cancellationToken);
            return options;
        }

        private async Task<Models.Settings.RegisterLoadCarriersOptions> GetRegisterLoadCarriersOptions(string tenantId, string siteKey, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.RegisterLoadCarriersOptions));
            var options = await GetOptionInternal<Models.Settings.RegisterLoadCarriersOptions>(queryDefinition, tenantId, siteKey, cancellationToken);
            return options;
        }

        private async Task<Models.Settings.RequireLoadCarrierOptions> RequireLoadCarrierOptions(string tenantId, string siteKey, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.RequireLoadCarrierOptions));
            var options = await GetOptionInternal<Models.Settings.RequireLoadCarrierOptions>(queryDefinition, tenantId, siteKey, cancellationToken);
            return options;
        }

        public async Task<Models.Settings.OverrideCustomerSubstitutionSettingOptions> OverrideCustomerSubstitutionSettingOptions(string tenantId, string siteKey, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.OverrideCustomerSubstitutionSettingOptions));
            var options = await GetOptionInternal<Models.Settings.OverrideCustomerSubstitutionSettingOptions>(queryDefinition, tenantId, siteKey, cancellationToken);

            return options;
        }

        public async Task<IEnumerable<Models.Settings.Optimization.PicklistOptions>> GetAllSitePicklistOptions(string tenantId, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition($"SELECT * FROM c Where c.Key LIKE @key")
                .WithParameter("@key", $"{nameof(Models.Settings.Optimization.PicklistOptions)}|%|{tenantId}");
            var options = await GetAllOptions<Models.Settings.Optimization.PicklistOptions>(queryDefinition, tenantId, cancellationToken);
            return options;
        }

        public async Task<Models.Settings.RequireCompileOptions> RequireCompileOptions(string tenantId, string siteKey, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.RequireCompileOptions));
            var options = await GetOptionInternal<Models.Settings.RequireCompileOptions>(queryDefinition, tenantId, siteKey, cancellationToken);

            return options;
        }

        public async Task<Models.Settings.Optimization.CustomProductLocationOptions> GetCustomProductLocationOptions(string tenantId, string siteKey, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.Optimization.CustomProductLocationOptions));
            var options = await GetOptionInternal<Models.Settings.Optimization.CustomProductLocationOptions>(queryDefinition, tenantId, siteKey, cancellationToken);

            return options;
        }

        private async Task<Models.Settings.Automation.OrderStateTransitionRules> OrderStateTransitionRules(string tenantId, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.Automation.OrderStateTransitionRules));
            var options = await GetTenantOption<Models.Settings.Automation.OrderStateTransitionRules>(queryDefinition, tenantId, cancellationToken);

            return options;
        }

        private async Task<Models.Settings.AllowAddOrderLineOptions> AllowAddOrderLineOptions(string tenantId, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.AllowAddOrderLineOptions));
            var options = await GetTenantOption<Models.Settings.AllowAddOrderLineOptions>(queryDefinition, tenantId, cancellationToken);

            return options;
        }

        public async Task<Models.Settings.Optimization.ExternalCarrierIdOptions> GetExternalCarrierIdOptions(string tenantId, string siteKey, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.Optimization.ExternalCarrierIdOptions));
            var options = await GetOptionInternal<Models.Settings.Optimization.ExternalCarrierIdOptions>(queryDefinition, tenantId, siteKey, cancellationToken);

            return options;
        }

        public async Task<Models.Settings.Optimization.TrolleyLocationsOptions> GetTrolleyLocationsOptions(string tenantId, string siteKey, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.Optimization.TrolleyLocationsOptions));
            var options = await GetOptionInternal<Models.Settings.Optimization.TrolleyLocationsOptions>(queryDefinition, tenantId, siteKey, cancellationToken);

            return options;
        }

        private async Task<Models.Settings.Optimization.LegacyLocationBarcodeSupportOptions> GetLegacyLocationBarcodeSupportOptions(string tenantId, CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($@"SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", nameof(Models.Settings.Optimization.LegacyLocationBarcodeSupportOptions));
            var options = await GetOptionInternal<Models.Settings.Optimization.LegacyLocationBarcodeSupportOptions>(queryDefinition, tenantId, string.Empty, cancellationToken);
            return options;
        }
        #endregion

        public async Task<T> GetOptionInternal<T>(QueryDefinition queryDefinition, string tenantId, string siteKey, CancellationToken cancellationToken = default) where T : Entity
        {
            if (string.IsNullOrEmpty(siteKey))
                siteKey = "*";

            if (siteKey != "*")
            {
                var entityType = GetEntityTypeFromOptionsType(typeof(T));
                var managementPolicy = await tenantClient.GetTenantManagementPolicy(tenantId, entityType);
                if (!managementPolicy.Failed && managementPolicy.Data is null)
                {
                    return await GetSiteOption<T>(queryDefinition, tenantId, siteKey, cancellationToken);
                }
            }
            return await GetTenantOption<T>(queryDefinition, tenantId, cancellationToken);
        }
        private async Task<T> GetTenantOption<T>(QueryDefinition queryDefinition, string tenantId, CancellationToken cancellationToken) where T : Entity
        {
            var tenantRepo = repositoryFactory.GetTenantRepository<T>(tenantId);
            var options = await tenantRepo.QueryItem<T>(queryDefinition, cancellationToken);

            return options;
        }

        private async Task<T> GetSiteOption<T>(QueryDefinition queryDefinition, string tenantId, string siteKey, CancellationToken cancellationToken) where T : Entity
        {
            var siteRepo = repositoryFactory.GetSiteRepository<T>(tenantId, siteKey);
            var options = await siteRepo.QueryItem<T>(queryDefinition, cancellationToken);

            return options ?? await GetTenantOption<T>(queryDefinition, tenantId, cancellationToken);
        }

        private async Task<IEnumerable<T>> GetAllOptions<T>(QueryDefinition queryDefinition, string tenantId, CancellationToken cancellationToken = default) where T : Entity
        {
            var repo = repositoryFactory.GetCrossPartitionKeyRepository<T>(tenantId);
            var settings =  await repo.QueryItems<T>( queryDefinition, cancellationToken);

            return settings;
        }

        private readonly Dictionary<Type, ManagementPolicyEntityType> optionsTypeToEntityTypeMap = new Dictionary<Type, ManagementPolicyEntityType>
        {
                {typeof(Api.Core.Models.Settings.PickingThresholdOptions), ManagementPolicyEntityType.PickingDeviationLimits },
                {typeof(Api.Core.Models.Settings.ManualPickingOptions), ManagementPolicyEntityType.ManualPicking },
                {typeof(Api.Core.Models.Settings.SubstitutionLimitationsOptions), ManagementPolicyEntityType.SubstitutionLimitations },
                {typeof(Api.Core.Models.Settings.ForcePrintWeightLabelOptions), ManagementPolicyEntityType.ScaleSettings },
                {typeof(Api.Core.Models.Settings.OrderStatusOnDeviationsOptions), ManagementPolicyEntityType.OrderStatusOnDeviations },
                {typeof(Api.Core.Models.Settings.ShowCustomerInfoOptions), ManagementPolicyEntityType.CustomerInfo },
                {typeof(Api.Core.Models.Settings.ForcePickAllLinesOptions), ManagementPolicyEntityType.ForcePickAllOrderLines },
                {typeof(Api.Core.Models.Settings.ApproveDeviationsOptions), ManagementPolicyEntityType.ApproveDeviations },
                {typeof(Api.Core.Models.Settings.OverrideExpirationDateOptions), ManagementPolicyEntityType.ConfirmExpirationDate },
                {typeof(Api.Core.Models.Settings.RegisterLoadCarriersOptions), ManagementPolicyEntityType.RegisterLoadCarriers },
                {typeof(Api.Core.Models.Settings.RequireLoadCarrierOptions), ManagementPolicyEntityType.RequireLoadCarrier },
                {typeof(Api.Core.Models.Settings.AppUI.AppUIOptions), ManagementPolicyEntityType.OrderPickingAppUI },
                {typeof(Api.Core.Models.Settings.AppFeatures.AppFeatureOptions), ManagementPolicyEntityType.OrderPickingAppFeatures },
                {typeof(Api.Core.Models.Settings.RequireCompileOptions), ManagementPolicyEntityType.RequireCompile },
                {typeof(Api.Core.Models.Settings.PriceToMassOptions), ManagementPolicyEntityType.PriceToMass },
                {typeof(Api.Core.Models.Settings.Optimization.CustomProductLocationOptions), ManagementPolicyEntityType.CustomProductLocations },
                {typeof(Api.Core.Models.Settings.Optimization.ExternalCarrierIdOptions), ManagementPolicyEntityType.ExternalCarrierId },
                {typeof(Api.Core.Models.Settings.Optimization.PicklistOptions), ManagementPolicyEntityType.InactivePicklist },
                {typeof(Api.Core.Models.Settings.Optimization.TrolleyLocationsOptions), ManagementPolicyEntityType.TrolleyLocations }
        };

        private ManagementPolicyEntityType GetEntityTypeFromOptionsType(Type optionsType)
        {
            if (optionsTypeToEntityTypeMap.TryGetValue(optionsType, out var entityType)) return entityType;

            return ManagementPolicyEntityType.None;
        }
    }
}