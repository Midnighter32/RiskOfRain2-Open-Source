using System;

namespace RoR2
{
	// Token: 0x020001B6 RID: 438
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class ConCommandAttribute : SearchableAttribute
	{
		// Token: 0x040009BE RID: 2494
		public string commandName;

		// Token: 0x040009BF RID: 2495
		public ConVarFlags flags;

		// Token: 0x040009C0 RID: 2496
		public string helpText = "";
	}
}
