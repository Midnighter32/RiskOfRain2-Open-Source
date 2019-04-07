using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000221 RID: 545
	[CreateAssetMenu]
	public class CharacterCameraParams : ScriptableObject
	{
		// Token: 0x04000E04 RID: 3588
		public float minPitch = -70f;

		// Token: 0x04000E05 RID: 3589
		public float maxPitch = 70f;

		// Token: 0x04000E06 RID: 3590
		public float wallCushion = 0.1f;

		// Token: 0x04000E07 RID: 3591
		public float pivotVerticalOffset = 1.6f;

		// Token: 0x04000E08 RID: 3592
		public Vector3 standardLocalCameraPos = new Vector3(0f, 0f, -5f);
	}
}
