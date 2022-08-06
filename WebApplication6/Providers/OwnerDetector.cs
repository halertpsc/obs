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
        private readonly ObserverOptions options;

        public OwnerDetector(IOptions<ObserverOptions> options)
        {
            this.options = options.Value;
        }

        public async Task<bool> IsOwnerHere()
        {
            var ping = new Ping();
            for (int i = 0; i < 5; i++)
            {
                var pingResult = await ping.SendPingAsync(options.OwnerIp);
                if (pingResult.Status == IPStatus.Success)
                {
                    return true;
                };
            }
            return false;
        }
    }
}
