// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.utils;
using Clever_Vpn.ViewModel;
using Clever_Vpn_Windows_Kit.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.UI;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.ApplicationModel.Resources;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Clever_Vpn.Pages.LogPage;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LogPage : Page
{
    VpnViewModel Vm { get; } = ((App)Application.Current).ViewModel;
    private static readonly Brush LevelBrush = new SolidColorBrush(Colors.Gray);
    private static readonly Regex AnsiColorRegex = new("\u001B\\[(?<codes>[0-9;]*)m", RegexOptions.Compiled);
    private static readonly ResourceLoader ResourceLoader = ResourceLoader.GetForViewIndependentUse();

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
        var sb = new StringBuilder();
        foreach (var item in Vm.Logs)
            sb.AppendLine($"[{FormatLogLevel(item.Level)}] {item.Message}");

        // 2. 弹出保存对话框
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            FileTypeChoices = { { GetResource("LogSaveFileTypeText", "Text File"), new[] { ".txt" } } },
            SuggestedFileName = GetResource("LogSaveFileName", "Logs")
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
       
        InfoBar.Message = string.Format(GetResource("LogSaveSuccessMessageFormat", "Log file {0} save successfully!"), file.Path);

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
    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        await Vm.SetLogSubscriptionEnabled(true);
    }

    private async void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        await Vm.SetLogSubscriptionEnabled(false);
    }

    public static string FormatLogLevel(Level level)
    {
        return level switch
        {
            Level.LevelPanic => "PANIC",
            Level.LevelFatal => "FATAL",
            Level.LevelError => "ERROR",
            Level.LevelWarn => "WARN",
            Level.LevelInfo => "INFO",
            Level.LevelDebug => "DEBUG",
            Level.LevelTrace => "TRACE",
            _ => level.ToString(),
        };
    }

    private void OnLogContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (args.ItemContainer?.ContentTemplateRoot is not RichTextBlock richTextBlock)
        {
            return;
        }

        if (args.InRecycleQueue)
        {
            richTextBlock.Blocks.Clear();
            return;
        }

        if (args.Item is LogEntry entry)
        {
            RenderLogEntry(richTextBlock, entry);
        }
    }

    private static void RenderLogEntry(RichTextBlock richTextBlock, LogEntry entry)
    {
        richTextBlock.Blocks.Clear();
        var paragraph = new Paragraph();

        paragraph.Inlines.Add(new Run { Text = "[", FontSize = 12, Foreground = LevelBrush });
        paragraph.Inlines.Add(new Run { Text = FormatLogLevel(entry.Level), FontSize = 12, Foreground = LevelBrush });
        paragraph.Inlines.Add(new Run { Text = "] ", FontSize = 12, Foreground = LevelBrush });

        AppendAnsiMessage(paragraph, entry.Message);

        richTextBlock.Blocks.Add(paragraph);
    }

    private static void AppendAnsiMessage(Paragraph paragraph, string? message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        Brush? currentBrush = null;
        var currentIndex = 0;

        foreach (Match match in AnsiColorRegex.Matches(message))
        {
            if (match.Index > currentIndex)
            {
                AddRun(paragraph, message.Substring(currentIndex, match.Index - currentIndex), currentBrush);
            }

            var codeText = match.Groups["codes"].Value;
            ApplyAnsiCodes(codeText, ref currentBrush);
            currentIndex = match.Index + match.Length;
        }

        if (currentIndex < message.Length)
        {
            AddRun(paragraph, message[currentIndex..], currentBrush);
        }
    }

    private static void ApplyAnsiCodes(string codeText, ref Brush? currentBrush)
    {
        if (string.IsNullOrEmpty(codeText))
        {
            currentBrush = null;
            return;
        }

        foreach (var item in codeText.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            if (!int.TryParse(item, out var code))
            {
                continue;
            }

            if (code == 0 || code == 39)
            {
                currentBrush = null;
                continue;
            }

            var color = code switch
            {
                30 => Colors.Black,
                31 => Colors.Firebrick,
                32 => Colors.SeaGreen,
                33 => Colors.DarkGoldenrod,
                34 => Colors.RoyalBlue,
                35 => Colors.MediumVioletRed,
                36 => Colors.DarkCyan,
                37 => Colors.Gray,
                90 => Colors.DimGray,
                91 => Colors.IndianRed,
                92 => Colors.ForestGreen,
                93 => Colors.Goldenrod,
                94 => Colors.SteelBlue,
                95 => Colors.MediumOrchid,
                96 => Colors.Teal,
                97 => Colors.Black,
                _ => default,
            };

            if (color != default)
            {
                currentBrush = new SolidColorBrush(color);
            }
        }
    }

    private static void AddRun(Paragraph paragraph, string text, Brush? brush)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var run = new Run
        {
            Text = text,
            FontSize = 12,
        };

        if (brush != null)
        {
            run.Foreground = brush;
        }

        paragraph.Inlines.Add(run);
    }

    private static string GetResource(string key, string fallback)
    {
        try
        {
            var value = ResourceLoader.GetString(key);
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }
        catch
        {
            return fallback;
        }
    }

}
