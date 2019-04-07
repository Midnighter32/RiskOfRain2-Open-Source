using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ClaymanMonster
{
	// Token: 0x020001B3 RID: 435
	public class Leap : BaseState
	{
		// Token: 0x06000886 RID: 2182 RVA: 0x0002AB64 File Offset: 0x00028D64
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

		// Token: 0x06000887 RID: 2183 RVA: 0x0002ABF8 File Offset: 0x00028DF8
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

		// Token: 0x06000888 RID: 2184 RVA: 0x0002AD19 File Offset: 0x00028F19
		public override void OnExit()
		{
			base.PlayAnimation("Body", "Idle");
			base.OnExit();
		}

		// Token: 0x04000B60 RID: 2912
		public static string leapSoundString;

		// Token: 0x04000B61 RID: 2913
		public static float minimumDuration;

		// Token: 0x04000B62 RID: 2914
		public static float verticalJumpSpeed;

		// Token: 0x04000B63 RID: 2915
		public static float horizontalJumpSpeedCoefficient;

		// Token: 0x04000B64 RID: 2916
		private Vector3 forwardDirection;

		// Token: 0x04000B65 RID: 2917
		private Animator animator;

		// Token: 0x04000B66 RID: 2918
		private float stopwatch;

		// Token: 0x04000B67 RID: 2919
		private bool playedImpact;
	}
}
