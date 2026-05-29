// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.ViewModel;
using Clever_Vpn_Windows_Kit;
using Clever_Vpn_Windows_Kit.Common;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Dispatching;
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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
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
        private readonly DispatcherQueueTimer _copyIconResetTimer;
        private CancellationTokenSource? _errorAutoCloseCancellation;
        private const string CopyGlyph = "\uE8C8";
        private const string CopySuccessGlyph = "\uE73E";

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

            vm.PropertyChanged += vm_PropertyChanged;
            MainErrorBar.IsOpen = vm.LastError != null;

            _copyIconResetTimer = DispatcherQueue.CreateTimer();
            _copyIconResetTimer.Interval = TimeSpan.FromSeconds(1.5);
            _copyIconResetTimer.IsRepeating = false;
            _copyIconResetTimer.Tick += OnCopyIconResetTimerTick;

            Closed += OnWindowClosed;
        }

        private void vm_PropertyChanged(object? s, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VpnViewModel.LastError))
            {
                var hasError = vm.LastError != null;
                MainErrorBar.IsOpen = hasError;
                CancelErrorAutoClose();

                if (!hasError)
                {
                    CopyErrorIcon.Glyph = CopyGlyph;
                }
                else
                {
                    var cancellation = new CancellationTokenSource();
                    _errorAutoCloseCancellation = cancellation;
                    _ = ScheduleErrorAutoCloseAsync(cancellation);
                }
            }

            if (e.PropertyName == nameof(VpnViewModel.VpnState)) {
                if (utils.Utils.IsPackaged())
                {
                   switch(vm.VpnState)
                    {
                        case CleverVpnState.Started:
                            BadgeNotificationManager.Current.SetBadgeAsGlyph(BadgeNotificationGlyph.Available);
                            break;
                        case CleverVpnState.Idle:
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
                        case CleverVpnState.Started:
                            this.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/appIcon-with-check.ico"));
                            break;
                        case CleverVpnState.Idle:
                            this.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/appIcon.ico"));
                            break;
                        default:
                            break;
                    }
                }
                if (vm.VpnState == CleverVpnState.Started)
                {
                    this.Title = "Clever VPN - Connected";
                }
                else if (vm.VpnState == CleverVpnState.Idle)
                {
                    this.Title = "Clever VPN - Disconnected";
                }
            }
        }

        private async Task ScheduleErrorAutoCloseAsync(CancellationTokenSource cancellation)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellation.Token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (_errorAutoCloseCancellation != cancellation)
            {
                return;
            }

            RunOnMainUiThread(() =>
            {
                if (_errorAutoCloseCancellation != cancellation || vm.LastError == null)
                {
                    return;
                }

                vm.ClearLastErrorCommand.Execute(null);
            });
        }

        private void CancelErrorAutoClose()
        {
            var cancellation = Interlocked.Exchange(ref _errorAutoCloseCancellation, null);
            if (cancellation == null)
            {
                return;
            }

            cancellation.Cancel();
            cancellation.Dispose();
        }

        private void OnCopyErrorClick(object sender, RoutedEventArgs e)
        {
            var content = vm.LastError?.FormattedMessage;
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            try
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(content);
                Clipboard.SetContent(dataPackage);
                Clipboard.Flush();

                CopyErrorIcon.Glyph = CopySuccessGlyph;
                _copyIconResetTimer.Stop();
                _copyIconResetTimer.Start();
            }
            catch
            {
                CopyErrorIcon.Glyph = CopyGlyph;
            }
        }

        private void OnCopyIconResetTimerTick(DispatcherQueueTimer sender, object args)
        {
            sender.Stop();
            CopyErrorIcon.Glyph = CopyGlyph;
        }

        private void OnWindowClosed(object sender, WindowEventArgs args)
        {
            vm.PropertyChanged -= vm_PropertyChanged;
            CancelErrorAutoClose();
            _copyIconResetTimer.Stop();
            _copyIconResetTimer.Tick -= OnCopyIconResetTimerTick;
        }

        private void RunOnMainUiThread(Action action)
        {
            if (DispatcherQueue.HasThreadAccess)
            {
                action();
                return;
            }

            DispatcherQueue.TryEnqueue(() => action());
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
