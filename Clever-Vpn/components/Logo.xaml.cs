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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn.components
{
    public partial class RatioToSizeConverter : IValueConverter
    {
        // 传入 Ratio（绑定值）和 WindowHeight（通过 ConverterParameter）
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double ratio )
            {
                return ratio * Clever_Vpn.config.AppConfig.Height;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }

    public sealed partial class Logo : UserControl
    {
        public Logo()
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
                //new PropertyMetadata(0.2, OnRatioChanged)); // 注册回调
                new PropertyMetadata(0.2)); // 注册回调

    }
}
