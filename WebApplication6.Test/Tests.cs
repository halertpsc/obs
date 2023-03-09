using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using WebApplication6.Providers;
using WebApplication6.Service;

namespace WebApplication6.Test
{
    public class Tests
    {
        [Test, Timeout(5000)]
        public void Cancelled_SHouldBeCompletedRightAway()
        {
            var optionsMock = new Mock<IOptions<ObserverOptions>>();
            optionsMock.Setup(p => p.Value).Returns(() => new ObserverOptions() { ObserveTimeoutInMinutes = 5, OutsidePort = "100" });
            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(p => p.Notify(It.IsAny<string>(), It.IsAny<Stream>()));
            var ipProviderMock = new Mock<IIpProvider>();
            ipProviderMock.Setup(p => p.GetMyIpAsync(It.IsAny<CancellationToken>())).ReturnsAsync("127.0.0.2");
            var pictureProviderMock = new Mock<IPictureProvider>();
            pictureProviderMock.Setup(p => p.GetPngPicture()).Returns(new MemoryStream());
            var motionDetectionMock = new Mock<IMotionDetection>();
            motionDetectionMock.Setup(p => p.Detect()).Returns(false);

            var observerService = new ObserverService(
                optionsMock.Object,
                notificationServiceMock.Object,
                ipProviderMock.Object,
                pictureProviderMock.Object,
                new KeyStorage(),
                motionDetectionMock.Object,
                new NullLogger<ObserverService>());

            var cancellatinTokenSource = new CancellationTokenSource();

            var observerTask = observerService.Observe(cancellatinTokenSource.Token);
            Task.Delay(TimeSpan.FromSeconds(1));
            cancellatinTokenSource.Cancel();
            
            Assert.That(observerTask.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        }
    }
}