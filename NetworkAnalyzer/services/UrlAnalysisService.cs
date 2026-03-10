using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using NetworkAnalyzer.Models;

namespace NetworkAnalyzer.Services
{
    public class UrlAnalysisService
    {
        private readonly NetworkService _networkService;

        public UrlAnalysisService()
        {
            _networkService = new NetworkService();
        }

        public UrlAnalysisResult AnalyzeUrl(string urlString)
        {
            var result = new UrlAnalysisResult
            {
                OriginalUrl = urlString,
                AnalysisTime = DateTime.Now
            };

            try
            {
                // Добавляем схему по умолчанию, если её нет
                if (!urlString.StartsWith("http://") && !urlString.StartsWith("https://"))
                {
                    urlString = "http://" + urlString;
                }

                Uri uri = new Uri(urlString);

                // Базовые компоненты URL
                result.Scheme = uri.Scheme;
                result.Host = uri.Host;
                result.Port = uri.Port;
                result.Path = string.IsNullOrEmpty(uri.AbsolutePath) ? "/" : uri.AbsolutePath;
                result.Query = uri.Query;
                result.Fragment = uri.Fragment;

                // Определение типа адреса
                try
                {
                    var addresses = Dns.GetHostAddresses(uri.Host);
                    result.DnsAddresses = addresses.Select(a => a.ToString()).ToArray();

                    if (addresses.Length > 0)
                    {
                        result.AddressType = _networkService.GetAddressType(addresses[0]);
                    }

                    // Ping проверка
                    using (var ping = new Ping())
                    {
                        var reply = ping.Send(uri.Host, 1000);
                        result.IsPingSuccessful = reply.Status == IPStatus.Success;
                        if (result.IsPingSuccessful)
                        {
                            result.PingTime = reply.RoundtripTime;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.AddressType = $"Ошибка определения: {ex.Message}";
                    result.IsPingSuccessful = false;
                }
            }
            catch (UriFormatException)
            {
                throw new ArgumentException("Некорректный формат URL");
            }

            return result;
        }
    }
}