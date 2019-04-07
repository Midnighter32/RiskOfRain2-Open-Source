using System;

namespace RoR2.ConVar
{
	// Token: 0x02000686 RID: 1670
	public class StringConVar : BaseConVar
	{
		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06002544 RID: 9540 RVA: 0x000AED4F File Offset: 0x000ACF4F
		// (set) Token: 0x06002545 RID: 9541 RVA: 0x000AED57 File Offset: 0x000ACF57
		public string value { get; protected set; }

		// Token: 0x06002546 RID: 9542 RVA: 0x00037E38 File Offset: 0x00036038
		public StringConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x06002547 RID: 9543 RVA: 0x000AED60 File Offset: 0x000ACF60
		public override void SetString(string newValue)
		{
			this.value = newValue;
		}

		// Token: 0x06002548 RID: 9544 RVA: 0x000AED69 File Offset: 0x000ACF69
		public override string GetString()
		{
			return this.value;
		}
	}
}
