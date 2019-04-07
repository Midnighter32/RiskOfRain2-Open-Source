using System;
using UnityEngine;

namespace EntityStates.Headstompers
{
	// Token: 0x0200015F RID: 351
	public class HeadstompersCharge : BaseHeadstompersState
	{
		// Token: 0x060006CF RID: 1743 RVA: 0x00020774 File Offset: 0x0001E974
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				if (base.ReturnToIdleIfGrounded())
				{
					return;
				}
				this.inputStopwatch = (base.jumpButtonDown ? (this.inputStopwatch + Time.deltaTime) : 0f);
				if (this.inputStopwatch >= HeadstompersCharge.maxChargeDuration)
				{
					this.outer.SetNextState(new HeadstompersFall());
					return;
				}
				if (!base.jumpButtonDown)
				{
					this.outer.SetNextState(new HeadstompersIdle());
					return;
				}
				if (this.bodyMotor)
				{
					Vector3 velocity = this.bodyMotor.velocity;
					if (velocity.y < HeadstompersCharge.minVelocityY)
					{
						velocity.y = Mathf.MoveTowards(velocity.y, HeadstompersCharge.minVelocityY, HeadstompersCharge.accelerationY * Time.deltaTime);
						this.bodyMotor.velocity = velocity;
					}
				}
			}
		}

		// Token: 0x0400085E RID: 2142
		private float inputStopwatch;

		// Token: 0x0400085F RID: 2143
		public static float maxChargeDuration = 0.5f;

		// Token: 0x04000860 RID: 2144
		public static float minVelocityY = 1f;

		// Token: 0x04000861 RID: 2145
		public static float accelerationY = 10f;
	}
}
