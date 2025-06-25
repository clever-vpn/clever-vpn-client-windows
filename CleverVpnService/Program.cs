using Clever_Vpn_Windows_Kit;

namespace CleverVpnService;

public class Program
{
    public static async Task Main(string[] args)
    {
        //Console.WriteLine("test");

        await Service.Run();
    }
}