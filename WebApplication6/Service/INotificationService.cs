using System.IO;
using System.Threading.Tasks;

namespace WebApplication6.Service
{
    public interface INotificationService
    {
        Task Notify(string subj, string message, Stream picture);
    }
}