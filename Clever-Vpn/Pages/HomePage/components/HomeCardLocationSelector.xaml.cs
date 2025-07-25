// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.utils;
using Clever_Vpn.ViewModel;
using Clever_Vpn_Windows_Kit.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn.Pages.HomePage.components;

public sealed partial class HomeCardLocationSelector : UserControl
{
    VpnViewModel Vm { get; } = ((App)Application.Current).ViewModel;

    public HomeCardLocationSelector()
    {
        InitializeComponent();
    }

    private ImageSource GetFlag(UserInfo? userInfo)
    {
        string? code = null;
        var locationId = userInfo?.LocationId;

        if (locationId != null)
        {
            code = Vm.Locations.Find(x => x.Id == locationId)?.Code;
        }

        return GetFlagFromCode(code);
    }

    private string GetLocationName(UserInfo? userInfo)
    {
       var locationId = userInfo?.LocationId;
        var name = GetI18nFromKey("AutoSelected");
        if (locationId != null)
        {
            name = Vm.Locations.Find(x => x.Id == locationId)?.Label ?? name;
        } 

        return name;
    }

    private async void  OpenDlg(object sender, RoutedEventArgs e)
    {
        UpdateUILocations();
        await MyDlg.ShowAsync();
    }
    private async void OnItemClick(object sender, ItemClickEventArgs e)
    {
        var location = (Location)e.ClickedItem;
        int? id = (location.Id == -1) ? null : location.Id;
        await Vm.UpdateLocation(id);
        MyDlg.Hide();
    }

    private async void OnUpdateClick(object sender, RoutedEventArgs e)
    {
        UpdateButtonLoading.Visibility = Visibility.Visible;
        UpdateButtonText.Opacity = 0;

        await Vm.UpdateLocations(true);
        UpdateUILocations();

        UpdateButtonLoading.Visibility = Visibility.Collapsed;
        UpdateButtonText.Opacity = 1;
    }

    private void UpdateUILocations()
    {

        List<Location> newList = (new[] { new Location(-1, "", GetI18nFromKey("AutoSelected")) }).Concat(Vm.Locations).ToList();
        AddressListView.ItemsSource = newList;
    }

    private string GetI18nFromKey(string key)
    {
        var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
        return  resourceLoader.GetString(key);
        
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        MyDlg.Hide();
    }

    public static Visibility ShowSelected(int id)
    {
        VpnViewModel Vm = ((App)Application.Current).ViewModel;
        if (Vm.UserInfo?.LocationId == id)
        {
            return Visibility.Visible;
        }
        else
        {
            return Visibility.Collapsed;
        }
    }

    public static ImageSource GetFlagFromCode(string? code)
    {
        if (code?.Length > 0)
        {
            return new SvgImageSource(new Uri($"ms-appx:///Assets/Flags/{code.ToLower()}.svg"));
        }
        else
        {
            return new SvgImageSource(new Uri("ms-appx:///Assets/Flags/global.svg"));

        }
    }


}
