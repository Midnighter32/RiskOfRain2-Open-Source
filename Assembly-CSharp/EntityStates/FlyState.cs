using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000B1 RID: 177
	internal class FlyState : BaseState
	{
		// Token: 0x0600037E RID: 894 RVA: 0x0000DD40 File Offset: 0x0000BF40
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
			base.PlayAnimation("Body", "Idle");
			if (this.modelAnimator)
			{
				this.hasPivotPitchLayer = (this.modelAnimator.GetLayerIndex("PivotPitch") != -1);
				this.hasPivotYawLayer = (this.modelAnimator.GetLayerIndex("PivotYaw") != -1);
				this.hasPivotRollLayer = (this.modelAnimator.GetLayerIndex("PivotRoll") != -1);
			}
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0000DDD0 File Offset: 0x0000BFD0
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0000DDD8 File Offset: 0x0000BFD8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.rigidbodyDirection)
			{
				Quaternion rotation = base.transform.rotation;
				Quaternion rhs = Util.QuaternionSafeLookRotation(base.rigidbodyDirection.aimDirection);
				Quaternion quaternion = Quaternion.Inverse(rotation) * rhs;
				if (this.modelAnimator)
				{
					if (this.hasPivotPitchLayer)
					{
						this.modelAnimator.SetFloat(FlyState.pivotPitchCycle, Mathf.Clamp01(Util.Remap(quaternion.x * Mathf.Sign(quaternion.w), -1f, 1f, 0f, 1f)), 1f, Time.fixedDeltaTime);
					}
					if (this.hasPivotYawLayer)
					{
						this.modelAnimator.SetFloat(FlyState.pivotYawCycle, Mathf.Clamp01(Util.Remap(quaternion.y * Mathf.Sign(quaternion.w), -1f, 1f, 0f, 1f)), 1f, Time.fixedDeltaTime);
					}
					if (this.hasPivotRollLayer)
					{
						this.modelAnimator.SetFloat(FlyState.pivotRollCycle, Mathf.Clamp01(Util.Remap(quaternion.z * Mathf.Sign(quaternion.w), -1f, 1f, 0f, 1f)), 1f, Time.fixedDeltaTime);
					}
				}
			}
			if (base.isAuthority)
			{
				if (base.inputBank)
				{
					if (base.rigidbodyMotor)
					{
						base.rigidbodyMotor.moveVector = base.inputBank.moveVector * base.characterBody.moveSpeed;
						if (this.modelAnimator)
						{
							this.modelAnimator.SetFloat(FlyState.flyRate, Vector3.Magnitude(base.rigidbodyMotor.rigid.velocity));
						}
					}
					if (base.rigidbodyDirection)
					{
						base.rigidbodyDirection.aimDirection = base.GetAimRay().direction;
					}
					this.skill1InputReceived = base.inputBank.skill1.down;
					this.skill2InputReceived = base.inputBank.skill2.down;
					this.skill3InputReceived = base.inputBank.skill3.down;
					this.skill4InputReceived = base.inputBank.skill4.down;
				}
				if (base.skillLocator)
				{
					if (this.skill1InputReceived && base.skillLocator.primary)
					{
						base.skillLocator.primary.ExecuteIfReady();
					}
					if (this.skill2InputReceived && base.skillLocator.secondary)
					{
						base.skillLocator.secondary.ExecuteIfReady();
					}
					if (this.skill3InputReceived && base.skillLocator.utility)
					{
						base.skillLocator.utility.ExecuteIfReady();
					}
					if (this.skill4InputReceived && base.skillLocator.special)
					{
						base.skillLocator.special.ExecuteIfReady();
					}
				}
			}
		}

		// Token: 0x0400032E RID: 814
		private Animator modelAnimator;

		// Token: 0x0400032F RID: 815
		private bool skill1InputReceived;

		// Token: 0x04000330 RID: 816
		private bool skill2InputReceived;

		// Token: 0x04000331 RID: 817
		private bool skill3InputReceived;

		// Token: 0x04000332 RID: 818
		private bool skill4InputReceived;

		// Token: 0x04000333 RID: 819
		private bool hasPivotPitchLayer;

		// Token: 0x04000334 RID: 820
		private bool hasPivotYawLayer;

		// Token: 0x04000335 RID: 821
		private bool hasPivotRollLayer;

		// Token: 0x04000336 RID: 822
		private static readonly int pivotPitchCycle = Animator.StringToHash("pivotPitchCycle");

		// Token: 0x04000337 RID: 823
		private static readonly int pivotYawCycle = Animator.StringToHash("pivotYawCycle");

		// Token: 0x04000338 RID: 824
		private static readonly int pivotRollCycle = Animator.StringToHash("pivotRollCycle");

		// Token: 0x04000339 RID: 825
		private static readonly int flyRate = Animator.StringToHash("fly.rate");
	}
}
