using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SteamAPIValidator
{
	// Token: 0x02000099 RID: 153
	public static class SteamApiValidator
	{
		// Token: 0x060002FE RID: 766
		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary(string dllToLoad);

		// Token: 0x060002FF RID: 767
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		// Token: 0x06000300 RID: 768
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern uint GetModuleFileName([In] IntPtr hModule, [Out] StringBuilder lpFilename, [MarshalAs(UnmanagedType.U4)] [In] int nSize);

		// Token: 0x06000301 RID: 769 RVA: 0x0000C0A8 File Offset: 0x0000A2A8
		public static bool IsValidSteamApiDll()
		{
			string text = Environment.Is64BitProcess ? "steam_api64.dll" : "steam_api.dll";
			IntPtr intPtr = SteamApiValidator.GetModuleHandle(text);
			if (intPtr == IntPtr.Zero)
			{
				intPtr = SteamApiValidator.LoadLibrary(text);
			}
			if (intPtr == IntPtr.Zero)
			{
				return false;
			}
			if (intPtr != IntPtr.Zero)
			{
				StringBuilder stringBuilder = new StringBuilder(32767);
				if (SteamApiValidator.GetModuleFileName(intPtr, stringBuilder, 32767) > 0u)
				{
					return SteamApiValidator.CheckIfValveSigned(stringBuilder.ToString());
				}
			}
			return false;
		}

		// Token: 0x06000302 RID: 770 RVA: 0x0000C128 File Offset: 0x0000A328
		public static bool IsValidSteamClientDll()
		{
			IntPtr moduleHandle = SteamApiValidator.GetModuleHandle(Environment.Is64BitProcess ? "steamclient64.dll" : "steamclient.dll");
			if (moduleHandle != IntPtr.Zero)
			{
				StringBuilder stringBuilder = new StringBuilder(32767);
				if (SteamApiValidator.GetModuleFileName(moduleHandle, stringBuilder, 32767) > 0u)
				{
					return SteamApiValidator.CheckIfValveSigned(stringBuilder.ToString());
				}
			}
			return false;
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0000C184 File Offset: 0x0000A384
		private static bool CheckIfValveSigned(string filePath)
		{
			bool result;
			try
			{
				IntPtr zero = IntPtr.Zero;
				IntPtr zero2 = IntPtr.Zero;
				IntPtr zero3 = IntPtr.Zero;
				int num;
				int num2;
				int num3;
				if (!WinCrypt.CryptQueryObject(1, Marshal.StringToHGlobalUni(filePath), 16382, 14, 0, out num, out num2, out num3, ref zero, ref zero2, ref zero3))
				{
					result = false;
				}
				else
				{
					result = (num2 == 10);
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x040002B8 RID: 696
		private const int MAX_PATH_SIZE = 32767;
	}
}
