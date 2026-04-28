// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.config;
using Microsoft.Win32;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Clever_Vpn.utils
{
    internal static class AutoStartHelper
    {
        private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "CleverVpn";
        private static string AutoStartCommand => $"\"{Environment.ProcessPath!}\" {AppConfig.AutoStartUpFlag}";

        /// <summary>
        /// 启用/禁用开机自启
        /// </summary>
        public static void SetAutoStartUnpackaged(bool enable)
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true);
            if (key == null) return;

            if (enable)
            {
                key.SetValue(AppName, AutoStartCommand);
            }
            else
            {
                key.DeleteValue(AppName, throwOnMissingValue: false);
            }
        }

        public static bool IsAutoStartEnabledUnPackaged()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: false);
            if (key == null) return false;
            var val = key.GetValue(AppName) as string;
            if (string.IsNullOrEmpty(val)) return false;

            return IsCurrentExecutableRunCommand(val);
        }

        public static async Task<bool> IsAutoStartEnabledPackaged()
        {
            var task = await StartupTask.GetAsync("MyStartupTask");
            return task.State == StartupTaskState.Enabled;
        }

        public static async Task SetAutoStartPackaged(bool enable)
        {
            var task = await StartupTask.GetAsync("MyStartupTask");
            if (enable)
            {
                if (task.State == StartupTaskState.Disabled)
                    await task.RequestEnableAsync();
            }
            else
            {
                if (task.State == StartupTaskState.Enabled)
                    task.Disable();
            }
        }

        public static bool IsAutoStartup()
        {

            if (Utils.IsPackaged())
            {
                var activationArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
                return (activationArgs.Kind == ExtendedActivationKind.StartupTask);
            }
            else
            {
                string[] args = Environment.GetCommandLineArgs();
                return args.Length > 1 ? string.Equals(args[1], AppConfig.AutoStartUpFlag, StringComparison.OrdinalIgnoreCase) : false;
            }
        }

        private static bool IsCurrentExecutableRunCommand(string command)
        {
            var exePath = Environment.ProcessPath!;
            var trimmed = command.Trim();

            if (string.Equals(trimmed.Trim('"'), exePath, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var expectedPrefix = $"\"{exePath}\"";
            if (!trimmed.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var remainder = trimmed.Substring(expectedPrefix.Length).Trim();
            return string.Equals(remainder, AppConfig.AutoStartUpFlag, StringComparison.OrdinalIgnoreCase);
        }
    }
}
