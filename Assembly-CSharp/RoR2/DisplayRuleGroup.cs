using System;

namespace RoR2
{
	// Token: 0x0200010B RID: 267
	[Serializable]
	public struct DisplayRuleGroup : IEquatable<DisplayRuleGroup>
	{
		// Token: 0x060004FF RID: 1279 RVA: 0x00014280 File Offset: 0x00012480
		public void AddDisplayRule(ItemDisplayRule itemDisplayRule)
		{
			if (this.rules == null)
			{
				this.rules = Array.Empty<ItemDisplayRule>();
			}
			HGArrayUtilities.ArrayAppend<ItemDisplayRule>(ref this.rules, ref itemDisplayRule);
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x000142A2 File Offset: 0x000124A2
		public bool Equals(DisplayRuleGroup other)
		{
			return HGArrayUtilities.SequenceEquals<ItemDisplayRule>(this.rules, other.rules);
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x000142B8 File Offset: 0x000124B8
		public override bool Equals(object obj)
		{
			if (obj is DisplayRuleGroup)
			{
				DisplayRuleGroup other = (DisplayRuleGroup)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x000142DF File Offset: 0x000124DF
		public override int GetHashCode()
		{
			if (this.rules == null)
			{
				return 0;
			}
			return this.rules.GetHashCode();
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000503 RID: 1283 RVA: 0x000142F6 File Offset: 0x000124F6
		public bool isEmpty
		{
			get
			{
				return this.rules == null;
			}
		}

		// Token: 0x040004D5 RID: 1237
		public static readonly DisplayRuleGroup empty = new DisplayRuleGroup
		{
			rules = null
		};

		// Token: 0x040004D6 RID: 1238
		public ItemDisplayRule[] rules;
	}
}
