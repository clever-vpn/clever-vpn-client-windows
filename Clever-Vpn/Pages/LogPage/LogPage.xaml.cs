// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn.Pages.LogPage;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LogPage : Page
{
    VpnViewModel Vm { get; } = ((App)Application.Current).ViewModel;

    public LogPage()
    {
        InitializeComponent();
    }


    void OnBackClick(object sender, RoutedEventArgs e)
    {
        ((App)Application.Current).MainWindow.GoBack();
    }

    async void  OnSaveClick(object sender, RoutedEventArgs e)
    {
        //((App)Application.Current).MainWindow.GoBack();
        // 1. 准备文本内容
        var sb = new StringBuilder();
        foreach (var item in Vm.LogItems)
            sb.AppendLine($"{item}");

        // 2. 弹出保存对话框
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            FileTypeChoices = { { "文本文件", new[] { ".txt" } } },
            SuggestedFileName = "Logs"
        };

        var win = ((App)Application.Current).MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(win);

        // WinUI 3 Unpackaged 应用需要：
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

        StorageFile file = await picker.PickSaveFileAsync();
        if (file == null)
            return; // 用户取消

        // 3. 写入
        CachedFileManager.DeferUpdates(file);
        await FileIO.WriteTextAsync(file, sb.ToString());
        await CachedFileManager.CompleteUpdatesAsync(file);
        InfoBar.IsOpen = true;
       
        InfoBar.Message = $"Log file {file.Path} save successfully!";

     }

    private async void OnListViewLoaded(object sender, RoutedEventArgs e)
    {
        var listView = sender as ListView;
        await Task.Delay(200);
        if (listView != null)
        {
            var count = listView.Items.Count;
            if (count > 0)
            {
                var lastItem = listView.Items[count - 1];
                listView.ScrollIntoView(lastItem);
            }
        }
    }
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        Vm.StartFetchLogs();
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        Vm.StopFetchLogs();
    }

    public static string FormatDateTime(DateTime dateTime)
    {
        return dateTime.ToString("MM-dd HH:mm:ss");
    }

}
