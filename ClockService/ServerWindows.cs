using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ClockService
{
    class ServerWindows
    {
        /// <summary>
        /// 开启窗体 使用后用 System.Windows.Forms.Application.Run(_Form);来启动窗体
        /// </summary>
        /// <param name="s">日志</param>
        public static void ServiceForm(System.Diagnostics.EventLog p_ServiceLog)
        {
            try
            {
                GetDesktopWindow();
                IntPtr _WindowsStation = GetProcessWindowStation();
                IntPtr _ThreadID = GetCurrentThreadId();
                IntPtr _DeskTop = GetThreadDesktop(_ThreadID);
                IntPtr _HwinstaUser = OpenWindowStation("WinSta0", false, 33554432);
                if (_HwinstaUser == IntPtr.Zero)
                {
                    RpcRevertToSelf();
                    return;
                }
                SetProcessWindowStation(_HwinstaUser);
                IntPtr _HdeskUser = OpenDesktop("Default", 0, false, 33554432);
                RpcRevertToSelf();
                if (_HdeskUser == IntPtr.Zero)
                {
                    SetProcessWindowStation(_WindowsStation);
                    CloseWindowStation(_HwinstaUser);
                    return;
                }
                SetThreadDesktop(_HdeskUser);
                IntPtr _GuiThreadId = _ThreadID;
                _GuiThreadId = IntPtr.Zero;
                SetThreadDesktop(_DeskTop);
                SetProcessWindowStation(_WindowsStation);
                CloseDesktop(_HdeskUser);
                CloseWindowStation(_HwinstaUser);
            }
            catch (Exception ex)
            {
                p_ServiceLog.WriteEntry(ex.ToString());
            }
        }
        #region User32.DLL
        [DllImport("user32.dll")]
        private static extern int GetDesktopWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetProcessWindowStation();
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThreadId();
        [DllImport("user32.dll")]
        private static extern IntPtr GetThreadDesktop(IntPtr dwThread);
        [DllImport("user32.dll")]
        private static extern IntPtr OpenWindowStation(string lpszWinSta, bool fInherit, int dwDesiredAccess);
        [DllImport("User32.dll")]
        private static extern IntPtr OpenDesktop(string lpsxDesktop, uint dwFlags, bool fInherit, uint dwDesiredAccess);
        [DllImport("user32.dll")]
        private static extern IntPtr CloseDesktop(IntPtr hDesktop);
        [DllImport("user32.dll")]
        private static extern IntPtr SetThreadDesktop(IntPtr hDesktop);
        [DllImport("user32.dll")]
        private static extern IntPtr SetProcessWindowStation(IntPtr hWinSta);
        [DllImport("user32.dll")]
        private static extern IntPtr CloseWindowStation(IntPtr hWinSta);
        #endregion
        #region Rpcrt4.dll
        [DllImport("rpcrt4.dll", SetLastError = true)]
        private static extern IntPtr RpcImpersonatClient(int rpc);
        [DllImport("rpcrt4.dll", SetLastError = true)]
        private static extern IntPtr RpcRevertToSelf();
        #endregion 
    }
}
