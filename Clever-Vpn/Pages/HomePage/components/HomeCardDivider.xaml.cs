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
using Clever_Vpn.ViewModel;
using Microsoft.UI.Dispatching;
using Clever_Vpn_Windows_Kit.Common;


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

    private bool IsStartedState(CleverVpnState state)
    {
        return state == CleverVpnState.Started;
    }

    private bool IsIdleState(CleverVpnState state)
    {
        return state == CleverVpnState.Idle;
    }

    private bool IsTransitionState(CleverVpnState state)
    {
        return state == CleverVpnState.Starting || state == CleverVpnState.Stopping;
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
        StateTextBlock.Text = StateUIText(Vm.VpnState, Vm.StartTime);
    }

    private string GetStateText(CleverVpnState state, long startTime)
    {
       switch (state)
        {
            case CleverVpnState.Started:
                dqTimer.Start();
                break;
            default:
                dqTimer.Stop();
                break;
        }

        return StateUIText(state, startTime);
    }



    private string StateUIText(CleverVpnState state, long startTime)
    {
        if (state == CleverVpnState.Started)
        {
            var diff = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTime;
            if (diff < 0)
            {
                diff = 0;
            }

            TimeSpan timeSpan = TimeSpan.FromMilliseconds(diff);
            return timeSpan.ToString(@"hh\:mm\:ss");
        }

        return "Vpn Off";
    }
}
