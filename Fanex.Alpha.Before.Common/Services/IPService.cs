namespace Fanex.Alpha.Before.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    public interface IPService
    {
        string GetClientIp();
    }

    public class IpServiceImpl : IPService
    {
        [ThreadStatic]
        private static bool IsLittleEndian = BitConverter.IsLittleEndian;

        public static bool IsIPV6(string clientIP)
        {
            IPAddress address;

            return IPAddress.TryParse(clientIP, out address) 
                && address.AddressFamily == AddressFamily.InterNetworkV6;
        }

        public static ulong ConvertIPV4ToNumber(string ipAddress)
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

        public static ulong ConvertIPV6ToNumber(string ipAddress)
        {
            var addrBytes = IPAddress.Parse(ipAddress).GetAddressBytes();
            if (IsLittleEndian)
            {
                var byteList = new List<byte>(addrBytes);
                byteList.Reverse();
                addrBytes = byteList.ToArray();
            }

            var ipNum = BitConverter.ToUInt64(addrBytes, 8);
            ipNum <<= 64;
            ipNum += BitConverter.ToUInt64(addrBytes, 0);

            return ipNum;
        }

        internal static void SetIsLittleEndian(bool isLittleEndian)
        {
            IsLittleEndian = isLittleEndian;
        }

        public string GetClientIp() => Helpers.IPHelper.ClientIP();
    }
}
