// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.ViewModel;
using Clever_Vpn_Windows_Kit.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Threading.Tasks;
using WinUIEx;

namespace Clever_Vpn.components;

public partial class TrayIconView : UserControl
{
    VpnViewModel vm { get; } = ((App)Application.Current).ViewModel;

    public TrayIconView()
    {
        InitializeComponent();
    }

    [RelayCommand]
    public void ShowHideWindow()
    {
        var window = ((App)Application.Current).MainWindow;
        if (window == null)
        {
            return;
        }

        if (window.Visible)
        {
            window.AppWindow.Hide();


        }
        else
        {
            window.Restore();
            window.Activate();
        }
    }

    [RelayCommand]
    public async Task ExitApplication()
    {
        try
        {
            App.HandleClosedEvents = false;
            var app = ((App)Application.Current);
            app.MainWindow.Hide();

            await app.DisposeEx();
            TrayIcon.Dispose();
            app.Exit();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public ImageSource GetIconUrl(CleverVpnState state)
    {
        return vm.VpnState switch
        {
            CleverVpnState.Up => (BitmapImage)Application.Current.Resources["UpIconImage"],
            _ => (BitmapImage)Application.Current.Resources["NormalIconImage"]
        };
    }
}
