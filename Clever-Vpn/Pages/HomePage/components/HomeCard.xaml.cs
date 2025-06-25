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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn.Pages.HomePage.components;

public sealed partial class HomeCard : UserControl
{
    VpnViewModel Vm { get; } = ((App)Application.Current).ViewModel;

    public HomeCard()
    {
        InitializeComponent();
    }

    private async void OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender != null)
        {
            await Vm.ToggleVpn((sender as ToggleSwitch).IsOn);
        }
    }

    public bool UpdateToggleState(CleverVpnState s)
    {
        var ret = OnOffSwitch.IsOn;
        switch (s)
        {
            case CleverVpnState.Down:
                ret = false;
                break;
            case CleverVpnState.Up:
                ret = true;
                break;
            default:
                break;
        }
        return ret;
    }

    public bool UpdateToggleEnable(CleverVpnState s)
    {
        return s switch
        {
            CleverVpnState.Down or CleverVpnState.Up => true,
            _ => false,
        };
    }

}
