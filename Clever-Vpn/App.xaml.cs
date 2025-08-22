// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.config;
using Clever_Vpn.utils;
using Clever_Vpn.ViewModel;
using Clever_Vpn_Windows_Kit.Data;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.BadgeNotifications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

//using Windows.ApplicationModel;
//using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        //private static AppInstance _mainInstance;
        public static bool HandleClosedEvents { get; set; } = true;


        public VpnViewModel ViewModel { get; } = new();
        public services.AppSettings AppSettings { get; set; } = new();

        private Window? _window;
        public MainWindow MainWindow => (MainWindow)_window!;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {            
            GlobalExceptionHandler.Initialize();
            if (!Utils.IsPackaged())
            {
                try
                {
                    NativeMethods.SetCurrentProcessExplicitAppUserModelID("net.clever-vpn.window-app");
                }
                catch (Exception ex)
                {
                    GlobalExceptionHandler.DebugLog("Failed to set AppUserModelID: " + ex.Message);
                }
            }
            InitializeComponent();
            
            GlobalExceptionHandler.DebugLog("Clever-Vpn started!");
            
        }


        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
#if UNPACKAGED
            // —— 如果是“非打包(Unpackaged)”项目，要主动给进程设置一个 AUMID，供 AppInstance 使用
            AppInstance.SetAppUserModelId("net.clever-vpn.windows.app");
#endif

            var _mainInstance = AppInstance.FindOrRegisterForKey("MySingleInstanceKey");
            _mainInstance.Activated += async (sender, e) =>
            {
                // 1. 找到主窗口实体
                if (MainWindow == null)
                    return;
                // 2. 调用 WinUIEx 的 WindowEx 扩展方法，先恢复最小化、再置于最前

                var dispatcherQueue = MainWindow.DispatcherQueue;
                if (dispatcherQueue.HasThreadAccess)
                {
                    DoWindowActivationBehavior();
                }
                else
                {
                    dispatcherQueue.TryEnqueue(() =>
                    {
                        DoWindowActivationBehavior();
                    });
                }
            };
            // 如果不是“主实例”，就把启动事件重定向给已有实例，然后自己退出
            if (!_mainInstance.IsCurrent)
            {
                var activationArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
                await _mainInstance.RedirectActivationToAsync(activationArgs);
                Environment.Exit(0);
                return;
            }

            AppSettings = await services.SettingsService.LoadAsync();
            _window = new MainWindow();
            ViewModel.PropertyChanged += vm_PropertyChanged;

            _window.Closed += (s, e) =>
            {
                if (HandleClosedEvents)
                {
                    e.Handled = true; // 如果处理关闭事件，就设置为已处理
                    MainWindow.Hide();
                }
            };

            //_window.AppWindow.Closing += OnAppWindowClosing;
            await ViewModel.Init();

            if (AutoStartHelper.IsAutoStartup())
            {
                //await ViewModel.Init();
                _window.Hide();
            }
            else
            {
                _window.Activate();
                //await ViewModel.Init();
            }

            if (AppSettings.VpnIsOn)
            {
                await ViewModel.ToggleVpn(true);
            }

        }

        private void vm_PropertyChanged(object? s, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VpnViewModel.VpnState))
            {
                if (utils.Utils.IsPackaged())
                {
                    switch (ViewModel.VpnState)
                    {
                        case CleverVpnState.Up:
                            BadgeNotificationManager.Current.SetBadgeAsGlyph(BadgeNotificationGlyph.Available);
                            break;
                        case CleverVpnState.Down:
                            BadgeNotificationManager.Current.SetBadgeAsGlyph(BadgeNotificationGlyph.None);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (ViewModel.VpnState)
                    {
                        case CleverVpnState.Up:
                            MainWindow.SetIcon(System.IO.Path.Combine(AppContext.BaseDirectory, "Assets/appIcon-with-check.ico"));
                            break;
                        case CleverVpnState.Down:
                            MainWindow.SetIcon(System.IO.Path.Combine(AppContext.BaseDirectory, "Assets/appIcon.ico"));
                            break;
                        default:
                            break;
                    }
                }
                if (ViewModel.VpnState == CleverVpnState.Up)
                {
                    MainWindow.Title = "Clever VPN - Connected";
                }
                else if (ViewModel.VpnState == CleverVpnState.Down)
                {
                    MainWindow.Title = "Clever VPN - Disconnected";
                }
            }
        }

        public async Task DisposeEx()
        {
            ViewModel.PropertyChanged -= vm_PropertyChanged;
            var settings = new services.AppSettings();
            settings.PolicyAccepted = AppSettings.PolicyAccepted;
            settings.VpnIsOn = AppSettings.VpnIsOn;

            await ViewModel.ToggleVpn(false);
            await services.SettingsService.SaveAsync(settings);
        }

        private void DoWindowActivationBehavior()
        {
            try
            {
                GlobalExceptionHandler.DebugLog("DoWindowActivationBehavior running...");
                MainWindow.Restore();
                MainWindow.BringToFront();
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.DebugLog("Error in window activation behavior: " + ex);
            }
        }


    }

}
