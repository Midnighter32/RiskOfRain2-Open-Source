using System;
using System.Runtime.InteropServices;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

// Token: 0x02000025 RID: 37
public static class FlashWindow
{
	// Token: 0x0600009D RID: 157
	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool FlashWindowEx(ref FlashWindow.FLASHWINFO pwfi);

	// Token: 0x0600009E RID: 158
	[DllImport("user32.dll")]
	private static extern IntPtr GetActiveWindow();

	// Token: 0x0600009F RID: 159
	[DllImport("user32.dll")]
	private static extern bool EnumWindows(FlashWindow.EnumWindowsProc enumProc, IntPtr lParam);

	// Token: 0x060000A0 RID: 160
	[DllImport("user32.dll")]
	private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

	// Token: 0x060000A1 RID: 161
	[DllImport("kernel32.dll")]
	private static extern IntPtr GetCurrentProcessId();

	// Token: 0x060000A2 RID: 162 RVA: 0x00005170 File Offset: 0x00003370
	private static bool GetWindowEnum(IntPtr hWnd, IntPtr lParam)
	{
		IntPtr value;
		FlashWindow.GetWindowThreadProcessId(hWnd, out value);
		if (value == FlashWindow.myProcessId)
		{
			FlashWindow.myWindow = hWnd;
			return false;
		}
		return true;
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x0000519C File Offset: 0x0000339C
	private static void UpdateCurrentWindow()
	{
		FlashWindow.EnumWindows(new FlashWindow.EnumWindowsProc(FlashWindow.GetWindowEnum), IntPtr.Zero);
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x000051B8 File Offset: 0x000033B8
	private static bool IsCurrentWindowValid()
	{
		IntPtr value;
		FlashWindow.GetWindowThreadProcessId(FlashWindow.myWindow, out value);
		if (value != FlashWindow.myProcessId)
		{
			FlashWindow.myWindow = IntPtr.Zero;
		}
		return FlashWindow.myWindow != IntPtr.Zero;
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x000051F8 File Offset: 0x000033F8
	public static IntPtr GetWindowHandle()
	{
		if (!FlashWindow.IsCurrentWindowValid())
		{
			FlashWindow.UpdateCurrentWindow();
		}
		return FlashWindow.myWindow;
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x0000520C File Offset: 0x0000340C
	public static bool Flash(IntPtr formHandle)
	{
		if (FlashWindow.Win2000OrLater)
		{
			FlashWindow.FLASHWINFO flashwinfo = FlashWindow.Create_FLASHWINFO(formHandle, 15U, uint.MaxValue, 0U);
			return FlashWindow.FlashWindowEx(ref flashwinfo);
		}
		return false;
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x00005234 File Offset: 0x00003434
	private static FlashWindow.FLASHWINFO Create_FLASHWINFO(IntPtr handle, uint flags, uint count, uint timeout)
	{
		FlashWindow.FLASHWINFO flashwinfo = default(FlashWindow.FLASHWINFO);
		flashwinfo.cbSize = Convert.ToUInt32(Marshal.SizeOf<FlashWindow.FLASHWINFO>(flashwinfo));
		flashwinfo.hwnd = handle;
		flashwinfo.dwFlags = flags;
		flashwinfo.uCount = count;
		flashwinfo.dwTimeout = timeout;
		return flashwinfo;
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x0000527C File Offset: 0x0000347C
	public static bool Flash(IntPtr formHandle, uint count)
	{
		if (FlashWindow.Win2000OrLater)
		{
			FlashWindow.FLASHWINFO flashwinfo = FlashWindow.Create_FLASHWINFO(formHandle, 3U, count, 0U);
			return FlashWindow.FlashWindowEx(ref flashwinfo);
		}
		return false;
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x000052A4 File Offset: 0x000034A4
	public static bool Flash()
	{
		IntPtr windowHandle = FlashWindow.GetWindowHandle();
		Debug.LogFormat("FlashWindow.Flash({0})", new object[]
		{
			windowHandle
		});
		return FlashWindow.Flash(windowHandle);
	}

	// Token: 0x060000AA RID: 170 RVA: 0x000052D8 File Offset: 0x000034D8
	public static bool Start(IntPtr formHandle)
	{
		if (FlashWindow.Win2000OrLater)
		{
			FlashWindow.FLASHWINFO flashwinfo = FlashWindow.Create_FLASHWINFO(formHandle, 3U, uint.MaxValue, 0U);
			return FlashWindow.FlashWindowEx(ref flashwinfo);
		}
		return false;
	}

	// Token: 0x060000AB RID: 171 RVA: 0x00005300 File Offset: 0x00003500
	public static bool Stop(IntPtr formHandle)
	{
		if (FlashWindow.Win2000OrLater)
		{
			FlashWindow.FLASHWINFO flashwinfo = FlashWindow.Create_FLASHWINFO(formHandle, 0U, uint.MaxValue, 0U);
			return FlashWindow.FlashWindowEx(ref flashwinfo);
		}
		return false;
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x060000AC RID: 172 RVA: 0x00005327 File Offset: 0x00003527
	private static bool Win2000OrLater
	{
		get
		{
			return Environment.OSVersion.Version.Major >= 5;
		}
	}

	// Token: 0x060000AD RID: 173 RVA: 0x00005340 File Offset: 0x00003540
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		SceneManager.activeSceneChanged += delegate(Scene previousScene, Scene newScene)
		{
			if (newScene.name == "lobby")
			{
				FlashWindow.Flash();
			}
		};
		GameNetworkManager.onClientConnectGlobal += delegate(NetworkConnection conn)
		{
			FlashWindow.Flash();
		};
	}

	// Token: 0x040000B2 RID: 178
	private static IntPtr myWindow;

	// Token: 0x040000B3 RID: 179
	private static readonly IntPtr myProcessId = FlashWindow.GetCurrentProcessId();

	// Token: 0x040000B4 RID: 180
	public const uint FLASHW_STOP = 0U;

	// Token: 0x040000B5 RID: 181
	public const uint FLASHW_CAPTION = 1U;

	// Token: 0x040000B6 RID: 182
	public const uint FLASHW_TRAY = 2U;

	// Token: 0x040000B7 RID: 183
	public const uint FLASHW_ALL = 3U;

	// Token: 0x040000B8 RID: 184
	public const uint FLASHW_TIMER = 4U;

	// Token: 0x040000B9 RID: 185
	public const uint FLASHW_TIMERNOFG = 12U;

	// Token: 0x02000026 RID: 38
	// (Invoke) Token: 0x060000B0 RID: 176
	private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

	// Token: 0x02000027 RID: 39
	private struct FLASHWINFO
	{
		// Token: 0x040000BA RID: 186
		public uint cbSize;

		// Token: 0x040000BB RID: 187
		public IntPtr hwnd;

		// Token: 0x040000BC RID: 188
		public uint dwFlags;

		// Token: 0x040000BD RID: 189
		public uint uCount;

		// Token: 0x040000BE RID: 190
		public uint dwTimeout;
	}
}
