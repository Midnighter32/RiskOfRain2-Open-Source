using System;

namespace RoR2.ConVar
{
	// Token: 0x02000685 RID: 1669
	public class FloatConVar : BaseConVar
	{
		// Token: 0x1700032C RID: 812
		// (get) Token: 0x0600253F RID: 9535 RVA: 0x000AED03 File Offset: 0x000ACF03
		// (set) Token: 0x06002540 RID: 9536 RVA: 0x000AED0B File Offset: 0x000ACF0B
		public float value { get; protected set; }

		// Token: 0x06002541 RID: 9537 RVA: 0x00037E38 File Offset: 0x00036038
		public FloatConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x06002542 RID: 9538 RVA: 0x000AED14 File Offset: 0x000ACF14
		public override void SetString(string newValue)
		{
			float num;
			if (TextSerialization.TryParseInvariant(newValue, out num) && !float.IsNaN(num) && !float.IsInfinity(num))
			{
				this.value = num;
			}
		}

		// Token: 0x06002543 RID: 9539 RVA: 0x000AED42 File Offset: 0x000ACF42
		public override string GetString()
		{
			return TextSerialization.ToStringInvariant(this.value);
		}
	}
}
