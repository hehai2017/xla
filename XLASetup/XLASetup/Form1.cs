using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace XLASetup
{
    public partial class Form1 : Form
    {
        private static string MainUrl = "www.baidu.com";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {           
            OpenFireFox(MainUrl);
        }

        public static void OpenBrowserUrl(string url)
          {
              try
              {
                  // 64位注册表路径
                  var openKey = @"SOFTWARE\Wow6432Node\Google\Chrome";
                  if (IntPtr.Size == 4)
                  {
                      // 32位注册表路径
                      openKey = @"SOFTWARE\Google\Chrome";
                  }
                  RegistryKey appPath = Registry.LocalMachine.OpenSubKey(openKey);
                  // 谷歌浏览器就用谷歌打开，没找到就用系统默认的浏览器
                  // 谷歌卸载了，注册表还没有清空，程序会返回一个"系统找不到指定的文件。"的bug
                  if (appPath != null)
                  {
                      var result = Process.Start("chrome.exe", url);
                      if (result == null)
                      {
                          OpenIe(url);
                      }
                  }
                  else
                  {
                      var result = Process.Start("chrome.exe", url);
                      if (result == null)
                      {
                          OpenDefaultBrowserUrl(url);
                      }
                  }
              }
              catch
              {
                  // 出错调用用户默认设置的浏览器，还不行就调用IE
                  OpenDefaultBrowserUrl(url);
              }
        }

        /// <summary>
         /// 火狐浏览器打开网页
         /// </summary>
         /// <param name="url"></param>
        public static void OpenFireFox(string url)
         {
             try
             {
                 // 64位注册表路径
                 var openKey = @"SOFTWARE\Wow6432Node\Mozilla\Mozilla Firefox";
                 if (IntPtr.Size == 4)
                 {
                     // 32位注册表路径
                     openKey = @"SOFTWARE\Mozilla\Mozilla Firefox";
                 }
                 RegistryKey appPath = Registry.LocalMachine.OpenSubKey(openKey);
                 if (appPath != null)
                 {
                    ProcessStartInfo startInfo = new ProcessStartInfo("firefox.exe");
                    Process process1 = new Process();
                    startInfo.Arguments = "-url " + MainUrl;
                    process1.StartInfo = startInfo;
                    var result = process1.Start();

                    if (result == false)
                    {
                        OpenIe(url);
                    }
                }
                 else
                 {
                    var result = Process.Start("D:\\release\\Firefox_54.0.1.6388_setup.exe");
                    
                    Thread t1 = new Thread(new ThreadStart(TestMethod));
                    t1.IsBackground = true;
                    t1.Start();
                }
             }
             catch
             {
                 OpenDefaultBrowserUrl(url);
             }
         }

        private static void TestMethod()
        {
            try
            {
                Thread.Sleep(3000);
                Process[] processes;
                while (true)
                {
                    processes = Process.GetProcessesByName("Firefox_54.0.1.6388_setup");
                    {
                        if (processes.Length == 0)
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(3000);                           
                        }
                    }
                }
                LoadMainPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 用IE打开浏览器
        /// </summary>
        /// <param name="url"></param>
        public static void OpenIe(string url)
        {
            try
            {
                Process.Start("iexplore.exe", url);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                // IE浏览器路径安装：C:\Program Files\Internet Explorer
                // at System.Diagnostics.process.StartWithshellExecuteEx(ProcessStartInfo startInfo)注意这个错误
                try
                {
                    if (File.Exists(@"C:\Program Files\Internet Explorer\iexplore.exe"))
                    {
                        ProcessStartInfo processStartInfo = new ProcessStartInfo
                        {
                            FileName = @"C:\Program Files\Internet Explorer\iexplore.exe",
                            Arguments = url,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        Process.Start(processStartInfo);
                    }
                    else
                    {
                        if (File.Exists(@"C:\Program Files (x86)\Internet Explorer\iexplore.exe"))
                        {
                            ProcessStartInfo processStartInfo = new ProcessStartInfo
                            {
                                FileName = @"C:\Program Files (x86)\Internet Explorer\iexplore.exe",
                                Arguments = url,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            };
                            Process.Start(processStartInfo);
                        }
                        else
                        {
                            if (MessageBox.Show("系统未安装IE浏览器，是否下载安装？", null, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                // 打开下载链接，从微软官网下载
                                OpenDefaultBrowserUrl("http://windows.microsoft.com/zh-cn/internet-explorer/download-ie");
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        /// <summary>
        /// 打开系统默认浏览器（用户自己设置了默认浏览器）
        /// </summary>
        /// <param name="url"></param>
        public static void OpenDefaultBrowserUrl(string url)
        {
            try
            {
                // 方法1
                //从注册表中读取默认浏览器可执行文件路径
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
                if (key != null)
                {
                    string s = key.GetValue("").ToString();
                    //s就是你的默认浏览器，不过后面带了参数，把它截去，不过需要注意的是：不同的浏览器后面的参数不一样！
                    //"D:\Program Files (x86)\Google\Chrome\Application\chrome.exe" -- "%1"
                    var lastIndex = s.IndexOf(".exe", StringComparison.Ordinal);
                    var path = s.Substring(1, lastIndex + 3);
                    var result = Process.Start(path, url);
                    if (result == null)
                    {
                        // 方法2
                        // 调用系统默认的浏览器 
                        var result1 = Process.Start("explorer.exe", url);
                        if (result1 == null)
                        {
                            // 方法3
                            Process.Start(url);
                        }
                    }
                }
                else
                {
                    // 方法2
                    // 调用系统默认的浏览器 
                    var result1 = Process.Start("explorer.exe", url);
                    if (result1 == null)
                    {
                        // 方法3
                        Process.Start(url);
                    }
                }
            }
            catch
            {
                OpenIe(url);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                //取消"关闭窗口"事件
                e.Cancel = true; // 取消关闭窗体 

                //使关闭时窗口向右下角缩小的效果
                this.WindowState = FormWindowState.Minimized;
                this.mainNotifyIcon.Visible = true;
                //this.m_cartoonForm.CartoonClose();
                this.Hide();
                return;
            }
        }

        private void mainNotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.WindowState = FormWindowState.Minimized;
                this.mainNotifyIcon.Visible = true;
                this.Hide();
            }
            else
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你确定要退出？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {

                this.mainNotifyIcon.Visible = false;
                this.Close();
                this.Dispose();
                System.Environment.Exit(System.Environment.ExitCode);

            }
        }

        private void btn_Reload_Click(object sender, EventArgs e)
        {
            KillFirefox();
            Thread.Sleep(1000);
            LoadMainPage();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KillFirefox();
            Thread.Sleep(500);
            LoadMainPage();
        }

        private static void LoadMainPage()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("firefox.exe");
            Process process1 = new Process();
            startInfo.Arguments = "-url " + MainUrl;
            process1.StartInfo = startInfo;
            var result = process1.Start();
        }

        private static void KillFirefox()
        {
            try
            {
                System.Diagnostics.Process[] myProcesses = System.Diagnostics.Process.GetProcesses();
                foreach (System.Diagnostics.Process myProcess in myProcesses)
                {
                    if (myProcess.ProcessName.ToLower() == "firefox")
                    {
                        myProcess.Kill();
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }

        }
    }

}

