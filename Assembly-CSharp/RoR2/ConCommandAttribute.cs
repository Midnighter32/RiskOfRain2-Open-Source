using System;

namespace RoR2
{
	// Token: 0x020002A7 RID: 679
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class ConCommandAttribute : Attribute
	{
		// Token: 0x040011D5 RID: 4565
		public string commandName;

		// Token: 0x040011D6 RID: 4566
		public ConVarFlags flags;

		// Token: 0x040011D7 RID: 4567
		public string helpText = "";
	}
}
