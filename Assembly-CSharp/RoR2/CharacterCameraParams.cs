using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000101 RID: 257
	[CreateAssetMenu]
	public class CharacterCameraParams : ScriptableObject
	{
		// Token: 0x040004AA RID: 1194
		public float minPitch = -70f;

		// Token: 0x040004AB RID: 1195
		public float maxPitch = 70f;

		// Token: 0x040004AC RID: 1196
		public float wallCushion = 0.1f;

		// Token: 0x040004AD RID: 1197
		public float pivotVerticalOffset = 1.6f;

		// Token: 0x040004AE RID: 1198
		public Vector3 standardLocalCameraPos = new Vector3(0f, 0f, -5f);
	}
}
