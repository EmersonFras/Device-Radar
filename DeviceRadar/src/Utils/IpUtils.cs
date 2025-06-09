using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace DeviceRadar.Utils
{
    public static class IpUtils
    {
        /*
         * GetLocalIPv4 - Private
         * 
         * Returns the local IPv4 address of the machine.
         * 
         * @return string - The local IPv4 address as a string.
         */
        private static IPAddress GetLocalIPv4()
        {
            // Get the local IP address
            var AddressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;


            foreach (var host in AddressList)
            {
                /**
                 * Assures we only get IPv4 addresses and not loopback addresses
                 *      InterNetwork is for IPv4
                 *      IsLoopBack ensures we skip loopback addresses
                **/
                if (host.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(host))
                {
                    return host;
                }
            }
            throw new InvalidOperationException("No active IPv4 address found.");
        }

        /*
         * GetSubnetMask - Private
         * 
         * @param ipAddress - The IP address to get the subnet mask for.
         * @return IPAddress - The subnet mask.
         */
        private static IPAddress GetSubnetMask(IPAddress ipAddress)
        {
            string subMask = string.Empty;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork && ip.Address.ToString() == ipAddress.ToString())
                        {
                            return ip.IPv4Mask;
                        }
                    }
                }
            }

            throw new InvalidOperationException("No Subnet Mask was found.");
        }

        /*
         * IptoUInt32 - Private
         * 
         * Returns the IP address as a 32-bit unsigned integer.
         * 
         * @param ipAddress - The IP address to convert.
         * @return uint - The IP address as a 32-bit unsigned integer.
         */
        private static uint IptoUInt32(IPAddress ipAddress)
        {
            byte[] bytes = ipAddress.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes); // Network byte order is big-endian
            }
            return BitConverter.ToUInt32(bytes, 0);
        }

        /*
         * UInt32ToIp - Private
         * 
         * Returns the IP address as a string from a 32-bit unsigned integer.
         * 
         * @param ipAddress - The 32-bit unsigned integer to convert.
         * @return IPAddress - The IP address as an IPAddress object.
         */
        private static IPAddress UInt32ToIp(uint ipAddress)
        {
            byte[] bytes = BitConverter.GetBytes(ipAddress);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return new IPAddress(bytes);
        }

        /*
         * GetUsableIpRange - Public
         * 
         * Returns the usable IP range for the local network.
         * 
         * @return (IPAddress start, IPAddress end) - A tuple containing the start and end of the usable IP range.
         */
        public static (IPAddress start, IPAddress end) GetUsableIpRange()
        {
            IPAddress localIp = GetLocalIPv4();
            IPAddress subnetMask = GetSubnetMask(localIp);

            uint ipValue = IptoUInt32(localIp);
            uint maskValue = IptoUInt32(subnetMask);

            uint network = ipValue & maskValue;
            uint broadcast = network | ~maskValue;

            return (UInt32ToIp(network + 1), UInt32ToIp(broadcast - 1));
        }
    }
}
