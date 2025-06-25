
using Microsoft.UI.Xaml.Controls;
using System;
using System.Xml.Linq;
using Windows.ApplicationModel;

namespace Clever_Vpn.utils;

//public record LastVpnError(string ErrorType, string Message)
//{
//    public record ApiError(string ErrorType, string Message): LastVpnError(ErrorType, Message);
//    public record ProviderFrozen() : LastVpnError("provider_frozen",
//        "vpn provider account is frozen!");
//    public record CustomerFrozen() : LastVpnError("customer_frozen",
//        "the key is frozen!");
//    public record  NoServer() : LastVpnError("no_server",
//        "no_server");
//    public record  NoCustomer() : LastVpnError("no_customer",
//        "cannot find valid activate key");
//    public record NoVpnProvider() : LastVpnError("no_vpn_provider",
//        "no_vpn_provider");
//    public record NoKey() : LastVpnError("no_key",
//     "no activate key");
//    public record  Invalid() : LastVpnError("invalid",
//        "licence invalid");
//    public record  ConnectError(Exception error) : LastVpnError("connect_error", $"connect error: {error.Message}");
//    public record  TunnelError(Exception error) : LastVpnError("tunnel_error", $"connect error: {error.Message}");

//    public string FormattedMessage => $"{ErrorType}: {Message}";
//}

//public record Location(int Id, string Code, string Label);

//public enum ProtocolType
//{
//    AUTO,
//    UDP,
//    KUDP,
//    TCP,
    
//}

//public record UserInfo(string Key, string AppId, int? LocationId, string? Url, ProtocolType ProtocolType);

//public record Traffic(long tx, long rx);

//public record LogItem(DateTime Stamp, string Message);