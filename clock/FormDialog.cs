using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace clock
{
    public partial class FormDialog : Form
    {
        private System.Timers.Timer timer1 = new System.Timers.Timer();
        private string logPath = ConfigurationManager.AppSettings["logPath"];
        private DateTime LatestTime = DateTime.Now;
        public FormDialog(string[] msg)
        {
            InitializeComponent();
            //this.label1.Text = "您已经连续工作了{0}，该去休息了。\r\n\r\n身体是革命的本钱，请珍惜！";
            //this.label1.Text = msg[0];
            //this.label1.Text = "欢迎使用本软件，" + numericUpDown1.Value + "分钟后提醒";
            this.timer1.Elapsed += Timer1_Elapsed;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.WriteLog("服务开始启动");
            this.timer1.AutoReset = true;
            try
            {
                this.timer1.Interval = 1000 * Convert.ToInt32(numericUpDown1.Value * 60);
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
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            this.WriteLog("服务开始停止");
            this.timer1.Stop();
        }
        private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.timer1.Stop();
            WriteLog("当前提醒时间" + DateTime.Now + "    上次提醒时间" + LatestTime);
            string t = "";
            TimeSpan ts = (DateTime.Now - LatestTime);
            t += ts.Hours + "小时 ";
            t += ts.Minutes + "分钟";
            string arg = string.Format("您已经连续工作了{0}，该去休息了。\r\n\r\n身体是革命的本钱，请珍惜！", t);
            LatestTime = DateTime.Now;

            try
            {
                this.timer1.Interval = 1000 * Convert.ToInt32(numericUpDown1.Value * 60);
            }
            finally
            {
                this.Invoke(new Action(() =>
                {
                    this.label1.Text = arg;
                    this.Show();
                }));
            }
        }

        private void WriteLog(string logmsg)
        {
            return;
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
        private void button1_Click(object sender, EventArgs e)
        {
            this.timer1.Start();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }
    }
}
