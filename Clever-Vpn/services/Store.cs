using Clever_Vpn.services;
using System;
using System.IO;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;


namespace Clever_Vpn.services;

//public interface IFileStorable<T>
//    where T : IFileStorable<T>
//{
//    static abstract string FileName { get; }
//}

public interface IFileStorable
{
    static abstract string FileName { get; }
}

public class Store
{
    private static readonly string _folder =
    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                 config.AppConfig.AppName);

    public static async Task SaveAsync<T>(T data) where T : IFileStorable

    {
        var filePath = Path.Combine(_folder, T.FileName);
        var info = JsonContext.Default.GetTypeInfo(typeof(T));

        if (info != null)
        {
            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);

            using var stream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(stream, data, info);

            //var json = JsonSerializer.Serialize(data, info);
            //Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            //await File.WriteAllTextAsync(filePath, json);
        }

    }

    public static async Task<T?> LoadAsync<T>() where T : IFileStorable

    {
        var filePath = Path.Combine(_folder, T.FileName);
        var info = JsonContext.Default.GetTypeInfo(typeof(T));

        if (info != null && File.Exists(filePath))
        {
            using var stream = File.OpenRead(filePath);
            return (T?) await JsonSerializer.DeserializeAsync(stream, info);
        }else
        {
            return default;
        }
      

        //if (!File.Exists(_filePath)) return default;
        //string json = await File.ReadAllTextAsync(_filePath);
        //return JsonSerializer.Deserialize(json, MyJsonContext.Default.GetTypeInfo<T>());
    }
}