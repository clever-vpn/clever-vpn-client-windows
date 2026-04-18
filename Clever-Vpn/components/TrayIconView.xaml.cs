// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.ViewModel;
using Clever_Vpn.utils;
using Clever_Vpn_Windows_Kit.Common;
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
        var app = (App)Application.Current;

        App.HandleClosedEvents = false;

        try
        {
            await app.DisposeEx();
        }
        catch (Exception ex)
        {
            GlobalExceptionHandler.DebugLog($"Exit cleanup failed: {ex}");
        }
        finally
        {
            try
            {
                TrayIcon.Dispose();
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.DebugLog($"TrayIcon dispose failed: {ex}");
            }

            try
            {
                app.MainWindow.Close();
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.DebugLog($"MainWindow close failed: {ex}");
            }

            app.Exit();
            // Ensure the process exits even if background threads keep the message loop alive.
            Environment.Exit(0);
        }
    }

    public ImageSource GetIconUrl(CleverVpnState state)
    {
        return vm.VpnState switch
        {
            CleverVpnState.Started => (BitmapImage)Application.Current.Resources["UpIconImage"],
            _ => (BitmapImage)Application.Current.Resources["NormalIconImage"]
        };
    }
}
