// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn.Pages.ActivatePage.components
{
    public sealed partial class ActivateCard : UserControl
    {
        services.AppSettings Settings { get; } = ((App)Application.Current).AppSettings;
        VpnViewModel Vm { get; } = ((App)Application.Current).ViewModel;

        public ActivateCard()
        {
            InitializeComponent();
            this.DataContext = this;

            PolicyCheckBox.IsChecked = Settings.PolicyAccepted;
        }

        private async void OnAccepted(object sender, RoutedEventArgs e)
        {
            Settings.PolicyAccepted = true;
           await services.SettingsService.SaveAsync(Settings);
        }

        private async void OnUnAccepted(object sender, RoutedEventArgs e)
        {
            Settings.PolicyAccepted = false;
            await services.SettingsService.SaveAsync(Settings);

        }


        private readonly string policyText = """
For the app to work with the Clever VPN service and in-app purchases, the following device data is associated with your account:

- Unique random device ID
- Device name or hostname
- OS version and type
- CPU architecture
- Activation Key

**Clever VPN values your privacy**. You can sign out from the device and delete your data by deactivating at any time.
""";

    }
}
