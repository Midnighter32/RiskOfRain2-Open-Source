using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003FF RID: 1023
	public class RuleCategoryDef
	{
		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x060018CD RID: 6349 RVA: 0x0006AC7C File Offset: 0x00068E7C
		public bool isHidden
		{
			get
			{
				Func<bool> func = this.hiddenTest;
				return func != null && func();
			}
		}

		// Token: 0x04001729 RID: 5929
		public int position;

		// Token: 0x0400172A RID: 5930
		public string displayToken;

		// Token: 0x0400172B RID: 5931
		public string emptyTipToken;

		// Token: 0x0400172C RID: 5932
		public Color color;

		// Token: 0x0400172D RID: 5933
		public List<RuleDef> children = new List<RuleDef>();

		// Token: 0x0400172E RID: 5934
		public Func<bool> hiddenTest;
	}
}
