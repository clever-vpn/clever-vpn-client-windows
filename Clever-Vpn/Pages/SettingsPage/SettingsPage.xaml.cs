// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.utils;
using Clever_Vpn.ViewModel;
using Clever_Vpn_Windows_Kit.Common;
using Clever_Vpn_Windows_Kit.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Dispatching;
using Windows.ApplicationModel.Resources;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn.Pages.SettingsPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        VpnViewModel vm { get; } = ((App)Application.Current).ViewModel;
        private const string CopyGlyph = "\uE8C8";
        private const string CopiedGlyph = "\uE73E";
        private readonly DispatcherQueueTimer _copyIconResetTimer;
        private static readonly ResourceLoader ResourceLoader = ResourceLoader.GetForViewIndependentUse();

        public SettingsPage()
        {
            InitializeComponent();
            Loaded += OnSettingsPageLoaded;
            Unloaded += OnSettingsPageUnloaded;

            _copyIconResetTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
            _copyIconResetTimer.Interval = TimeSpan.FromSeconds(2.5);
            _copyIconResetTimer.IsRepeating = false;
            _copyIconResetTimer.Tick += OnCopyIconResetTimerTick;
        }

        private async void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnSettingsPageLoaded;
            ResetCopyIcon();
            if (Utils.IsPackaged())
            {
                StartupToggle.IsOn = await AutoStartHelper.IsAutoStartEnabledPackaged();
            }
            else
            {
                StartupToggle.IsOn = AutoStartHelper.IsAutoStartEnabledUnPackaged();
            }
        }

        private void OnSettingsPageUnloaded(object sender, RoutedEventArgs e)
        {
            _copyIconResetTimer.Stop();
        }

        void OnBackClick(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).MainWindow.GoBack();
        }

         async void OnAutoStartupToggle(object sender, RoutedEventArgs e) {
            bool enable = StartupToggle.IsOn;
            if (Utils.IsPackaged())
            {
               await AutoStartHelper.SetAutoStartPackaged(enable);
            }else
            {
                AutoStartHelper.SetAutoStartUnpackaged(enable);
            }

        }       
        
        void OnLogPage(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).MainWindow.Navigate(typeof(LogPage.LogPage), null);
        }


        void OnProtocolSettingLoaded(object sender, RoutedEventArgs e)
        {
            var listView = (ListView)sender;
            if (listView != null)
            {
                listView.ItemsSource = Enum.GetValues(typeof(ProtocolType)).Cast<ProtocolType>();
                if (vm.UserInfo != null)
                {
                    listView.SelectedItem = vm.UserInfo.ProtocolType;
                }
            }

        }

        async void OnProtocolSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }

            var type = (ProtocolType)e.AddedItems[0];
            await vm.UpdateProtocolType(type);
        }

        void OnCopyKeyClick(object sender, RoutedEventArgs e)
        {
            var key = FormatKey(vm.UserInfo);
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            if (TryCopyToClipboard(key))
            {
                ShowCopiedIconState();
            }
        }

        private static bool TryCopyToClipboard(string text)
        {
            try
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(text);
                Clipboard.SetContent(dataPackage);
                Clipboard.Flush();
                return true;
            }
            catch (COMException)
            {
                // Fallback for environments where WinRT clipboard API is unavailable.
                return TryCopyWithClipExe(text);
            }
        }

        private static bool TryCopyWithClipExe(string text)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "clip",
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using var process = Process.Start(startInfo);
                if (process == null)
                {
                    return false;
                }

                process.StandardInput.Write(text);
                process.StandardInput.Close();
                process.WaitForExit();
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        private void ShowCopiedIconState()
        {
            if (CopyKeyIcon == null)
            {
                return;
            }

            CopyKeyIcon.Glyph = CopiedGlyph;
            ToolTipService.SetToolTip(CopyKeyIconButton, GetResource("SettingsCopiedTooltip", "Copied"));
            _copyIconResetTimer.Stop();
            _copyIconResetTimer.Start();
        }

        private void OnCopyIconResetTimerTick(DispatcherQueueTimer sender, object args)
        {
            sender.Stop();
            ResetCopyIcon();
        }

        private void ResetCopyIcon()
        {
            if (CopyKeyIcon == null)
            {
                return;
            }

            CopyKeyIcon.Glyph = CopyGlyph;
            ToolTipService.SetToolTip(CopyKeyIconButton, GetResource("SettingsCopyTooltip", "Copy activation code"));
        }

        private static string GetResource(string key, string fallback)
        {
            try
            {
                var value = ResourceLoader.GetString(key);
                return string.IsNullOrWhiteSpace(value) ? fallback : value;
            }
            catch
            {
                return fallback;
            }
        }

        void OnSplitSettingsClick(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).MainWindow.Navigate(typeof(SplitSettingsPage), null);
        }


        public static string TipOfProtocolType(ProtocolType type)
        {
            return type switch
            {
                ProtocolType.Auto => "auto Select Protocol (default)",
                ProtocolType.UdpTunnel => "for low packet loss",
                ProtocolType.UdpFast => "for low latency",
                ProtocolType.UdpStable => "for unstable network",
                ProtocolType.TcpFast => "for UDP not available",
                ProtocolType.TcpStable => "for high reliability",
                _ => string.Empty,
            };
        }

        public static Visibility ProtocolTypeSelector(ProtocolType type)
        {
            VpnViewModel vm  = ((App)Application.Current).ViewModel;
            return  (vm.UserInfo?.ProtocolType == type) ? Visibility.Visible : Visibility.Collapsed;
        }

       public string FormatKey(UserInfo? userInfo)
        {
            string key = userInfo?.Key?.Replace("-", "") ?? "";
            var l = key.Length;
            return string.Join("-", Enumerable.Range(0, 5)
                .Select(i =>
                {
                    int start = i * 4;
                    int len = Math.Min(4, key.Length - start);
                    return len > 0 ? key.Substring(start, len) : string.Empty;
                })
                .Where(s => s.Trim().Length > 0));
        }

        public string GetConnectionProtocol(VpnConnectionInfo? connectionInfo)
            => string.IsNullOrWhiteSpace(connectionInfo?.Protocol) ? "None" : connectionInfo!.Protocol;

        public string GetCurrentProtocolSummary(UserInfo? userInfo)
            => $"Current: {(userInfo?.ProtocolType ?? ProtocolType.Auto)}";

        public string GetConnectionSubtitle(VpnConnectionInfo? connectionInfo)
            => $"Protocol: {GetConnectionProtocol(connectionInfo)}";

        // Current kit model does not expose delay yet, keep placeholder as None.
        public string GetConnectionDelay(VpnConnectionInfo? connectionInfo)
            => "None";

        public string GetConnectionUpstream(VpnConnectionInfo? connectionInfo)
            => string.IsNullOrWhiteSpace(connectionInfo?.UpStream) ? "None" : connectionInfo!.UpStream!;

        //public string FormatKey(string? key)
        //{
        //    key = key?.Replace("-", "") ?? "***";
        //    var l = key.Length;
        //    return string.Join("-", Enumerable.Range(0, 5)
        //        .Select(i =>
        //        {
        //            int start = i * 4;
        //            int len = Math.Min(4, key.Length - start);
        //            return len > 0 ? key.Substring(start, len) : string.Empty;
        //        })
        //        .Where(s => s.Trim().Length > 0));
        //}

        string GetVersion { get; } = Utils.GetAppVersion();
        string SupportUrl => vm.UserInfo?.ProviderUrl ?? "https://www.clevert-vpn.net";

    }
}
