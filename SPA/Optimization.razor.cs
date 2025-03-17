using Api.Shared.Models;
using Gateway.Shared.Models.PnC.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Project.Models.Shared.Tenant;
using Spa.Client.Facades;
using Spa.Client.Services.Localization;
using Spa.Client.Services.Modal;
using Spa.Client.Services.Toast;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Gateway.Shared.Models.PnC.Settings.Optimization;
using Spa.Client.Pages.Settings.Base;

namespace Spa.Client.Pages.Settings.Optimization
{
    public class OptimizationBase : ComponentBase
    {
        [Inject] private ISettingsFacade SettingsFacade { get; set; }

        [Inject] private IManagementPolicyFacade PolicyFacade { get; set; }

        [Inject] private IUserFacade UserFacade { get; set; }

        [Inject] private ModalService ModalService { get; set; }

        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        [Inject] private ToastService ToastService { get; set; }

        [Inject] protected ILocalizer Localizer { get; set; }

        protected bool Loading { get; set; } = true;

        protected bool Saving { get; set; }

        protected ProductLocationsOptionsVM CustomProductLocationsOptions { get; set; } = new();

        protected PickSlotOptionsFormModel PickSlotOptions { get; set; } = new();

        protected AppFlowOptionsVM AppFlowOptions { get; set; } = new();

        protected ExternalCarrierIdOptionsVM ExternalCarrierIdOptions { get; set; } = new();

        protected TrolleyLocationsOptionsVM TrolleyLocationsOptions { get; set; } = new();

        protected PicklistInactivityOptionsVM PicklistInactivityOptions { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            Loading = true;

            var tasks = new List<Task>
            {
                GetPickSlotOptions(),
                GetAppFlowOptions(),
                GetCustomProductLocationOptions(),
                GetPicklistOptions(),
                GetExternalCarrierIdOptions(),
                GetTrolleyLocationsOptions(),

                GetManagementPolicies()
            };
            await Task.WhenAll(tasks);

            Loading = false;

            SaveState();

            await base.OnInitializedAsync();
        }

        public async Task OnBeforeNavigationHandler(LocationChangingContext locationChangingContext)
        {
            if (NavigationManager.Uri != locationChangingContext.TargetLocation && StateHasChanges())
            {
                if (!await ModalService.ConfirmUnsavedChangesDialog())
                {
                    locationChangingContext.PreventNavigation();
                }
            }
        }

        protected bool StateHasChanges() => CustomProductLocationsOptions.StateHasChanged() ||
                                            PickSlotOptions.StateHasChanged() ||
                                            AppFlowOptions.StateHasChanged() ||
                                            PicklistInactivityOptions.StateHasChanged() ||
                                            ExternalCarrierIdOptions.StateHasChanged() ||
                                            TrolleyLocationsOptions.StateHasChanged();

        private void SaveState()
        {
            CustomProductLocationsOptions.SaveState();
            PickSlotOptions.SaveState();
            AppFlowOptions.SaveState();
            ExternalCarrierIdOptions.SaveState();
            TrolleyLocationsOptions.SaveState();
            PicklistInactivityOptions.SaveState();
        }

        private Task GetManagementPolicies()
        {
            var policies = new List<ManagementPolicyEntityType>
            {
                ManagementPolicyEntityType.OrderPickingAppUI,
                ManagementPolicyEntityType.CustomProductLocations,
                ManagementPolicyEntityType.ExternalCarrierId,
                ManagementPolicyEntityType.TrolleyLocations,
                ManagementPolicyEntityType.ExternalCarrierId,
                ManagementPolicyEntityType.InactivePicklist
            };

            return Task.WhenAll(policies.Select(p => PolicyFacade.HasPolicy(p).ContinueWith(t =>
            {
                var locked = t.Result;
                switch (p)
                {
                    case ManagementPolicyEntityType.OrderPickingAppUI:
                        CustomProductLocationsOptions.AppUISiteOverride = !locked;
                        break;
                    case ManagementPolicyEntityType.CustomProductLocations:
                        CustomProductLocationsOptions.CustomProductLocationsSiteOverride = !locked;
                        break;
                    case ManagementPolicyEntityType.ExternalCarrierId:
                        ExternalCarrierIdOptions.SiteOverride = !locked;
                        break;
                    case ManagementPolicyEntityType.InactivePicklist:
                        PicklistInactivityOptions.PicklistInactivitySiteOverride = !locked;
                        break;
                    case ManagementPolicyEntityType.TrolleyLocations:
                        TrolleyLocationsOptions.SiteOverride = !locked;
                        break;
                }
            })));
        }

        private async Task GetPickSlotOptions()
        {
            var pickSlotOptionsResponse = await SettingsFacade.GetPickSlotOptions();
            if (pickSlotOptionsResponse.ResponseType == ApiResponseType.Ok)
            {
                var pickSlotOptions = pickSlotOptionsResponse.Result;
                var identity = await UserFacade.GetNameById(pickSlotOptions.UpdatedBy);

                PickSlotOptions.TimeSpanHours = (int)pickSlotOptions.DefaultPickSlotTimeSpan.TotalHours;
                PickSlotOptions.TimeSpanMinutes = pickSlotOptions.DefaultPickSlotTimeSpan.Minutes;
                PickSlotOptions.MarginHours = (int)pickSlotOptions.DefaultPickSlotMargin.TotalHours;
                PickSlotOptions.MarginMinutes = pickSlotOptions.DefaultPickSlotMargin.Minutes;
                PickSlotOptions.AutoUpdatePickSlot = pickSlotOptions.AutoUpdatePickSlot;
                PickSlotOptions.SetUpdatedByAt(pickSlotOptions.UpdatedBy, pickSlotOptions.UpdatedUtc);
                PickSlotOptions.UpdatedByDisplayName = identity;
            }
        }

        private async Task GetPicklistOptions()
        {
            var picklistOptions = await SettingsFacade.GetPicklistOptions();
            if (picklistOptions.ResponseType == ApiResponseType.Ok)
            {
                var options = picklistOptions.Result;
                var identity = await UserFacade.GetNameById(picklistOptions.Result.UpdatedBy);

                PicklistInactivityOptions.PreparationStatesInactivityThresholdInMinutes = (int)options.PreparationStatesInactivityThreshold.TotalMinutes;
                PicklistInactivityOptions.InProgressStatesInactivityThresholdInMinutes = (int)options.InProgressStatesInactivityThreshold.TotalMinutes;
                PicklistInactivityOptions.UseAutoReleaseOfPicklist = options.ReleaseInactivePicklist;
                PicklistInactivityOptions.SetUpdatedByAt(options.UpdatedBy, options.UpdatedUtc);
                PicklistInactivityOptions.UpdatedByDisplayName = identity;
            }
        }

        private async Task GetAppFlowOptions()
        {
            var appFlowOptionsResponse = await SettingsFacade.GetAppFlowOptions();
            if (appFlowOptionsResponse.ResponseType == ApiResponseType.Ok)
            {
                var appFlowOptions = appFlowOptionsResponse.Result;
                var identity = await UserFacade.GetNameById(appFlowOptions.UpdatedBy);

                AppFlowOptions.UsePrinterCentricWorkflow = appFlowOptions.UsePrinterCentricWorkflow;
                AppFlowOptions.RegisterTypePostPick = appFlowOptions.RegisterTypePostPick;
                AppFlowOptions.UpdatedBy = appFlowOptions.UpdatedBy;
                AppFlowOptions.UpdatedByDisplayName = identity;
                AppFlowOptions.UpdatedAt = appFlowOptions.UpdatedUtc.ToLocalTime();
            }
        }

        private async Task GetCustomProductLocationOptions()
        {
            var customProductLocationsResponse = await SettingsFacade.GetCustomProductLocationsOptions();
            if (customProductLocationsResponse.ResponseType == ApiResponseType.Ok)
            {
                var customProductLocationsOptions = customProductLocationsResponse.Result;
                var identity = await UserFacade.GetNameById(customProductLocationsOptions.UpdatedBy);

                CustomProductLocationsOptions.UseCustomProductLocations = customProductLocationsOptions.UseCustomProductLocations;
                CustomProductLocationsOptions.UpdatedBy = customProductLocationsOptions.UpdatedBy;
                CustomProductLocationsOptions.UpdatedByDisplayName = identity;
                CustomProductLocationsOptions.UpdatedAt = customProductLocationsOptions.UpdatedUtc.ToLocalTime();
            }
        }

        private async Task GetExternalCarrierIdOptions()
        {
            var externalCarrierIdOptionsResponse = await SettingsFacade.GetExternalCarrierIdOptions();
            if (externalCarrierIdOptionsResponse.ResponseType == ApiResponseType.Ok)
            {
                var externalCarrierIdOptions = externalCarrierIdOptionsResponse.Result;
                var identity = await UserFacade.GetNameById(externalCarrierIdOptions.UpdatedBy);

                ExternalCarrierIdOptions.UseExternalCarrierId = externalCarrierIdOptions.UseExternalCarrierId;
                ExternalCarrierIdOptions.UpdatedBy = externalCarrierIdOptions.UpdatedBy;
                ExternalCarrierIdOptions.UpdatedByDisplayName = identity;
                ExternalCarrierIdOptions.UpdatedAt = externalCarrierIdOptions.UpdatedUtc.ToLocalTime();

                ExternalCarrierIdOptions.SaveState();
            };
        }

        private async Task GetTrolleyLocationsOptions()
        {
            var trolleyLocationsOptionsResponse = await SettingsFacade.GetTrolleyLocationsOptions();
            if (trolleyLocationsOptionsResponse.ResponseType == ApiResponseType.Ok)
            {
                var trolleyLocationsOptions = trolleyLocationsOptionsResponse.Result;
                var identity = await UserFacade.GetNameById(trolleyLocationsOptions.UpdatedBy);

                TrolleyLocationsOptions.UseTrolleyLocations = trolleyLocationsOptions.UseTrolleyLocations;
                TrolleyLocationsOptions.UpdatedBy = trolleyLocationsOptions.UpdatedBy;
                TrolleyLocationsOptions.UpdatedByDisplayName = identity;
                TrolleyLocationsOptions.UpdatedAt = trolleyLocationsOptions.UpdatedUtc.ToLocalTime();

                TrolleyLocationsOptions.SaveState();
            };
        }

        public async Task Save()
        {
            if (!StateHasChanges()) return;

            var dialogOptions = new DialogOptions()
            {
                Title = Localizer["#SAVECONFIGURATION"],
                BodyDescription = Localizer["#SAVEOPTIMIZATIONBODY"],
                OkButton = Localizer["#YES"],
                CancelButton = Localizer["#NO"],
                BodyQuestion = Localizer["#DOYOUWANTTOCONTINUE"]
            };

            var result = await ModalService.ShowDialogAsync(dialogOptions);
            if (!result)
            {
                return;
            }

            var tasks = new List<Task<ApiResponse>>();

            if (AppFlowOptions.StateHasChanged())
            {
                var appflow = SettingsFacade.UpsertAppFlowOptions(new AppFlowOptionsUpdateRequest(
                    AppFlowOptions.UsePrinterCentricWorkflow,
                    AppFlowOptions.RegisterTypePostPick));
                tasks.Add(appflow);
            }

            if (CustomProductLocationsOptions.ProperyHasChanged(() => CustomProductLocationsOptions.UseCustomProductLocations))
            {
                var customLocations = SettingsFacade.UpdateCustomProductLocationsOptions(new CustomProductLocationOptionsUpdateRequest(CustomProductLocationsOptions.UseCustomProductLocations));
                tasks.Add(customLocations);
            }

            if (PicklistInactivityOptions.StateHasChanged())
            {
                var task = SettingsFacade.UpdatePicklistOptions(new PicklistOptionsUpdateRequest(
                    PicklistInactivityOptions.UseAutoReleaseOfPicklist,
                    TimeSpan.FromMinutes(PicklistInactivityOptions.PreparationStatesInactivityThresholdInMinutes),
                    TimeSpan.FromMinutes(PicklistInactivityOptions.InProgressStatesInactivityThresholdInMinutes)));
                tasks.Add(task);
            }

            if (PickSlotOptions.StateHasChanged())
            {
                var pickSlotOptions = SettingsFacade.UpdatePickSlotOptions(
                    new PickSlotOptionsUpdateRequest(
                        TimeSpan.FromHours(PickSlotOptions.TimeSpanHours) + TimeSpan.FromMinutes(PickSlotOptions.TimeSpanMinutes),
                        TimeSpan.FromHours(PickSlotOptions.MarginHours) + TimeSpan.FromMinutes(PickSlotOptions.MarginMinutes),
                        PickSlotOptions.AutoUpdatePickSlot));

                tasks.Add(pickSlotOptions);
            }

            if (ExternalCarrierIdOptions.StateHasChanged())
            {
                var externalCarrierIdOptions = SettingsFacade.UpsertExternalCarrierIdOptions(new ExternalCarrierIdOptionsUpdateRequest(
                    ExternalCarrierIdOptions.UseExternalCarrierId));
                tasks.Add(externalCarrierIdOptions);
            }

            if (TrolleyLocationsOptions.StateHasChanged())
            {
                var trolleyLocationsOptions = SettingsFacade.UpsertTrolleyLocationsOptions(new TrolleyLocationsOptionsUpsertRequest(
                    TrolleyLocationsOptions.UseTrolleyLocations));
                tasks.Add(trolleyLocationsOptions);
            }

            await Task.WhenAll(tasks);

            if (tasks.Any(t => !t.IsCompletedSuccessfully || t.Result.ResponseType != ApiResponseType.Ok))
            {
                ToastService.ShowToast(Localizer["#SOMETHINGWENTWRONGTRYAGAIN!"], ToastLevel.Error);
                return;
            }

            var updateTasks = new List<Task>
            {
                GetPickSlotOptions(),
                GetAppFlowOptions(),
                GetCustomProductLocationOptions(),
                GetExternalCarrierIdOptions(),
                GetTrolleyLocationsOptions(),
                GetPicklistOptions()
            };
            await Task.WhenAll(updateTasks);

            SaveState();

            ToastService.ShowToast(Localizer["#SUCCESSFULLYUPDATED!"], ToastLevel.Success);
        }

        protected class PickSlotOptionsFormModel : BaseSettingTrackableObjectVm
        {
            [Range(0, int.MaxValue, ErrorMessage = "#VALREQ")]
            public int TimeSpanHours { get; set; }

            [Range(0, 59, ErrorMessage = "Minutes must be in range 0-59.")]
            public int TimeSpanMinutes { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "#VALREQ")]
            public int MarginHours { get; set; }

            [Range(0, 59, ErrorMessage = "Minutes must be in range 0-59.")]
            public int MarginMinutes { get; set; }

            [Range(typeof(bool), "false", "true", ErrorMessage = "Invalid value")]
            public bool AutoUpdatePickSlot { get; set; }

            public bool Equals(PickSlotOptions other)
            {
                return TimeSpanHours == (int)other.DefaultPickSlotTimeSpan.TotalHours
                       && TimeSpanMinutes == other.DefaultPickSlotTimeSpan.Minutes
                       && MarginHours == (int)other.DefaultPickSlotMargin.TotalHours
                       && MarginMinutes == other.DefaultPickSlotMargin.Minutes
                       && AutoUpdatePickSlot == other.AutoUpdatePickSlot;
            }
        }

        protected class ProductLocationsOptionsVM : BaseSettingTrackableObjectVm
        {
            public bool AppUISiteOverride { get; set; }
            public bool CustomProductLocationsSiteOverride { get; set; }
            public bool UseCustomProductLocations { get; set; }
        }

        protected class AppFlowOptionsVM : BaseSettingTrackableObjectVm
        {
            public bool UsePrinterCentricWorkflow { get; set; }
            public bool RegisterTypePostPick { get; set; }
        }

        protected class ExternalCarrierIdOptionsVM : BaseSettingTrackableObjectVm
        {
            public bool UseExternalCarrierId { get; set; }
            public bool SiteOverride { get; set; }
        }

        protected class PicklistInactivityOptionsVM : BaseSettingTrackableObjectVm
        {
            public bool UseAutoReleaseOfPicklist { get; set; }
            public int PreparationStatesInactivityThresholdInMinutes { get; set; }
            public int InProgressStatesInactivityThresholdInMinutes { get; set; }
            public bool PicklistInactivitySiteOverride { get; set; }
        }

        protected class TrolleyLocationsOptionsVM : BaseSettingTrackableObjectVm
        {
            public bool UseTrolleyLocations { get; set; }
            public bool SiteOverride { get; set; }
        }
    }
}