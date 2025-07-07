// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics;

namespace Clever_Vpn.utils; 

public static class Utils
{

    public static bool IsPackaged()
    {
        try
        {
            //only in packaged app, Windows.ApplicationModel.Package.Current.Id will not be null
            return Windows.ApplicationModel.Package.Current.Id != null;
        }
        catch
        {
            // Unpackaged 
            return false;
        }
    }

    public static string GetAppVersion()
    {
        if (IsPackaged())
        {
            var version = Windows.ApplicationModel.Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
        else
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return version?.ToString() ?? "Unknown";
        }
    }

    public static void ReSizeWindow(Microsoft.UI.Xaml.Window win, double width, double height)
    {
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(win);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

        var dpi = NativeMethods.GetDpiForWindow(hwnd);
        var scale = dpi / 96.0f; // 96 is the default DPI for 100% scaling
        appWindow.Resize(new SizeInt32((int)(width * scale), (int)(height * scale)));

    }


    /// <summary>
    /// change traffic bytes to a human-readable format.
    /// </summary>
    /// <param name="bytes">raw traffice bytes。</param>
    /// <returns>return formated string</returns>
    public static string PrettyBytes(long bytes)
    {
        if (bytes < 0)
            throw new ArgumentOutOfRangeException(nameof(bytes), "字节数必须为非负数");

        // Unit
        string[] units = { "B", "KiB", "MiB", "GiB", "TiB" };
        double size = bytes;
        int unitIndex = 0;

        // divide by 1024 until size is less than 1024 or we reach the last unit
        while (size >= 1024 && unitIndex < units.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        // format the size with one decimal place if it's not an integer
        string format = size % 1 == 0 ? "0" : "0.##";
        return $"{size.ToString(format)} {units[unitIndex]}";
    }

}
