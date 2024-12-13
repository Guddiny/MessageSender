using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.DependencyInjection;
using MessageSender.Models;
using MessageSender.Services.Configuration;
using MessageSender.Services.Interaction;
using MessageSender.Services.Routing;
using MessageSender.State;
using MessageSender.Utils.ActionWrapper;
using MessageSender.ViewModels;
using MessageSender.ViewModels.Controls;
using MessageSender.ViewModels.Dialogs;
using MessageSender.ViewModels.Pages;
using MessageSender.Views;
using MessageSender.Views.Controls;
using MessageSender.Views.Dialogs;
using MessageSender.Views.Pages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace MessageSender;

public partial class App : Application
{
    private ApplicationSettings _savedSettings = default!;

    private MainViewModel _mainViewModel = default!;

    public App()
    {
        LoadSettingsAndConfigs();
        ConfigureServices();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _mainViewModel = Ioc.Default.GetService<MainViewModel>()!;
            var viewLocator = Ioc.Default.GetService<RoutingViewLocator>()!;

            ApplyRestoredSettings();

            desktop.MainWindow = new MainWindow { DataContext = _mainViewModel };
            desktop.ShutdownRequested += DesktopShutdownRequested;

            DataTemplates.Add(viewLocator);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ApplyRestoredSettings()
    {
        _mainViewModel.AppState.AppData.SelectedDevice = new Models.Device { DeviceId = "1" };
        _mainViewModel.AppState.AppData.MessageBody = new TextDocument(_savedSettings.CurrentMessageBody);
        _mainViewModel.AppState.AppData.UserProperties = new TextDocument(_savedSettings.CurrentMessageUserProperties);
        _mainViewModel.AppState.AppData.SelectedDeviceIndex = _savedSettings.CurrentDeviceIndex;
        _mainViewModel.AppState.AppData.Devices = new ObservableCollection<Device>(_savedSettings.Devices);
        _mainViewModel.AppState.AppData.Messages = new ObservableCollection<StoredMessage>(_savedSettings.Messages);
        _mainViewModel.AppState.Settings.ThemeVariant = _savedSettings.ThemeVariant;
        RequestedThemeVariant = GetThemeVariant(_savedSettings.ThemeVariant);

        ActualThemeVariantChanged += App_ActualThemeVariantChanged;
    }

    private ThemeVariant GetThemeVariant(string storedVariant)
    {
        return storedVariant switch
        {
            "Dark" => ThemeVariant.Dark,
            "Light" => ThemeVariant.Light,
            _ => throw new ArgumentOutOfRangeException(nameof(storedVariant)),
        };
    }

    private void App_ActualThemeVariantChanged(object? sender, System.EventArgs e)
    {
        _mainViewModel.AppState.Settings.ThemeVariant = (sender as Application).ActualThemeVariant.ToString();
    }

    private void LoadSettingsAndConfigs()
    {
        _savedSettings = AppSettingsService.LoadAppState();

        var configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: true)
              .Build();
    }

    private static void ConfigureServices()
    {
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
            // Service
            .AddSingleton<AppState>()
            .AddSingleton<MqttService>()
            .AddTransient<ActionDispatcher>()
            .AddRouting(vl =>
            {
                vl.RegisterForNavigation<SideBarViewModel, SideBarView>();
                vl.RegisterForNavigation<SenderViewModel, SenderView>();
                vl.RegisterForNavigation<DeviceManagementViewModel, DeviceManagementView>();
                vl.RegisterForNavigation<MessageManagementViewModel, MessageManagementView>();
                vl.RegisterForNavigation<LogManagementViewModel, LogManagementView>();
                vl.RegisterForNavigation<ConfirmationDialogViewModel, ConfirmationDialogView>();
                vl.RegisterForNavigation<AddEditDeviceDialogViewModel, AddEditDeviceDialogView>();
                vl.RegisterForNavigation<AddEditMessageViewModel, AddEditMessageView>();
            })
            // ViewModels
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<MainViewModel>()
            .AddSingleton<SideBarViewModel>()
            .AddSingleton<SenderViewModel>()
            .AddSingleton<DeviceManagementViewModel>()
            .AddSingleton<MessageManagementViewModel>()
            .AddSingleton<LogManagementViewModel>()
            .AddSingleton<ConfirmationDialogViewModel>()
            .BuildServiceProvider());
    }

    private void DesktopShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        ApplicationSettings applicationSettings = new()
        {
            CurrentDevice = _mainViewModel.AppState.AppData.SelectedDevice,
            CurrentMessageBody = _mainViewModel.AppState.AppData.MessageBody.Text,
            CurrentMessageUserProperties = _mainViewModel.AppState.AppData.UserProperties.Text,
            CurrentDeviceIndex = _mainViewModel.AppState.AppData.SelectedDeviceIndex,
            Devices = _mainViewModel.AppState.AppData.Devices.ToList(),
            Messages = _mainViewModel.AppState.AppData.Messages.ToList(),
            ThemeVariant = _mainViewModel.AppState.Settings.ThemeVariant,
        };

        AppSettingsService.SaveAppState(applicationSettings);
    }
}