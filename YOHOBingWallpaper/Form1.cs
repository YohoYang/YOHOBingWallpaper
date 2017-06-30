using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace YOHOBingWallpaper
{
    public partial class Form1 : Form
    {
        private bool FirstTime = true;
        private bool FirstTimeFinish = false;
        public Form1()
        {
            SetAutoStartup();//启动时设置开机启动项
            System.Threading.Timer Thread_Time = new System.Threading.Timer(TimeEvent, null, 0, 1000);//每秒启动一次计时器事件
        }
        protected override void OnLoad(EventArgs e)//OnLoad函数在窗体构建时自动调用
        {
            Visible = false; // 隐藏界面
            ShowInTaskbar = false; // 不在任务栏显示
            base.OnLoad(e);
        }
        private void TimeEvent(object source)//计时器事件
        {
            int intSecond = DateTime.Now.Second;
            if (FirstTime)//首次运行执行
            {
                FirstTime = false;
                if (!GetBingWallpaper.ReturnNetError())
                {
                    GoToWorker();
                    if (Worker.RetuenNoUpdate())
                    {
                        GoToGetWallpaper(false);
                        GoToWorker();
                        ShowBalloonTip(0);
                    }
                    else
                    {
                        GoToGetWallpaper(true);
                    }
                }
                else
                {
                    GoToWorker();
                    Worker.SetNoUpdate(true);
                }
                FirstTimeFinish = true;
            }
            if (intSecond == 00 && FirstTimeFinish == true)//每整分钟执行
            {
                int intHour = DateTime.Now.Hour;
                int intMinute = DateTime.Now.Minute;
                bool isANewDay = (intHour == 00 && intMinute == 00);
                if (isANewDay)//如果到新一天0点
                {
                    GoToWorker();
                    ShowBalloonTip(-1);
                }
                else//其他情况
                {
                    if (Worker.RetuenNoUpdate())//检测到图片需要更新
                    {
                        if (!GetBingWallpaper.ReturnNetError())//如果网络正常
                        {
                            GoToGetWallpaper(false);
                            GoToWorker();
                        }
                    }
                    else if (GetBingWallpaper.todayCopyright.Equals("NetworkERROR"))
                    {
                        if (!GetBingWallpaper.ReturnNetError())//如果网络正常
                        {
                            GoToGetWallpaper(true);
                        }
                    }
                }
            }
        }
        private void GoToGetWallpaper(bool onlyGetCopyRight)//跳转到GetBingWallpaper
        {
            GetBingWallpaper get = new GetBingWallpaper();
            get.GetWallpaper(onlyGetCopyRight);
        }
        private void GoToWorker()//跳转到Worker
        {
            Worker.StartWorking();
        }
        private void SetAutoStartup()//添加到注册表开机自动启动,取最后一次执行程序的位置
        {
            string path = Application.ExecutablePath;
            RegistryKey rk = Registry.CurrentUser;
            RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            rk2.SetValue("YOHOBingWallpaper", path);
            rk2.Close();
            rk.Close();
            rk.Dispose();
            rk2.Dispose();
        }
        public static void UnSetAutoStartup()//取消注册表开机自动启动
        {
            RegistryKey rk = Registry.CurrentUser;
            RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            rk2.SetValue("YOHOBingWallpaper", "");
            rk2.DeleteValue("YOHOBingWallpaper");
            rk2.Close();
            rk.Close();
            rk.Dispose();
            rk2.Dispose();
        }
        private static void ShowBalloonTip(int Time)
        {
            if (!GetBingWallpaper.todayCopyright.Equals("NetworkERROR"))
            {
                NotifyIcon notify = new NotifyIcon();
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
                notify.Text = "YOHOBingWallpaper";
                notify.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
                notify.Visible = true;
                notify.BalloonTipText = GetBingWallpaper.ReturnCopyright(Time) + " \n" + DateTime.Now.AddDays(0).ToShortDateString();
                notify.BalloonTipTitle = "YohoBingWallpaper";
                notify.ShowBalloonTip(10000);
                System.Threading.Thread.Sleep(5000);
                notify.Visible = false;
                notify.Dispose();
            }
        }
    }
}
