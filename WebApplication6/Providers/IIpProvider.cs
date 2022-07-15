using System.Threading;
using System.Threading.Tasks;

namespace WebApplication6.Providers
{
    public interface IIpProvider
    {
        Task<string> GetMyIpAsync(CancellationToken cancellationToken);
    }
}