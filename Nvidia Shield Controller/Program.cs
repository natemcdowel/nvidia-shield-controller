using System;
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;

namespace MyTrayApp
{
    public class SysTrayApp : Form
    {
        [STAThread]
        public static void Main()
        {
            Application.Run(new SysTrayApp());
        }

        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        public SysTrayApp()
        {
            SetStartup();
            createTaskbar();
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        private void createTaskbar()
        {
            // Create a simple tray menu
            trayMenu = new ContextMenu();

            parseMenuOptions();

            trayMenu.MenuItems.Add("Exit", OnExit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "Nvidia Shield Controller";
            trayIcon.Icon = new Icon("favicon.ico", 40, 40);

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }

        private void SetStartup()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            rk.SetValue("Nvidia Shield Controller", Application.ExecutablePath.ToString());
        }

        private void parseMenuOptions()
        {
            Console.WriteLine(File.Exists(@"C:\Program Files (x86)\NVIDIA Corporation\NvStreamSrv\rxinput.dll"));
            if (File.Exists(@"C:\Program Files (x86)\NVIDIA Corporation\NvStreamSrv\rxinput.dll"))
            {
                trayMenu.MenuItems.Add("Disable Shield Controller", disableShieldController);
            }
            else
            {
                trayMenu.MenuItems.Add("Enable Shield Controller", enableShieldController);
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void disableShieldController(object sender, EventArgs e)
        {
            File.Move(@"C:\Program Files (x86)\NVIDIA Corporation\NvStreamSrv\rxinput.dll", @"C:\Program Files (x86)\NVIDIA Corporation\NvStreamSrv\rxinput.bak.dll");
            File.Move(@"C:\Program Files\NVIDIA Corporation\NvStreamSrv\rxinput.dll", @"C:\Program Files\NVIDIA Corporation\NvStreamSrv\rxinput.bak.dll");
            trayIcon.Dispose();
            createTaskbar();
        }

        private void enableShieldController(object sender, EventArgs e)
        {
            File.Move(@"C:\Program Files (x86)\NVIDIA Corporation\NvStreamSrv\rxinput.bak.dll", @"C:\Program Files (x86)\NVIDIA Corporation\NvStreamSrv\rxinput.dll");
            File.Move(@"C:\Program Files\NVIDIA Corporation\NvStreamSrv\rxinput.bak.dll", @"C:\Program Files\NVIDIA Corporation\NvStreamSrv\rxinput.dll");
            trayIcon.Dispose();
            createTaskbar();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}