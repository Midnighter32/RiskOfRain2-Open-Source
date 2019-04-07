using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000211 RID: 529
	public struct CharacterAnimParamAvailability
	{
		// Token: 0x06000A50 RID: 2640 RVA: 0x00033A5C File Offset: 0x00031C5C
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

		// Token: 0x04000DC2 RID: 3522
		public bool isMoving;

		// Token: 0x04000DC3 RID: 3523
		public bool turnAngle;

		// Token: 0x04000DC4 RID: 3524
		public bool isGrounded;

		// Token: 0x04000DC5 RID: 3525
		public bool mainRootPlaybackRate;

		// Token: 0x04000DC6 RID: 3526
		public bool forwardSpeed;

		// Token: 0x04000DC7 RID: 3527
		public bool rightSpeed;

		// Token: 0x04000DC8 RID: 3528
		public bool upSpeed;

		// Token: 0x04000DC9 RID: 3529
		public bool walkSpeed;

		// Token: 0x04000DCA RID: 3530
		public bool isSprinting;
	}
}
