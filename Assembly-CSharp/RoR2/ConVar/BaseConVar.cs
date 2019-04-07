using System;

namespace RoR2.ConVar
{
	// Token: 0x02000682 RID: 1666
	public abstract class BaseConVar
	{
		// Token: 0x06002531 RID: 9521 RVA: 0x000AEC28 File Offset: 0x000ACE28
		protected BaseConVar(string name, ConVarFlags flags, string defaultValue, string helpText)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.name = name;
			this.flags = flags;
			this.defaultValue = defaultValue;
			if (helpText == null)
			{
				throw new ArgumentNullException("helpText");
			}
			this.helpText = helpText;
		}

		// Token: 0x06002532 RID: 9522
		public abstract void SetString(string newValue);

		// Token: 0x06002533 RID: 9523
		public abstract string GetString();

		// Token: 0x0400284E RID: 10318
		public string name;

		// Token: 0x0400284F RID: 10319
		public ConVarFlags flags;

		// Token: 0x04002850 RID: 10320
		public string defaultValue;

		// Token: 0x04002851 RID: 10321
		public string helpText;
	}
}
