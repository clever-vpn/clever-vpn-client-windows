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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn.Pages.HomePage.components;

public sealed partial class HomeCardIcon : UserControl
{
    VpnViewModel Vm { get; } = ((App)Application.Current).ViewModel;

    public HomeCardIcon()
    {
        InitializeComponent();
    }

    private Visibility ShowOnIcon(CleverVpnState state)
    {
        return state switch
        {
            CleverVpnState.Up or CleverVpnState.Reconnecting or CleverVpnState.Disconnecting => Visibility.Visible,
            _ => Visibility.Collapsed,
        };
    }

    private Visibility ShowOffIcon(CleverVpnState state)
    {
        return state switch
        {
            CleverVpnState.Down or CleverVpnState.Connecting => Visibility.Visible,
            _ => Visibility.Collapsed,
        };
    }
}
