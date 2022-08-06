using System.Threading;
using System.Threading.Tasks;

namespace WebApplication6.Service
{
    public interface IObserverService
    {
        Task Observe(CancellationToken stoppingToken);
    }
}