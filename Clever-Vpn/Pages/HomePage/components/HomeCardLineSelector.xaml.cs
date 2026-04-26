// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.utils;
using Clever_Vpn.ViewModel;
using Clever_Vpn_Windows_Kit.Common;
using Clever_Vpn_Windows_Kit.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Clever_Vpn.Pages.HomePage.components;

public sealed partial class HomeCardLineSelector : UserControl
{
    VpnViewModel Vm { get; } = ((App)Application.Current).ViewModel;
    private bool IsRefreshing => Vm.VpnApiState == ApiState.Running;

    public HomeCardLineSelector()
    {
        InitializeComponent();
        Vm.PropertyChanged += OnVmPropertyChanged;
        UpdateRefreshButtonVisuals();
    }

    private void OnVmPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VpnViewModel.VpnApiState))
        {
            RunOnMainUiThread(UpdateRefreshButtonVisuals);
        }
    }

    private void RunOnMainUiThread(Action action)
    {
        var app = Application.Current as App;
        var dispatcherQueue = app?.MainWindow?.DispatcherQueue;

        if (dispatcherQueue == null)
        {
            action();
            return;
        }

        if (dispatcherQueue.HasThreadAccess)
        {
            action();
            return;
        }

        dispatcherQueue.TryEnqueue(() => action());
    }

    private void UpdateRefreshButtonVisuals()
    {
        var spinnerVisibility = IsRefreshing ? Visibility.Visible : Visibility.Collapsed;
        var textVisibility = IsRefreshing ? Visibility.Collapsed : Visibility.Visible;

        if (RefreshButton != null)
        {
            RefreshButton.IsEnabled = !IsRefreshing;
        }

        if (RefreshButtonText != null)
        {
            RefreshButtonText.Visibility = textVisibility;
        }

        if (RefreshButtonSpinner != null)
        {
            RefreshButtonSpinner.Visibility = spinnerVisibility;
        }
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

    private void OnLineSelectorFlyoutOpening(object sender, object e)
    {
        if (XamlRoot != null)
        {
            const double horizontalPadding = 24;
            const double preferredWidth = 420;
            const double minWidth = 280;

            var maxWidth = Math.Max(minWidth, XamlRoot.Size.Width - horizontalPadding);
            LineSelectorPanel.MaxWidth = maxWidth;
            LineSelectorPanel.Width = Math.Min(preferredWidth, maxWidth);
        }

        UpdateRefreshButtonVisuals();
        RefreshLineList();
    }

    private async void OnLineItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not Line line) return;
        await Vm.UpdateLine(line.Id);
        LineSelectorFlyout.Hide();
    }

    private async void OnRefreshLinesClick(object sender, RoutedEventArgs e)
    {
        await Vm.RefreshLines();
        RefreshLineList();
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

    public static ImageSource GetLineIcon(Line? line)
    {
        if (line == null)
            return GetLineIconPath(null, "service");
        return GetLineIconPath(line.Icon, line.IconKind);
    }

    public static ImageSource GetLineIconPath(string? icon, string? iconKind)
    {
        var relativePath = LineIconResolver.GetSvgRelativePath(icon, iconKind);
        var msAppxUri = new Uri($"ms-appx:///{relativePath}");

        //if (Utils.IsPackaged())
        //{
        //    return new SvgImageSource(msAppxUri);
        //}

        //var baseDirPath = AppContext.BaseDirectory.Replace('/', Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar);
        //var normalizedRelativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);
        //var filePath = Path.Combine(baseDirPath, normalizedRelativePath);
        //if (File.Exists(filePath))
        //{
        //    return new SvgImageSource(new Uri(filePath, UriKind.Absolute));
        //}

        return new SvgImageSource(msAppxUri);
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
