using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace BingWallpaper
{
    public class BingDownloader
    {
        public BingDownloader(string folder)
        {
            DownloadFolder = folder;

            UseHttps = false;
            Market = BingMarket.World;
            Count = 1;
            Size = new Rect {Bottom = 768, Right = 1366};
        }

        public string DownloadFolder { get; set; }

        public bool UseHttps { get; set; }

        public string Market { get; set; }

        public int Count { get; set; }

        public Rect Size { get; set; }

        public string DownloadSync()
        {
            string url = string.Format(@"{0}://www.bing.com/hpimagearchive.aspx?format=xml&idx=0&n={1}&mkt={2}",
                                       UseHttps ? "https" : "http",
                                       /*Count,*/1,
                                       Market);

            try
            {
                var client = new WebClientEx(10 * 1000);
                byte[] buffer = client.DownloadData(url);

                var xmlContent = new XmlDocument();
                xmlContent.Load(new MemoryStream(buffer));

                string date = xmlContent.SelectSingleNode(@"/images/image/startdate/text()").Value;
                string baseUrl = xmlContent.SelectSingleNode(@"/images/image/urlBase/text()").Value;

                return ProcessDownload(DownloadFolder,
                                       string.Format(@"{0}://www.bing.com{1}_{2}.jpg",
                                                     UseHttps ? "https" : "http",
                                                     baseUrl,
                                                     GetBestSize(Size)));
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string ProcessDownload(string folder, string url)
        {
            try
            {
                string localFile = Path.Combine(folder, Path.GetFileName(new Uri(url).LocalPath));

                var client = new WebClientEx(10 * 1000);

                client.DownloadFile(url, localFile);

                return localFile;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string GetBestSize(Rect size)
        {
            int width = size.Right;
            int height = size.Bottom;

            Rect[] availableSizes =
            {
                new Rect {Right = 1024, Bottom = 768},
                new Rect {Right = 1280, Bottom = 720},
                new Rect {Right = 1366, Bottom = 768},
                new Rect {Right = 1920, Bottom = 1080},
                new Rect {Right = 1920, Bottom = 1200}
            };

            IEnumerable<Rect> bestSize = availableSizes.Where(s => s.Bottom >= height && s.Right >= width);

            if (!bestSize.Any())
            {
                Rect result = availableSizes[availableSizes.Length - 1];

                return string.Format(@"{0}x{1}", result.Right, result.Bottom);
            }

            return string.Format(@"{0}x{1}", bestSize.First().Right, bestSize.First().Bottom);
        }
    }

    public class BingMarket
    {
        public static string World = "en-WW";
        public static string USA = "en-US";
        public static string China = "zh-CN";
        public static string Japan = "ja-JP";
    }
}