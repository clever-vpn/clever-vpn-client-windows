// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
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

namespace Clever_Vpn.components;

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
            typeof(Logo),
            new PropertyMetadata(0.2, OnRatioChanged)); // 注册回调

    private static void OnRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Logo)d;
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
