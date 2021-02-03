using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.IO;

namespace Wallpapers
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(
        UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        private static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
        private static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
        private static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;

        static public int defineOSVersion() {
            OperatingSystem os = Environment.OSVersion;
            Version vs = os.Version;

            return vs.Major;
        }

        static public void SetWallpaper(string path)
        {
            int OS = defineOSVersion();
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            if (OS >= 10)
            {
                key.SetValue(@"WallpaperStyle", 22.ToString());
            }
            else
            {
                key.SetValue(@"WallpaperStyle", 10.ToString());
            }
            key.SetValue(@"TileWallpaper", 0.ToString());

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        static public void SetLockScreenWallpaper(string path) {
            int OS = defineOSVersion();

            if (OS >= 10) {
                string[] values = { "LockScreenImagePath", "LockScreenImageUrl" };
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion", true);

                key.CreateSubKey("PersonalizationCSP");
                key = key.OpenSubKey("PersonalizationCSP", true);

                foreach (string value in values)
                {
                    key.SetValue(value, path);
                }
            }
        }

        static void Main(string[] args)
        {
            string filePath = args[0];
            bool shouldChangeLockScreen = bool.Parse(args[1]);

            if (File.Exists(filePath))
            {
                SetWallpaper(filePath);

                if (shouldChangeLockScreen) {
                    SetLockScreenWallpaper(filePath);
                }
                
                return;
            }

        }
    }
}