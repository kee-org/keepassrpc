using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using KeePassLib;
using KeePassLib.Utility;

namespace KeePassRPC
{
    internal class Native
    {
        //from: KeePass and http://stackoverflow.com/questions/46030/c-force-form-focus/46092#46092

        // Sets the window to be foreground
        [DllImport("User32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("User32.dll")]
        private static extern void SwitchToThisWindow(
          IntPtr hWnd,
          int fAltTab
        );

        // Activate or minimize a window
        [DllImportAttribute("User32.DLL")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;

        private static bool? m_bIsUnix = null;
        public static bool IsUnix()
        {
            if (m_bIsUnix.HasValue) return m_bIsUnix.Value;

            PlatformID p = GetPlatformID();

            // Mono defines Unix as 128 in early .NET versions
#if !KeePassLibSD
            m_bIsUnix = ((p == PlatformID.Unix) || (p == PlatformID.MacOSX) ||
                ((int)p == 128));
#else
            m_bIsUnix = (((int)p == 4) || ((int)p == 6) || ((int)p == 128));
#endif
            return m_bIsUnix.Value;
        }

        private static PlatformID? m_platID = null;
        public static PlatformID GetPlatformID()
        {
            if (m_platID.HasValue) return m_platID.Value;

#if KeePassUAP
            m_platID = EnvironmentExt.OSVersion.Platform;
#else
            m_platID = Environment.OSVersion.Platform;
#endif

#if (!KeePassLibSD && !KeePassUAP)
            // Mono returns PlatformID.Unix on Mac OS X, workaround this
            if (m_platID.Value == PlatformID.Unix)
            {
                if ((RunConsoleApp("uname", null) ?? string.Empty).Trim().Equals(
                    "Darwin", StrUtil.CaseIgnoreCmp))
                    m_platID = PlatformID.MacOSX;
            }
#endif

            return m_platID.Value;
        }
        public static string RunConsoleApp(string strAppPath, string strParams)
        {
            return RunConsoleApp(strAppPath, strParams, null);
        }

        public static string RunConsoleApp(string strAppPath, string strParams,
            string strStdInput)
        {
            return RunConsoleApp(strAppPath, strParams, strStdInput,
                (AppRunFlags.GetStdOutput | AppRunFlags.WaitForExit));
        }

        private delegate string RunProcessDelegate();

        public static string RunConsoleApp(string strAppPath, string strParams,
            string strStdInput, AppRunFlags f)
        {
            if (strAppPath == null) throw new ArgumentNullException("strAppPath");
            if (strAppPath.Length == 0) throw new ArgumentException("strAppPath");

            bool bStdOut = ((f & AppRunFlags.GetStdOutput) != AppRunFlags.None);

            RunProcessDelegate fnRun = delegate ()
            {
                Process pToDispose = null;
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo();

                    psi.CreateNoWindow = true;
                    psi.FileName = strAppPath;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    psi.UseShellExecute = false;
                    psi.RedirectStandardOutput = bStdOut;

                    if (strStdInput != null) psi.RedirectStandardInput = true;

                    if (!string.IsNullOrEmpty(strParams)) psi.Arguments = strParams;

                    Process p = Process.Start(psi);
                    pToDispose = p;

                    if (strStdInput != null)
                    {
                        EnsureNoBom(p.StandardInput);

                        p.StandardInput.Write(strStdInput);
                        p.StandardInput.Close();
                    }

                    string strOutput = string.Empty;
                    if (bStdOut) strOutput = p.StandardOutput.ReadToEnd();

                    if ((f & AppRunFlags.WaitForExit) != AppRunFlags.None)
                        p.WaitForExit();
                    else if ((f & AppRunFlags.GCKeepAlive) != AppRunFlags.None)
                    {
                        pToDispose = null; // Thread disposes it

                        Thread th = new Thread(delegate ()
                        {
                            try { p.WaitForExit(); p.Dispose(); }
                            catch (Exception) { Debug.Assert(false); }
                        });
                        th.Start();
                    }

                    return strOutput;
                }
#if DEBUG
                catch (Exception ex) { Debug.Assert(ex is ThreadAbortException); }
#else
                catch(Exception) { }
#endif
                finally
                {
                    try { if (pToDispose != null) pToDispose.Dispose(); }
                    catch (Exception) { Debug.Assert(false); }
                }

                return null;
            };

            if ((f & AppRunFlags.DoEvents) != AppRunFlags.None)
            {
                List<Form> lDisabledForms = new List<Form>();
                if ((f & AppRunFlags.DisableForms) != AppRunFlags.None)
                {
                    foreach (Form form in Application.OpenForms)
                    {
                        if (!form.Enabled) continue;

                        lDisabledForms.Add(form);
                        form.Enabled = false;
                    }
                }

                IAsyncResult ar = fnRun.BeginInvoke(null, null);

                while (!ar.AsyncWaitHandle.WaitOne(0))
                {
                    Application.DoEvents();
                    Thread.Sleep(2);
                }

                string strRet = fnRun.EndInvoke(ar);

                for (int i = lDisabledForms.Count - 1; i >= 0; --i)
                    lDisabledForms[i].Enabled = true;

                return strRet;
            }

            return fnRun();
        }

        private static void EnsureNoBom(StreamWriter sw)
        {
            if (sw == null) { Debug.Assert(false); return; }
            if (!MonoWorkarounds.IsRequired(1219)) return;

            try
            {
                Encoding enc = sw.Encoding;
                if (enc == null) { Debug.Assert(false); return; }
                byte[] pbBom = enc.GetPreamble();
                if ((pbBom == null) || (pbBom.Length == 0)) return;

                // For Mono >= 4.0 (using Microsoft's reference source)
                try
                {
                    FieldInfo fi = typeof(StreamWriter).GetField("haveWrittenPreamble",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    if (fi != null)
                    {
                        fi.SetValue(sw, true);
                        return;
                    }
                }
                catch (Exception) { Debug.Assert(false); }

                // For Mono < 4.0
                FieldInfo fiPD = typeof(StreamWriter).GetField("preamble_done",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                if (fiPD != null) fiPD.SetValue(sw, true);
                else { Debug.Assert(false); }
            }
            catch (Exception) { Debug.Assert(false); }
        }

        internal static IntPtr GetForegroundWindowHandle()
        {
            if (!Native.IsUnix())
                return GetForegroundWindow(); // Windows API

            try
            {
                return new IntPtr(long.Parse(RunXDoTool(
                    "getactivewindow").Trim()));
            }
            catch (Exception) { Debug.Assert(false); }
            return IntPtr.Zero;
        }


        internal static string RunXDoTool(string strParams)
        {
            try
            {
                Application.DoEvents(); // E.g. for clipboard updates
                string strOutput = Native.RunConsoleApp("xdotool", strParams);
                Application.DoEvents(); // E.g. for clipboard updates
                return (strOutput ?? string.Empty);
            }
            catch (Exception) { Debug.Assert(false); }

            return string.Empty;
        }


        internal static bool SetForegroundWindowEx(IntPtr hWnd)
        {
            if (!Native.IsUnix())
                return SetForegroundWindow(hWnd);

            return (RunXDoTool("windowactivate " +
                hWnd.ToInt64().ToString()).Trim().Length == 0);
        }

        [DllImport("User32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        internal static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr SetFocus(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)] 
        private static extern bool SetWindowPlacement(IntPtr hWnd,
           [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern
            IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        private static extern
            IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, int fAttach);

        [DllImport("Kernel32.dll")]
        internal static extern IntPtr GetCurrentThreadId();

        [DllImport("User32.dll")]
        internal static extern int BringWindowToTop(IntPtr hWnd);

        [DllImport("User32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(
           int hWnd,               // window handle
           int hWndInsertAfter,    // placement-order handle
           int X,                  // horizontal position
           int Y,                  // vertical position
           int cx,                 // width
           int cy,                 // height
           uint uFlags);           // window positioning flags
        public const int HWND_BOTTOM = 0x1;
        public const int HWND_TOP = 0x0;
        public const int HWND_TOPMOST = -1;
        public const uint SWP_NOSIZE = 0x1;
        public const uint SWP_NOMOVE = 0x2;
        public const uint SWP_SHOWWINDOW = 0x40;

        [StructLayout(LayoutKind.Sequential)]
        private struct ANIMATIONINFO
        {
            public ANIMATIONINFO(bool iMinAnimate)
            {
                this.cbSize = GetSize();
                if (iMinAnimate) this.iMinAnimate = 1;
                else this.iMinAnimate = 0;
            }

            public uint cbSize;
            private int iMinAnimate;

            public bool IMinAnimate
            {
                get
                {
                    if (this.iMinAnimate == 0) return false;
                    else return true;
                }
                set
                {
                    if (value == true) this.iMinAnimate = 1;
                    else this.iMinAnimate = 0;
                }
            }

            public static uint GetSize()
            {
                return (uint)Marshal.SizeOf(typeof(ANIMATIONINFO));
            }
        }

        //http://www.codeproject.com/KB/cs/CapturingMinimizedWindow.aspx
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam,
         ref ANIMATIONINFO pvParam, SPIF fWinIni);

        [Flags]
        private enum SPIF
           {
               None = 0x00,
               SPIF_UPDATEINIFILE = 0x01,  // Writes the new system-wide parameter setting to the user profile.
               SPIF_SENDCHANGE = 0x02,  // Broadcasts the WM_SETTINGCHANGE message after updating the user profile.
               SPIF_SENDWININICHANGE = 0x02   // Same as SPIF_SENDCHANGE.
           }
        public static void SetMinimizeMaximizeAnimation(bool status)
        {
            ANIMATIONINFO animationInfo = new ANIMATIONINFO(status);
            SystemParametersInfo(0x0048, ANIMATIONINFO.GetSize(),
             ref animationInfo, SPIF.None);

            if (animationInfo.IMinAnimate != status)
            {
                animationInfo.IMinAnimate = status;
                SystemParametersInfo(0x0049, ANIMATIONINFO.GetSize(),
                 ref animationInfo, SPIF.SPIF_SENDCHANGE);
            }
        }

        /// <summary>
           /// Gets or Sets MinAnimate Effect
           /// </summary>
           public static bool MinAnimate
           {
               get
               {
                   ANIMATIONINFO animationInfo = new ANIMATIONINFO(false);

                   SystemParametersInfo(0x0048, ANIMATIONINFO.GetSize(), ref animationInfo, SPIF.None);
                   return animationInfo.IMinAnimate;
               }
               set
               {
                   ANIMATIONINFO animationInfo = new ANIMATIONINFO(value);
                   SystemParametersInfo(0x0049, ANIMATIONINFO.GetSize(), ref animationInfo, SPIF.SPIF_SENDCHANGE);
               }
           }

        internal static void ActivateApplication(IntPtr hnd)
        {
            ShowWindow(hnd, SW_RESTORE);
            SetForegroundWindow(hnd);

        }

        internal static bool IsWindowEx(IntPtr hWnd)
        {
            if (!Native.IsUnix()) // Windows
                return IsWindow(hWnd);

            return true;
        }

        internal static bool EnsureForegroundWindow(IntPtr hWnd)
        {
            if (!IsWindowEx(hWnd)) return false;

            IntPtr hWndInit = GetForegroundWindowHandle();

            if (!SetForegroundWindowEx(hWnd))
            {
                Debug.Assert(false);
                return false;
            }

            int nStartMS = Environment.TickCount;
            while ((Environment.TickCount - nStartMS) < 250)
            {
                IntPtr h = GetForegroundWindowHandle();
                if (h == hWnd) return true;

                // Some applications (like Microsoft Edge) have multiple
                // windows and automatically redirect the focus to other
                // windows, thus also break when a different window gets
                // focused (except when h is zero, which can occur while
                // the focus transfer occurs)
                if ((h != IntPtr.Zero) && (h != hWndInit)) return true;

                Application.DoEvents();
            }

            return false;
        }

        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }


        internal static bool AttachToActiveAndBringToForeground(IntPtr hWnd)
        {
            IntPtr appThread = GetCurrentThreadId();

            IntPtr foreThread = GetWindowThreadProcessId(GetForegroundWindow(),
                                                        IntPtr.Zero);

            if (foreThread != appThread)
            {
                AttachThreadInput(foreThread, appThread, 1);

                bool originalAnimate = MinAnimate;
                MinAnimate = false;

                // These cause a funny animation but that seems to be sufficient to work around windows annoying window activation limitations
                ShowWindow(hWnd, SW_MINIMIZE);
                ShowWindow(hWnd, SW_RESTORE);

                MinAnimate = originalAnimate;

                SetForegroundWindow(hWnd);
                BringWindowToTop(hWnd);
                SetFocus(hWnd);
                AttachThreadInput(foreThread, appThread, 0);
            }
            else
            {
                SetForegroundWindow(hWnd);
                BringWindowToTop(hWnd);
                ShowWindow(hWnd, SW_SHOW);
            }
            return true;
        }

        internal static void EnsureBackgroundWindow(IntPtr hWnd)
        {
            // all we really want to do is push it lower than whatever application called KeePassRPC but we don't know the z-order to use
            //TODO: can we find out the z-order somehow before we prompt for the master key?
            SetWindowPos((int)hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }
    }
}
