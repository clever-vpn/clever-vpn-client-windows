using Clever_Vpn.utils;
using Clever_Vpn.ViewModel;
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
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn.Pages.SettingsPage.componets;

public sealed partial class SelectedIcon : UserControl
{
    VpnViewModel Vm { get; } = ((App)Application.Current).ViewModel;

    public SelectedIcon()
    {
        InitializeComponent();
    }

    public utils.ProtocolType ProtocolType
    {
        get => (utils.ProtocolType)GetValue(ProtocolTypeProperty);
        set => SetValue(ProtocolTypeProperty, value);
    }
    public static readonly DependencyProperty ProtocolTypeProperty =
        DependencyProperty.Register(
            nameof(ProtocolType),
            typeof(utils.ProtocolType),
            typeof(SelectedIcon),
            new PropertyMetadata(utils.ProtocolType.AUTO));

   Visibility ProtocolTypeSelector(utils.ProtocolType e, utils.ProtocolType t) {
        if (e == t)
        {
            return Visibility.Visible;
        }
        else
        {
            return Visibility.Collapsed;
        }
    }

}
