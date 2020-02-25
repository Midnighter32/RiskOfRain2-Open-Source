using System;

namespace RoR2.ConVar
{
	// Token: 0x02000679 RID: 1657
	public class IntConVar : BaseConVar
	{
		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x060026E1 RID: 9953 RVA: 0x000A9936 File Offset: 0x000A7B36
		// (set) Token: 0x060026E2 RID: 9954 RVA: 0x000A993E File Offset: 0x000A7B3E
		public int value { get; protected set; }

		// Token: 0x060026E3 RID: 9955 RVA: 0x0000972B File Offset: 0x0000792B
		public IntConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x060026E4 RID: 9956 RVA: 0x000A9948 File Offset: 0x000A7B48
		public override void SetString(string newValue)
		{
			int value;
			if (TextSerialization.TryParseInvariant(newValue, out value))
			{
				this.value = value;
			}
		}

		// Token: 0x060026E5 RID: 9957 RVA: 0x000A9966 File Offset: 0x000A7B66
		public override string GetString()
		{
			return TextSerialization.ToStringInvariant(this.value);
		}
	}
}
