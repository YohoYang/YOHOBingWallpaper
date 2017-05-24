using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace YOHOBingWallpaper
{
    class SetWallpaper
    {
        /*
        * Thanks to Neil N. for the wallpaper code:
        * http://stackoverflow.com/a/1061682/4322228
        */
        public enum Style
        {
            Tiled,
            Centered,
            Stretched
        }

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public static void SetWallpaperWrok(String path, Style style)
        {

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            if (key == null)
            {
                throw new ApplicationException("Open RegistryKey 'Control Panel\\Desktop' failed.");
            }

            switch (style)
            {
                case Style.Stretched:
                    key.SetValue(@"WallpaperStyle", "2");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case Style.Centered:
                    key.SetValue(@"WallpaperStyle", "1");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case Style.Tiled:
                    key.SetValue(@"WallpaperStyle", "1");
                    key.SetValue(@"TileWallpaper", "1");
                    break;
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                Path.GetFullPath(path),
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
