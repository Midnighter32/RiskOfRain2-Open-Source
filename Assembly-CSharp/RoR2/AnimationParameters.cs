using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000BD RID: 189
	public static class AnimationParameters
	{
		// Token: 0x0400033B RID: 827
		public static readonly int isMoving = Animator.StringToHash("isMoving");

		// Token: 0x0400033C RID: 828
		public static readonly int turnAngle = Animator.StringToHash("turnAngle");

		// Token: 0x0400033D RID: 829
		public static readonly int isGrounded = Animator.StringToHash("isGrounded");

		// Token: 0x0400033E RID: 830
		public static readonly int mainRootPlaybackRate = Animator.StringToHash("mainRootPlaybackRate");

		// Token: 0x0400033F RID: 831
		public static readonly int forwardSpeed = Animator.StringToHash("forwardSpeed");

		// Token: 0x04000340 RID: 832
		public static readonly int rightSpeed = Animator.StringToHash("rightSpeed");

		// Token: 0x04000341 RID: 833
		public static readonly int upSpeed = Animator.StringToHash("upSpeed");

		// Token: 0x04000342 RID: 834
		public static readonly int walkSpeed = Animator.StringToHash("walkSpeed");

		// Token: 0x04000343 RID: 835
		public static readonly int isSprinting = Animator.StringToHash("isSprinting");

		// Token: 0x04000344 RID: 836
		public static readonly int aimWeight = Animator.StringToHash("aimWeight");
	}
}
