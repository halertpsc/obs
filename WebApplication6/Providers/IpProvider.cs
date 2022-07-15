using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication6.Providers
{
    public class IpProvider : IIpProvider
    {
        private readonly ObserverOptions _options;
        private readonly HttpClient _httpClient;
        private readonly ILogger<IpProvider> _logger;

        public IpProvider(IOptions<ObserverOptions> options, HttpClient httpClient, ILogger<IpProvider> logger)
        {
            _options = options.Value;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.ipify.org/");
            _logger = logger;
        }

        public async Task<string> GetMyIpAsync(CancellationToken cancellationToken)
        {
            var res = await _httpClient.GetAsync("?format=json", cancellationToken);
            if (res.IsSuccessStatusCode)
            {
                _logger.LogInformation("succesfully got ip");
                return JsonConvert.DeserializeObject<IpModel>(await res.Content.ReadAsStringAsync()).Ip;
                
            }
            _logger.LogError("Ip detection service didn't respon succesfully");
            return null;
        }


        private class IpModel
        {
            public string Ip { get; set; }
        }

    }
}
