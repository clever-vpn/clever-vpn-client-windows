using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Clever_Vpn.utils;

public static partial class NativeMethods
{
    // 基本声明：调用 user32.dll 的 GetDpiForWindow
    [LibraryImport("user32.dll")]
    internal static partial uint GetDpiForWindow(IntPtr hWnd);

}
