using System;

namespace NetworkAnalyzer.Models
{
    public class UrlAnalysisResult
    {
        public string OriginalUrl { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }
        public string Fragment { get; set; }
        public DateTime AnalysisTime { get; set; }
        public bool IsPingSuccessful { get; set; }
        public long PingTime { get; set; }
        public string AddressType { get; set; }
        public string[] DnsAddresses { get; set; }
    }
}