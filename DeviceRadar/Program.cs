// See https://aka.ms/new-console-template for more information
using System.Net.NetworkInformation;
using System.Net.Sockets;
using DeviceRadar.Utils;

class Program
{
    static void Main()
    {
        var (start, end) = IpUtils.GetUsableIpRange();
        Console.WriteLine($"Usable range: {start} - {end}");
    }
}