using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ClaymanMonster
{
	// Token: 0x020008CD RID: 2253
	public class Leap : BaseState
	{
		// Token: 0x06003281 RID: 12929 RVA: 0x000DA698 File Offset: 0x000D8898
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			Util.PlaySound(Leap.leapSoundString, base.gameObject);
			if (base.characterMotor)
			{
				base.characterMotor.velocity.y = Leap.verticalJumpSpeed;
			}
			this.forwardDirection = Vector3.ProjectOnPlane(base.inputBank.aimDirection, Vector3.up);
			base.characterDirection.moveVector = this.forwardDirection;
			base.PlayCrossfade("Body", "LeapAirLoop", 0.15f);
		}

		// Token: 0x06003282 RID: 12930 RVA: 0x000DA72C File Offset: 0x000D892C
		public override void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			this.animator.SetFloat("Leap.cycle", Mathf.Clamp01(Util.Remap(base.characterMotor.velocity.y, -Leap.verticalJumpSpeed, Leap.verticalJumpSpeed, 1f, 0f)));
			Vector3 velocity = this.forwardDirection * base.characterBody.moveSpeed * Leap.horizontalJumpSpeedCoefficient;
			velocity.y = base.characterMotor.velocity.y;
			base.characterMotor.velocity = velocity;
			base.FixedUpdate();
			if (base.characterMotor.isGrounded && this.stopwatch > Leap.minimumDuration && !this.playedImpact)
			{
				this.playedImpact = true;
				int layerIndex = this.animator.GetLayerIndex("Impact");
				if (layerIndex >= 0)
				{
					this.animator.SetLayerWeight(layerIndex, 1.5f);
					this.animator.PlayInFixedTime("LightImpact", layerIndex, 0f);
				}
				if (base.isAuthority)
				{
					this.outer.SetNextStateToMain();
					return;
				}
			}
		}

		// Token: 0x06003283 RID: 12931 RVA: 0x000DA84D File Offset: 0x000D8A4D
		public override void OnExit()
		{
			base.PlayAnimation("Body", "Idle");
			base.OnExit();
		}

		// Token: 0x04003181 RID: 12673
		public static string leapSoundString;

		// Token: 0x04003182 RID: 12674
		public static float minimumDuration;

		// Token: 0x04003183 RID: 12675
		public static float verticalJumpSpeed;

		// Token: 0x04003184 RID: 12676
		public static float horizontalJumpSpeedCoefficient;

		// Token: 0x04003185 RID: 12677
		private Vector3 forwardDirection;

		// Token: 0x04003186 RID: 12678
		private Animator animator;

		// Token: 0x04003187 RID: 12679
		private float stopwatch;

		// Token: 0x04003188 RID: 12680
		private bool playedImpact;
	}
}
