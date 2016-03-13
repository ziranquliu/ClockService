using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading;

namespace ClockService
{
    public partial class ServiceMain : ServiceBase
    {
        [DllImport("user32.dll", EntryPoint = "MessageBox")]
        public static extern int MsgBox(int hWnd, String text, String caption, uint type);
        private System.Timers.Timer timer1 = new System.Timers.Timer();
        private readonly string logPath = ConfigurationSettings.AppSettings["logPath"];
        private DateTime LatestTime = DateTime.Now;
        public ServiceMain()
        {
            InitializeComponent();
            this.timer1.Elapsed += Timer1_Elapsed;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                this.WriteLog("服务开始启动");
                this.timer1.AutoReset = true;
                this.timer1.Interval = 1000 * Convert.ToInt32(ConfigurationManager.AppSettings["interval"]);
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.StackTrace);
                this.timer1.Interval = 3600 * 1000;
            }
            finally
            {
                this.timer1.Start();
            }
        }

        protected override void OnStop()
        {
            this.WriteLog("服务开始停止");
            this.timer1.Stop();
        }

        private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            WriteLog("当前提醒时间" + DateTime.Now + "    上次提醒时间" + LatestTime);

            string arg = string.Format("您已经连续工作了{0}，该去休息了。\r\n\r\n身体是革命的本钱，请珍惜！", (DateTime.Now - LatestTime).ToString());
            //Run(Application.StartupPath + "\\clock.exe" + arg.ToString());
            new Thread((p) =>
            {
                Process.Start(Application.StartupPath + "\\clock.exe", p.ToString());
            }).Start(arg);

            //Thread _TestForm = new Thread(new ParameterizedThreadStart((i) =>
            //{
            //    try {
            //        ServerWindows.ServiceForm(EventLog);
            //        FormDialog _MyForm = new FormDialog();
            //        _MyForm.label1.Text = string.Format("您已经连续工作了{0}，该去休息了。\r\n\r\n身体是革命的本钱，请珍惜！",
            //            i);
            //        Application.Run(_MyForm);
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}));
            //_TestForm.Start((LatestTime - DateTime.Now));

            //MsgBox(0,"您已经连续工作了" + (LatestTime - DateTime.Now) + "，该去休息了。身体是革命的本钱，请珍惜！", "休息提醒", 0);
            LatestTime = DateTime.Now;
        }

        private void WriteLog(string logmsg)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(logPath, true, Encoding.UTF8))
                {
                    sw.WriteLine(logmsg);
                    sw.Flush();
                }
            }
            finally
            {

            }
        }

        public void Run(string argm)
        {
            Util.STARTUPINFO sInfo = new Util.STARTUPINFO();
            sInfo.lpDesktop = "Winsta0\\Default";
            Util.PROCESS_INFORMATION pInfo = new Util.PROCESS_INFORMATION();
            if (!Util.CreateProcess(null, new StringBuilder(argm), null, null, false, 0, null, null, ref sInfo, ref pInfo))
            {
                throw new Exception("调用失败");
            }
            uint i = 0;
            Util.WaitForSingleObject(pInfo.hProcess, int.MaxValue);
            Util.GetExitCodeProcess(pInfo.hProcess, ref i);
            Util.CloseHandle(pInfo.hProcess);
            Util.CloseHandle(pInfo.hThread);
        }
    }
}
