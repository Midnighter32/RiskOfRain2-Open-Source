using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000160 RID: 352
	public class BodyAnimatorSmoothingParameters : MonoBehaviour
	{
		// Token: 0x040006CE RID: 1742
		public BodyAnimatorSmoothingParameters.SmoothingParameters smoothingParameters;

		// Token: 0x040006CF RID: 1743
		public static BodyAnimatorSmoothingParameters.SmoothingParameters defaultParameters = new BodyAnimatorSmoothingParameters.SmoothingParameters
		{
			walkMagnitudeSmoothDamp = 0.2f,
			walkAngleSmoothDamp = 0.2f,
			forwardSpeedSmoothDamp = 0.1f,
			rightSpeedSmoothDamp = 0.1f,
			intoJumpTransitionTime = 0.05f,
			turnAngleSmoothDamp = 1f
		};

		// Token: 0x02000161 RID: 353
		[Serializable]
		public struct SmoothingParameters
		{
			// Token: 0x040006D0 RID: 1744
			public float walkMagnitudeSmoothDamp;

			// Token: 0x040006D1 RID: 1745
			public float walkAngleSmoothDamp;

			// Token: 0x040006D2 RID: 1746
			public float forwardSpeedSmoothDamp;

			// Token: 0x040006D3 RID: 1747
			public float rightSpeedSmoothDamp;

			// Token: 0x040006D4 RID: 1748
			public float intoJumpTransitionTime;

			// Token: 0x040006D5 RID: 1749
			public float turnAngleSmoothDamp;
		}
	}
}
