using System;
using System.IO;
using System.Windows.Forms;

namespace BingWallpaper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\Bing Wallpapers";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var bd =
                new BingDownloader(path)
                {
                    UseHttps = true,
                    Size =
                        new Rect
                        {
                            Right = Screen.PrimaryScreen.Bounds.Width,
                            Bottom = Screen.PrimaryScreen.Bounds.Height
                        }
                };

            string file = bd.DownloadSync();

            if (!string.IsNullOrEmpty(file))
                SetAsWallPaper(file);
        }

        private static void SetAsWallPaper(string file)
        {
            try
            {
                var wallpaper = (IDesktopWallpaper)(new DesktopWallpaperClass());

                for (uint i = 0; i < wallpaper.GetMonitorDevicePathCount(); i++)
                {
                    wallpaper.SetWallpaper(wallpaper.GetMonitorDevicePathAt(i), file);
                    wallpaper.SetPosition(DesktopWallpaperPosition.Fill);
                }
            }
            catch
            {
                Environment.Exit(0);
            }
        }
    }
}