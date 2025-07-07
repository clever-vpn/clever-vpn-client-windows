// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Clever_Vpn.config;
using Clever_Vpn.ViewModel;
using Clever_Vpn_Windows_Kit.Data;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn.Pages.HomePage.components;

public sealed partial class BottomCard : UserControl
{
    VpnViewModel Vm { get; } = ((App)Application.Current).ViewModel;

    public BottomCard()
    {
        InitializeComponent();
    }

    private bool LoadLogo(CleverVpnState s)
    {
        return s switch
        {
            CleverVpnState.Down or CleverVpnState.Connecting => true,
            _ => false,
        };
    }

    private bool LoadTrafficCard(CleverVpnState s)
    {
        return s switch
        {
            CleverVpnState.Up or CleverVpnState.Reconnecting or CleverVpnState.Disconnecting => true,
            _ => false,
        };
    }
}
