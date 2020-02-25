using System;
using UnityEngine;

namespace RoR2.Skills
{
	// Token: 0x020004C3 RID: 1219
	public class ToolbotWeaponSkillDef : SkillDef
	{
		// Token: 0x04001A55 RID: 6741
		public string stanceName;

		// Token: 0x04001A56 RID: 6742
		public string entrySound;

		// Token: 0x04001A57 RID: 6743
		public string entryAnimState;

		// Token: 0x04001A58 RID: 6744
		public string exitAnimState;

		// Token: 0x04001A59 RID: 6745
		public int animatorWeaponIndex;

		// Token: 0x04001A5A RID: 6746
		public GameObject crosshairPrefab;

		// Token: 0x04001A5B RID: 6747
		public AnimationCurve crosshairSpreadCurve;
	}
}
