using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SteamAPIValidator
{
	// Token: 0x0200009E RID: 158
	public static class SteamApiValidator
	{
		// Token: 0x0600033A RID: 826
		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary(string dllToLoad);

		// Token: 0x0600033B RID: 827
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		// Token: 0x0600033C RID: 828
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern uint GetModuleFileName([In] IntPtr hModule, [Out] StringBuilder lpFilename, [MarshalAs(UnmanagedType.U4)] [In] int nSize);

		// Token: 0x0600033D RID: 829 RVA: 0x0000CB50 File Offset: 0x0000AD50
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
				if (SteamApiValidator.GetModuleFileName(intPtr, stringBuilder, 32767) > 0U)
				{
					return SteamApiValidator.CheckIfValveSigned(stringBuilder.ToString());
				}
			}
			return false;
		}

		// Token: 0x0600033E RID: 830 RVA: 0x0000CBD0 File Offset: 0x0000ADD0
		public static bool IsValidSteamClientDll()
		{
			IntPtr moduleHandle = SteamApiValidator.GetModuleHandle(Environment.Is64BitProcess ? "steamclient64.dll" : "steamclient.dll");
			if (moduleHandle != IntPtr.Zero)
			{
				StringBuilder stringBuilder = new StringBuilder(32767);
				if (SteamApiValidator.GetModuleFileName(moduleHandle, stringBuilder, 32767) > 0U)
				{
					return SteamApiValidator.CheckIfValveSigned(stringBuilder.ToString());
				}
			}
			return false;
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0000CC2C File Offset: 0x0000AE2C
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

		// Token: 0x040002C3 RID: 707
		private const int MAX_PATH_SIZE = 32767;
	}
}
