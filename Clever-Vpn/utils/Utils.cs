using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics;

namespace Clever_Vpn.utils; 

public static class Utils
{

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
    /// 将字节数转换成带单位的字符串，单位自动选择 B/KiB/MiB/GiB/TiB。
    /// </summary>
    /// <param name="bytes">原始字节数，非负。</param>
    /// <returns>格式化后的字符串，例如 "512 B", "1.23 KiB", "4 MiB"</returns>
    public static string PrettyBytes(long bytes)
    {
        if (bytes < 0)
            throw new ArgumentOutOfRangeException(nameof(bytes), "字节数必须为非负数");

        // 单位数组
        string[] units = { "B", "KiB", "MiB", "GiB", "TiB" };
        double size = bytes;
        int unitIndex = 0;

        // 不断除以 1024，直到不足 1024 或达到最后一个单位
        while (size >= 1024 && unitIndex < units.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        // 格式化：整数显示为无小数，小数显示最多两位
        string format = size % 1 == 0 ? "0" : "0.##";
        return $"{size.ToString(format)} {units[unitIndex]}";
    }

}

public static class AppVersionHelper
{
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

    private static bool IsPackaged()
    {
        try
        {
            _ = Windows.ApplicationModel.Package.Current;
            return true;
        }
        catch
        {
            return false;
        }
    }
}

