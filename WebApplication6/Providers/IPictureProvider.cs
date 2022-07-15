using OpenCvSharp;
using System.IO;

namespace WebApplication6.Providers
{
    public interface IPictureProvider
    {
        Stream GetPngPicture();
        Mat GetMat();
    }
}