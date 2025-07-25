// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Clever_Vpn.services;


public class AppSettings : IFileStorable

{
    public static string FileName => "settings.json";

    /// <summary>
    /// Accepted the privacy policy or not.
    /// </summary>
    [JsonPropertyName("policy_accepted")]
    public bool PolicyAccepted { get; set; } = false;
    [JsonPropertyName("vpn_is_on")]
    public bool VpnIsOn { get; set; } = false;
}


public static class SettingsService
{
    private static AppSettings? _cache;

    /// <summary>
    /// Loads the application settings from the storage.
    /// </summary>
    public static async Task<AppSettings> LoadAsync()
    {
        if (_cache != null)
            return _cache;

        try
        {
            _cache = await Store.LoadAsync<AppSettings>();
            if (_cache == null){
                _cache = new AppSettings();
                await SaveAsync(_cache);
            }
        }
        catch
        {
            // 读取失败时返回默认
            _cache = new AppSettings();
        }

        return _cache;
    }

    /// <summary>
    /// Saves the current settings to the storage.
    /// </summary>
    public static async Task SaveAsync(AppSettings? settings = null)
    {
        if (settings != null)
            _cache = settings;

        if (_cache == null)
            return;

        await Store.SaveAsync(_cache);
    }

}
