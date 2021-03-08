using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PCSXEmul.Util
{
    [ComImport]
    [Guid("00000001-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IClassFactory
    {
        [return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 1)]
        void CreateInstance(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
            [In] ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 1)] out object pUnknown);

        void LockServer(
            [MarshalAs(UnmanagedType.Bool)] bool fLock);
    }

    struct Win32NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SetDllDirectory(
            [MarshalAs(UnmanagedType.LPWStr)] string lpPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibrary(
            [MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(
            IntPtr hModule,
            [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint DllGetClassObjectDelegate(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 1)] out object pUnknown);
    }

    class Native
    {
        private const int ALT = 0xA4;
        private const int EXTENDEDKEY = 0x1;
        private const int KEYUP = 0x2;
        private const int SHOW_MAXIMIZED = 3;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void ActivateWindow(IntPtr mainWindowHandle)
        {
            // Guard: check if window already has focus.
            if (mainWindowHandle == GetForegroundWindow()) return;

            // Show window maximized.
            ShowWindow(mainWindowHandle, SHOW_MAXIMIZED);

            // Simulate an "ALT" key press.
            keybd_event((byte)ALT, 0x45, EXTENDEDKEY | 0, 0);

            // Simulate an "ALT" key release.
            keybd_event((byte)ALT, 0x45, EXTENDEDKEY | KEYUP, 0);

            // Show window in forground.
            SetForegroundWindow(mainWindowHandle);
        }
        private static bool ProcessIsBrowser(Process proc, string a_lProcessName)
        {
            return proc.MainWindowTitle.EndsWith("Notepad", StringComparison.InvariantCultureIgnoreCase);
        }


        /// <summary>
        /// Reads path of default browser from registry
        /// source:
        /// http://www.seirer.net/blog/2014/6/10/solved-how-to-open-a-url-in-the-default-browser-in-csharp
        /// </summary>
        /// <returns>Rooted path to the browser</returns>
        private static ProcessStartInfo GetDefaultBrowserPath()
        {
            string urlAssociation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http";
            string browserPathKey = @"$BROWSER$\shell\open\command";

            try
            {
                //Read default browser path from userChoiceLKey
                var userChoiceKey = Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false);

                //If user choice was not found, try machine default
                if (userChoiceKey == null)
                {
                    //Read default browser path from Win XP registry key
                    var browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false) ?? Registry.CurrentUser.OpenSubKey(urlAssociation, false);

                    //If browser path wasn’t found, try Win Vista (and newer) registry key
                    var path = CleanifyBrowserPath(browserKey.GetValue(null) as string);
                    browserKey.Close();
                    return new ProcessStartInfo(path);
                }
                else
                {
                    // user defined browser choice was found
                    string progId = (userChoiceKey.GetValue("ProgId").ToString());
                    userChoiceKey.Close();

                    if (progId.StartsWith("AppX"))
                    {
                        string appUserModelID;
                        using (var applicationInfo = Registry.ClassesRoot.OpenSubKey(progId + @"\Application"))
                        {
                            appUserModelID = applicationInfo.GetValue("AppUserModelID") as string;
                        }
                        return new ProcessStartInfo("explorer.exe", @"shell:AppsFolder\" + appUserModelID);
                    }
                    else
                    {
                        // now look up the path of the executable
                        string concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
                        var kp = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false);
                        var browserPath = CleanifyBrowserPath(kp.GetValue(null) as string);
                        kp.Close();
                        return new ProcessStartInfo(browserPath);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private static string CleanifyBrowserPath(string p)
        {
            string[] url = p.Split('"');
            string clean = url[1];
            return clean;
        }
    }
}
