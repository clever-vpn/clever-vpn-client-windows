// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.ViewModel;
using Clever_Vpn_Windows_Kit;
using Clever_Vpn_Windows_Kit.Data;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.BadgeNotifications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.Graphics.Display;
using Windows.UI.WindowManagement;
using WinUIEx;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    //public static partial class NativeMethods
    //{
    //    // 基本声明：调用 user32.dll 的 GetDpiForWindow
    //    [LibraryImport("user32.dll")]
    //    internal static partial uint GetDpiForWindow(IntPtr hWnd);

    //}

    public sealed partial class MainWindow : WindowEx
    {
        VpnViewModel vm { get; } = ((App)Application.Current).ViewModel;
        // The main frame of the application. This is used to navigate between different pages.
        public MainWindow()
        {
            InitializeComponent();

            this.AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/appIcon.ico"));


            utils.Utils.ReSizeWindow(this, config.AppConfig.Width, config.AppConfig.Height);

            if (utils.Utils.IsPackaged())
            {
                BadgeNotificationManager.Current.SetBadgeAsGlyph(BadgeNotificationGlyph.None);
            }

            //vm.PropertyChanged += vm_PropertyChanged;
        }



        private void vm_PropertyChanged(object? s, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VpnViewModel.VpnState)) {
                if (utils.Utils.IsPackaged())
                {
                   switch(vm.VpnState)
                    {
                        case CleverVpnState.Up:
                            BadgeNotificationManager.Current.SetBadgeAsGlyph(BadgeNotificationGlyph.Available);
                            break;
                        case CleverVpnState.Down:
                            BadgeNotificationManager.Current.SetBadgeAsGlyph(BadgeNotificationGlyph.None);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch(vm.VpnState)
                    {
                        case CleverVpnState.Up:
                            this.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/appIcon-with-check.ico"));
                            break;
                        case CleverVpnState.Down:
                            this.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/appIcon.ico"));
                            break;
                        default:
                            break;
                    }
                }
                if (vm.VpnState == CleverVpnState.Up)
                {
                    this.Title = "Clever VPN - Connected";
                }
                else if (vm.VpnState == CleverVpnState.Down)
                {
                    this.Title = "Clever VPN - Disconnected";
                }
            }
        }


        public bool Navigate(Type pageType, object? parameter)
        {
            if (MainFrame.Content?.GetType() == pageType)
            {
                return false;
            }

            return MainFrame.Navigate(pageType, parameter, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        public void GoBack()
        {
            MainFrame.GoBack();
        }

    }
}
