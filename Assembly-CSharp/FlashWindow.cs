using System;
using System.Runtime.InteropServices;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

// Token: 0x02000029 RID: 41
public static class FlashWindow
{
	// Token: 0x060000BB RID: 187
	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool FlashWindowEx(ref FlashWindow.FLASHWINFO pwfi);

	// Token: 0x060000BC RID: 188
	[DllImport("user32.dll")]
	private static extern IntPtr GetActiveWindow();

	// Token: 0x060000BD RID: 189
	[DllImport("user32.dll")]
	private static extern bool EnumWindows(FlashWindow.EnumWindowsProc enumProc, IntPtr lParam);

	// Token: 0x060000BE RID: 190
	[DllImport("user32.dll")]
	private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

	// Token: 0x060000BF RID: 191
	[DllImport("kernel32.dll")]
	private static extern IntPtr GetCurrentProcessId();

	// Token: 0x060000C0 RID: 192 RVA: 0x00005294 File Offset: 0x00003494
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

	// Token: 0x060000C1 RID: 193 RVA: 0x000052C0 File Offset: 0x000034C0
	private static void UpdateCurrentWindow()
	{
		FlashWindow.EnumWindows(new FlashWindow.EnumWindowsProc(FlashWindow.GetWindowEnum), IntPtr.Zero);
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x000052DC File Offset: 0x000034DC
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

	// Token: 0x060000C3 RID: 195 RVA: 0x0000531C File Offset: 0x0000351C
	public static IntPtr GetWindowHandle()
	{
		if (!FlashWindow.IsCurrentWindowValid())
		{
			FlashWindow.UpdateCurrentWindow();
		}
		return FlashWindow.myWindow;
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x00005330 File Offset: 0x00003530
	public static bool Flash(IntPtr formHandle)
	{
		if (FlashWindow.Win2000OrLater)
		{
			FlashWindow.FLASHWINFO flashwinfo = FlashWindow.Create_FLASHWINFO(formHandle, 15u, uint.MaxValue, 0u);
			return FlashWindow.FlashWindowEx(ref flashwinfo);
		}
		return false;
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x00005358 File Offset: 0x00003558
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

	// Token: 0x060000C6 RID: 198 RVA: 0x000053A0 File Offset: 0x000035A0
	public static bool Flash(IntPtr formHandle, uint count)
	{
		if (FlashWindow.Win2000OrLater)
		{
			FlashWindow.FLASHWINFO flashwinfo = FlashWindow.Create_FLASHWINFO(formHandle, 3u, count, 0u);
			return FlashWindow.FlashWindowEx(ref flashwinfo);
		}
		return false;
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x000053C8 File Offset: 0x000035C8
	public static bool Flash()
	{
		IntPtr windowHandle = FlashWindow.GetWindowHandle();
		Debug.LogFormat("FlashWindow.Flash({0})", new object[]
		{
			windowHandle
		});
		return FlashWindow.Flash(windowHandle);
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x000053FC File Offset: 0x000035FC
	public static bool Start(IntPtr formHandle)
	{
		if (FlashWindow.Win2000OrLater)
		{
			FlashWindow.FLASHWINFO flashwinfo = FlashWindow.Create_FLASHWINFO(formHandle, 3u, uint.MaxValue, 0u);
			return FlashWindow.FlashWindowEx(ref flashwinfo);
		}
		return false;
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x00005424 File Offset: 0x00003624
	public static bool Stop(IntPtr formHandle)
	{
		if (FlashWindow.Win2000OrLater)
		{
			FlashWindow.FLASHWINFO flashwinfo = FlashWindow.Create_FLASHWINFO(formHandle, 0u, uint.MaxValue, 0u);
			return FlashWindow.FlashWindowEx(ref flashwinfo);
		}
		return false;
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x060000CA RID: 202 RVA: 0x0000544B File Offset: 0x0000364B
	private static bool Win2000OrLater
	{
		get
		{
			return Environment.OSVersion.Version.Major >= 5;
		}
	}

	// Token: 0x060000CB RID: 203 RVA: 0x00005464 File Offset: 0x00003664
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

	// Token: 0x040000B1 RID: 177
	private static IntPtr myWindow;

	// Token: 0x040000B2 RID: 178
	private static readonly IntPtr myProcessId = FlashWindow.GetCurrentProcessId();

	// Token: 0x040000B3 RID: 179
	public const uint FLASHW_STOP = 0u;

	// Token: 0x040000B4 RID: 180
	public const uint FLASHW_CAPTION = 1u;

	// Token: 0x040000B5 RID: 181
	public const uint FLASHW_TRAY = 2u;

	// Token: 0x040000B6 RID: 182
	public const uint FLASHW_ALL = 3u;

	// Token: 0x040000B7 RID: 183
	public const uint FLASHW_TIMER = 4u;

	// Token: 0x040000B8 RID: 184
	public const uint FLASHW_TIMERNOFG = 12u;

	// Token: 0x0200002A RID: 42
	// (Invoke) Token: 0x060000CE RID: 206
	private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

	// Token: 0x0200002B RID: 43
	private struct FLASHWINFO
	{
		// Token: 0x040000B9 RID: 185
		public uint cbSize;

		// Token: 0x040000BA RID: 186
		public IntPtr hwnd;

		// Token: 0x040000BB RID: 187
		public uint dwFlags;

		// Token: 0x040000BC RID: 188
		public uint uCount;

		// Token: 0x040000BD RID: 189
		public uint dwTimeout;
	}
}
