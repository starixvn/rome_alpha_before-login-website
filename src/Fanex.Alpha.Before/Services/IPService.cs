using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Fanex.Alpha.Before.Criteria;
using Fanex.Alpha.Before.Models;
using Fanex.Data.Repository;

[assembly: InternalsVisibleTo("Fanex.Alpha.Before.UnitTests")]
namespace Fanex.Alpha.Before.Services
{
    public interface IPService
    {
        IPInformationModel GetIPInformation(string ip);
    }

    public class IpServiceImpl : IPService
    {
        private readonly IDynamicRepository repository;

        [ThreadStatic]
        private static bool IsLittleEndian = BitConverter.IsLittleEndian;

        public IpServiceImpl(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public IPInformationModel GetIPInformation(string ip)
            => repository.Get<IPInformationModel>(new GetIPInformationCriteria(ip));

        public static bool IsIPV6(string clientIP)
        {
            return IPAddress.TryParse(clientIP, out IPAddress address)
                && address.AddressFamily == AddressFamily.InterNetworkV6;
        }

        public static ulong ConvertIPV4ToNumber(string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out IPAddress parsedIpAddress))
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
            ipNum += BitConverter.ToUInt64(addrBytes, 0);

            return ipNum;
        }

        internal static void SetIsLittleEndian(bool isLittleEndian)
        {
            IsLittleEndian = isLittleEndian;
        }
    }
}
