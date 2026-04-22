using Clever_Vpn.ViewModel;
using Clever_Vpn_Windows_Kit.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clever_Vpn.Pages.SettingsPage;

public sealed partial class SplitSettingsPage : Page
{
    private readonly VpnViewModel _vm = ((App)Application.Current).ViewModel;
    private bool _isSaving;
    private string _regionCode = string.Empty;

    public SplitSettingsPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        LoadFromUserInfo();
    }

    private void LoadFromUserInfo()
    {
        var split = _vm.UserInfo?.SplitInfo ?? new SplitInfo();
        var useDefaultToggles = IsSplitConfigurationEmpty(split);

        RegionEnableSwitch.IsOn = useDefaultToggles ? true : split.Region?.Enable == true;
        _regionCode = split.Region?.Code ?? string.Empty;
        RegionCodeValueTextBlock.Text = string.IsNullOrWhiteSpace(_regionCode) ? "-" : _regionCode;

        IpEnableSwitch.IsOn = useDefaultToggles ? false : split.Ip?.Enable == true;
        IpEditor.Text = string.Join(Environment.NewLine, split.Ip?.IpCidr ?? []);

        DomainEnableSwitch.IsOn = useDefaultToggles ? false : split.Domain?.Enable == true;
        var basicDomainLines = new List<string>();
        basicDomainLines.AddRange(split.Domain?.Domain ?? []);
        basicDomainLines.AddRange((split.Domain?.DomainSuffix ?? []).Select(s => s.StartsWith('.') ? s : $".{s}"));
        BasicDomainEditor.Text = string.Join(Environment.NewLine, basicDomainLines);

        AdvancedDomainEditor.Text = string.Join(Environment.NewLine, split.Domain?.DomainRegex ?? []);
    }

    private static bool IsSplitConfigurationEmpty(SplitInfo? split)
    {
        if (split == null)
        {
            return true;
        }

        var hasRegionCode = !string.IsNullOrWhiteSpace(split.Region?.Code);
        var hasIpRules = split.Ip?.IpCidr?.Count > 0;
        var hasDomainRules = (split.Domain?.Domain?.Count > 0)
            || (split.Domain?.DomainSuffix?.Count > 0)
            || (split.Domain?.DomainRegex?.Count > 0);

        var hasAnyEnabled = (split.Region?.Enable == true)
            || (split.Ip?.Enable == true)
            || (split.Domain?.Enable == true);

        return !hasRegionCode && !hasIpRules && !hasDomainRules && !hasAnyEnabled;
    }

    private async Task SaveSplitAsync()
    {
        if (_isSaving)
        {
            return;
        }

        _isSaving = true;
        try
        {
            var split = BuildSplitInfo();
            await _vm.UpdateSplit(split);
        }
        finally
        {
            _isSaving = false;
        }
    }

    private SplitInfo BuildSplitInfo()
    {
        var ipCidr = ParseMultiline(IpEditor.Text);

        var basic = ParseMultiline(BasicDomainEditor.Text);

        var domains = new List<string>();
        var suffixes = new List<string>();
        foreach (var item in basic)
        {
            if (item.StartsWith('.'))
            {
                var suffix = item.TrimStart('.');
                if (!string.IsNullOrWhiteSpace(suffix))
                {
                    suffixes.Add(suffix);
                }
            }
            else
            {
                domains.Add(item);
            }
        }

        var regex = ParseMultiline(AdvancedDomainEditor.Text);

        return new SplitInfo
        {
            Region = new RegionInfo
            {
                Enable = RegionEnableSwitch.IsOn,
                Code = _regionCode,
            },
            Ip = new IpRule
            {
                Enable = IpEnableSwitch.IsOn,
                IpCidr = ipCidr,
            },
            Domain = new DomainRule
            {
                Enable = DomainEnableSwitch.IsOn,
                Domain = domains,
                DomainSuffix = suffixes,
                DomainRegex = regex,
            },
        };
    }

    private static List<string> ParseMultiline(string? text)
    {
        return (text ?? string.Empty)
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }

    private async void OnBackClick(object sender, RoutedEventArgs e)
    {
        await SaveSplitAsync();
        ((App)Application.Current).MainWindow.GoBack();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        _ = SaveSplitAsync();
        base.OnNavigatedFrom(e);
    }
}
