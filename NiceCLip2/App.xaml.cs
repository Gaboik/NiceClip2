using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace NiceCLip2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        protected static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        protected static extern bool UnregisterHotKey(IntPtr Hwnd, int id);

        private void Application_Startup(object sender, StartupEventArgs e)
        {
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Unregister clipboard viewer
            WindowInteropHelper iHelper = new WindowInteropHelper(Current.MainWindow);
            MainWindow main = (MainWindow) Current.MainWindow;
            ChangeClipboardChain(iHelper.Handle, main.NextClipboardViewer);

            // Unregister HotKey
            UnregisterHotKey(iHelper.Handle, 0x1);
        }
    }
}
