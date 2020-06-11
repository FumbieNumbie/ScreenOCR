using System;
using System.Diagnostics;
using System.Drawing;

namespace ScreenOCR
{
	public partial class MouseHook : IDisposable
	{
		#region Constants
		const int WM_LBUTTONDOWN = 0x0201;
		const int WM_LBUTTONUP = 0x0202;
		const int MK_LBUTTON = 0x0001;
		#endregion

		#region Conversion methods
		private int LowOrder(int input) {
			return unchecked((short)(long)input);
		}
		private int HighOrder(int input) {
			return unchecked((short)((uint)input >> 16));
		}
		#endregion

		

		//Variables used in the call to SetWindowsHookEx
		private HookHandlerDelegate proc;
		private IntPtr hookID = IntPtr.Zero;
		internal delegate IntPtr HookHandlerDelegate(int nCode, IntPtr wParam, ref MSLLHOOKSTRUCT lParam);
		internal struct MSLLHOOKSTRUCT
		{
			internal Point pt;
			internal uint mouseData;
			internal int flags;
			internal int time;
			internal int dwExtraInfo;
		}

		/// <summary>
		/// Sets a hook.
		/// </summary>
		public MouseHook() {
			proc = new HookHandlerDelegate(HookCallback);
			using (Process currentP = Process.GetCurrentProcess())
			using (ProcessModule currentModule = currentP.MainModule) {
				hookID = WinFunctions.SetWindowsHookEx(14, proc, WinFunctions.GetModuleHandle(currentModule.ModuleName), 0);
			}
		}
		#region Events and delegates
		public delegate void MouseDownEventHandler(object sender, MouseHookEventArgs e);
		public event MouseDownEventHandler MouseDown;
		public delegate void MouseUpEventHandler(object sender, MouseHookEventArgs e);
		public event MouseUpEventHandler MouseUp;
		#endregion

		#region Event handlers
		public void OnMouseDownEvent(MouseHookEventArgs e) {
			MouseDown?.Invoke(this, e);
		}
		public void OnMouseUpEvent(MouseHookEventArgs e) {
			MouseUp?.Invoke(this, e);
		}
		#endregion
		private IntPtr HookCallback(int nCode, IntPtr wParam, ref MSLLHOOKSTRUCT lParam) {
			if (nCode >= 0) {
				if (wParam == (IntPtr)WM_LBUTTONDOWN) {
					OnMouseDownEvent(new MouseHookEventArgs(lParam.pt));
				}
				if (wParam == (IntPtr)WM_LBUTTONUP) {
					OnMouseUpEvent(new MouseHookEventArgs(lParam.pt));
				}
			}
			//Pass information to next application
			return WinFunctions.CallNextHookEx(hookID, nCode, wParam, ref lParam);
		}

		public void Dispose() {

			WinFunctions.UnhookWindowsHookEx(hookID);
		}
	}

}
