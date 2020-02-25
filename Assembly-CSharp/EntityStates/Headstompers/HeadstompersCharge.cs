using System;
using UnityEngine;

namespace EntityStates.Headstompers
{
	// Token: 0x02000841 RID: 2113
	public class HeadstompersCharge : BaseHeadstompersState
	{
		// Token: 0x06002FD4 RID: 12244 RVA: 0x000CCE74 File Offset: 0x000CB074
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				if (base.ReturnToIdleIfGrounded())
				{
					return;
				}
				this.inputStopwatch = (base.slamButtonDown ? (this.inputStopwatch + Time.deltaTime) : 0f);
				if (this.inputStopwatch >= HeadstompersCharge.maxChargeDuration)
				{
					this.outer.SetNextState(new HeadstompersFall());
					return;
				}
				if (!base.slamButtonDown)
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

		// Token: 0x04002DA4 RID: 11684
		private float inputStopwatch;

		// Token: 0x04002DA5 RID: 11685
		public static float maxChargeDuration = 0.5f;

		// Token: 0x04002DA6 RID: 11686
		public static float minVelocityY = 1f;

		// Token: 0x04002DA7 RID: 11687
		public static float accelerationY = 10f;
	}
}
