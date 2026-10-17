using Clever_Vpn.ViewModel;
using Clever_Vpn_Windows_Kit.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace Clever_Vpn.Pages.SettingsPage;

public sealed partial class SplitSettingsPage : Page
{
    private readonly VpnViewModel _vm = ((App)Application.Current).ViewModel;
    private static readonly ResourceLoader ResourceLoader = ResourceLoader.GetForViewIndependentUse();
    private bool _isSaving;
    private bool _regionListVisible;
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

        // region 分流**只能由用户显式开启**（空配置 ⇒ 三个开关都是关）。
        // 地区代码**永远不为空**：空/非法一律落到缺省（cn）。
        RegionEnableSwitch.IsOn = split.Region?.Enable == true;
        _regionCode = SplitRegionOptions.Normalize(split.Region?.Code) ?? SplitRegionOptions.DefaultCode;
        UpdateRegionCodeDisplay();
        HideRegionList();
        UpdateRegionDetailVisibility();

        IpEnableSwitch.IsOn = split.Ip?.Enable == true;
        IpEditor.Text = string.Join(Environment.NewLine, split.Ip?.IpCidr ?? []);

        DomainEnableSwitch.IsOn = split.Domain?.Enable == true;
        var basicDomainLines = new List<string>();
        basicDomainLines.AddRange(split.Domain?.Domain ?? []);
        basicDomainLines.AddRange((split.Domain?.DomainSuffix ?? []).Select(s => s.StartsWith('.') ? s : $".{s}"));
        BasicDomainEditor.Text = string.Join(Environment.NewLine, basicDomainLines);

        AdvancedDomainEditor.Text = string.Join(Environment.NewLine, split.Domain?.DomainRegex ?? []);
    }

    private void UpdateRegionCodeDisplay()
    {
        // Windows 不渲染国旗 emoji（Segoe UI Emoji 没有国旗字形，会退化成「US」两个
        // 字母），所以这里用「英文名 (代码)」，避免出现「US us」这种重复。
        var option = SplitRegionOptions.Option(_regionCode);
        RegionCodeTextBlock.Text = $"{option.Name} ({option.Code})";
    }

    private void UpdateRegionDetailVisibility()
    {
        // 选择控件只在开启时才显示（关闭时"选地区"没有意义；值仍保留在 userInfo 里）。
        RegionDetailPanel.Visibility = RegionEnableSwitch.IsOn ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---- 地区子列表页（分组 + 搜索 + 点选即返回），与 iOS/Android 同形态 ----

    private void OnRegionCodeButtonClick(object sender, RoutedEventArgs e)
    {
        ShowRegionList();
    }

    private void ShowRegionList()
    {
        _regionListVisible = true;
        RegionSearchBox.Text = string.Empty;
        RebuildRegionList();
        RegionListPanel.Visibility = Visibility.Visible;
        SplitFormPanel.Visibility = Visibility.Collapsed;
    }

    private void HideRegionList()
    {
        _regionListVisible = false;
        RegionListPanel.Visibility = Visibility.Collapsed;
        SplitFormPanel.Visibility = Visibility.Visible;
    }

    private void RebuildRegionList()
    {
        var keyword = (RegionSearchBox.Text ?? string.Empty).Trim();
        var groups = new List<RegionGroup>();

        foreach (var section in SplitRegionOptions.Sections)
        {
            var items = section.Options
                .Where(option => RegionMatches(option, keyword))
                .Select(option => new RegionItem(option, option.Code == _regionCode))
                .ToList();

            if (items.Count > 0)
            {
                groups.Add(new RegionGroup(GroupTitle(section.Group), items));
            }
        }

        // 当前值不在内置列表里（自定义代码）时额外补一段，保证列表里总能勾出当前值。
        var current = SplitRegionOptions.Option(_regionCode);
        if (!SplitRegionOptions.All.Any(option => option.Code == current.Code)
            && RegionMatches(current, keyword))
        {
            groups.Insert(0, new RegionGroup(GroupTitle(SplitRegionGroup.Custom),
                new List<RegionItem> { new(current, true) }));
        }

        // 分组数据必须交给 CollectionViewSource 的 View 才能用 GroupStyle 表头；
        // 之前只设了 CVS.Source，而 ListView 的 ItemsSource 从未赋值，所以列表是空的。
        var view = new CollectionViewSource { IsSourceGrouped = true, Source = groups };
        RegionListView.ItemsSource = view.View;
    }

    /// <summary>搜索按「地区代码 + 英文名」匹配（列表里只显示「🇯🇵  jp」，不塞英文名）。</summary>
    private static bool RegionMatches(SplitRegionOption option, string keyword)
    {
        if (keyword.Length == 0)
        {
            return true;
        }

        return option.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            || option.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

    private void OnRegionSearchChanged(object sender, TextChangedEventArgs e)
    {
        RebuildRegionList();
    }

    private void OnRegionItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not RegionItem item)
        {
            return;
        }

        _regionCode = item.Code;
        UpdateRegionCodeDisplay();
        HideRegionList();
    }

    /// <summary>分组标题（大洲）走文案表；缺失则回退英文。</summary>
    private static string GroupTitle(SplitRegionGroup group)
    {
        var (key, fallback) = group switch
        {
            SplitRegionGroup.Asia => ("SplitRegionGroupAsia", "Asia"),
            SplitRegionGroup.Americas => ("SplitRegionGroupAmericas", "Americas"),
            SplitRegionGroup.Europe => ("SplitRegionGroupEurope", "Europe"),
            SplitRegionGroup.Oceania => ("SplitRegionGroupOceania", "Oceania"),
            _ => ("SplitRegionGroupCustom", "Custom"),
        };

        try
        {
            var value = ResourceLoader.GetString(key + ".Text");
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }
        catch
        {
            return fallback;
        }
    }

    private void OnRegionEnableToggled(object sender, RoutedEventArgs e)
    {
        // 开启时地区必须有一个值：没有就落缺省（cn）；已有值保留用户的。
        if (RegionEnableSwitch.IsOn && SplitRegionOptions.Normalize(_regionCode) is null)
        {
            _regionCode = SplitRegionOptions.DefaultCode;
            UpdateRegionCodeDisplay();
        }

        UpdateRegionDetailVisibility();
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
                // 落盘时永远写一个合法值（空/非法 ⇒ 缺省 cn）；`enable=true` + 空 code
                // 会让引擎去装配名为 `geoip-` 的垃圾规则集（URL 404）。
                Code = SplitRegionOptions.Normalize(_regionCode) ?? SplitRegionOptions.DefaultCode,
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
        // 子列表页看的时候，返回 = 关列表（不离开本页）。
        if (_regionListVisible)
        {
            HideRegionList();
            return;
        }

        await SaveSplitAsync();
        ((App)Application.Current).MainWindow.GoBack();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        _ = SaveSplitAsync();
        base.OnNavigatedFrom(e);
    }
}

/// <summary>地区子列表分组（大洲）。必须是集合类型，才能用 ListView 的 IsSourceGrouped。</summary>
public sealed class RegionGroup : List<RegionItem>
{
    public string Title { get; }

    public RegionGroup(string title, IEnumerable<RegionItem> items) : base(items)
    {
        Title = title;
    }
}

/// <summary>地区子列表的一行。</summary>
public sealed class RegionItem
{
    public string Code { get; }
    public string Label { get; }
    public Visibility CheckVisibility { get; }

    public RegionItem(SplitRegionOption option, bool selected)
    {
        Code = option.Code;
        // option.Label 是「🇨🇳 cn」：Windows 没有国旗字形，会退化成「CN cn」。
        Label = $"{option.Name} ({option.Code})";
        CheckVisibility = selected ? Visibility.Visible : Visibility.Collapsed;
    }
}
