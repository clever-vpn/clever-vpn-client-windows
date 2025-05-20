using Clever_Vpn.ViewModel;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.Graphics.Display;

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

    public sealed partial class MainWindow : Window
    {
        VpnViewModel vm { get; } = ((App)Application.Current).ViewModel;
        // The main frame of the application. This is used to navigate between different pages.
        public MainWindow()
        {
            InitializeComponent();

            this.AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/appIcon.ico"));

            utils.Utils.ReSizeWindow(this, config.AppConfig.Width, config.AppConfig.Height);

            //this.RootLayout.DataContext = vm;

            //vm.PropertyChanged += Vm_PropertyChanged;
            //this.Closed += MainWindow_Closed;

            //NavigateByActivateState();
        }

        //private void Vm_PropertyChanged(object? s, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == nameof(VpnViewModel.ActivateState))
        //        NavigateByActivateState();
        //}

        //private void MainWindow_Closed(object? sender, WindowEventArgs e)
        //{
        //    vm.PropertyChanged -= Vm_PropertyChanged;
        //    this.Closed -= MainWindow_Closed;
        //}

        //void NavigateByActivateState()
        //{
        //    switch (vm.ActivateState)
        //    {
        //        case ActivationState.Activated:
        //            Navigate(typeof(Pages.HomePage.HomePage), null);
        //            break;
        //        default:
        //            Navigate(typeof(Pages.ActivatePage.ActivatePage), null);
        //            break;
        //    }

        //    MainFrame.BackStack.Clear();
        //}

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
