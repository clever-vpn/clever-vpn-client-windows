using Clever_Vpn.utils;
using Clever_Vpn.ViewModel;
using Clever_Vpn_Windows_Kit.Data;
using Microsoft.UI.Dispatching;
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

public sealed partial class TrafficCard : UserControl
{
    VpnViewModel Vm { get; } = ((App)Application.Current).ViewModel;
    readonly DispatcherQueueTimer dqTimer;

    public TrafficCard()
    {
        InitializeComponent();
        var dq = DispatcherQueue.GetForCurrentThread();
        dqTimer = dq.CreateTimer();
        dqTimer.Interval = TimeSpan.FromMilliseconds(1000);
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        dqTimer.Start();
        dqTimer.Tick += DqTimer_Tick;
        DqTimer_Tick(null, null);
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        dqTimer.Tick -= DqTimer_Tick;
        dqTimer.Stop();
    }

    private async void DqTimer_Tick(DispatcherQueueTimer? sender, object? args)
    {
        Traffic traffic = await Task<Traffic>.Run(async () =>
        {
            return await Vm.GetTraffic();
        });

        RxTextBlock.Text = Utils.PrettyBytes(traffic.Rx);
        TxTextBlock.Text = Utils.PrettyBytes(traffic.Tx);

    }

}
