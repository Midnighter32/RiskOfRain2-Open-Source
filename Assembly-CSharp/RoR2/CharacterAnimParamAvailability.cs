using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000DD RID: 221
	public struct CharacterAnimParamAvailability
	{
		// Token: 0x0600044D RID: 1101 RVA: 0x000119B0 File Offset: 0x0000FBB0
		public static CharacterAnimParamAvailability FromAnimator(Animator animator)
		{
			return new CharacterAnimParamAvailability
			{
				isMoving = Util.HasAnimationParameter(AnimationParameters.isMoving, animator),
				turnAngle = Util.HasAnimationParameter(AnimationParameters.turnAngle, animator),
				isGrounded = Util.HasAnimationParameter(AnimationParameters.isGrounded, animator),
				mainRootPlaybackRate = Util.HasAnimationParameter(AnimationParameters.mainRootPlaybackRate, animator),
				forwardSpeed = Util.HasAnimationParameter(AnimationParameters.forwardSpeed, animator),
				rightSpeed = Util.HasAnimationParameter(AnimationParameters.rightSpeed, animator),
				upSpeed = Util.HasAnimationParameter(AnimationParameters.upSpeed, animator),
				walkSpeed = Util.HasAnimationParameter(AnimationParameters.walkSpeed, animator),
				isSprinting = Util.HasAnimationParameter(AnimationParameters.isSprinting, animator)
			};
		}

		// Token: 0x0400041B RID: 1051
		public bool isMoving;

		// Token: 0x0400041C RID: 1052
		public bool turnAngle;

		// Token: 0x0400041D RID: 1053
		public bool isGrounded;

		// Token: 0x0400041E RID: 1054
		public bool mainRootPlaybackRate;

		// Token: 0x0400041F RID: 1055
		public bool forwardSpeed;

		// Token: 0x04000420 RID: 1056
		public bool rightSpeed;

		// Token: 0x04000421 RID: 1057
		public bool upSpeed;

		// Token: 0x04000422 RID: 1058
		public bool walkSpeed;

		// Token: 0x04000423 RID: 1059
		public bool isSprinting;
	}
}
