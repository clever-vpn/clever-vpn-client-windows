using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn.components
{
    public sealed partial class LogoEx : UserControl
    {
        public LogoEx()
        {
            InitializeComponent();
        }

        public double Ratio
        {
            get => (double)GetValue(RatioProperty);
            set => SetValue(RatioProperty, value);
        }

        public static readonly DependencyProperty RatioProperty =
            DependencyProperty.Register(
                nameof(Ratio),
                typeof(double),
                typeof(LogoEx),
                new PropertyMetadata(0.2, OnRatioChanged)); // 注册回调
                                                            //new PropertyMetadata(0.2)); // 注册回调

        private static void OnRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (LogoEx)d;
            double newValue = (double)e.NewValue;

            // 在这里响应属性变化
            control.OnRatioChanged(newValue);
        }

        private void OnRatioChanged(double newValue)
        {
            double size = newValue * Clever_Vpn.config.AppConfig.Height;
            this.Width = size;
            this.Height = size;
        }

    }
}
