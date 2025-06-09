// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using DeviceRadar.Utils;

class Program
{
    static async Task Main()
    {
        List<IPAddress> activeIPs = await IpUtils.GetActiveIPs();
        Console.WriteLine("");

        foreach (var ip in activeIPs)
        {
            Console.WriteLine($"Responded to Ping Sweep: {ip.ToString()}");
        }
    }
}