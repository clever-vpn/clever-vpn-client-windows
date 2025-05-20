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

    private Visibility ShowLogo(VpnState s)
    {
               return s switch
        {
            VpnState.Down or VpnState.Connecting  => Visibility.Visible,
            _ => Visibility.Collapsed,
        };
    }

    private Visibility ShowTrafficCard(VpnState s)
    {
        return s switch
        {
            VpnState.Up or VpnState.Reconnecting or VpnState.Disconnecting => Visibility.Visible,
            _ => Visibility.Collapsed,
        };
    }


    private bool LoadLogo(VpnState s)
    {
        return s switch
        {
            VpnState.Down or VpnState.Connecting => true,
            _ => false,
        };
    }

    private bool LoadTrafficCard(VpnState s)
    {
        return s switch
        {
            VpnState.Up or VpnState.Reconnecting or VpnState.Disconnecting => true,
            _ => false,
        };
    }
}
