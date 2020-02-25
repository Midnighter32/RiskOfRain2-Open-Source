using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200010E RID: 270
	[CreateAssetMenu]
	public class RoachParams : ScriptableObject
	{
		// Token: 0x040004DB RID: 1243
		public float reorientTimerMin = 0.2f;

		// Token: 0x040004DC RID: 1244
		public float reorientTimerMax = 0.5f;

		// Token: 0x040004DD RID: 1245
		public float turnSpeed = 72f;

		// Token: 0x040004DE RID: 1246
		public float acceleration = 400f;

		// Token: 0x040004DF RID: 1247
		public float maxSpeed = 100f;

		// Token: 0x040004E0 RID: 1248
		public float backupDuration = 0.1f;

		// Token: 0x040004E1 RID: 1249
		public float wiggle = 720f;

		// Token: 0x040004E2 RID: 1250
		public float stepSize = 0.1f;

		// Token: 0x040004E3 RID: 1251
		public float minSimulationDuration = 3f;

		// Token: 0x040004E4 RID: 1252
		public float maxSimulationDuration = 7f;

		// Token: 0x040004E5 RID: 1253
		public float chanceToFinishOnBump = 0.5f;

		// Token: 0x040004E6 RID: 1254
		public float keyframeInterval = 0.06666667f;

		// Token: 0x040004E7 RID: 1255
		public float minReactionTime;

		// Token: 0x040004E8 RID: 1256
		public float maxReactionTime = 0.2f;

		// Token: 0x040004E9 RID: 1257
		public GameObject roachPrefab;
	}
}
