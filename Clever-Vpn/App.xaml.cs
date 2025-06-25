using Clever_Vpn.utils;
using Clever_Vpn.ViewModel;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

//using Windows.ApplicationModel;
//using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private static AppInstance _mainInstance;

        public VpnViewModel ViewModel { get; } = new();
        public services.AppSettings AppSettings { get; set; } = new();

        private Window? _window;
        public MainWindow MainWindow => (MainWindow)_window!;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {

            InitializeComponent();
            GlobalExceptionHandler.Initialize();

        }


        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
#if UNPACKAGED
            // —— 如果是“非打包(Unpackaged)”项目，要主动给进程设置一个 AUMID，供 AppInstance 使用
            AppInstance.SetAppUserModelId("net.clever-vpn.windows.app"); 
#endif

            _mainInstance = AppInstance.FindOrRegisterForKey("MySingleInstanceKey");
            _mainInstance.Activated += async (sender, e) =>
            {
                // 1. 找到主窗口实体
                if (MainWindow == null)
                    return;
                // 2. 调用 WinUIEx 的 WindowEx 扩展方法，先恢复最小化、再置于最前

                var dispatcherQueue = MainWindow.DispatcherQueue;
                if (dispatcherQueue.HasThreadAccess)
                {
                    DoWindowActivationBehavior();
                }
                else
                {
                    dispatcherQueue.TryEnqueue(() =>
                    {
                        DoWindowActivationBehavior();
                    });
                }
            };


            // 如果不是“主实例”，就把启动事件重定向给已有实例，然后自己退出
            if (!_mainInstance.IsCurrent)
            {
                var activationArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
                await _mainInstance.RedirectActivationToAsync(activationArgs);
                Environment.Exit(0);
                return;
            }

            AppSettings = await services.SettingsService.LoadAsync();
            _window = new MainWindow();
            _window.Activate();
            await ViewModel.Init();
        }

       private  void DoWindowActivationBehavior()
        {
            try
            {
                GlobalExceptionHandler.DebugLog("DoWindowActivationBehavior running...");
                MainWindow.Restore();
                MainWindow.BringToFront();
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.DebugLog("Error in window activation behavior: " + ex);
            }
        }


        //        protected override void OnLaunched(LaunchActivatedEventArgs args)
        //        {
        //#if UNPACKAGED
        //            // —— 如果是“非打包(Unpackaged)”项目，要主动给进程设置一个 AUMID，供 AppInstance 使用
        //            AppInstance.SetAppUserModelId("com.mycompany.myapp"); 
        //#endif
        //            // —— 下面这一行在 Packaged／Unpackaged 都要执行，拿到当前“实例”对象
        //            _mainInstance = AppInstance.FindOrRegisterForKey("MySingleInstanceKey");

        //            // 如果不是“主实例”，就把启动事件重定向给已有实例，然后自己退出
        //            if (!_mainInstance.IsCurrent)
        //            {
        //                _mainInstance.RedirectActivationTo();
        //                Environment.Exit(0);
        //                return;
        //            }

        //            // 到这里说明“我是主实例”，就正常创建窗口
        //            _window = new MainWindow();
        //            _window.Activate();
        //        }

        //private async Task<bool> IsFirstAppInstance()
        //{
        //    var mainInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("main");

        //    // If the instance that's executing the OnLaunched handler right now
        //    // isn't the "main" instance.
        //    if (!mainInstance.IsCurrent)
        //    {
        //        // Redirect the activation (and args) to the "main" instance, and exit.
        //        var activatedEventArgs =
        //            Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
        //        await mainInstance.RedirectActivationToAsync(activatedEventArgs);
        //        System.Diagnostics.Process.GetCurrentProcess().Kill();
        //        return false;
        //    }

        //    return true;

        //}


    }

}
