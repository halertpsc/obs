using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace WebApplication6.Providers
{
    public class OwnerDetector : IOwnerDetector
    {
        private readonly ObserverOptions _options;

        private readonly ILogger<OwnerDetector> _logger;

        public OwnerDetector(IOptions<ObserverOptions> options, ILogger<OwnerDetector> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<bool> IsOwnerHere()
        {
            var ping = new Ping();
            for (int i = 0; i < 15; i++)
            {
                var pingResult = await ping.SendPingAsync(_options.OwnerIp);
                if (pingResult.Status == IPStatus.Success)
                {
                    return true;
                };

                _logger.LogInformation("Ping result: {PingResult}", pingResult.Status);
            }
            return false;
        }
    }
}
