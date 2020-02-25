using System;
using System.Runtime.InteropServices;
using RoR2;
using RoR2.ConVar;
using UnityEngine;

// Token: 0x0200006D RID: 109
public static class ProcessorAffinity
{
	// Token: 0x060001C1 RID: 449
	[DllImport("kernel32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool SetProcessAffinityMask(IntPtr hProcess, IntPtr dwProcessAffinityMask);

	// Token: 0x060001C2 RID: 450
	[DllImport("kernel32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetProcessAffinityMask(IntPtr hProcess, ref IntPtr lpProcessAffinityMask, ref IntPtr lpSystemAffinityMask);

	// Token: 0x060001C3 RID: 451
	[DllImport("kernel32.dll")]
	private static extern IntPtr GetCurrentProcess();

	// Token: 0x0200006E RID: 110
	private class ProcessorAffinityConVar : BaseConVar
	{
		// Token: 0x060001C4 RID: 452 RVA: 0x0000972B File Offset: 0x0000792B
		private ProcessorAffinityConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00009738 File Offset: 0x00007938
		public override void SetString(string newValue)
		{
			ulong num;
			if (TextSerialization.TryParseInvariant(newValue, out num) && num != 0UL)
			{
				ProcessorAffinity.SetProcessAffinityMask(ProcessorAffinity.GetCurrentProcess(), (IntPtr)((long)num));
				return;
			}
			Debug.LogFormat("Could not accept value \"{0}\"", new object[]
			{
				newValue
			});
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x00009778 File Offset: 0x00007978
		public override string GetString()
		{
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			ProcessorAffinity.GetProcessAffinityMask(ProcessorAffinity.GetCurrentProcess(), ref zero, ref zero2);
			return zero.ToString();
		}

		// Token: 0x040001D6 RID: 470
		public static ProcessorAffinity.ProcessorAffinityConVar instance = new ProcessorAffinity.ProcessorAffinityConVar("processor_affinity", ConVarFlags.Engine, null, "The processor affinity mask.");
	}
}
