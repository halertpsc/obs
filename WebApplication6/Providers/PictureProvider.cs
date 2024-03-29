﻿using Microsoft.Extensions.Options;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication6.Providers
{
    public class PictureProvider : IPictureProvider, IDisposable
    {
        private readonly ObserverOptions _observerOptions;
        private readonly VideoCapture _capture;
        private readonly object _locker = new object();

        private static readonly object _staticLocker = new object();
        private static IPictureProvider _instance;
        private static int _counter = 0;
        private PictureProvider(ObserverOptions observerOptions)
        {
            _observerOptions = observerOptions;
            _capture = new VideoCapture(_observerOptions.PhotoDeviceId);
            _capture.Set(VideoCaptureProperties.FrameHeight, 1024);
            _capture.Set(VideoCaptureProperties.FrameWidth, 768);
            _capture.Open(_observerOptions.PhotoDeviceId);
        }

        public void Dispose()
        {
            Interlocked.Decrement(ref _counter);
            if(_counter<1)
            {
                _capture.Dispose();
                if (_instance != null)
                {
                    lock (_staticLocker)
                    {
                        if (_instance != null)
                        {
                            _instance = null;
                        }
                    }
                }
            }
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

        public static IPictureProvider GetInstance(ObserverOptions options)
        {
            if (_instance == null)
            {
                lock (_staticLocker)
                {
                    if (_instance == null)
                    {
                        _instance = new PictureProvider(options);
                    }
                }
            }

            Interlocked.Increment(ref _counter);
            return _instance;
        }
    }
}
