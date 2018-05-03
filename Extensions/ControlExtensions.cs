using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Visual_SICXE.Extensions
{
    public static class ControlExtensions
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        private const int WM_SETREDRAW = 0x000B;

        public static void SuspendDrawing(this Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(this Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, true, 0);
            control.Refresh();
        }

        /// <summary>
        /// Enables or disables double bufffering on this control.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="enable">If true, double buffering will be enabled. If false, it will be disabled.</param>
        public static void DoubleBuffer(this Control control, bool enable)
        {
            var pi = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, enable, null);
        }
    }
}
