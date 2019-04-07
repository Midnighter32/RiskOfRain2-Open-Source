using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000229 RID: 553
	[Serializable]
	public struct ItemDisplayRule
	{
		// Token: 0x04000E1F RID: 3615
		public ItemDisplayRuleType ruleType;

		// Token: 0x04000E20 RID: 3616
		public GameObject followerPrefab;

		// Token: 0x04000E21 RID: 3617
		public string childName;

		// Token: 0x04000E22 RID: 3618
		public Vector3 localPos;

		// Token: 0x04000E23 RID: 3619
		public Vector3 localAngles;

		// Token: 0x04000E24 RID: 3620
		public Vector3 localScale;

		// Token: 0x04000E25 RID: 3621
		public LimbFlags limbMask;
	}
}
