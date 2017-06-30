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
        public void GetWallpaper(bool onlyGetCopyRight)
        {
            string Picturespath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Pictures\\YOHOBingWallpaper";
            if (!Directory.Exists(Picturespath))//如果路径不存在
            {
                Directory.CreateDirectory(Picturespath);//创建一个路径的文件夹
            }
            if (onlyGetCopyRight)
            {
                GetXML(0);
                GetXML(-1);
            }
            else
            {
                DownloadWallpaper(GetXML(0), Picturespath + "\\today.jpg", "today.bmp");
                DownloadWallpaper(GetXML(-1), Picturespath + "\\tomorrow.jpg", "tomorrow.bmp");
            }
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
            return "1920x1080";//去掉下载不同分辨率的功能,改为裁剪,因为1920*1200有水印,1920*1080是最大美观分辨率
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
            SavatoBMP(filename, BmpFileName);
        }
        //检测网络连接是否正常
        private static void PingBing()
        {
            String BingURL = "223.6.6.6";//使用阿里DNS的ip来检测,避免部分原域名解析导致的bug
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
        private static void SavatoBMP(String filename, String BmpFileName)//重写使支持不同分辨率裁剪为最佳分辨率后设置,避免画面拉伸和压缩
        {
            if (File.Exists(filename))
            {
                Rectangle screen = Screen.PrimaryScreen.Bounds;
                double screenRate = (double)screen.Width / screen.Height;
                double picRate = (double)1920 / 1080;
                Rectangle fromR = new Rectangle(0, 0, 0, 0);
                Rectangle toR = new Rectangle(0, 0, 0, 0);
                Bitmap bitmap = new Bitmap(filename);
                if (screenRate == picRate)
                {
                    bitmap.Save(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Pictures\\YOHOBingWallpaper\\" + BmpFileName, System.Drawing.Imaging.ImageFormat.Bmp);
                }
                else
                {
                    if (screenRate > picRate)
                    {
                        fromR.X = 0;
                        fromR.Y = (1080 - (int)(1920 / screenRate)) / 2;
                        fromR.Width = 1920;
                        fromR.Height = (int)(1920 / screenRate);
                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = 1920;
                        toR.Height = (int)(1920 / screenRate);
                    }
                    else
                    {
                        fromR.X = (1920 - (int)(1080 * screenRate)) / 2;
                        fromR.Y = 0;
                        fromR.Width = (int)(1080 * screenRate);
                        fromR.Height = 1080;
                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = (int)(1080 * screenRate);
                        toR.Height = 1080;
                    }
                    var bt = new Bitmap(toR.Width, toR.Height);
                    var graphics = Graphics.FromImage(bt);
                    graphics.DrawImage(bitmap, toR, fromR, GraphicsUnit.Pixel);
                    bt.Save(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Pictures\\YOHOBingWallpaper\\" + BmpFileName, System.Drawing.Imaging.ImageFormat.Bmp);
                    bt.Dispose();
                    graphics.Dispose();
                }
                bitmap.Dispose();
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
