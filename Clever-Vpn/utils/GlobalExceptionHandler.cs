// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clever_Vpn.utils
{
    public static class GlobalExceptionHandler
    {
        public static void Initialize()
        {
            // UI 线程异常
            Application.Current.UnhandledException += (s, e) =>
            {
                e.Handled = true;
                Log(e.Exception, "UI Thread");
            };

            // 后台线程异常
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    Log(ex, "AppDomain");
                }
            };

            // 未观察到的 Task 异常
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                e.SetObserved();
                Log(e.Exception, "TaskScheduler");
            };
        }

        private static readonly string CrashLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Clever-Vpn", "crash.log");
        private static void Log(Exception ex, string source)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(CrashLogPath)!);
            File.AppendAllText(CrashLogPath,
                $"[{DateTime.Now}] [{source}] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n");
        }

        public static void DebugLog(string msg)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(CrashLogPath)!);
            File.AppendAllText(CrashLogPath,
                $"[{DateTime.Now}] [Debug] {msg}\n");
        }
    }
}
