using System;
using System.Net.NetworkInformation;

namespace NetworkAnalyzer.Models
{
    public class NetworkInterfaceInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }
        public string SubnetMask { get; set; }
        public string MacAddress { get; set; }
        public OperationalStatus Status { get; set; }
        public long Speed { get; set; }
        public NetworkInterfaceType InterfaceType { get; set; }
        public string SpeedFormatted => Speed > 0 ? $"{Speed / 1000000} Мбит/с" : "Недоступно";
        public string StatusColor => Status == OperationalStatus.Up ? "Green" : "Red";
    }
}