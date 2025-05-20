using Clever_Vpn.utils;
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

namespace Clever_Vpn.ViewModel;

public enum ActivationState
{
    DeActivated,
    Activated,
    LocalCheck,
    Activating,
}

public enum VpnState
{
    Down,
    Up,
    Connecting,
    Reconnecting,
    Disconnecting,
}


public partial class VpnViewModel : ObservableObject
{
    [ObservableProperty]
    public partial UserInfo? UserInfo { get; set; }

    [ObservableProperty]
    public partial ActivationState ActivateState { get; set; } = ActivationState.LocalCheck;

    [ObservableProperty]
    public partial VpnState VpnState { get; set; } = VpnState.Down;

    [ObservableProperty]
    public partial long StartTime { get; set; } = 0;

    [ObservableProperty]
    public partial LastVpnError? LastError { get; set; }

    public ObservableCollection<LogItem> LogItems { get; } = [];

    public List<Location> Locations { get; set; } = new List<Location>()
    {
        new Location(1, "US", "United States"),
        new Location(2, "GB", "United Kingdom"),
        new Location(3, "JP", "Japan"),
        new Location(4, "DE", "Germany"),
        new Location(5, "FR", "France"),
        new Location(6, "AU", "Australia"),
    };

    bool x = true;

    [RelayCommand(CanExecute = nameof(CanActivate))]
    private async Task Activate()
    {


        ActivateState = ActivationState.Activating;
        await Task.Delay(1000);

        x = !x;
        if (x)
        {
            LastError = new LastVpnError.NoVpnProvider();
        }
        else
        {
            LastError = new LastVpnError.NoCustomer();
        }

        ActivateState = ActivationState.Activated;
    }

    [ObservableProperty]
    public partial bool CanActivate { get; set; } = true;

    [RelayCommand()]
    private async Task DeActivate()
    {
       await Task.Delay(100);
       ActivateState = ActivationState.DeActivated;
    }


    [RelayCommand()]
    private void ClearLastError()
    {
        LastError = null;
    }

    public async Task ToggleVpn(bool isOn)
    {
        if (isOn)
        {
            if (VpnState == VpnState.Down)
            {
                VpnState = VpnState.Connecting;
                await Task.Delay(1000);
                StartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                VpnState = VpnState.Up;
            }

        }
        else
        {
            if (VpnState == VpnState.Up)
            {
                VpnState = VpnState.Disconnecting;
                await Task.Delay(1000);
                VpnState = VpnState.Down;
            }
        }
    }

    public int _trafficCount = 0;
    public async Task<Traffic> GetTraffic()
    {
        await Task.Delay(100);

        _trafficCount++;
        return new Traffic(1024 + _trafficCount * 100000, 2048 + _trafficCount * 90000);
    }

    private CancellationTokenSource? logTaskCts;
    private int _testCount = 0;
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
                     var items = await Task<List<LogItem>>.Run(async () =>
                        {
                             await Task.Delay(1000);
                            _testCount++;

                            // Simulate fetching logs from a server or database 
                            return new List<LogItem>()
                             {
                                new( DateTime.Now, $"Log abcd abcd abcd abcd abcd abcd abcd abcd abcd abcd abcd abcd abcd abcd abcd abcd {_testCount}") ,

                             };
                         }, token);

                     ((App)Application.Current).MainWindow.DispatcherQueue.TryEnqueue(() =>
                     {
                         foreach (var item in items)
                         {
                             LogItems.Add(item);
                         }

                     });
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
            await Task.Delay(100);
        });

        if (UserInfo != null)
        {
            UserInfo = UserInfo with { ProtocolType = type };
        }
    }

    public async Task UpdateLocations(bool api = false)
    {
        await Task.Run(async () =>
          {
              await Task.Delay(1000);
              Locations.Add(new Location(Locations.Last().Id + 1, "CN", "China"));
          });

    }
   
    public async Task Init()
    {
        UserInfo = new UserInfo("1234567890123456", "1234567890", 3, "https://www.clever-vpn.net", ProtocolType.AUTO);
        StartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        ActivateState = ActivationState.Activated;
        await Task.Delay(1000);
        //VpnState = VpnState.Connecting;
        //await Task.Delay(1000);
        //VpnState = VpnState.Up;
        //await Task.Delay(1000);
        //VpnState = VpnState.Reconnecting;
        //await Task.Delay(1000);
        //VpnState = VpnState.Down;
    }
}
