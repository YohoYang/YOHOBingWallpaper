﻿using Microsoft.Win32;
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
                        GoToGetWallpaper();
                        GoToWorker();
                        ShowBalloonTip();
                    }
                }
                else
                {
                    GoToWorker();
                }
                FirstTimeFinish = true;
            }
            if (intSecond == 00 && FirstTimeFinish == true)//每整分钟执行
            {
                int intHour = DateTime.Now.Hour;
                int intMinute = DateTime.Now.Minute;
                bool isANewDay = (intHour == 00 && intMinute == 00 && intSecond == 00);
                if (isANewDay)//如果到新一天0点
                {
                    GoToWorker();
                    if (!GetBingWallpaper.ReturnNetError())
                    {
                        GoToGetWallpaper();
                        ShowBalloonTip();
                        Worker.SetNoUpdate(false);
                    }
                }
                else//其他情况
                {
                    if (Worker.RetuenNoUpdate())//检测到图片需要更新
                    {
                        if (!GetBingWallpaper.ReturnNetError())//如果网络正常
                        {
                            GoToGetWallpaper();
                            ShowBalloonTip();
                            GoToWorker();
                        }
                    }
                }
            }
        }
        private void GoToGetWallpaper()//跳转到GetBingWallpaper
        {
            GetBingWallpaper get = new GetBingWallpaper();
            get.GetWallpaper();
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
        }
        public static void UnSetAutoStartup()//添加到注册表开机自动启动,取最后一次执行程序的位置
        {
            RegistryKey rk = Registry.CurrentUser;
            RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            rk2.SetValue("YOHOBingWallpaper", "");
            rk2.DeleteValue("YOHOBingWallpaper");
            rk2.Close();
            rk.Close();
        }
        private void ShowBalloonTip()
        {
            NotifyIcon notify = new NotifyIcon();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            notify.Text = "YOHOBingWallpaper";
            notify.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            notify.Visible = true;
            notify.BalloonTipText = GetBingWallpaper.ReturnCopyright() + " \n" + DateTime.Now.AddDays(0).ToShortDateString();
            notify.BalloonTipTitle = "YohoBingWallpaper";
            notify.ShowBalloonTip(4000);
            System.Threading.Thread.Sleep(4000);
            notify.Visible = false;
        }
    }

}