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
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.



namespace Clever_Vpn.components;

[ContentProperty(Name = nameof(Body))]
public sealed partial class Layout : UserControl
{
    public Layout()
    {
        InitializeComponent();
    }

    // TopBar
    public object TopBar
    {
        get => GetValue(TopBarProperty);
        set => SetValue(TopBarProperty, value);
    }
    public static readonly DependencyProperty TopBarProperty =
        DependencyProperty.Register(
            nameof(TopBar),
            typeof(object),
            typeof(TopBar),
            new PropertyMetadata(null));

    // Body
    public object Body
    {
        get => GetValue(BodyProperty);
        set => SetValue(BodyProperty, value);
    }
    public static readonly DependencyProperty BodyProperty =
        DependencyProperty.Register(
            nameof(Body),
            typeof(object),
            typeof(TopBar),
            new PropertyMetadata(null));

    // Footer
    public object Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }
    public static readonly DependencyProperty FooterProperty =
        DependencyProperty.Register(
            nameof(Footer),
            typeof(object),
            typeof(TopBar),
            new PropertyMetadata(null));
}