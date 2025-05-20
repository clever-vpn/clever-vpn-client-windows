using Clever_Vpn.utils;
using Clever_Vpn.ViewModel;
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
        }

        void OnBackClick(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).MainWindow.GoBack();
        }

        Vector3 BorderSetter()
        {
            return new Vector3(0, 0, 16);
        }
        void OnLogPage(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).MainWindow.Navigate(typeof(LogPage.LogPage), null);
        }
        void OnLogOff(object sender, RoutedEventArgs e) {
            Vm.ActivateState = ActivationState.DeActivated;
            }

        void OnProtocolSettingLoaded(object sender, RoutedEventArgs e)
        {
            var listView = (ListView)sender;
            if (listView != null)
            {
                listView.ItemsSource = Enum.GetValues(typeof(utils.ProtocolType)).Cast<utils.ProtocolType>();
            }

        }

        async void OnProtocolItemClick(object sender, ItemClickEventArgs e)
        {
            var type = (utils.ProtocolType)e.ClickedItem;
            await Vm.UpdateProtocolType(type);
            //OnProtocolSettingLoaded(sender, e);
        }

        public static string TipOfProtocolType(utils.ProtocolType type)
        {
            return type switch
            {
                utils.ProtocolType.AUTO => "auto Select Protocol (default)",
                utils.ProtocolType.UDP => "for low packet loss",
                utils.ProtocolType.KUDP => "for high packet loss",
                utils.ProtocolType.TCP => "for UDP not available",
                _ => string.Empty,
            };
        }

        public static Visibility ProtocolTypeSelector(utils.ProtocolType type)
        {
            VpnViewModel vm  = ((App)Application.Current).ViewModel;
            return  (vm.UserInfo?.ProtocolType == type) ? Visibility.Visible : Visibility.Collapsed;
        }
        string FormatKey(string key)
        {
            var l = key.Length;
            return string.Join("-", Enumerable.Range(0, 4)
                .Select(i => {
                    int start = i * 4;
                    int len = Math.Min(4, key.Length - start);
                    return len > 0 ? key.Substring(start, len) : string.Empty;
                    })
                .Where(s => s.Trim().Length > 0));
        }

        string GetVersion { get; } = utils.AppVersionHelper.GetAppVersion();

    }
}
