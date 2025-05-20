using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Clever_Vpn.services;


public class AppSettings
{
    public bool PolicyAccepted { get; set; } = false;

}


public static class SettingsService
{
    private static readonly string _folder =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                     config.AppConfig.AppName);

    //private static readonly string _folder =
    //Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
    //             config.AppConfig.AppName);

    private static readonly string _filePath = Path.Combine(_folder, config.AppConfig.SettingsFileName);

    private static AppSettings? _cache;

    /// <summary>
    /// 异步加载或创建默认设置
    /// </summary>
    public static async Task<AppSettings> LoadAsync()
    {
        if (_cache != null)
            return _cache;

        try
        {
            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);

            if (File.Exists(_filePath))
            {
                using var stream = File.OpenRead(_filePath);
                _cache = await JsonSerializer.DeserializeAsync<AppSettings>(stream, JsonContext.Default.AppSettings)
                         ?? new AppSettings();
            }
            else
            {
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
    /// 异步保存当前设置
    /// </summary>
    public static async Task SaveAsync(AppSettings? settings = null)
    {
        if (settings != null)
            _cache = settings;

        if (_cache == null)
            return;

        using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, _cache,  JsonContext.Default.AppSettings);
    }
}
