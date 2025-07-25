// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clever_Vpn.config
{
    public static class AppConfig
    {
        public const double Height = 680;
        public const double Width = 380;
        public const string AppName = "CleverVpn";
        public const int OnOffIconSize = (int) (Height / 5.5);
        public const string AutoStartUpFlag = "auto-startup";
        public static string DataDir = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData), config.AppConfig.AppName);
    }
}
