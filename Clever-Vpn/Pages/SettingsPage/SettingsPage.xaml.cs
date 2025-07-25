// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.utils;
using Clever_Vpn.ViewModel;
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
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn.Pages.SettingsPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        VpnViewModel Vm { get; } = ((App)Application.Current).ViewModel;

        public SettingsPage()
        {
            InitializeComponent();
            Loaded += OnSettingsPageLoaded;
        }

        private async void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnSettingsPageLoaded;
            if (Utils.IsPackaged())
            {
                StartupToggle.IsOn = await AutoStartHelper.IsAutoStartEnabledPackaged();
            }
            else
            {
                StartupToggle.IsOn = AutoStartHelper.IsAutoStartEnabledUnPackaged();
            }
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
            }

        }

        async void OnProtocolItemClick(object sender, ItemClickEventArgs e)
        {
            var type = (ProtocolType)e.ClickedItem;
            await Vm.UpdateProtocolType(type);
            //OnProtocolSettingLoaded(sender, e);
        }


        public static string TipOfProtocolType(ProtocolType type)
        {
            return type switch
            {
                ProtocolType.AUTO => "auto Select Protocol (default)",
                ProtocolType.UDP => "for low packet loss",
                ProtocolType.KUDP => "for high packet loss",
                ProtocolType.TCP => "for UDP not available",
                _ => string.Empty,
            };
        }

        public static Visibility ProtocolTypeSelector(ProtocolType type)
        {
            VpnViewModel vm  = ((App)Application.Current).ViewModel;
            return  (vm.UserInfo?.ProtocolType == type) ? Visibility.Visible : Visibility.Collapsed;
        }
        string FormatKey(string key)
        {
            key = key.Replace("-", "");
            var l = key.Length;
            return string.Join("-", Enumerable.Range(0, 5)
                .Select(i => {
                    int start = i * 4;
                    int len = Math.Min(4, key.Length - start);
                    return len > 0 ? key.Substring(start, len) : string.Empty;
                    })
                .Where(s => s.Trim().Length > 0));
        }

        string GetVersion { get; } = Utils.GetAppVersion();

    }
}
