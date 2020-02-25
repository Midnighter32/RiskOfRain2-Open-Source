using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000336 RID: 822
	[RequireComponent(typeof(Animator))]
	internal class SkateSparks : MonoBehaviour
	{
		// Token: 0x06001391 RID: 5009 RVA: 0x00053935 File Offset: 0x00051B35
		private void Awake()
		{
			this.animator = base.GetComponent<Animator>();
		}

		// Token: 0x06001392 RID: 5010 RVA: 0x00053944 File Offset: 0x00051B44
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

		// Token: 0x04001259 RID: 4697
		public float sparkFactor = 1f;

		// Token: 0x0400125A RID: 4698
		public ParticleSystem leftParticleSystem;

		// Token: 0x0400125B RID: 4699
		public ParticleSystem rightParticleSystem;

		// Token: 0x0400125C RID: 4700
		private Animator animator;

		// Token: 0x0400125D RID: 4701
		private static readonly int forwardSpeedParam = Animator.StringToHash("forwardSpeed");

		// Token: 0x0400125E RID: 4702
		private static readonly int rightSpeedParam = Animator.StringToHash("rightSpeed");

		// Token: 0x0400125F RID: 4703
		private static readonly int isGroundedParam = Animator.StringToHash("isGrounded");

		// Token: 0x04001260 RID: 4704
		private float previousForwardSpeed;

		// Token: 0x04001261 RID: 4705
		private float previousRightSpeed;

		// Token: 0x04001262 RID: 4706
		private bool previousIsGrounded = true;

		// Token: 0x04001263 RID: 4707
		private float sparkAccumulator;
	}
}
