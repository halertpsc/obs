using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication6.Providers;
using WebApplication6.Service;

namespace WebApplication6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        private readonly IPictureProvider _pictureProvider;
        private readonly INotificationService _notificationService;
        private readonly KeyStorage _keyStorage;

        public StreamController(IPictureProvider pictureProvider, KeyStorage keyStorage, INotificationService notificationService)
        {
            _pictureProvider = pictureProvider;
            _keyStorage = keyStorage;
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult Get(string k)
        {
            var sourceIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (k is null)
            {
                _notificationService.Notify("Attempt to access web interface", $"attempt to access without key. ip {sourceIp}", null);
                return Ok();
            }
            if (k.Equals(_keyStorage.Key))
            {
                return File(_pictureProvider.GetPngPicture(), "image/png");
            }
            _notificationService.Notify("Attempt to access web interface", $"attempt to access with wrong key. ip {sourceIp}, key {k}", null);
            return Ok();
        }
    }
}
