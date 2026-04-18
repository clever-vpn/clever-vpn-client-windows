// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.IO;
using Clever_Vpn.utils;

namespace Clever_Vpn.config
{
    public static class AppConfig
    {
        public const double Height = 680;
        public const double Width = 380;
        public const string AppName = "CleverVpn";
        public const int OnOffIconSize = (int) (Height / 5.5);
        public const string AutoStartUpFlag = "auto-startup";

        // Single source of truth for both app local data and kit initialization path.
        public static string DataDir => GetClientDataPath();

        private static string GetClientDataPath()
        {
            if (Utils.IsPackaged())
            {
                return Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "CleverVpnData");
            }

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CleverVpnData");
        }
    }
}
