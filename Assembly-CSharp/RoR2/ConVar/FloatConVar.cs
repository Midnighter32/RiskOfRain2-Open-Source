using System;

namespace RoR2.ConVar
{
	// Token: 0x0200067A RID: 1658
	public class FloatConVar : BaseConVar
	{
		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x060026E6 RID: 9958 RVA: 0x000A9973 File Offset: 0x000A7B73
		// (set) Token: 0x060026E7 RID: 9959 RVA: 0x000A997B File Offset: 0x000A7B7B
		public float value { get; protected set; }

		// Token: 0x060026E8 RID: 9960 RVA: 0x0000972B File Offset: 0x0000792B
		public FloatConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x060026E9 RID: 9961 RVA: 0x000A9984 File Offset: 0x000A7B84
		public override void SetString(string newValue)
		{
			float num;
			if (TextSerialization.TryParseInvariant(newValue, out num) && !float.IsNaN(num) && !float.IsInfinity(num))
			{
				this.value = num;
			}
		}

		// Token: 0x060026EA RID: 9962 RVA: 0x000A99B2 File Offset: 0x000A7BB2
		public override string GetString()
		{
			return TextSerialization.ToStringInvariant(this.value);
		}
	}
}
