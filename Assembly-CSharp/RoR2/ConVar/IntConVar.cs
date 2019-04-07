using System;

namespace RoR2.ConVar
{
	// Token: 0x02000684 RID: 1668
	public class IntConVar : BaseConVar
	{
		// Token: 0x1700032B RID: 811
		// (get) Token: 0x0600253A RID: 9530 RVA: 0x000AECC6 File Offset: 0x000ACEC6
		// (set) Token: 0x0600253B RID: 9531 RVA: 0x000AECCE File Offset: 0x000ACECE
		public int value { get; protected set; }

		// Token: 0x0600253C RID: 9532 RVA: 0x00037E38 File Offset: 0x00036038
		public IntConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x0600253D RID: 9533 RVA: 0x000AECD8 File Offset: 0x000ACED8
		public override void SetString(string newValue)
		{
			int value;
			if (TextSerialization.TryParseInvariant(newValue, out value))
			{
				this.value = value;
			}
		}

		// Token: 0x0600253E RID: 9534 RVA: 0x000AECF6 File Offset: 0x000ACEF6
		public override string GetString()
		{
			return TextSerialization.ToStringInvariant(this.value);
		}
	}
}
