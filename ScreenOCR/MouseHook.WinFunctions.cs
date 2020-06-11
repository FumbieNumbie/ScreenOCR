using System;
using System.Runtime.InteropServices;

namespace ScreenOCR
{
	public partial class MouseHook
	{
		/// <summary>
		/// Imports necessary functions from DLLs 
		/// </summary>
		public partial class WinFunctions
		{
			/// <summary>
			/// Sets the hook to monitor system events. In this case, mouse events.
			/// </summary>
			/// <param name="idHook">the type of the hook procedure to be intalled (WH_MOUSE_LL, 14 in our case)</param>
			/// <param name="lpfn">A pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a thread created by a different process, the lpfn parameter must point to a hook procedure in a DLL. Otherwise, lpfn can point to a hook procedure in the code associated with the current process.</param>
			/// <param name="hMod">A handle to the DLL containing the hook procedure pointed to by the lpfn parameter. The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by the current process and if the hook procedure is within the code associated with the current process.</param>
			/// <param name="dwThreadId">The identifier of the thread with which the hook procedure is to be associated. For desktop apps, if this parameter is zero, the hook procedure is associated with all existing threads running in the same desktop as the calling thread.</param>
			/// <returns>A handle to the hook.</returns>
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			internal static extern IntPtr SetWindowsHookEx(int idHook, HookHandlerDelegate lpfn, IntPtr hMod, uint dwThreadId);

			/// <summary>
			/// Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
			/// </summary>
			/// <param name="hhk">A handle to the hook to be removed. This parameter is a hook handle returned by a previous call to SetWindowsHookEx.</param>
			/// <returns>Returnes true if successfull.</returns>
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool UnhookWindowsHookEx(IntPtr hhk);


			/// <summary>
			/// Retrieves a module handle for the specified module. The module must have been loaded by the calling process.
			/// </summary>
			/// <param name="lpModuleName">The name of the module.</param>
			/// <returns>Returns a handle to the specified module.</returns>
			[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			internal static extern IntPtr GetModuleHandle(string lpModuleName);


			/// <summary>
			/// Passes the hook information to the next hook procedure in the current hook chain
			/// </summary>
			/// <param name="hhk">This parameter is ignored</param>
			/// <param name="nCode">The hook code passed to the current hook procedure. The next hook procedure uses this code to determine how to process the hook information.</param>
			/// <param name="wParam">The wParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
			/// <param name="lParam">The lParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
			/// <returns>This value is returned by the next hook procedure in the chain. The current hook procedure must also return this value.</returns>
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
			IntPtr wParam, ref MSLLHOOKSTRUCT lParam);
		}
	}

}
