using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NetworkAnalyzer.Models;

namespace NetworkAnalyzer.Services
{
    public class NetworkService
    {
        public List<NetworkInterfaceInfo> GetNetworkInterfaces()
        {
            var interfaces = new List<NetworkInterfaceInfo>();

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                var ipProperties = ni.GetIPProperties();
                var ipAddressInfo = ipProperties.UnicastAddresses
                    .FirstOrDefault(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork);

                if (ipAddressInfo != null)
                {
                    var interfaceInfo = new NetworkInterfaceInfo
                    {
                        Name = ni.Name,
                        Description = ni.Description,
                        IpAddress = ipAddressInfo.Address.ToString(),
                        SubnetMask = ipAddressInfo.IPv4Mask?.ToString() ?? "Н/Д",
                        MacAddress = BitConverter.ToString(ni.GetPhysicalAddress().GetAddressBytes()),
                        Status = ni.OperationalStatus,
                        Speed = ni.Speed,
                        InterfaceType = ni.NetworkInterfaceType
                    };

                    // Форматирование MAC-адреса
                    if (!string.IsNullOrEmpty(interfaceInfo.MacAddress))
                    {
                        interfaceInfo.MacAddress = interfaceInfo.MacAddress.Replace("-", ":");
                    }

                    interfaces.Add(interfaceInfo);
                }
            }

            return interfaces;
        }

        public string GetAddressType(IPAddress address)
        {
            if (IPAddress.IsLoopback(address))
                return "Loopback";
            
            byte[] bytes = address.GetAddressBytes();
            
            // Проверка на локальные адреса
            if (bytes[0] == 10) // 10.0.0.0/8
                return "Локальный (Class A)";
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) // 172.16.0.0/12
                return "Локальный (Class B)";
            if (bytes[0] == 192 && bytes[1] == 168) // 192.168.0.0/16
                return "Локальный (Class C)";
            
            return "Публичный";
        }
    }
}