using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;


namespace EMR
{
    static class Program
    {
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(System.IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
          
            bool createdNew;
            Mutex instance = new Mutex(true, "Global\\" + Application.ProductName, out createdNew);
            if (createdNew)
            {
                Application.Run(new Welcome(true));
                if (ThisAddIn.logon.ShowDialog() == DialogResult.Cancel) return;
                try
                {
                    Application.Run(new MainForm());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                Process current = Process.GetCurrentProcess();
                foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\").ToLower() == current.MainModule.FileName.ToLower())
                        {
                            MessageBox.Show("众心电子病历系统  已运行。", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            ShowWindowAsync(process.MainWindowHandle, 1);
                            SetForegroundWindow(process.MainWindowHandle);
                            break;
                        }
                    }
                }
                current.Kill();
            }
        }
    }
}