using System;
using System.Net;

namespace BingWallpaper
{
    public class WebClientEx : WebClient
    {
        public WebClientEx() : this(60 * 1000)
        {
        }

        public WebClientEx(int timeout)
        {
            Timeout = timeout;
        }

        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);

            request.Timeout = Timeout;

            return request;
        }
    }
}