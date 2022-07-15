using Microsoft.Extensions.Options;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WebApplication6.Providers
{
    public class PictureProvider : IPictureProvider, IDisposable
    {
        private readonly ObserverOptions _observerOptions;
        private readonly VideoCapture _capture;
        private readonly object _locker = new object();
        public PictureProvider(IOptions<ObserverOptions> observerOptions)
        {
            _observerOptions = observerOptions.Value;
            _capture = new VideoCapture(_observerOptions.PhotoDeviceId);
            _capture.Set(VideoCaptureProperties.FrameHeight, 1024);
            _capture.Set(VideoCaptureProperties.FrameWidth, 768);
            _capture.Open(_observerOptions.PhotoDeviceId);
        }

        public void Dispose()
        {
            _capture?.Dispose();
        }

        public Mat GetMat()
        {
            lock (_locker)
            {
                var frame = new Mat();
                _capture.Read(frame);
                return frame;

            }
        }

        public Stream GetPngPicture()
        {
            using var mat = GetMat();
            return mat.ToMemoryStream();
        }
    }
}
