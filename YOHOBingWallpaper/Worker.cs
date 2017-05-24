using System;
using System.IO;

namespace YOHOBingWallpaper
{
    class Worker
    {
        private static bool NoUpdate = false;
        public static void StartWorking()
        {
            if (GetFileExist())//如果文件存在
            {
                if (GetTodayFileDate().Equals(GetDate(0)))//判断today.jpg修改时间是不是等于今天
                {
                    SetWallpaper.SetWallpaperWrok(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Pictures\\YOHOBingWallpaper" + "\\today.bmp", SetWallpaper.Style.Stretched);
                    NoUpdate = false;
                }
                else if (GetTodayFileDate().Equals(GetDate(-1)))//判断today.jpg修改时间是不是等于昨天
                {
                    SetWallpaper.SetWallpaperWrok(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Pictures\\YOHOBingWallpaper" + "\\tomorrow.bmp", SetWallpaper.Style.Stretched);
                    NoUpdate = true;
                }
                else//today.jpg既不是今天的也不是昨天的,需要重新下载壁纸
                {
                    NoUpdate = true;
                }
            }
            else//文件不存在,直接重新下载壁纸
            {
                NoUpdate = true;
            }
        }
        public static bool RetuenNoUpdate()
        {
            return NoUpdate;
        }
        public static void SetNoUpdate(bool ASwitch)
        {
            NoUpdate = ASwitch;
        }
        private static String GetTodayFileDate()//获得today.jpg的日期
        {
            System.IO.FileInfo fileinfo = new System.IO.FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Pictures\\YOHOBingWallpaper" + "\\today.bmp");
            String filedate = fileinfo.LastWriteTime.ToString();
            filedate = filedate.Substring(0, filedate.IndexOf(" "));
            return filedate;
        }
        private static bool GetFileExist()//判断today.bmp和tomorrow.bmp是否存在
        {
            bool fileexist = File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Pictures\\YOHOBingWallpaper" + "\\today.bmp") && File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Pictures\\YOHOBingWallpaper" + "\\tomorrow.bmp");
            return fileexist;
        }
        private static String GetDate(int num)//获得今天日期
        {
            return DateTime.Now.AddDays(num).ToShortDateString();
        }

    }
}
