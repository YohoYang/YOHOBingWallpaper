using System;
using System.Threading;
using System.Windows.Forms;

namespace YOHOBingWallpaper
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            Mutex mutex = new Mutex(false, "30f41730-bd4c-43cd-b7b6-7c3340d8a9a0");
            // 检查程序是否已经在运行
            try
            {
                if (mutex.WaitOne(0, false))
                {
                    // 启动程序
                    Application.Run(new Form1());
                }
                else
                {
                    DialogResult ask = MessageBox.Show("程序已经在后台运行\nYOHOBingWallpaper自动更换必应壁纸 版本:0.0.3.0\n作者网站www.yohoyang.com  图片保存在我的文档内的图片目录\n\n此界面可以完成卸载:需要为你关闭后台程序并取消开机自动启动吗\n\n选择[是]将关闭后台并取消自启\n选择[否]将什么都不做", "YOHOBingWallpaper", MessageBoxButtons.YesNo);
                    if (ask == DialogResult.Yes)
                    {
                        //调用删除注册表的方法
                        Form1.UnSetAutoStartup();
                        //下面是结束除了自己之外其他同名程序的做法
                        System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcesses();//获取计算机上所有进程
                        foreach (System.Diagnostics.Process p in ps)
                        {
                            if (p.ProcessName == "YOHOBingWallpaper")//判断进程名称
                            {
                                if (System.Diagnostics.Process.GetCurrentProcess().Id != p.Id)//判断进程id不等于自己
                                {
                                    p.Kill();//停止进程
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.Close();
                    mutex = null;
                }
            }

        }
    }
}
