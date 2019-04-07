using System;

namespace RoR2.ConVar
{
	// Token: 0x02000683 RID: 1667
	public class BoolConVar : BaseConVar
	{
		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06002534 RID: 9524 RVA: 0x000AEC75 File Offset: 0x000ACE75
		// (set) Token: 0x06002535 RID: 9525 RVA: 0x000AEC7D File Offset: 0x000ACE7D
		public bool value { get; protected set; }

		// Token: 0x06002536 RID: 9526 RVA: 0x00037E38 File Offset: 0x00036038
		public BoolConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x06002537 RID: 9527 RVA: 0x000AEC86 File Offset: 0x000ACE86
		public void SetBool(bool newValue)
		{
			this.value = newValue;
		}

		// Token: 0x06002538 RID: 9528 RVA: 0x000AEC90 File Offset: 0x000ACE90
		public override void SetString(string newValue)
		{
			int num;
			if (TextSerialization.TryParseInvariant(newValue, out num))
			{
				this.value = (num != 0);
			}
		}

		// Token: 0x06002539 RID: 9529 RVA: 0x000AECB1 File Offset: 0x000ACEB1
		public override string GetString()
		{
			if (!this.value)
			{
				return "0";
			}
			return "1";
		}
	}
}
