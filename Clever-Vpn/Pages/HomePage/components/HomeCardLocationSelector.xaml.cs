// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.ViewModel;
using Clever_Vpn_Windows_Kit.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Clever_Vpn.Pages.HomePage.components;

public sealed partial class HomeCardLocationSelector : UserControl
{
    VpnViewModel Vm { get; } = ((App)Application.Current).ViewModel;

    public HomeCardLocationSelector()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Resolves the effective current Line: match by UserInfo.Line, fall back to the default Line.
    /// </summary>
    private static Line? ResolveCurrentLine(UserInfo? userInfo, List<Line> lines)
    {
        var lineId = userInfo?.Line ?? 0;
        if (lineId > 0)
        {
            var found = lines.Find(x => x.Id == lineId);
            if (found != null) return found;
        }
        return lines.Find(x => x.IsDefault == true);
    }

    // Instance methods for x:Bind – dependency on both UserInfo and Lines ensures re-evaluation on either change.
    private ImageSource GetLineIconSource(UserInfo? userInfo, List<Line> lines)
        => GetLineIcon(ResolveCurrentLine(userInfo, lines));

    private string GetLineName(UserInfo? userInfo, List<Line> lines)
        => ResolveCurrentLine(userInfo, lines)?.Label ?? string.Empty;

    private async void OnOpenLineSelectorClick(object sender, RoutedEventArgs e)
    {
        RefreshLineList();
        await LineSelectorDlg.ShowAsync();
    }

    private async void OnLineItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not Line line) return;
        await Vm.UpdateLine(line.Id);
        LineSelectorDlg.Hide();
    }

    private async void OnRefreshLinesClick(object sender, RoutedEventArgs e)
    {
        RefreshButtonLoading.Visibility = Visibility.Visible;
        RefreshButtonText.Opacity = 0;

        await Vm.RefreshLines();
        RefreshLineList();

        RefreshButtonLoading.Visibility = Visibility.Collapsed;
        RefreshButtonText.Opacity = 1;
    }

    private void RefreshLineList()
    {
        var lines = Vm.Lines
            .OrderByDescending(x => x.IsDefault == true)
            .ThenBy(x => x.Label, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        LineListView.ItemsSource = lines;

        var currentLine = ResolveCurrentLine(Vm.UserInfo, lines);
        LineListView.SelectedItem = currentLine != null
            ? lines.Find(x => x.Id == currentLine.Id)
            : null;
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        LineSelectorDlg.Hide();
    }

    public static ImageSource GetLineIcon(Line? line)
    {
        if (line == null)
            return GetLineIconPath(null, "service");
        return GetLineIconPath(line.Icon, line.IconKind);
    }

    public static ImageSource GetLineIconPath(string? icon, string? iconKind)
    {
        var relativePath = LineIconResolver.GetSvgRelativePath(icon, iconKind);
        return new SvgImageSource(new Uri($"ms-appx:///{relativePath}"));
    }

    public static ImageSource GetRelayIcon() => GetLineIconPath("relay", "service");

    public static ImageSource GetUpstreamIcon() => GetLineIconPath("upstream", "service");

    public static string GetLineFactorText(double factor)
    {
        var f = factor > 0 ? factor : 1d;
        return $"x{f.ToString("0.##", CultureInfo.InvariantCulture)}";
    }

    public static Visibility BoolToVisibility(bool value)
        => value ? Visibility.Visible : Visibility.Collapsed;
}
