using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Extraire_Donnees_Meteo
{
    class Internet
    {
        static private Internet _instance = new Internet();
        //The cookies will be here.
        private static CookieContainer _cookies = new CookieContainer();

        public static Internet getInstance()
        {
            return _instance;
        }
        private Internet()
        {

        }

        public HtmlDocument loadPage(string requete)
        {
            /*HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requete);
            request.Method = "GET";
            request.CookieContainer = _cookies;
            request.Method = WebRequestMethods.Http.Get;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0";
            request.AllowWriteStreamBuffering = true;
            request.ProtocolVersion = HttpVersion.Version11;
            request.AllowAutoRedirect = true;
            request.ContentLength = 0;
            request.ContentType = "application/x-www-form-urlencoded";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Add all the cookies
            foreach (Cookie cookie in response.Cookies)
                request.CookieContainer.Add(cookie);

            var stream = response.GetResponseStream();

            using (var reader = new StreamReader(stream))
            {
                string html = reader.ReadToEnd();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                return doc;
            }*/
            HtmlWeb web = new HtmlWeb();
            web.UseCookies = true;
            web.UserAgent = "Mozilla/5.0 (Windows NT x.y; WOW64; rv:10.0) Gecko/20100101 Firefox/10.0";
            return web.Load(requete);
        }


        static public bool OnPreRequest2(HttpWebRequest request)
        {
            request.CookieContainer = _cookies;
            return true;
        }
        static protected void OnAfterResponse2(HttpWebRequest request, HttpWebResponse response)
        {
            //do nothing
        }



        public static Image loadImage(string url)
        {
            HttpWebRequest wreq;
            HttpWebResponse wresp;
            Stream mystream;
            Bitmap bmp;

            bmp = null;
            mystream = null;
            wresp = null;
            try
            {
                wreq = (HttpWebRequest)WebRequest.Create(url);
                wreq.AllowWriteStreamBuffering = true;

                wresp = (HttpWebResponse)wreq.GetResponse();

                if ((mystream = wresp.GetResponseStream()) != null)
                    bmp = new Bitmap(mystream);
            }
            finally
            {
                if (mystream != null)
                    mystream.Close();

                if (wresp != null)
                    wresp.Close();
            }

            return bmp;
        }
    }
}
