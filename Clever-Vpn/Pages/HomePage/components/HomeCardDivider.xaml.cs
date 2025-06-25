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
using Clever_Vpn.ViewModel;
using Microsoft.UI.Dispatching;
using Clever_Vpn_Windows_Kit.Data;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn.Pages.HomePage.components;

public sealed partial class HomeCardDivider : UserControl
{
    VpnViewModel Vm { get; } = ((App)Application.Current).ViewModel;
    readonly DispatcherQueueTimer dqTimer;
    public HomeCardDivider()
    {
        InitializeComponent();

        var dq = DispatcherQueue.GetForCurrentThread();
        dqTimer = dq.CreateTimer();
        dqTimer.Interval = TimeSpan.FromMilliseconds(1000);
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        dqTimer.Tick += DqTimer_Tick;
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        dqTimer.Tick -= DqTimer_Tick;
        dqTimer.Stop();
    }

    private void DqTimer_Tick(DispatcherQueueTimer sender, object args)
    {
        StateTextBlock.Text = StateUIText();
    }

    private string GetStateText(CleverVpnState s)
    {
       switch (s)
        {
            case CleverVpnState.Up:
                dqTimer.Start();
                break;
            case CleverVpnState.Down:
                dqTimer.Stop();
                break;
            default:
                break;
        }

        return StateUIText();
    }



    private string StateUIText()
    {
        if (Vm.VpnState == CleverVpnState.Up || Vm.VpnState == CleverVpnState.Reconnecting)
        {
            var diff = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Vm.StartTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(diff);
            return timeSpan.ToString(@"hh\:mm\:ss");
        }
        else
        {
            return "Vpn Off";

        }
    }
}
