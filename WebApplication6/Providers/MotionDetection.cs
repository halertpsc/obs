using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication6.Providers
{
    public class MotionDetection
    {

        private readonly IPictureProvider _pictureProvider;
        private readonly ILogger<MotionDetection> _logger;
        private readonly ObserverOptions _options;
        private Mat _previousPicture;


        public MotionDetection(IPictureProvider pictureProvider, ILogger<MotionDetection> logger, IOptions<ObserverOptions> options)
        {
            _pictureProvider = pictureProvider ?? throw new ArgumentNullException(nameof(pictureProvider));
            _options = options.Value;
            _logger = logger;
        }

        public bool Detect()
        {
            if (_previousPicture is null)
            {
                _previousPicture = _pictureProvider.GetMat();
                Cv2.CvtColor(_previousPicture, _previousPicture, ColorConversionCodes.BGR2GRAY);
                return false;
            }

            using var newPicture = _pictureProvider.GetMat();
            Cv2.CvtColor(newPicture, newPicture, ColorConversionCodes.BGR2GRAY);

            var resultMat = new Mat();
            Cv2.Absdiff(_previousPicture, newPicture, resultMat);
            newPicture.CopyTo(_previousPicture);
            Cv2.Threshold(resultMat, resultMat, _options.Threshold, 255, ThresholdTypes.Binary);
            var nonZero = Cv2.CountNonZero(resultMat);
          
            if (nonZero > _options.NonBlack)
            {
                _logger.LogInformation("Non zero pixels {nonZeroPixels}", nonZero.ToString());
                _logger.LogInformation("!!!!!!!!!!!!!!!MOTION WAS DETECTED!!!!!!!!!!!!!!!!!!!!!!!");
                return true;
            }

            return false;
        }
    }
}
