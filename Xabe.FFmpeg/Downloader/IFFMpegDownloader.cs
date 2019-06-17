
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xabe.FFmpeg.Downloader
{

    /// <summary>Downloads the Latest Version of FFMpeg</summary>
    public interface IFFMpegDownloader
    {
        /// <summary>Do the download and install FFMpeg</summary>
        Task GetLatestVersion();
    }
}