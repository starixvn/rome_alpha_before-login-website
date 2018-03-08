namespace Fanex.Alpha.Before.Common.Services
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    public interface IPService
    {
        string GetClientIp();
    }

    public class IpServiceImpl : IPService
    {
        public static bool IsIPV6(string clientIP)
        {
            IPAddress address;

            return IPAddress.TryParse(clientIP, out address) && address.AddressFamily == AddressFamily.InterNetworkV6;
        }

        public static long ConvertIPToNumber(string ipAddress)
        {
            IPAddress parsedIpAddress;

            if (!IPAddress.TryParse(ipAddress, out parsedIpAddress))
            {
                return 0;
            }

            var addressBytes = parsedIpAddress.GetAddressBytes();
            Array.Reverse(addressBytes);

            return BitConverter.ToUInt32(addressBytes, 0);
        }

        public string GetClientIp() => Helpers.IPHelper.ClientIP();
    }
}
