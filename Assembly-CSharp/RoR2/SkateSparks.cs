using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003E2 RID: 994
	[RequireComponent(typeof(Animator))]
	internal class SkateSparks : MonoBehaviour
	{
		// Token: 0x060015AE RID: 5550 RVA: 0x00067D42 File Offset: 0x00065F42
		private void Awake()
		{
			this.animator = base.GetComponent<Animator>();
		}

		// Token: 0x060015AF RID: 5551 RVA: 0x00067D50 File Offset: 0x00065F50
		private void FixedUpdate()
		{
			float @float = this.animator.GetFloat(SkateSparks.forwardSpeedParam);
			float float2 = this.animator.GetFloat(SkateSparks.rightSpeedParam);
			bool @bool = this.animator.GetBool(SkateSparks.isGroundedParam);
			float num = (float2 - this.previousRightSpeed) * Time.fixedDeltaTime;
			float num2 = (@float - this.previousForwardSpeed) * Time.fixedDeltaTime;
			float num3 = Mathf.Sqrt(num * num + num2 * num2);
			this.sparkAccumulator += num3 * this.sparkFactor;
			if (@bool != this.previousIsGrounded)
			{
				this.sparkAccumulator += 2f * this.sparkFactor;
			}
			if (this.sparkAccumulator > 0f)
			{
				int num4 = Mathf.FloorToInt(this.sparkAccumulator);
				if (@bool)
				{
					if (this.leftParticleSystem)
					{
						this.leftParticleSystem.Emit(num4);
					}
					if (this.rightParticleSystem)
					{
						this.rightParticleSystem.Emit(num4);
					}
				}
				this.sparkAccumulator -= (float)num4;
			}
			this.previousForwardSpeed = @float;
			this.previousRightSpeed = float2;
			this.previousIsGrounded = @bool;
		}

		// Token: 0x04001906 RID: 6406
		public float sparkFactor = 1f;

		// Token: 0x04001907 RID: 6407
		public ParticleSystem leftParticleSystem;

		// Token: 0x04001908 RID: 6408
		public ParticleSystem rightParticleSystem;

		// Token: 0x04001909 RID: 6409
		private Animator animator;

		// Token: 0x0400190A RID: 6410
		private static readonly int forwardSpeedParam = Animator.StringToHash("forwardSpeed");

		// Token: 0x0400190B RID: 6411
		private static readonly int rightSpeedParam = Animator.StringToHash("rightSpeed");

		// Token: 0x0400190C RID: 6412
		private static readonly int isGroundedParam = Animator.StringToHash("isGrounded");

		// Token: 0x0400190D RID: 6413
		private float previousForwardSpeed;

		// Token: 0x0400190E RID: 6414
		private float previousRightSpeed;

		// Token: 0x0400190F RID: 6415
		private bool previousIsGrounded = true;

		// Token: 0x04001910 RID: 6416
		private float sparkAccumulator;
	}
}
