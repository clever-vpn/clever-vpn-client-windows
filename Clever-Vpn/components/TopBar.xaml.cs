using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using CommunityToolkit.Mvvm;
using Microsoft.UI.Xaml.Markup;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn.components;

[ContentProperty(Name = nameof(CenterContent))]

public sealed partial class TopBar : UserControl
{
    public TopBar()
    {
        //DataContext = this;
        this.InitializeComponent();
    }

    // 左侧内容依赖属性
    public object LeftContent
    {
        get => GetValue(LeftContentProperty);
        set => SetValue(LeftContentProperty, value);
    }
    public static readonly DependencyProperty LeftContentProperty =
        DependencyProperty.Register(
            nameof(LeftContent),
            typeof(object),
            typeof(TopBar),
            new PropertyMetadata(null));

    // 中间内容依赖属性
    public object CenterContent
    {
        get => GetValue(CenterContentProperty);
        set => SetValue(CenterContentProperty, value);
    }
    public static readonly DependencyProperty CenterContentProperty =
        DependencyProperty.Register(
            nameof(CenterContent),
            typeof(object),
            typeof(TopBar),
            new PropertyMetadata(null));

    // 右侧内容依赖属性
    public object RightContent
    {
        get => GetValue(RightContentProperty);
        set => SetValue(RightContentProperty, value);
    }
    public static readonly DependencyProperty RightContentProperty =
        DependencyProperty.Register(
            nameof(RightContent),
            typeof(object),
            typeof(TopBar),
            new PropertyMetadata(null));

}
