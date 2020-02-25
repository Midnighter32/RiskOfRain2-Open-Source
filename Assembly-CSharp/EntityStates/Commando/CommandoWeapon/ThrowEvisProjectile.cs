using System;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008B8 RID: 2232
	public class ThrowEvisProjectile : FireFMJ
	{
		// Token: 0x0600320C RID: 12812 RVA: 0x000D821E File Offset: 0x000D641E
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.characterMotor)
			{
				base.characterMotor.velocity.y = Mathf.Max(base.characterMotor.velocity.y, ThrowEvisProjectile.shortHopVelocity);
			}
		}

		// Token: 0x0600320D RID: 12813 RVA: 0x000D8260 File Offset: 0x000D6460
		protected override void PlayAnimation(float duration)
		{
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				bool @bool = modelAnimator.GetBool("isMoving");
				bool bool2 = modelAnimator.GetBool("isGrounded");
				if (@bool || !bool2)
				{
					base.PlayAnimation("Gesture, Additive", "GroundLight3", "GroundLight.playbackRate", duration);
					base.PlayAnimation("Gesture, Override", "GroundLight3", "GroundLight.playbackRate", duration);
					return;
				}
				base.PlayAnimation("FullBody, Override", "GroundLight3", "GroundLight.playbackRate", duration);
			}
		}

		// Token: 0x040030C4 RID: 12484
		public static float shortHopVelocity;
	}
}
