using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x02000708 RID: 1800
	public class FlyState : BaseState
	{
		// Token: 0x060029F8 RID: 10744 RVA: 0x000B0268 File Offset: 0x000AE468
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

		// Token: 0x060029F9 RID: 10745 RVA: 0x000B02F8 File Offset: 0x000AE4F8
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x060029FA RID: 10746 RVA: 0x000B0300 File Offset: 0x000AE500
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

		// Token: 0x040025C1 RID: 9665
		private Animator modelAnimator;

		// Token: 0x040025C2 RID: 9666
		private bool skill1InputReceived;

		// Token: 0x040025C3 RID: 9667
		private bool skill2InputReceived;

		// Token: 0x040025C4 RID: 9668
		private bool skill3InputReceived;

		// Token: 0x040025C5 RID: 9669
		private bool skill4InputReceived;

		// Token: 0x040025C6 RID: 9670
		private bool hasPivotPitchLayer;

		// Token: 0x040025C7 RID: 9671
		private bool hasPivotYawLayer;

		// Token: 0x040025C8 RID: 9672
		private bool hasPivotRollLayer;

		// Token: 0x040025C9 RID: 9673
		private static readonly int pivotPitchCycle = Animator.StringToHash("pivotPitchCycle");

		// Token: 0x040025CA RID: 9674
		private static readonly int pivotYawCycle = Animator.StringToHash("pivotYawCycle");

		// Token: 0x040025CB RID: 9675
		private static readonly int pivotRollCycle = Animator.StringToHash("pivotRollCycle");

		// Token: 0x040025CC RID: 9676
		private static readonly int flyRate = Animator.StringToHash("fly.rate");
	}
}
