using System.Threading.Tasks;

namespace WebApplication6.Providers
{
    public interface IOwnerDetector
    {
        Task<bool> IsOwnerHere();
    }
}