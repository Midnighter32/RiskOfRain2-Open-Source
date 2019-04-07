using System;

namespace RoR2
{
	// Token: 0x0200022A RID: 554
	[Serializable]
	public struct DisplayRuleGroup
	{
		// Token: 0x06000AAD RID: 2733 RVA: 0x00034C88 File Offset: 0x00032E88
		public void AddDisplayRule(ItemDisplayRule itemDisplayRule)
		{
			int num = ((this.rules != null) ? this.rules.Length : 0) + 1;
			ItemDisplayRule[] array = new ItemDisplayRule[num];
			if (num != 0 && this.rules != null)
			{
				this.rules.CopyTo(array, 0);
			}
			array[num - 1] = itemDisplayRule;
			this.rules = array;
		}

		// Token: 0x04000E26 RID: 3622
		public ItemDisplayRule[] rules;
	}
}
