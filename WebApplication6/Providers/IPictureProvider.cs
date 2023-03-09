using OpenCvSharp;
using System;
using System.IO;

namespace WebApplication6.Providers
{
    public interface IPictureProvider : IDisposable
    {
        Stream GetPngPicture();
        Mat GetMat();
    }
}