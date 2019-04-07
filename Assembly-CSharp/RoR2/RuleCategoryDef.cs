using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200047D RID: 1149
	public class RuleCategoryDef
	{
		// Token: 0x17000264 RID: 612
		// (get) Token: 0x060019AC RID: 6572 RVA: 0x0007A8B6 File Offset: 0x00078AB6
		public bool isHidden
		{
			get
			{
				Func<bool> func = this.hiddenTest;
				return func != null && func();
			}
		}

		// Token: 0x04001CFE RID: 7422
		public int position;

		// Token: 0x04001CFF RID: 7423
		public string displayToken;

		// Token: 0x04001D00 RID: 7424
		public string emptyTipToken;

		// Token: 0x04001D01 RID: 7425
		public Color color;

		// Token: 0x04001D02 RID: 7426
		public List<RuleDef> children = new List<RuleDef>();

		// Token: 0x04001D03 RID: 7427
		public Func<bool> hiddenTest;
	}
}
