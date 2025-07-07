// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.components;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
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

namespace Clever_Vpn.Pages.SettingsPage.componets;

[ContentProperty(Name = nameof(Body))]
public sealed partial class SettingPanel : UserControl
{
    public SettingPanel()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string) GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(SettingPanel),
            new PropertyMetadata(""));

    public UIElement Body
    {
        get =>(UIElement) GetValue(BodyProperty);
        set => SetValue(BodyProperty, value);
    }
    public static readonly DependencyProperty BodyProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(UIElement),
            typeof(SettingPanel),
            new PropertyMetadata(null));


}
