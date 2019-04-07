using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200026A RID: 618
	public class BodyAnimatorSmoothingParameters : MonoBehaviour
	{
		// Token: 0x04000F86 RID: 3974
		public BodyAnimatorSmoothingParameters.SmoothingParameters smoothingParameters;

		// Token: 0x04000F87 RID: 3975
		public static BodyAnimatorSmoothingParameters.SmoothingParameters defaultParameters = new BodyAnimatorSmoothingParameters.SmoothingParameters
		{
			walkMagnitudeSmoothDamp = 0.2f,
			walkAngleSmoothDamp = 0.2f,
			forwardSpeedSmoothDamp = 0.1f,
			rightSpeedSmoothDamp = 0.1f,
			intoJumpTransitionTime = 0.05f,
			turnAngleSmoothDamp = 1f
		};

		// Token: 0x0200026B RID: 619
		[Serializable]
		public struct SmoothingParameters
		{
			// Token: 0x04000F88 RID: 3976
			public float walkMagnitudeSmoothDamp;

			// Token: 0x04000F89 RID: 3977
			public float walkAngleSmoothDamp;

			// Token: 0x04000F8A RID: 3978
			public float forwardSpeedSmoothDamp;

			// Token: 0x04000F8B RID: 3979
			public float rightSpeedSmoothDamp;

			// Token: 0x04000F8C RID: 3980
			public float intoJumpTransitionTime;

			// Token: 0x04000F8D RID: 3981
			public float turnAngleSmoothDamp;
		}
	}
}
