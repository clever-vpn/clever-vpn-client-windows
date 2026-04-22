// Copyright (c) 2025 CleverVPN Team
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Clever_Vpn.utils;
using Clever_Vpn_Windows_Kit.Client;
using Clever_Vpn_Windows_Kit.Common;
using Clever_Vpn_Windows_Kit.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Media.Protection.PlayReady;

namespace Clever_Vpn.ViewModel;

public enum ActivationState
{
    DeActivated,
    Activated,
    Waiting,
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
    private Client _client;
    public VpnViewModel()
    {
        _client = new Client();
        SubscribeClientEvents();
    }


    [ObservableProperty]
    public partial string ActivationKey { get; set; } = string.Empty;

    [ObservableProperty]
    public partial UserInfo? UserInfo { get; set; }

    [ObservableProperty]
    public partial ActivationState ActivateState { get; set; } = ActivationState.Waiting;

    [ObservableProperty]
    public partial ApiState VpnApiState { get; set; } = ApiState.Idle;

    [ObservableProperty]
    public partial CleverVpnState VpnState { get; set; } = CleverVpnState.Idle;

    [ObservableProperty]
    public partial long StartTime { get; set; } = 0;

    [ObservableProperty]
    public partial CleverVpnError? LastError { get; set; }

    [ObservableProperty]
    public partial Traffic? Traffic { get; set; }

    [ObservableProperty]
    public partial VpnConnectionInfo? ConnectionInfo { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<LogEntry> Logs { get; set; } = [];

    [ObservableProperty]
    public partial List<Line> Lines { get; set; } = [];

    [RelayCommand(CanExecute = nameof(CanActivate))]
    private async Task Activate()
    {
        await _client.Activate(ActivationKey);
    }

    [ObservableProperty]
    public partial bool CanActivate { get; set; } = true;

    [RelayCommand()]
    private async Task DeActivate()
    {
        await _client.Deactivate();
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
            if (VpnState == CleverVpnState.Idle)
            {
                await _client.Start();
                setting.VpnIsOn = true;
            }

        }
        else
        {
            if (VpnState == CleverVpnState.Started || VpnState == CleverVpnState.Starting)
            {
                await _client.Stop();
                setting.VpnIsOn = false;
            }
        }
    }

    public async Task UpdateProtocolType(ProtocolType type)
    {
            await _client.UpdateProtocolType(type);
    }

    public async Task UpdateLine(int? id)
    {
        await _client.UpdateLine(id);
    }

    public async Task UpdateSplit(SplitInfo splitInfo)
    {
        await _client.UpdateSplit(splitInfo);
    }

    public async Task RefreshLines()
    {
        await _client.RefreshLines();
    }

    public async Task SetTrafficSubscriptionEnabled(bool enabled)
    {
        await _client.SetTrafficSubscriptionEnabled(enabled);
    }

    public async Task SetLogSubscriptionEnabled(bool enabled)
    {
        await _client.SetLogSubscriptionEnabled(enabled);
    }


    private void SubscribeClientEvents()
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
            runUI(() =>
            {
                LastError = error;
            });
        };

        _client.TrafficChanged += (s, traffic) =>
        {
            runUI(() =>
            {
                Traffic = traffic;
            });
        };

        _client.ConnectionInfoChanged += (s, connectionInfo) =>
        {
            runUI(() =>
            {
                ConnectionInfo = connectionInfo;
            });
        };

        _client.ConnectionStartTimeChanged += (s, startTime) =>
        {
            runUI(() =>
            {
                StartTime = startTime;
            });
        };

        _client.LinesChanged += (s, lines) =>
        {
            runUI(() =>
            {
                Lines = lines;
            });
        };

        _client.ApiStateChanged += (s, state) =>
        {
            runUI(() =>
            {
                VpnApiState = state;
            });
        };

        _client.UserInfoChanged += (s, userInfo) =>
        {
            runUI(() =>
            {
                UserInfo = userInfo;
                UpdateActivateState();
            });
        };

        _client.LogsChanged += (s, logs) =>
        {
            runUI(() =>
            {
                SyncLogs(logs);
            });
        };
    }

    public async Task Init()
    {
        VpnApiState = _client.VpnApiState;
        VpnState = _client.VpnState;
        StartTime = _client.ConnectionStartTime;
        UserInfo = _client.UserInfo;
        Traffic = _client.Traffic;
        SyncLogs(_client.Logs);
        Lines = _client.Lines;
        UpdateActivateState();
    }

    private void SyncLogs(IReadOnlyList<LogEntry> logs)
    {
        if (logs == null || logs.Count == 0)
        {
            Logs.Clear();
            return;
        }

        // Fast path: when incoming snapshot preserves the current prefix, append only the tail.
        if (logs.Count >= Logs.Count)
        {
            var prefixMatches = true;
            for (var i = 0; i < Logs.Count; i++)
            {
                if (!Equals(Logs[i], logs[i]))
                {
                    prefixMatches = false;
                    break;
                }
            }

            if (prefixMatches)
            {
                for (var i = Logs.Count; i < logs.Count; i++)
                {
                    Logs.Add(logs[i]);
                }

                return;
            }
        }

        Logs.Clear();
        foreach (var log in logs)
        {
            Logs.Add(log);
        }
    }

    private void UpdateActivateState()
    {
        ActivateState = UserInfo?.Activated == true
            ? ActivationState.Activated
            : ActivationState.DeActivated;
    }

    private void runUI(Action action)
    {
        var app = Application.Current as App;
        var dispatcherQueue = app?.MainWindow?.DispatcherQueue;

        if (dispatcherQueue == null)
        {
            action();
            return;
        }

        if (dispatcherQueue.HasThreadAccess)
        {
            action();
            return;
        }

        dispatcherQueue.TryEnqueue(() => action());
    }
}
