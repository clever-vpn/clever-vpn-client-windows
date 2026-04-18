// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.utils;
using Clever_Vpn.ViewModel;
using Clever_Vpn_Windows_Kit.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn.Pages.HomePage.components;

public sealed partial class TrafficCard : UserControl
{
    VpnViewModel Vm { get; } = ((App)Application.Current).ViewModel;

    public TrafficCard()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        Vm.PropertyChanged += Vm_PropertyChanged;
        UpdateTrafficText();
        await Vm.SetTrafficSubscriptionEnabled(true);
    }

    private async void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        Vm.PropertyChanged -= Vm_PropertyChanged;
        await Vm.SetTrafficSubscriptionEnabled(false);
    }

    private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VpnViewModel.Traffic))
        {
            UpdateTrafficText();
        }
    }

    private void UpdateTrafficText()
    {
        var traffic = Vm.Traffic ?? new Traffic(0, 0);
        RxTextBlock.Text = Utils.PrettyBytes(traffic.Rx);
        TxTextBlock.Text = Utils.PrettyBytes(traffic.Tx);
    }

}
