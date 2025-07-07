// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.utils;
using Clever_Vpn_Windows_Kit;
using Clever_Vpn_Windows_Kit.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Media.Protection.PlayReady;

namespace Clever_Vpn.ViewModel;

public enum ActivationState
{
    DeActivated,
    Activated,
    LocalCheck,
    Activating,
}

//public enum VpnState
//{
//    Down,
//    Up,
//    Connecting,
//    Reconnecting,
//    Disconnecting,
//}


public partial class VpnViewModel : ObservableObject
{
    private Client _client = Client.Init(Utils.GetAppVersion());
    [ObservableProperty]
    public partial string ActivationKey { get; set; } = string.Empty;

    [ObservableProperty]
    public partial UserInfo? UserInfo { get; set; }

    [ObservableProperty]
    public partial ActivationState ActivateState { get; set; } = ActivationState.LocalCheck;

    [ObservableProperty]
    public partial CleverVpnState VpnState { get; set; } = CleverVpnState.Down;

    [ObservableProperty]
    public partial long StartTime { get; set; } = 0;

    [ObservableProperty]
    public partial CleverVpnError? LastError { get; set; }

    public ObservableCollection<string> LogItems { get; } = [];

    public List<Location> Locations { get; set; } = new List<Location>();

    [RelayCommand(CanExecute = nameof(CanActivate))]
    private async Task Activate()
    {
        ActivateState = ActivationState.Activating;
        await _client.Activate(ActivationKey);
        UserInfo = await _client.GetUserInfo();
        ActivateState = (UserInfo == null) ? ActivationState.DeActivated : ActivationState.Activated;
    }

    [ObservableProperty]
    public partial bool CanActivate { get; set; } = true;

    [RelayCommand()]
    private async Task DeActivate()
    {
        await _client.Deactivate();
        UserInfo = await _client.GetUserInfo();
        ActivateState = (UserInfo == null) ? ActivationState.DeActivated : ActivationState.Activated;
    }


    [RelayCommand()]
    private void ClearLastError()
    {
        LastError = null;
    }

    public async Task ToggleVpn(bool isOn)
    {
       var setting = await services.SettingsService.LoadAsync();
        if (isOn)
        {
            if (VpnState == CleverVpnState.Down)
            {
                await _client.Start();
                StartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                setting.VpnIsOn = true;
            }

        }
        else
        {
            if (VpnState == CleverVpnState.Up)
            {
                await _client.Stop();
                setting.VpnIsOn = false;
            }
        }
    }

    public async Task<Traffic> GetTraffic()
    {
        return await Task.Run(_client.GetTraffic);
    }

    private CancellationTokenSource? logTaskCts;
    public async void StartFetchLogs()
    {
        logTaskCts = new();
        var token = logTaskCts.Token;
        try
        {
            await Task.Run(async () =>
             {
                 while (true)
                 {
                     token.ThrowIfCancellationRequested();
                     var items = await Task.Run(async () => await _client.GetLogLines(), token);
                     runUI(() =>
                     {
                         foreach (var item in items)
                         {
                             LogItems.Add(item);
                         }
                     });

                     await Task.Delay(1000);
                 }

             }, token);
        }
        catch (OperationCanceledException)
        {
        }
    }

    public void StopFetchLogs()
    {
        logTaskCts?.Cancel();
    }
   
    public async Task UpdateProtocolType(ProtocolType type)
    {
        await Task.Run(async () =>
        {
            await _client.UpdateProtocolType(type);
            var _userInfo = await _client.GetUserInfo();
            runUI(()=> { UserInfo = _userInfo; });
        });
    }

    public async Task UpdateLocation(int? id)
    {
        await Task.Run(async () =>
        {
            await _client.UpdateLocation(id);
            var _userInfo = await _client.GetUserInfo();
            runUI(() => { UserInfo = _userInfo; });
        });
    }
    public async Task UpdateLocations(bool api = false)
    {
        await Task.Run(async () =>
          {
              var items = await _client.GetLocations(api);
              runUI( () =>
              {
                  Locations = items;
              });
          });

    }
   
    
    public async Task Init()
    {
        _client.VpnStateChanged += (s, state) =>
        {
            runUI(() =>
            {
                        VpnState = state;
            });
        };
        _client.LastErrorChanged += (s, error) =>
        {
            if (error != null)
            {
                runUI(() =>
                {
                    LastError = error;
                });
            }
        };

        UserInfo = await _client.GetUserInfo();
        ActivateState = (UserInfo == null) ? ActivationState.DeActivated : ActivationState.Activated;
        Locations = await _client.GetLocations();
    }

    private void runUI(Action action)
    {
        // Fix for CS1503: Convert 'System.Action' to 'Microsoft.UI.Dispatching.DispatcherQueueHandler'
        ((App)Application.Current).MainWindow.DispatcherQueue.TryEnqueue(() => action());
    }
}
