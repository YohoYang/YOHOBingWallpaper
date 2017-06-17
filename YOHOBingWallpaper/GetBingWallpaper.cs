using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Xml;

namespace YOHOBingWallpaper
{
    public class GetBingWallpaper
    {
        private String baseURL = "https://www.bing.com/HPImageArchive.aspx";
        private String fullURL;
        private String imageURL;
        public static String todayCopyright = "NetworkERROR";
        public static String tomorrowCopyright = "NetworkERROR";
        public static bool Neterror = false;
        public void GetWallpaper()
        {
            string Picturespath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Pictures\\YOHOBingWallpaper";
            if (!Directory.Exists(Picturespath))//如果路径不存在
            {
                Directory.CreateDirectory(Picturespath);//创建一个路径的文件夹
            }
            DownloadWallpaper(GetXML(0), Picturespath + "\\today.jpg", "today.bmp");
            DownloadWallpaper(GetXML(-1), Picturespath + "\\tomorrow.jpg", "tomorrow.bmp");
        }
        private String GetXML(Int32 Time)
        {
            fullURL = baseURL + "?/format=xml&n=1&idx=" + Time;
            WebRequest req = WebRequest.Create(fullURL);
            String XML;
            using (WebResponse res = req.GetResponse())
            {
                using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                {
                    XML = sr.ReadToEnd();
                }
            }
            return AnalyzeXML(XML, Time);
        }
        private String AnalyzeXML(String XML, Int32 Time)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XML);
            var urlBaseNode = doc.SelectSingleNode("//urlBase");
            var copyrightNode = doc.SelectSingleNode("//copyright");
            SaveCopyright(Time, copyrightNode.InnerText);//保存两天的CopyRight信息
            imageURL = "https://www.bing.com" + urlBaseNode.InnerText + "_" + SelectASize() + ".jpg";
            return imageURL;
        }
        private String SelectASize()
        {
            Rectangle screen = Screen.PrimaryScreen.Bounds;
            double ScreenResolution = (double)screen.Width / screen.Height;
            if (ScreenResolution >= 0 && ScreenResolution < 1.4)
            {
                return "1024x768";
            }
            else if (ScreenResolution >= 1.4 && ScreenResolution < 1.7)
            {
                return "1920x1200";
            }
            else
            {
                return "1920x1080";
            }
        }
        private void DownloadWallpaper(String address, String filename, String BmpFileName)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(address, filename);
                }
                catch (Exception)
                {
                    Worker.SetNoUpdate(true);
                    throw;
                }
            }
            JPGtoBMP(filename, BmpFileName);
        }
        //检测网络连接是否正常
        private static void PingBing()
        {
            String BingURL = "www.bing.com";
            Ping ping = new Ping();
            try
            {
                PingReply pr;
                pr = ping.Send(BingURL);
                if (pr.Status != IPStatus.Success)
                {
                    Neterror = true;
                }
                else
                {
                    Neterror = false;
                }
            }
            catch
            {
                Neterror = true;
            }
            ping.Dispose();
        }
        public static bool ReturnNetError()
        {
            PingBing();
            return Neterror;
        }
        public static String ReturnCopyright(int Time)
        {
            if (Time == 0)
            {
                return todayCopyright;
            }
            else
            {
                return tomorrowCopyright;
            }

        }
        private static void JPGtoBMP(String filename, String BmpFileName)
        {
            if (File.Exists(filename))
            {
                FileStream stream;
                stream = File.OpenRead(filename);
                Bitmap jpg = new Bitmap(stream);
                jpg.Save(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Pictures\\YOHOBingWallpaper\\" + BmpFileName, System.Drawing.Imaging.ImageFormat.Bmp);
                stream.Dispose();
                jpg.Dispose();
                File.Delete(filename);
            }
            else
            {
                Worker.SetNoUpdate(true);
            }
        }
        private static void SaveCopyright(int Time, String CP)
        {
            if (Time == -1)
            {
                tomorrowCopyright = CP;
            }
            else
            {
                todayCopyright = CP;
            }
        }
    }
}
